using NexusCore.Application.Common;

namespace NexusCore.Application.Payslips;

/// <summary>
/// บริการสลิปเงินเดือน: ดูรายการ ดาวน์โหลด และอัปโหลด (HR)
/// </summary>
public interface IPayslipService
{
    /// <summary>รายการสลิปของพนักงาน (ตนเองหรือที่ HR ระบุ)</summary>
    Task<IReadOnlyList<PayslipResponse>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default);

    /// <summary>ดาวน์โหลดไฟล์สลิป</summary>
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>อัปโหลดสลิปให้พนักงาน (เฉพาะ HR/Admin)</summary>
    Task<ServiceResult<PayslipResponse>> UploadAsync(UploadPayslipRequest request, string fileName, Stream content, CancellationToken cancellationToken = default);
}
