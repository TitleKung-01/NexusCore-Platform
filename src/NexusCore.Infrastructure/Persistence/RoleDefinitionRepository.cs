using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงข้อมูลนิยามบทบาท/สิทธิ์ (RoleDefinitions)</summary>
public class RoleDefinitionRepository(AppDbContext db) : IRoleDefinitionRepository
{
    /// <summary>ดึงบทบาททั้งหมดเรียงตามชื่อ</summary>
    public async Task<IReadOnlyList<RoleDefinition>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.RoleDefinitions.AsNoTracking().OrderBy(r => r.Name).ToListAsync(cancellationToken);

    /// <summary>ค้นหาบทบาทจากชื่อ (ไม่สนตัวพิมพ์)</summary>
    public Task<RoleDefinition?> FindByNameAsync(string name, CancellationToken cancellationToken = default) =>
        db.RoleDefinitions.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower(), cancellationToken);

    /// <summary>เพิ่มนิยามบทบาทใหม่</summary>
    public async Task AddAsync(RoleDefinition role, CancellationToken cancellationToken = default) =>
        await db.RoleDefinitions.AddAsync(role, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
