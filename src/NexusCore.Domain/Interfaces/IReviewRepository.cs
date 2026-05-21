using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IReviewRepository
{
    Task<IReadOnlyList<ReviewCycle>> ListCyclesAsync(CancellationToken cancellationToken = default);
    Task<ReviewCycle?> FindCycleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PerformanceReview?> FindReviewByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PerformanceReview?> FindReviewByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PerformanceReview>> ListReviewsForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PerformanceReview>> ListReviewsForManagerAsync(Guid managerId, CancellationToken cancellationToken = default);
    Task AddCycleAsync(ReviewCycle cycle, CancellationToken cancellationToken = default);
    Task AddReviewAsync(PerformanceReview review, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
