using System.Text.Json;
using AutoMapper;
using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Options;
using CommunicationService.Contracts.Notifications;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.Extensions.Options;

namespace CommunicationService.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly INotificationTemplateRepository _templates;
    private readonly INotificationLogRepository _logs;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly CommunicationOptions _options;
    private readonly IValidator<CreateNotificationRequestDto> _createValidator;
    private readonly IValidator<SendTemplateNotificationRequestDto> _sendTemplateValidator;

    public NotificationService(
        INotificationRepository notifications,
        INotificationTemplateRepository templates,
        INotificationLogRepository logs,
        IMapper mapper,
        ITenantContext tenant,
        IOptions<CommunicationOptions> options,
        IValidator<CreateNotificationRequestDto> createValidator,
        IValidator<SendTemplateNotificationRequestDto> sendTemplateValidator)
    {
        _notifications = notifications;
        _templates = templates;
        _logs = logs;
        _mapper = mapper;
        _tenant = tenant;
        _options = options.Value;
        _createValidator = createValidator;
        _sendTemplateValidator = sendTemplateValidator;
    }

    public async Task<BaseResponse<NotificationResponseDto>> CreateAsync(
        CreateNotificationRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<NotificationResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<NotificationResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var facilityId = _tenant.FacilityId.Value;

        var notification = new ComNotification
        {
            EventType = dto.EventType.Trim(),
            ReferenceId = dto.ReferenceId,
            ContextJson = dto.Context is null ? null : JsonSerializer.Serialize(dto.Context),
            PriorityReferenceValueId = dto.PriorityReferenceValueId,
            StatusReferenceValueId = dto.StatusReferenceValueId ?? _options.ReferenceValueIds.NotificationStatusQueued
        };

        AuditHelper.ApplyCreate(notification, _tenant);
        notification.FacilityId = facilityId;

        foreach (var r in dto.Recipients)
        {
            var recipient = new ComNotificationRecipient
            {
                RecipientTypeReferenceValueId = r.RecipientTypeReferenceValueId,
                RecipientId = r.RecipientId,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                IsPrimary = r.IsPrimary
            };
            AuditHelper.ApplyCreate(recipient, _tenant);
            recipient.FacilityId = facilityId;
            notification.Recipients.Add(recipient);
        }

        foreach (var ch in dto.Channels)
        {
            var code = ch.TemplateCode!.Trim();
            var template = await _templates.GetByCodeAndChannelAsync(
                facilityId,
                code,
                ch.ChannelTypeReferenceValueId,
                cancellationToken);

            if (template is null)
            {
                return BaseResponse<NotificationResponseDto>.Fail(
                    $"Template '{code}' not found for the given channel type.");
            }

            var channel = new ComNotificationChannel
            {
                ChannelTypeReferenceValueId = ch.ChannelTypeReferenceValueId,
                TemplateId = template.Id,
                StatusReferenceValueId = _options.ReferenceValueIds.ChannelDeliveryPending,
                AttemptCount = 0
            };
            AuditHelper.ApplyCreate(channel, _tenant);
            channel.FacilityId = facilityId;
            notification.Channels.Add(channel);
        }

        var queue = new ComNotificationQueue
        {
            ScheduledOn = dto.ScheduledOnUtc ?? DateTime.UtcNow,
            StatusReferenceValueId = _options.ReferenceValueIds.QueuePending
        };
        AuditHelper.ApplyCreate(queue, _tenant);
        queue.FacilityId = facilityId;
        notification.Queues.Add(queue);

        await _notifications.AddAsync(notification, cancellationToken);
        await _notifications.SaveChangesAsync(cancellationToken);

        return BaseResponse<NotificationResponseDto>.Ok(_mapper.Map<NotificationResponseDto>(notification), "Created.");
    }

    public async Task<BaseResponse<NotificationResponseDto>> SendTemplateAsync(
        SendTemplateNotificationRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<NotificationResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var validation = await _sendTemplateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<NotificationResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var create = new CreateNotificationRequestDto
        {
            EventType = dto.EventType,
            ReferenceId = dto.ReferenceId,
            Context = dto.Context,
            PriorityReferenceValueId = dto.PriorityReferenceValueId,
            ScheduledOnUtc = dto.ScheduledOnUtc,
            Recipients = dto.Recipients,
            Channels = new[]
            {
                new ChannelRequestDto
                {
                    ChannelTypeReferenceValueId = dto.ChannelTypeReferenceValueId,
                    TemplateCode = dto.TemplateCode
                }
            }
        };

        return await CreateAsync(create, cancellationToken);
    }

    public async Task<BaseResponse<NotificationResponseDto>> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _notifications.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<NotificationResponseDto>.Fail("Notification not found.");

        return BaseResponse<NotificationResponseDto>.Ok(_mapper.Map<NotificationResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<NotificationLogResponseDto>>> GetLogsPagedAsync(
        long? notificationChannelId,
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _logs.GetPagedAsync(
            notificationChannelId,
            query.Page,
            query.PageSize,
            cancellationToken);

        var paged = new PagedResponse<NotificationLogResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<NotificationLogResponseDto>>(items),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<NotificationLogResponseDto>>.Ok(paged);
    }

    public async Task<BaseResponse<PagedResponse<NotificationTemplateResponseDto>>> GetTemplatesPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _templates.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.Search,
            cancellationToken);

        var paged = new PagedResponse<NotificationTemplateResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<NotificationTemplateResponseDto>>(items),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<NotificationTemplateResponseDto>>.Ok(paged);
    }
}
