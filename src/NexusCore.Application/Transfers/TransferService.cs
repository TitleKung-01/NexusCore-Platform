using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Transfers;

public class TransferService(
    ICurrentUserService currentUser,
    IEmployeeTransferRepository transfers,
    IEmployeeProfileRepository profiles) : ITransferService
{
    public async Task<IReadOnlyList<EmployeeTransferResponse>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default)
    {
        var target = employeeId ?? currentUser.UserId;
        if (target is null)
            return [];

        if (target != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin, UserRoles.Manager))
            return [];

        if (target != currentUser.UserId && currentUser.IsInRole(UserRoles.Manager))
        {
            var profile = await profiles.FindByUserIdAsync(target.Value, cancellationToken);
            if (profile?.ManagerId != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
                return [];
        }

        var list = await transfers.ListAsync(target, cancellationToken);
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
