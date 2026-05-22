using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงประวัติการโอนย้ายแผนก — ใช้ audit และรายงาน HR
/// </summary>
public interface IEmployeeTransferRepository
{
    /// <summary>รายการโอนย้าย (กรองตามพนักงานและจำกัดจำนวนได้)</summary>
    Task<IReadOnlyList<EmployeeTransfer>> ListAsync(
        Guid? employeeId,
        int? limit,
        CancellationToken cancellationToken = default);
    /// <summary>บันทึกรายการโอนย้ายใหม่</summary>
    Task AddAsync(EmployeeTransfer transfer, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
