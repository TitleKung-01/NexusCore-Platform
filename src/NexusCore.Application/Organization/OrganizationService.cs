using NexusCore.Application.Common;
using NexusCore.Application.Employees;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Organization;

/// <summary>
/// สร้างแผนกและนิยามบทบาทองค์กร
/// </summary>
public class OrganizationService(
    ICurrentUserService currentUser,
    IDepartmentRepository departments,
    IRoleDefinitionRepository roles) : IOrganizationService
{
    /// <inheritdoc />
    public async Task<ServiceResult<DepartmentResponse>> CreateDepartmentAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<DepartmentResponse>.Fail("Forbidden.", 403);

        var name = request.Name.Trim();
        var code = request.Code.Trim().ToUpperInvariant();
        if (name.Length == 0 || code.Length == 0)
            return ServiceResult<DepartmentResponse>.Fail("Name and code are required.", 400);
        if (code.Length > 16)
            return ServiceResult<DepartmentResponse>.Fail("Code must be at most 16 characters.", 400);

        if (await departments.FindByCodeAsync(code, cancellationToken) is not null)
            return ServiceResult<DepartmentResponse>.Fail("Department code already exists.", 409);

        var dept = new Department
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code
        };

        await departments.AddAsync(dept, cancellationToken);
        await departments.SaveChangesAsync(cancellationToken);
        return ServiceResult<DepartmentResponse>.Ok(new DepartmentResponse(dept.Id, dept.Name, dept.Code));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoleDefinitionResponse>> ListRolesAsync(CancellationToken cancellationToken = default)
    {
        var list = await roles.GetAllAsync(cancellationToken);
        return list.Select(MapRole).ToList();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<RoleDefinitionResponse>> CreateRoleAsync(
        CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<RoleDefinitionResponse>.Fail("Forbidden.", 403);

        var name = request.Name.Trim();
        if (name.Length == 0)
            return ServiceResult<RoleDefinitionResponse>.Fail("Role name is required.", 400);
        if (name.Length > 64)
            return ServiceResult<RoleDefinitionResponse>.Fail("Role name must be at most 64 characters.", 400);

        if (await roles.FindByNameAsync(name, cancellationToken) is not null)
            return ServiceResult<RoleDefinitionResponse>.Fail("Role name already exists.", 409);

        var role = new RoleDefinition
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            CanApprove = request.CanApprove,
            IsHrAccess = request.IsHrAccess,
            IsBuiltIn = false
        };

        await roles.AddAsync(role, cancellationToken);
        await roles.SaveChangesAsync(cancellationToken);
        return ServiceResult<RoleDefinitionResponse>.Ok(MapRole(role));
    }

    private static RoleDefinitionResponse MapRole(RoleDefinition r) =>
        new(r.Id, r.Name, r.Description, r.CanApprove, r.IsHrAccess, r.IsBuiltIn);
}