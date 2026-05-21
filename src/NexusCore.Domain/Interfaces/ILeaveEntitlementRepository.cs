using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface ILeaveEntitlementRepository
{
    Task<LeaveEntitlement?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveEntitlement>> ListForEmployeeAsync(Guid employeeId, int? year, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveEntitlement entitlement, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
