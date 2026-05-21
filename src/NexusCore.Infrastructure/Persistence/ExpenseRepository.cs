using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Enums;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class ExpenseRepository(AppDbContext db) : IExpenseRepository
{
    private IQueryable<ExpenseClaim> WithIncludes() =>
        db.ExpenseClaims
            .AsNoTracking()
            .Include(e => e.LineItems)
            .Include(e => e.Employee)
            .ThenInclude(emp => emp!.User);

    public Task<ExpenseClaim?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        WithIncludes().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<ExpenseClaim?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.ExpenseClaims
            .Include(e => e.LineItems)
            .Include(e => e.Employee)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ExpenseClaim>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ExpenseClaim>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.Status == LeaveStatus.Pending && e.Employee.ManagerId == managerUserId)
            .OrderBy(e => e.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ExpenseClaim>> GetAllPendingAsync(CancellationToken cancellationToken = default) =>
        await WithIncludes()
            .Where(e => e.Status == LeaveStatus.Pending)
            .OrderBy(e => e.SubmittedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ExpenseClaim claim, CancellationToken cancellationToken = default) =>
        await db.ExpenseClaims.AddAsync(claim, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
