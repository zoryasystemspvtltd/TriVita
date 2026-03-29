using System.Text.Json;
using CommunicationService.Application.Abstractions;
using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using CommunicationService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastructure.Background;

/// <summary>Polls COM_NotificationQueue and dispatches channels via configured providers.</summary>
public sealed class NotificationProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CommunicationOptions _options;
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<CommunicationOptions> options,
        ILogger<NotificationProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var queueRepo = scope.ServiceProvider.GetRequiredService<INotificationQueueRepository>();
                var pending = await queueRepo.GetPendingBatchAsync(_options.Processor.BatchSize, stoppingToken);

                foreach (var q in pending)
                {
                    using var itemScope = _scopeFactory.CreateScope();
                    await ProcessQueueItemAsync(itemScope, q.Id, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notification processor iteration failed.");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _options.Processor.PollIntervalSeconds)), stoppingToken);
        }
    }

    private async Task ProcessQueueItemAsync(IServiceScope scope, long queueId, CancellationToken ct)
    {
        var opts = scope.ServiceProvider.GetRequiredService<IOptions<CommunicationOptions>>().Value;
        var queueRepo = scope.ServiceProvider.GetRequiredService<INotificationQueueRepository>();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var renderer = scope.ServiceProvider.GetRequiredService<ITemplateRenderer>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();
        var waService = scope.ServiceProvider.GetRequiredService<IWhatsAppService>();

        var queue = await queueRepo.GetByIdIgnoreFiltersAsync(queueId, ct);
        if (queue is null)
            return;

        queue.StatusReferenceValueId = opts.ReferenceValueIds.QueueProcessing;
        queue.ModifiedOn = DateTime.UtcNow;
        await queueRepo.SaveChangesAsync(ct);

        if (queue.FacilityId is null)
        {
            queue.StatusReferenceValueId = opts.ReferenceValueIds.QueueFailed;
            queue.ProcessedOn = DateTime.UtcNow;
            queue.ModifiedOn = DateTime.UtcNow;
            await queueRepo.SaveChangesAsync(ct);
            _logger.LogWarning("Queue {QueueId} missing FacilityId.", queueId);
            return;
        }

        var notification = await notificationRepo.GetByIdForProcessingAsync(
            queue.TenantId,
            queue.FacilityId.Value,
            queue.NotificationId,
            ct);

        if (notification is null)
        {
            queue.StatusReferenceValueId = opts.ReferenceValueIds.QueueFailed;
            queue.ProcessedOn = DateTime.UtcNow;
            queue.ModifiedOn = DateTime.UtcNow;
            await queueRepo.SaveChangesAsync(ct);
            _logger.LogWarning("Notification {Id} missing for queue {QueueId}", queue.NotificationId, queueId);
            return;
        }

        var context = ParseContext(notification.ContextJson);

        foreach (var channel in notification.Channels)
        {
            if (channel.StatusReferenceValueId == opts.ReferenceValueIds.ChannelDeliverySent)
                continue;

            await ProcessChannelAsync(
                notification,
                channel,
                context,
                opts,
                renderer,
                emailService,
                smsService,
                waService,
                notificationRepo,
                queue,
                ct);
        }

        var sent = notification.Channels.Any(c => c.StatusReferenceValueId == opts.ReferenceValueIds.ChannelDeliverySent);
        var allFailed = notification.Channels.All(c =>
            c.StatusReferenceValueId == opts.ReferenceValueIds.ChannelDeliveryFailed);

        notification.StatusReferenceValueId = sent
            ? opts.ReferenceValueIds.NotificationStatusCompleted
            : allFailed
                ? opts.ReferenceValueIds.NotificationStatusFailed
                : opts.ReferenceValueIds.NotificationStatusFailed;

        notification.ModifiedOn = DateTime.UtcNow;

        queue.StatusReferenceValueId = sent
            ? opts.ReferenceValueIds.QueueCompleted
            : opts.ReferenceValueIds.QueueFailed;
        queue.ProcessedOn = DateTime.UtcNow;
        queue.ModifiedOn = DateTime.UtcNow;

        await notificationRepo.SaveChangesAsync(ct);
    }

    private static IReadOnlyDictionary<string, string> ParseContext(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var d = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return d ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private async Task ProcessChannelAsync(
        ComNotification notification,
        ComNotificationChannel channel,
        IReadOnlyDictionary<string, string> context,
        CommunicationOptions opts,
        ITemplateRenderer renderer,
        IEmailService emailService,
        ISmsService smsService,
        IWhatsAppService waService,
        INotificationRepository notificationRepo,
        ComNotificationQueue queue,
        CancellationToken ct)
    {
        var template = channel.Template;
        if (template is null)
        {
            channel.StatusReferenceValueId = opts.ReferenceValueIds.ChannelDeliveryFailed;
            channel.ErrorMessage = "Template navigation not loaded.";
            channel.ModifiedOn = DateTime.UtcNow;
            await notificationRepo.SaveChangesAsync(ct);
            return;
        }

        var subject = renderer.Render(template.SubjectTemplate, context);
        var body = renderer.Render(template.BodyTemplate, context);
        var max = opts.Processor.MaxRetryAttempts;

        ChannelSendResult? lastResult = null;

        for (var attempt = 1; attempt <= max; attempt++)
        {
            channel.AttemptCount = attempt;
            channel.LastAttemptOn = DateTime.UtcNow;
            channel.ModifiedOn = DateTime.UtcNow;
            await notificationRepo.SaveChangesAsync(ct);

            lastResult = await DispatchAsync(
                notification,
                channel,
                subject,
                body,
                opts,
                emailService,
                smsService,
                waService,
                ct);

            var log = new ComNotificationLog
            {
                NotificationChannelId = channel.Id,
                AttemptNo = attempt,
                RequestPayload = JsonSerializer.Serialize(new { subject, body, channel.ChannelTypeReferenceValueId }),
                ResponsePayload = lastResult.ResponseText,
                StatusReferenceValueId = lastResult.Success
                    ? opts.ReferenceValueIds.LogAttemptSuccess
                    : opts.ReferenceValueIds.LogAttemptFailed
            };
            WorkerAuditHelper.ApplyCreate(log, queue);
            await notificationRepo.AddNotificationLogAsync(log, ct);
            await notificationRepo.SaveChangesAsync(ct);

            if (lastResult.Success)
            {
                channel.SentOn = DateTime.UtcNow;
                channel.StatusReferenceValueId = opts.ReferenceValueIds.ChannelDeliverySent;
                channel.ErrorMessage = null;
                channel.ModifiedOn = DateTime.UtcNow;
                await notificationRepo.SaveChangesAsync(ct);
                return;
            }

            channel.ErrorMessage = lastResult.Error;
            channel.ModifiedOn = DateTime.UtcNow;
            await notificationRepo.SaveChangesAsync(ct);
        }

        channel.StatusReferenceValueId = opts.ReferenceValueIds.ChannelDeliveryFailed;
        channel.ModifiedOn = DateTime.UtcNow;
        await notificationRepo.SaveChangesAsync(ct);

        _logger.LogWarning(
            "Channel {ChannelId} failed after {Attempts}: {Error}",
            channel.Id,
            max,
            lastResult?.Error);
    }

    private static async Task<ChannelSendResult> DispatchAsync(
        ComNotification notification,
        ComNotificationChannel channel,
        string subject,
        string body,
        CommunicationOptions opts,
        IEmailService emailService,
        ISmsService smsService,
        IWhatsAppService waService,
        CancellationToken ct)
    {
        var typeId = channel.ChannelTypeReferenceValueId;

        if (typeId == opts.ReferenceValueIds.ChannelTypeEmail)
        {
            var emails = notification.Recipients
                .Select(r => r.Email)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (emails.Count == 0)
                return ChannelSendResult.Fail("No recipient email addresses.");

            return await emailService.SendAsync(
                new EmailMessage
                {
                    ToAddresses = emails,
                    Subject = subject,
                    Body = body,
                    IsHtml = false
                },
                ct);
        }

        if (typeId == opts.ReferenceValueIds.ChannelTypeSms)
        {
            var phones = notification.Recipients
                .Select(r => r.PhoneNumber)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p!.Trim())
                .Distinct()
                .ToList();

            if (phones.Count == 0)
                return ChannelSendResult.Fail("No recipient phone numbers for SMS.");

            ChannelSendResult? last = null;
            foreach (var phone in phones)
            {
                last = await smsService.SendAsync(new SmsMessage { ToPhoneNumber = phone, Body = body }, ct);
                if (!last.Success)
                    return last;
            }

            return last ?? ChannelSendResult.Fail("SMS dispatch error.");
        }

        if (typeId == opts.ReferenceValueIds.ChannelTypeWhatsApp)
        {
            var phones = notification.Recipients
                .Select(r => r.PhoneNumber)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p!.Trim())
                .Distinct()
                .ToList();

            if (phones.Count == 0)
                return ChannelSendResult.Fail("No recipient phone numbers for WhatsApp.");

            ChannelSendResult? last = null;
            foreach (var phone in phones)
            {
                last = await waService.SendAsync(new WhatsAppMessage { ToPhoneNumber = phone, Body = body }, ct);
                if (!last.Success)
                    return last;
            }

            return last ?? ChannelSendResult.Fail("WhatsApp dispatch error.");
        }

        return ChannelSendResult.Fail($"Unknown channel type reference id: {typeId}.");
    }
}
