namespace NexusCore.Application.Organization;

/// <summary>คำขอสร้างแผนกใหม่</summary>
public record CreateDepartmentRequest(string Name, string Code);

/// <summary>คำขอสร้างนิยามบทบาทใหม่</summary>
public record CreateRoleRequest(
    string Name,
    string? Description,
    bool CanApprove,
    bool IsHrAccess);

/// <summary>นิยามบทบาทในองค์กรพร้อมสิทธิ์</summary>
public record RoleDefinitionResponse(
    Guid Id,
    string Name,
    string? Description,
    bool CanApprove,
    bool IsHrAccess,
    bool IsBuiltIn);
