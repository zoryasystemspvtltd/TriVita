using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using HMSService.Application.Integration;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1.Extended;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hms/integration/lab-test-bookings")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS → LMS lab test booking proxy")]
public sealed class HmsLmsLabTestBookingIntegrationController : ControllerBase
{
    private readonly ILmsTestBookingClient _client;

    public HmsLmsLabTestBookingIntegrationController(ILmsTestBookingClient client) => _client = client;

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LmsTestBookingClientResponseDto>>> Create(
        [FromBody] CreateLmsTestBookingClientRequestDto dto,
        CancellationToken cancellationToken) =>
        Ok(await _client.CreateBookingAsync(dto, cancellationToken));
}
