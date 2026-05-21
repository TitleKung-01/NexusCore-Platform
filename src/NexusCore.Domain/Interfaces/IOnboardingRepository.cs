using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IOnboardingRepository
{
    Task<IReadOnlyList<OnboardingTemplate>> ListTemplatesAsync(CancellationToken cancellationToken = default);
    Task<OnboardingTemplate?> FindTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OnboardingTemplate?> FindTemplateByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddTemplateAsync(OnboardingTemplate template, CancellationToken cancellationToken = default);
    void RemoveTemplateTasks(IEnumerable<OnboardingTemplateTask> tasks);
    Task<IReadOnlyList<EmployeeOnboardingTask>> ListTasksForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task<EmployeeOnboardingTask?> FindTaskByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddTasksAsync(IEnumerable<EmployeeOnboardingTask> tasks, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
