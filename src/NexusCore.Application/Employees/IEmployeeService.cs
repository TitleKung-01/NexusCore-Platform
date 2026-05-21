using NexusCore.Application.Common;

namespace NexusCore.Application.Employees;

public interface IEmployeeService
{
    Task<MeResponse?> GetMeAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<MeResponse>> UpdateMeAsync(UpdateMeRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepartmentResponse>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveTypeResponse>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeListItem>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<EmployeeListItem>> UpdateEmployeeAsync(Guid userId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
}
