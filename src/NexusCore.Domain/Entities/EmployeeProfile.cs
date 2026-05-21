namespace NexusCore.Domain.Entities;

public class EmployeeProfile
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public Guid? ManagerId { get; set; }
    public EmployeeProfile? Manager { get; set; }
}
