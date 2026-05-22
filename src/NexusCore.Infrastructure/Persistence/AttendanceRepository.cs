using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงบันทึกการเข้างาน (AttendanceRecords)</summary>
public class AttendanceRepository(AppDbContext db) : IAttendanceRepository
{
    /// <summary>ค้นหาบันทึกจากรหัส</summary>
    public Task<AttendanceRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <summary>ค้นหาบันทึกของพนักงานในวันที่กำหนด (อ่านอย่างเดียว)</summary>
    public Task<AttendanceRecord?> FindByEmployeeAndDateAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default) =>
        db.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.WorkDate == workDate, cancellationToken);

    /// <summary>ค้นหาบันทึกของพนักงานในวันที่กำหนด (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<AttendanceRecord?> FindByEmployeeAndDateTrackedAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default) =>
        db.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.WorkDate == workDate, cancellationToken);

    /// <summary>ดึงบันทึกในช่วงวันที่ (กรองตามพนักงานได้)</summary>
    public async Task<IReadOnlyList<AttendanceRecord>> ListAsync(Guid? employeeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
    {
        var query = db.AttendanceRecords
            .AsNoTracking()
            .Include(a => a.Employee)
                .ThenInclude(e => e.User)
            .Include(a => a.Employee)
                .ThenInclude(e => e.Department)
            .Where(a => a.WorkDate >= from && a.WorkDate <= to);

        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);

        return await query.OrderBy(a => a.WorkDate).ToListAsync(cancellationToken);
    }

    /// <summary>เพิ่มบันทึกการเข้างานใหม่</summary>
    public async Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default) =>
        await db.AttendanceRecords.AddAsync(record, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
