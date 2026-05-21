namespace NexusCore.Application.Reviews;

public record ReviewCycleResponse(Guid Id, string Name, string StartDate, string EndDate, bool IsOpen);

public record PerformanceReviewResponse(
    Guid Id,
    Guid CycleId,
    string CycleName,
    Guid EmployeeId,
    string EmployeeName,
    Guid? ManagerId,
    string? SelfComment,
    int? SelfScore,
    string? ManagerComment,
    int? ManagerScore);

public record CreateReviewCycleRequest(string Name, string StartDate, string EndDate);

public record SubmitSelfReviewRequest(string? SelfComment, int? SelfScore);

public record SubmitManagerReviewRequest(string? ManagerComment, int? ManagerScore);
