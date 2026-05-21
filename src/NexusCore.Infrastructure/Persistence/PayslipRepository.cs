using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class PayslipRepository(AppDbContext db) : IPayslipRepository
{
    public Task<Payslip?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Payslips.AsNoTracking().Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Payslip>> ListForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await db.Payslips
            .AsNoTracking()
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Payslip payslip, CancellationToken cancellationToken = default) =>
        await db.Payslips.AddAsync(payslip, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
