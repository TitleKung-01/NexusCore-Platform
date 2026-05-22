using NexusCore.Application.Common;

namespace NexusCore.Application.Attendance;

/// <summary>
/// บริการลงเวลาเข้า-ออกและดูประวัติการเข้างาน
/// </summary>
public interface IAttendanceService
{
    /// <summary>ดึงบันทึกการลงเวลาของวันนี้ (ตามเวลาไทย)</summary>
    Task<AttendanceResponse?> GetTodayAsync(CancellationToken cancellationToken = default);

    /// <summary>ดึงประวัติลงเวลาในช่วงวันที่</summary>
    Task<IReadOnlyList<AttendanceResponse>> ListAsync(string from, string to, Guid? employeeId, CancellationToken cancellationToken = default);

    /// <summary>ลงเวลาเข้างาน</summary>
    Task<ServiceResult<AttendanceResponse>> CheckInAsync(CheckInRequest request, CancellationToken cancellationToken = default);

    /// <summary>ลงเวลาออกงานพร้อมสรุปงาน</summary>
    Task<ServiceResult<AttendanceResponse>> CheckOutAsync(CheckOutRequest request, CancellationToken cancellationToken = default);
}
