using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<ExpenseClaim?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExpenseClaim?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseClaim>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseClaim>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExpenseClaim>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ExpenseClaim claim, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
