namespace NexusCore.Domain.Entities;

public class EmployeeTransfer
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public Guid FromDepartmentId { get; set; }
    public Department FromDepartment { get; set; } = null!;
    public Guid ToDepartmentId { get; set; }
    public Department ToDepartment { get; set; } = null!;
    public DateOnly EffectiveDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
