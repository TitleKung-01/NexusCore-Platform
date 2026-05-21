using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IEmployeeTransferRepository
{
    Task<IReadOnlyList<EmployeeTransfer>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default);
    Task AddAsync(EmployeeTransfer transfer, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
