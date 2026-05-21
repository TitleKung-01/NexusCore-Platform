namespace NexusCore.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public DateTime CreatedAtUtc { get; set; }
    public EmployeeProfile? Profile { get; set; }
}
