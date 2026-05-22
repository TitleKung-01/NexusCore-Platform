using NexusCore.Application.Common;

namespace NexusCore.Application.Leave;

/// <summary>
/// บริการจัดการใบลา: สร้าง ส่งอนุมัติ อนุมัติ/ปฏิเสธ ยอดคงเหลือ ปฏิทิน และไฟล์แนบ
/// </summary>
public interface ILeaveService
{
    /// <summary>ดึงรายการใบลาตาม scope (ของตนเอง รออนุมัติ ประวัติ)</summary>
    Task<IReadOnlyList<LeaveRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายละเอียดใบลาตามรหัส (ตรวจสิทธิ์การดู)</summary>
    Task<LeaveRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างใบลาแบบร่าง</summary>
    Task<ServiceResult<LeaveRequestResponse>> CreateAsync(CreateLeaveRequest request, CancellationToken cancellationToken = default);

    /// <summary>ส่งใบลาเพื่อรออนุมัติ (ตรวจโควตาและวันซ้อน)</summary>
    Task<ServiceResult<LeaveRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>อนุมัติใบลาที่รอดำเนินการ</summary>
    Task<ServiceResult<LeaveRequestResponse>> ApproveAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default);

    /// <summary>ปฏิเสธใบลาที่รอดำเนินการ</summary>
    Task<ServiceResult<LeaveRequestResponse>> RejectAsync(Guid id, DecideLeaveRequest request, CancellationToken cancellationToken = default);

    /// <summary>ยกเลิกใบลาแบบร่างหรือรออนุมัติ</summary>
    Task<ServiceResult<LeaveRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ดึงยอดวันลาคงเหลือตามปีและพนักงาน</summary>
    Task<IReadOnlyList<LeaveBalanceResponse>> GetBalancesAsync(int? year, Guid? employeeId, CancellationToken cancellationToken = default);

    /// <summary>ดึงปฏิทินลาในช่วงวันที่ (กรองแผนกได้)</summary>
    Task<IReadOnlyList<LeaveCalendarEntryResponse>> GetCalendarAsync(string from, string to, Guid? departmentId, CancellationToken cancellationToken = default);

    /// <summary>อัปโหลดไฟล์แนบให้ใบลา</summary>
    Task<ServiceResult<LeaveAttachmentResponse>> UploadAttachmentAsync(Guid leaveRequestId, string fileName, Stream content, CancellationToken cancellationToken = default);

    /// <summary>ดาวน์โหลดไฟล์แนบ</summary>
    Task<(Stream Stream, string ContentType, string FileName)?> DownloadAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken = default);

    /// <summary>รายการไฟล์แนบของใบลา</summary>
    Task<IReadOnlyList<LeaveAttachmentResponse>> ListAttachmentsAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);
}
