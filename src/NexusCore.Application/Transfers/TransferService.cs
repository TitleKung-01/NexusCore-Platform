using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Transfers;

public class TransferService(
    ICurrentUserService currentUser,
    IEmployeeTransferRepository transfers,
    IEmployeeProfileRepository profiles) : ITransferService
{
    public async Task<IReadOnlyList<EmployeeTransferResponse>> ListAsync(
        Guid? employeeId,
        int? limit,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        Guid? filterEmployeeId;

        if (employeeId.HasValue)
        {
            if (employeeId != currentUser.UserId &&
                !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
                return [];

            if (employeeId != currentUser.UserId && currentUser.IsInRole(UserRoles.Manager))
            {
                var profile = await profiles.FindByUserIdAsync(employeeId.Value, cancellationToken);
                if (profile?.ManagerId != currentUser.UserId &&
                    !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                    return [];
            }

            filterEmployeeId = employeeId;
        }
        else if (currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            filterEmployeeId = null;
        else
            filterEmployeeId = currentUser.UserId;

        var list = await transfers.ListAsync(filterEmployeeId, limit, cancellationToken);
        return list.Select(t => new EmployeeTransferResponse(
            t.Id,
            t.EmployeeId,
            t.Employee.FullName,
            t.FromDepartment.Name,
            t.ToDepartment.Name,
            t.EffectiveDate.ToString("yyyy-MM-dd"),
            t.Note,
            t.CreatedAtUtc.ToString("o"))).ToList();
    }
}
