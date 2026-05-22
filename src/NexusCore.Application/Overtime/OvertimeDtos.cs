namespace NexusCore.Application.Overtime;

/// <summary>คำขอทำงานล่วงเวลาสำหรับแสดงและอนุมัติ</summary>
public record OvertimeRequestResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeUsername,
    string EmployeeEmail,
    string EmployeeDepartment,
    string EmployeeRole,
    string? EmployeeManagerName,
    string WorkDate,
    decimal Hours,
    string Reason,
    string Status,
    string? SubmittedAtUtc,
    string? DecidedAtUtc,
    string? ManagerComment);

/// <summary>คำขอสร้างใบ OT แบบร่าง</summary>
public record CreateOvertimeRequest(string WorkDate, decimal Hours, string Reason);

/// <summary>ความเห็นผู้อนุมัติเมื่ออนุมัติ/ปฏิเสธ OT</summary>
public record DecideOvertimeRequest(string? Comment);
