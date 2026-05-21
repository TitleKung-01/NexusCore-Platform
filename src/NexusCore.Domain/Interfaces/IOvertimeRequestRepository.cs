using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IOvertimeRequestRepository
{
    Task<OvertimeRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OvertimeRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OvertimeRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OvertimeRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OvertimeRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OvertimeRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default);
    Task AddAsync(OvertimeRequest request, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
