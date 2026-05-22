namespace NexusCore.Application.Leave;

/// <summary>ข้อมูลใบลาสำหรับแสดงในรายการและรายละเอียด พร้อมข้อมูลพนักงาน</summary>
public record LeaveRequestResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeUsername,
    string EmployeeEmail,
    string EmployeeDepartment,
    string EmployeeRole,
    string? EmployeeManagerName,
    Guid LeaveTypeId,
    string LeaveTypeName,
    string StartDate,
    string EndDate,
    string Reason,
    string Status,
    string? SubmittedAtUtc,
    string? DecidedAtUtc,
    string? ManagerComment);

/// <summary>คำขอสร้างใบลาแบบร่าง</summary>
public record CreateLeaveRequest(Guid LeaveTypeId, string StartDate, string EndDate, string Reason);

/// <summary>คำขออนุมัติหรือปฏิเสธใบลา (ความเห็นผู้จัดการ)</summary>
public record DecideLeaveRequest(string? Comment);

/// <summary>ยอดวันลาคงเหลือตามประเภทและปี</summary>
public record LeaveBalanceResponse(
    Guid LeaveTypeId,
    string LeaveTypeName,
    int Year,
    decimal DaysAllowed,
    decimal DaysUsed,
    decimal DaysRemaining);

/// <summary>รายการในปฏิทินลาสำหรับช่วงวันที่ที่กำหนด</summary>
public record LeaveCalendarEntryResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string LeaveTypeName,
    string StartDate,
    string EndDate,
    string Status);

/// <summary>ไฟล์แนบของใบลา</summary>
public record LeaveAttachmentResponse(
    Guid Id,
    Guid LeaveRequestId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string UploadedAtUtc);
