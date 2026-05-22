using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงวันหยุดบริษัท (CompanyHolidays)</summary>
public class CompanyHolidayRepository(AppDbContext db) : ICompanyHolidayRepository
{
    /// <summary>ดึงรายการวันหยุด (กรองตามปีได้)</summary>
    public async Task<IReadOnlyList<CompanyHoliday>> ListAsync(int? year, CancellationToken cancellationToken = default)
    {
        var query = db.CompanyHolidays.AsNoTracking().AsQueryable();
        if (year.HasValue)
        {
            var start = new DateOnly(year.Value, 1, 1);
            var end = new DateOnly(year.Value, 12, 31);
            query = query.Where(h => h.Date >= start && h.Date <= end);
        }

        return await query.OrderBy(h => h.Date).ToListAsync(cancellationToken);
    }

    /// <summary>ค้นหาวันหยุดจากรหัส (อ่านอย่างเดียว)</summary>
    public Task<CompanyHoliday?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.CompanyHolidays.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    /// <summary>ค้นหาวันหยุดจากรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<CompanyHoliday?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.CompanyHolidays.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    /// <summary>เพิ่มวันหยุดใหม่</summary>
    public async Task AddAsync(CompanyHoliday holiday, CancellationToken cancellationToken = default) =>
        await db.CompanyHolidays.AddAsync(holiday, cancellationToken);

    /// <summary>ลบวันหยุด</summary>
    public void Remove(CompanyHoliday holiday) => db.CompanyHolidays.Remove(holiday);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
