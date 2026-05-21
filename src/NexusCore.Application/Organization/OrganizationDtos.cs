namespace NexusCore.Application.Organization;

public record CreateDepartmentRequest(string Name, string Code);

public record CreateRoleRequest(
    string Name,
    string? Description,
    bool CanApprove,
    bool IsHrAccess);

public record RoleDefinitionResponse(
    Guid Id,
    string Name,
    string? Description,
    bool CanApprove,
    bool IsHrAccess,
    bool IsBuiltIn);
