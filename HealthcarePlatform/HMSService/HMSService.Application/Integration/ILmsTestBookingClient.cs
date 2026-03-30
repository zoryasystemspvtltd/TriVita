using Healthcare.Common.Responses;

namespace HMSService.Application.Integration;

/// <summary>Cross-service call from HMS to LMS workflow test-booking API.</summary>
public interface ILmsTestBookingClient
{
    Task<BaseResponse<LmsTestBookingClientResponseDto>> CreateBookingAsync(
        CreateLmsTestBookingClientRequestDto request,
        CancellationToken cancellationToken = default);
}

public sealed class CreateLmsTestBookingClientItemDto
{
    public long CatalogTestId { get; init; }
    public long WorkflowStatusReferenceValueId { get; init; }
    public string? LineNotes { get; init; }
}

public sealed class CreateLmsTestBookingClientRequestDto
{
    public long PatientId { get; init; }
    public long? VisitId { get; init; }
    public long? SourceReferenceValueId { get; init; }
    public string? BookingNotes { get; init; }
    public IReadOnlyList<CreateLmsTestBookingClientItemDto> Items { get; init; } = Array.Empty<CreateLmsTestBookingClientItemDto>();
}

public sealed class LmsTestBookingClientResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string BookingNo { get; init; } = null!;
    public long PatientId { get; init; }
    public long? VisitId { get; init; }
}
