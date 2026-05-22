using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงข้อมูลแผนก/หน่วยงาน (Departments)</summary>
public class DepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    /// <summary>ดึงแผนกทั้งหมดเรียงตามชื่อ</summary>
    public async Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Departments.AsNoTracking().OrderBy(d => d.Name).ToListAsync(cancellationToken);

    /// <summary>ค้นหาแผนกจากรหัส</summary>
    public Task<Department?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    /// <summary>ค้นหาแผนกจากรหัสย่อ (ไม่สนตัวพิมพ์)</summary>
    public Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Departments.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Code.ToLower() == code.ToLower(), cancellationToken);

    /// <summary>เพิ่มแผนกใหม่</summary>
    public async Task AddAsync(Department department, CancellationToken cancellationToken = default) =>
        await db.Departments.AddAsync(department, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
