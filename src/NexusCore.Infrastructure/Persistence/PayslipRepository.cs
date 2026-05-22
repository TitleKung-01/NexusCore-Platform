using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงสลิปเงินเดือน (Payslips)</summary>
public class PayslipRepository(AppDbContext db) : IPayslipRepository
{
    /// <summary>ค้นหาสลิปจากรหัส</summary>
    public Task<Payslip?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Payslips.AsNoTracking().Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <summary>ดึงสลิปทั้งหมดของพนักงาน เรียงตามปี/เดือนล่าสุด</summary>
    public async Task<IReadOnlyList<Payslip>> ListForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await db.Payslips
            .AsNoTracking()
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync(cancellationToken);

    /// <summary>เพิ่มสลิปใหม่</summary>
    public async Task AddAsync(Payslip payslip, CancellationToken cancellationToken = default) =>
        await db.Payslips.AddAsync(payslip, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
