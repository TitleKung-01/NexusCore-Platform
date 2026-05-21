namespace NexusCore.Application.Transfers;

public record EmployeeTransferResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string FromDepartment,
    string ToDepartment,
    string EffectiveDate,
    string? Note,
    string CreatedAtUtc);
