using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class OnboardingRepository(AppDbContext db) : IOnboardingRepository
{
    public async Task<IReadOnlyList<OnboardingTemplate>> ListTemplatesAsync(CancellationToken cancellationToken = default) =>
        await db.OnboardingTemplates
            .AsNoTracking()
            .Include(t => t.Tasks.OrderBy(x => x.SortOrder))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public Task<OnboardingTemplate?> FindTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.OnboardingTemplates
            .AsNoTracking()
            .Include(t => t.Tasks.OrderBy(x => x.SortOrder))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<EmployeeOnboardingTask>> ListTasksForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await db.EmployeeOnboardingTasks
            .AsNoTracking()
            .Where(t => t.EmployeeId == employeeId)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);

    public Task<EmployeeOnboardingTask?> FindTaskByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.EmployeeOnboardingTasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddTasksAsync(IEnumerable<EmployeeOnboardingTask> tasks, CancellationToken cancellationToken = default) =>
        await db.EmployeeOnboardingTasks.AddRangeAsync(tasks, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
