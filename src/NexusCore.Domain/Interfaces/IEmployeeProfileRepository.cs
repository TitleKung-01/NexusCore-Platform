using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IEmployeeProfileRepository
{
    Task<EmployeeProfile?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<EmployeeProfile?> FindByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeProfile>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(EmployeeProfile profile, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
