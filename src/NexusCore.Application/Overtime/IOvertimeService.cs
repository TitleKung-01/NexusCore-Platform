using NexusCore.Application.Common;

namespace NexusCore.Application.Overtime;

/// <summary>
/// บริการคำขอทำงานล่วงเวลา (OT): สร้าง ส่ง อนุมัติ/ปฏิเสธ และยกเลิก
/// </summary>
public interface IOvertimeService
{
    /// <summary>ดึงรายการ OT ตาม scope (ของตนเองหรือรออนุมัติ)</summary>
    Task<IReadOnlyList<OvertimeRequestResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายละเอียดคำขอ OT</summary>
    Task<OvertimeRequestResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างคำขอ OT แบบร่าง</summary>
    Task<ServiceResult<OvertimeRequestResponse>> CreateAsync(CreateOvertimeRequest request, CancellationToken cancellationToken = default);

    /// <summary>ส่งคำขอ OT เพื่อรออนุมัติ</summary>
    Task<ServiceResult<OvertimeRequestResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>อนุมัติคำขอ OT</summary>
    Task<ServiceResult<OvertimeRequestResponse>> ApproveAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default);

    /// <summary>ปฏิเสธคำขอ OT</summary>
    Task<ServiceResult<OvertimeRequestResponse>> RejectAsync(Guid id, DecideOvertimeRequest request, CancellationToken cancellationToken = default);

    /// <summary>ยกเลิกคำขอ OT แบบร่างหรือรออนุมัติ</summary>
    Task<ServiceResult<OvertimeRequestResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
