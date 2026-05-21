namespace NexusCore.Domain.Entities;

public class ReviewCycle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsOpen { get; set; }
}

public class PerformanceReview
{
    public Guid Id { get; set; }
    public Guid CycleId { get; set; }
    public ReviewCycle Cycle { get; set; } = null!;
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public string? SelfComment { get; set; }
    public int? SelfScore { get; set; }
    public string? ManagerComment { get; set; }
    public int? ManagerScore { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
