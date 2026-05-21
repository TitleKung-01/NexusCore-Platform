using NexusCore.Application.Common;
using NexusCore.Application.Employees;

namespace NexusCore.Application.Organization;

public interface IOrganizationService
{
    Task<ServiceResult<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleDefinitionResponse>> ListRolesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<RoleDefinitionResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
}
