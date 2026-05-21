using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Reviews;

public class ReviewService(
    ICurrentUserService currentUser,
    IReviewRepository reviews,
    IEmployeeProfileRepository profiles) : IReviewService
{
    public async Task<IReadOnlyList<ReviewCycleResponse>> ListCyclesAsync(CancellationToken cancellationToken = default)
    {
        var cycles = await reviews.ListCyclesAsync(cancellationToken);
        return cycles.Select(c => new ReviewCycleResponse(c.Id, c.Name, c.StartDate.ToString("yyyy-MM-dd"), c.EndDate.ToString("yyyy-MM-dd"), c.IsOpen)).ToList();
    }

    public async Task<ServiceResult<ReviewCycleResponse>> CreateCycleAsync(CreateReviewCycleRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<ReviewCycleResponse>.Fail("Forbidden.", 403);

        if (!DateOnly.TryParse(request.StartDate, out var start) || !DateOnly.TryParse(request.EndDate, out var end))
            return ServiceResult<ReviewCycleResponse>.Fail("Invalid date format. Use yyyy-MM-dd.", 400);

        var cycle = new ReviewCycle
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            StartDate = start,
            EndDate = end,
            IsOpen = true
        };

        await reviews.AddCycleAsync(cycle, cancellationToken);

        var employees = await profiles.GetAllWithDetailsAsync(cancellationToken);
        foreach (var emp in employees)
        {
            await reviews.AddReviewAsync(new PerformanceReview
            {
                Id = Guid.NewGuid(),
                CycleId = cycle.Id,
                EmployeeId = emp.UserId,
                ManagerId = emp.ManagerId,
                CreatedAtUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        await reviews.SaveChangesAsync(cancellationToken);
        return ServiceResult<ReviewCycleResponse>.Ok(new ReviewCycleResponse(cycle.Id, cycle.Name, cycle.StartDate.ToString("yyyy-MM-dd"), cycle.EndDate.ToString("yyyy-MM-dd"), cycle.IsOpen));
    }

    public async Task<IReadOnlyList<PerformanceReviewResponse>> ListReviewsAsync(string scope, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        IReadOnlyList<PerformanceReview> list;
        if (string.Equals(scope, "team", StringComparison.OrdinalIgnoreCase) &&
            currentUser.IsInAnyRole(UserRoles.Manager, UserRoles.Hr, UserRoles.Admin))
            list = await reviews.ListReviewsForManagerAsync(currentUser.UserId.Value, cancellationToken);
        else
            list = await reviews.ListReviewsForEmployeeAsync(currentUser.UserId.Value, cancellationToken);

        return list.Select(Map).ToList();
    }

    public async Task<ServiceResult<PerformanceReviewResponse>> SubmitSelfAsync(Guid id, SubmitSelfReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await reviews.FindReviewByIdTrackedAsync(id, cancellationToken);
        if (review is null)
            return ServiceResult<PerformanceReviewResponse>.Fail("Review not found.", 404);
        if (review.EmployeeId != currentUser.UserId)
            return ServiceResult<PerformanceReviewResponse>.Fail("Forbidden.", 403);

        review.SelfComment = request.SelfComment?.Trim();
        review.SelfScore = request.SelfScore;
        await reviews.SaveChangesAsync(cancellationToken);
        var updated = await reviews.FindReviewByIdAsync(id, cancellationToken);
        return ServiceResult<PerformanceReviewResponse>.Ok(Map(updated!));
    }

    public async Task<ServiceResult<PerformanceReviewResponse>> SubmitManagerAsync(Guid id, SubmitManagerReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await reviews.FindReviewByIdTrackedAsync(id, cancellationToken);
        if (review is null)
            return ServiceResult<PerformanceReviewResponse>.Fail("Review not found.", 404);

        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin) &&
            !(currentUser.IsInRole(UserRoles.Manager) && review.Employee.ManagerId == currentUser.UserId))
            return ServiceResult<PerformanceReviewResponse>.Fail("Forbidden.", 403);

        review.ManagerComment = request.ManagerComment?.Trim();
        review.ManagerScore = request.ManagerScore;
        await reviews.SaveChangesAsync(cancellationToken);
        var updated = await reviews.FindReviewByIdAsync(id, cancellationToken);
        return ServiceResult<PerformanceReviewResponse>.Ok(Map(updated!));
    }

    private static PerformanceReviewResponse Map(PerformanceReview r) =>
        new(r.Id, r.CycleId, r.Cycle.Name, r.EmployeeId, r.Employee.FullName, r.ManagerId,
            r.SelfComment, r.SelfScore, r.ManagerComment, r.ManagerScore);
}
