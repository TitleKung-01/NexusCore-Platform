using NexusCore.Application.Common;

namespace NexusCore.Application.Attendance;

public interface IAttendanceService
{
    Task<IReadOnlyList<AttendanceResponse>> ListAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default);
    Task<ServiceResult<AttendanceResponse>> CheckInAsync(CheckInRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<AttendanceResponse>> CheckOutAsync(CheckOutRequest request, CancellationToken cancellationToken = default);
}
