using NexusCore.Application.Common;
using NexusCore.Application.Employees;

namespace NexusCore.Application.Organization;

/// <summary>
/// บริการโครงสร้างองค์กร: สร้างแผนกและนิยามบทบาท (HR)
/// </summary>
public interface IOrganizationService
{
    /// <summary>สร้างแผนกใหม่</summary>
    Task<ServiceResult<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);

    /// <summary>รายการบทบาททั้งหมดในระบบ</summary>
    Task<IReadOnlyList<RoleDefinitionResponse>> ListRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>สร้างนิยามบทบาทใหม่</summary>
    Task<ServiceResult<RoleDefinitionResponse>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
}
