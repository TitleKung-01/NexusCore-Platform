namespace NexusCore.Application.Payslips;

/// <summary>สลิปเงินเดือนที่เผยแพร่ให้พนักงาน</summary>
public record PayslipResponse(
    Guid Id,
    Guid EmployeeId,
    int Year,
    int Month,
    string FileName,
    string PublishedAtUtc);

/// <summary>คำขออัปโหลดสลิปเงินเดือน (HR ระบุพนักงาน ปี เดือน)</summary>
public record UploadPayslipRequest(Guid EmployeeId, int Year, int Month);
