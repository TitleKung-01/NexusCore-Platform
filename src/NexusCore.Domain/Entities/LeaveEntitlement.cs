namespace NexusCore.Domain.Entities;

public class LeaveEntitlement
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public Guid LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
    public int Year { get; set; }
    public decimal DaysAllowed { get; set; }
}
