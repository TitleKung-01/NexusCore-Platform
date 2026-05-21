using NexusCore.Domain.Enums;

namespace NexusCore.Domain.Entities;

public class OvertimeRequest
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public DateOnly WorkDate { get; set; }
    public decimal Hours { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public Guid? DecidedById { get; set; }
    public string? ManagerComment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
