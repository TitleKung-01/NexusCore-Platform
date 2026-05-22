using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงข้อมูลบัญชีผู้ใช้ระบบ (Users)</summary>
public class UserRepository(AppDbContext db) : IUserRepository
{
    /// <summary>ค้นหาผู้ใช้จากชื่อผู้ใช้</summary>
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    /// <summary>ค้นหาผู้ใช้จากรหัส (อ่านอย่างเดียว)</summary>
    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    /// <summary>ค้นหาผู้ใช้จากรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<User?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    /// <summary>ตรวจว่ามีชื่อผู้ใช้นี้แล้วหรือไม่</summary>
    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Username == username, cancellationToken);

    /// <summary>ตรวจว่ามีผู้ใช้ในฐานข้อมูลหรือไม่</summary>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(cancellationToken);

    /// <summary>ดึงผู้ใช้ทั้งหมดเรียงตามชื่อผู้ใช้</summary>
    public async Task<IReadOnlyList<User>> GetAllOrderedAsync(CancellationToken cancellationToken = default) =>
        await db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync(cancellationToken);

    /// <summary>เพิ่มผู้ใช้ใหม่</summary>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }

    /// <summary>ลบผู้ใช้</summary>
    public void Remove(User user) => db.Users.Remove(user);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
