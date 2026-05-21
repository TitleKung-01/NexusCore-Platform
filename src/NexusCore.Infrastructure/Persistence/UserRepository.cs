using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

    public Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(u => u.Username == username, cancellationToken);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> GetAllOrderedAsync(CancellationToken cancellationToken = default) =>
        await db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
