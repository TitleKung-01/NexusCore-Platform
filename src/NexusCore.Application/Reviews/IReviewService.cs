using NexusCore.Application.Common;

namespace NexusCore.Application.Reviews;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewCycleResponse>> ListCyclesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<ReviewCycleResponse>> CreateCycleAsync(CreateReviewCycleRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PerformanceReviewResponse>> ListReviewsAsync(string scope, CancellationToken cancellationToken = default);
    Task<ServiceResult<PerformanceReviewResponse>> SubmitSelfAsync(Guid id, SubmitSelfReviewRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<PerformanceReviewResponse>> SubmitManagerAsync(Guid id, SubmitManagerReviewRequest request, CancellationToken cancellationToken = default);
}
