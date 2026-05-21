using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<LeaveRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetCalendarAsync(DateOnly from, DateOnly to, Guid? departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default);
    Task<decimal> SumApprovedDaysAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingAsync(Guid employeeId, DateOnly start, DateOnly end, Guid? excludeId, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveRequest request, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
