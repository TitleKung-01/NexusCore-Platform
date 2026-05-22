namespace NexusCore.Application.Attendance;

/// <summary>บันทึกการลงเวลาเข้า-ออกพร้อมสถานะสาย/ออกก่อนเวลา</summary>
public record AttendanceResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string WorkDate,
    string? CheckInUtc,
    string? CheckOutUtc,
    string? CheckInLocal,
    string? CheckOutLocal,
    bool IsLateCheckIn,
    int LateMinutes,
    bool IsEarlyCheckOut,
    string StatusLabel,
    string? WorkSummary,
    bool CanCheckIn,
    bool CanCheckOut);

/// <summary>คำขอลงเวลาเข้า (ระบุวันทำงาน)</summary>
public record CheckInRequest(string WorkDate);

/// <summary>คำขอลงเวลาออกพร้อมสรุปงาน (ถ้ามี)</summary>
public record CheckOutRequest(string WorkDate, string? WorkSummary);
