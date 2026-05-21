using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    Task<User?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Remove(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
