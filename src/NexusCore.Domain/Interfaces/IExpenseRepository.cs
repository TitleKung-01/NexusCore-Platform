using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงใบเบิกค่าใช้จ่าย — workflow อนุมัติคล้ายคำขอลา
/// </summary>
public interface IExpenseRepository
{
    /// <summary>ค้นหาใบเบิกตามรหัส (อ่านอย่างเดียว)</summary>
    Task<ExpenseClaim?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาใบเบิกพร้อมติดตามการแก้ไข</summary>
    Task<ExpenseClaim?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>รายการใบเบิกของพนักงานคนหนึ่ง</summary>
    Task<IReadOnlyList<ExpenseClaim>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    /// <summary>ใบเบิกที่รออนุมัติของลูกน้องภายใต้ผู้จัดการ</summary>
    Task<IReadOnlyList<ExpenseClaim>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    /// <summary>ใบเบิกที่รออนุมัติทั้งองค์กร (มุมมอง HR)</summary>
    Task<IReadOnlyList<ExpenseClaim>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    /// <summary>เพิ่มใบเบิกใหม่</summary>
    Task AddAsync(ExpenseClaim claim, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
