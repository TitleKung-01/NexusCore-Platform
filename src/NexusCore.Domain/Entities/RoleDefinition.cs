namespace NexusCore.Domain.Entities;

public class RoleDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool CanApprove { get; set; }
    public bool IsHrAccess { get; set; }
    public bool IsBuiltIn { get; set; }
}
