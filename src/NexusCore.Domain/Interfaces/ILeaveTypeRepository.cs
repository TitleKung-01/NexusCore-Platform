using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงประเภทการลา — อ่านรายการมาตรฐานสำหรับฟอร์มและโควตา
/// </summary>
public interface ILeaveTypeRepository
{
    /// <summary>รายการประเภทลาทั้งหมด</summary>
    Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาประเภทลาตามรหัส</summary>
    Task<LeaveType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
