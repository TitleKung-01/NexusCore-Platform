using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงโปรไฟล์พนักงาน (EmployeeProfiles) พร้อมแผนก ผู้ใช้ และหัวหน้า</summary>
public class EmployeeProfileRepository(AppDbContext db) : IEmployeeProfileRepository
{
    /// <summary>ค้นหาโปรไฟล์จากรหัสผู้ใช้ (อ่านอย่างเดียว)</summary>
    public Task<EmployeeProfile?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.EmployeeProfiles
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.User)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

    /// <summary>ค้นหาโปรไฟล์จากรหัสผู้ใช้ (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<EmployeeProfile?> FindByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.EmployeeProfiles
            .Include(e => e.Department)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

    /// <summary>ดึงพนักงานทั้งหมดพร้อมรายละเอียด เรียงตามชื่อ</summary>
    public async Task<IReadOnlyList<EmployeeProfile>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await db.EmployeeProfiles
            .AsNoTracking()
            .Include(e => e.Department)
            .Include(e => e.User)
            .Include(e => e.Manager)
            .OrderBy(e => e.FullName)
            .ToListAsync(cancellationToken);

    /// <summary>เพิ่มโปรไฟล์พนักงานใหม่</summary>
    public async Task AddAsync(EmployeeProfile profile, CancellationToken cancellationToken = default) =>
        await db.EmployeeProfiles.AddAsync(profile, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
