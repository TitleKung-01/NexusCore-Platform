using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Employees;

public class EmployeeService(
    ICurrentUserService currentUser,
    IEmployeeProfileRepository profiles,
    IDepartmentRepository departments,
    ILeaveTypeRepository leaveTypes,
    IUserRepository users) : IEmployeeService
{
    public async Task<MeResponse?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return null;

        var profile = await profiles.FindByUserIdAsync(currentUser.UserId.Value, cancellationToken);
        return profile is null ? null : MapMe(profile);
    }

    public async Task<ServiceResult<MeResponse>> UpdateMeAsync(UpdateMeRequest request, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return ServiceResult<MeResponse>.Fail("Unauthorized.", 401);

        var profile = await profiles.FindByUserIdTrackedAsync(currentUser.UserId.Value, cancellationToken);
        if (profile is null)
            return ServiceResult<MeResponse>.Fail("Profile not found.", 404);

        profile.FullName = request.FullName.Trim();
        profile.Email = request.Email.Trim();
        profile.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        await profiles.SaveChangesAsync(cancellationToken);
        var updated = await profiles.FindByUserIdAsync(currentUser.UserId.Value, cancellationToken);
        return ServiceResult<MeResponse>.Ok(MapMe(updated!));
    }

    public async Task<IReadOnlyList<DepartmentResponse>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        var list = await departments.GetAllAsync(cancellationToken);
        return list.Select(d => new DepartmentResponse(d.Id, d.Name, d.Code)).ToList();
    }

    public async Task<IReadOnlyList<LeaveTypeResponse>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
    {
        var list = await leaveTypes.GetAllAsync(cancellationToken);
        return list.Select(l => new LeaveTypeResponse(l.Id, l.Name, l.Code)).ToList();
    }

    public async Task<IReadOnlyList<EmployeeListItem>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return [];

        var list = await profiles.GetAllWithDetailsAsync(cancellationToken);
        return list.Select(MapListItem).ToList();
    }

    public async Task<ServiceResult<EmployeeListItem>> UpdateEmployeeAsync(Guid userId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<EmployeeListItem>.Fail("Forbidden.", 403);

        var profile = await profiles.FindByUserIdTrackedAsync(userId, cancellationToken);
        if (profile is null)
            return ServiceResult<EmployeeListItem>.Fail("Employee not found.", 404);

        if (request.DepartmentId.HasValue)
        {
            var dept = await departments.FindByIdAsync(request.DepartmentId.Value, cancellationToken);
            if (dept is null)
                return ServiceResult<EmployeeListItem>.Fail("Department not found.", 404);
            profile.DepartmentId = request.DepartmentId.Value;
        }

        if (request.ManagerId.HasValue)
        {
            if (request.ManagerId.Value == userId)
                return ServiceResult<EmployeeListItem>.Fail("Employee cannot be their own manager.", 400);
            var mgr = await profiles.FindByUserIdAsync(request.ManagerId.Value, cancellationToken);
            if (mgr is null)
                return ServiceResult<EmployeeListItem>.Fail("Manager not found.", 404);
            profile.ManagerId = request.ManagerId.Value;
        }
        else if (request.ManagerId == Guid.Empty)
            profile.ManagerId = null;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!UserRoles.All.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
                return ServiceResult<EmployeeListItem>.Fail("Invalid role.", 400);
            var user = await users.FindByIdTrackedAsync(userId, cancellationToken);
            if (user is null)
                return ServiceResult<EmployeeListItem>.Fail("User not found.", 404);
            user.Role = request.Role;
        }

        await profiles.SaveChangesAsync(cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        var updated = await profiles.FindByUserIdAsync(userId, cancellationToken);
        return ServiceResult<EmployeeListItem>.Ok(MapListItem(updated!));
    }

    private static MeResponse MapMe(EmployeeProfile p) =>
        new(
            p.UserId,
            p.User.Username,
            p.User.Role,
            p.FullName,
            p.Email,
            p.Phone,
            p.DepartmentId,
            p.Department.Name,
            p.ManagerId,
            p.Manager?.FullName);

    private static EmployeeListItem MapListItem(EmployeeProfile p) =>
        new(
            p.UserId,
            p.User.Username,
            p.User.Role,
            p.FullName,
            p.Email,
            p.Department.Name,
            p.Manager?.FullName);
}
