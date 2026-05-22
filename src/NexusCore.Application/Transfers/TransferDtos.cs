namespace NexusCore.Application.Transfers;

/// <summary>ประวัติการย้ายแผนกของพนักงาน</summary>
public record EmployeeTransferResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string FromDepartment,
    string ToDepartment,
    string EffectiveDate,
    string? Note,
    string CreatedAtUtc);
