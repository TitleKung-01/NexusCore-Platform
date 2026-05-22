using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงคำเบิกค่าใช้จ่าย (ExpenseClaims) พร้อมรายการและข้อมูลพนักงาน</summary>
public class ExpenseRepository(AppDbContext db) : IExpenseRepository
{
    /// <summary>Query พื้นฐานพร้อม Include ที่จำเป็น</summary>
    private IQueryable<ExpenseClaim> WithIncludes() =>
        db.ExpenseClaims
            .AsNoTracking()
            .Include(e => e.LineItems)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.User)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Department)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Manager);

    /// <summary>ค้นหาคำเบิกจากรหัส (อ่านอย่างเดียว)</summary>
    public Task<ExpenseClaim?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithIncludes().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <summary>ค้นหาคำเบิกจากรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<ExpenseClaim?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.ExpenseClaims
            .Include(e => e.LineItems)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.User)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Department)
            .Include(e => e.Employee)
                .ThenInclude(emp => emp!.Manager)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <summary>ดึงคำเบิกของพนักงานคนหนึ่ง</summary>
    public async Task<IReadOnlyList<ExpenseClaim>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ดึงคำเบิกที่รออนุมัติของทีมภายใต้หัวหน้าคนนี้</summary>
    public async Task<IReadOnlyList<ExpenseClaim>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.Status == LeaveStatus.Pending && e.Employee.ManagerId == managerUserId)
            .OrderBy(e => e.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ดึงคำเบิกที่รออนุมัติทั้งองค์กร</summary>
    public async Task<IReadOnlyList<ExpenseClaim>> GetAllPendingAsync(CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.Status == LeaveStatus.Pending)
            .OrderBy(e => e.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>เพิ่มคำเบิกใหม่</summary>
    public async Task AddAsync(ExpenseClaim claim, CancellationToken cancellationToken = default) =>
        await db.ExpenseClaims.AddAsync(claim, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
