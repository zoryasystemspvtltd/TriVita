using Asp.Versioning;
using CommunicationService.Application.Abstractions;
using CommunicationService.Contracts.Notifications;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CommunicationService.API.Controllers.v1;

/// <summary>
/// Event-driven notifications: create, template send, logs, templates (DTO-only responses).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[RequirePermission(TriVitaPermissions.CommunicationApi)]
[SwaggerTag("Notifications")]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>Creates a notification with recipients, channels, and queue entry.</summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Create notification", OperationId = "Notifications_Create")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped notification id and metadata.", typeof(BaseResponse<NotificationResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<NotificationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<NotificationResponseDto>>> Create(
        [FromBody] CreateNotificationRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _notificationService.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Sends using a single template + channel (convenience over POST /notifications).</summary>
    [HttpPost("send-template")]
    [SwaggerOperation(Summary = "Send via template", OperationId = "Notifications_SendTemplate")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped notification id and metadata.", typeof(BaseResponse<NotificationResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<NotificationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<NotificationResponseDto>>> SendTemplate(
        [FromBody] SendTemplateNotificationRequestDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _notificationService.SendTemplateAsync(dto, cancellationToken);
        return Ok(result);
    }

    /// <summary>Gets notification by id (includes aggregate counts).</summary>
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get notification by id", OperationId = "Notifications_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Wrapped notification.", typeof(BaseResponse<NotificationResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<NotificationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<NotificationResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _notificationService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists delivery attempt logs (paged), optionally filtered by channel id.</summary>
    [HttpGet("logs")]
    [SwaggerOperation(Summary = "List notification logs (paged)", OperationId = "Notifications_GetLogs")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paged logs.", typeof(BaseResponse<PagedResponse<NotificationLogResponseDto>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<NotificationLogResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<PagedResponse<NotificationLogResponseDto>>>> GetLogs(
        [FromQuery] PagedQuery query,
        [FromQuery] long? notificationChannelId,
        CancellationToken cancellationToken)
    {
        var result = await _notificationService.GetLogsPagedAsync(notificationChannelId, query, cancellationToken);
        return Ok(result);
    }

    /// <summary>Lists templates for the current tenant/facility scope (paged).</summary>
    [HttpGet("templates")]
    [SwaggerOperation(Summary = "List templates (paged)", OperationId = "Notifications_GetTemplates")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paged templates.", typeof(BaseResponse<PagedResponse<NotificationTemplateResponseDto>>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Missing or invalid JWT.")]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<NotificationTemplateResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<PagedResponse<NotificationTemplateResponseDto>>>> GetTemplates(
        [FromQuery] PagedQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _notificationService.GetTemplatesPagedAsync(query, cancellationToken);
        return Ok(result);
    }
}
