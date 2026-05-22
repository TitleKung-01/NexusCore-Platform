using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงประวัติการย้ายแผนก (EmployeeTransfers)</summary>
public class EmployeeTransferRepository(AppDbContext db) : IEmployeeTransferRepository
{
    /// <summary>ดึงรายการย้ายแผนก (กรองพนักงานและจำกัดจำนวนได้)</summary>
    public async Task<IReadOnlyList<EmployeeTransfer>> ListAsync(
        Guid? employeeId,
        int? limit,
        CancellationToken cancellationToken = default)
    {
        var query = db.EmployeeTransfers
            .AsNoTracking()
            .Include(t => t.FromDepartment)
            .Include(t => t.ToDepartment)
            .Include(t => t.Employee)
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(t => t.EmployeeId == employeeId.Value);

        query = query.OrderByDescending(t => t.CreatedAtUtc);

        if (limit is > 0)
            query = query.Take(limit.Value);

        return await query.ToListAsync(cancellationToken);
    }

    /// <summary>เพิ่มบันทึกการย้ายแผนก</summary>
    public async Task AddAsync(EmployeeTransfer transfer, CancellationToken cancellationToken = default) =>
        await db.EmployeeTransfers.AddAsync(transfer, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
