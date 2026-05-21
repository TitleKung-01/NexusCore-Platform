namespace NexusCore.Application.Payslips;

public record PayslipResponse(
    Guid Id,
    Guid EmployeeId,
    int Year,
    int Month,
    string FileName,
    string PublishedAtUtc);

public record UploadPayslipRequest(Guid EmployeeId, int Year, int Month);
