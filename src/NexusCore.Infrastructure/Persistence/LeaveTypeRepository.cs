using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงข้อมูลประเภทการลา (LeaveTypes)</summary>
public class LeaveTypeRepository(AppDbContext db) : ILeaveTypeRepository
{
    /// <summary>ดึงประเภทลาทั้งหมดเรียงตามชื่อ</summary>
    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.LeaveTypes.AsNoTracking().OrderBy(l => l.Name).ToListAsync(cancellationToken);

    /// <summary>ค้นหาประเภทลาจากรหัส</summary>
    public Task<LeaveType?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.LeaveTypes.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
}
