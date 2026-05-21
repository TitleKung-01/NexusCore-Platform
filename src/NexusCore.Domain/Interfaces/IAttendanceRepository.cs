using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IAttendanceRepository
{
    Task<AttendanceRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> FindByEmployeeAndDateAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default);
    Task<AttendanceRecord?> FindByEmployeeAndDateTrackedAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceRecord>> ListAsync(Guid? employeeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
