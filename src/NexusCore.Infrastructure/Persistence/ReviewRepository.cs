using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class ReviewRepository(AppDbContext db) : IReviewRepository
{
    public async Task<IReadOnlyList<ReviewCycle>> ListCyclesAsync(CancellationToken cancellationToken = default) =>
        await db.ReviewCycles.AsNoTracking().OrderByDescending(c => c.StartDate).ToListAsync(cancellationToken);

    public Task<ReviewCycle?> FindCycleByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.ReviewCycles.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<PerformanceReview?> FindReviewByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.PerformanceReviews
            .AsNoTracking()
            .Include(r => r.Cycle)
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<PerformanceReview?> FindReviewByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.PerformanceReviews
            .Include(r => r.Cycle)
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<PerformanceReview>> ListReviewsForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await db.PerformanceReviews
            .AsNoTracking()
            .Include(r => r.Cycle)
            .Where(r => r.EmployeeId == employeeId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PerformanceReview>> ListReviewsForManagerAsync(Guid managerId, CancellationToken cancellationToken = default) =>
        await db.PerformanceReviews
            .AsNoTracking()
            .Include(r => r.Cycle)
            .Include(r => r.Employee)
            .Where(r => r.ManagerId == managerId || r.Employee.ManagerId == managerId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public async Task AddCycleAsync(ReviewCycle cycle, CancellationToken cancellationToken = default) =>
        await db.ReviewCycles.AddAsync(cycle, cancellationToken);

    public async Task AddReviewAsync(PerformanceReview review, CancellationToken cancellationToken = default) =>
        await db.PerformanceReviews.AddAsync(review, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
