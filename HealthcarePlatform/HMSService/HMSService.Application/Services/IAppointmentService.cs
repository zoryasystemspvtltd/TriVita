using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Appointments;

namespace HMSService.Application.Services;

public interface IAppointmentService
{
    Task<BaseResponse<AppointmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<AppointmentResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientId,
        long? doctorId,
        DateTime? scheduledFrom,
        DateTime? scheduledTo,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<AppointmentResponseDto>> CreateAsync(
        CreateAppointmentDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<AppointmentResponseDto>> UpdateAsync(
        long id,
        UpdateAppointmentDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
