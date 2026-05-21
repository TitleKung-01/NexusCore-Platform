using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Onboarding;

public class OnboardingService(
    ICurrentUserService currentUser,
    IOnboardingRepository onboarding,
    IEmployeeProfileRepository profiles) : IOnboardingService
{
    public async Task<IReadOnlyList<OnboardingTemplateResponse>> ListTemplatesAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return [];

        var templates = await onboarding.ListTemplatesAsync(cancellationToken);
        return templates.Select(t => new OnboardingTemplateResponse(
            t.Id,
            t.Name,
            t.Tasks.Select(x => new OnboardingTemplateTaskResponse(x.Id, x.Title, x.SortOrder)).ToList())).ToList();
    }

    public async Task<IReadOnlyList<EmployeeOnboardingTaskResponse>> ListTasksAsync(Guid? employeeId, CancellationToken cancellationToken = default)
    {
        var target = employeeId ?? currentUser.UserId;
        if (target is null)
            return [];

        if (target != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return [];

        var tasks = await onboarding.ListTasksForEmployeeAsync(target.Value, cancellationToken);
        return tasks.Select(MapTask).ToList();
    }

    public async Task<ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>> AssignTemplateAsync(AssignOnboardingRequest request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>.Fail("Forbidden.", 403);

        var template = await onboarding.FindTemplateByIdAsync(request.TemplateId, cancellationToken);
        if (template is null)
            return ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>.Fail("Template not found.", 404);

        var employee = await profiles.FindByUserIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>.Fail("Employee not found.", 404);

        var tasks = template.Tasks.Select(t => new EmployeeOnboardingTask
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            TemplateId = template.Id,
            Title = t.Title,
            SortOrder = t.SortOrder,
            IsCompleted = false
        }).ToList();

        await onboarding.AddTasksAsync(tasks, cancellationToken);
        await onboarding.SaveChangesAsync(cancellationToken);
        return ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>.Ok(tasks.Select(MapTask).ToList());
    }

    public async Task<ServiceResult<EmployeeOnboardingTaskResponse>> CompleteTaskAsync(Guid taskId, CompleteOnboardingTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await onboarding.FindTaskByIdTrackedAsync(taskId, cancellationToken);
        if (task is null)
            return ServiceResult<EmployeeOnboardingTaskResponse>.Fail("Task not found.", 404);

        if (task.EmployeeId != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<EmployeeOnboardingTaskResponse>.Fail("Forbidden.", 403);

        task.IsCompleted = request.IsCompleted;
        task.CompletedAtUtc = request.IsCompleted ? DateTime.UtcNow : null;
        await onboarding.SaveChangesAsync(cancellationToken);
        return ServiceResult<EmployeeOnboardingTaskResponse>.Ok(MapTask(task));
    }

    private static EmployeeOnboardingTaskResponse MapTask(EmployeeOnboardingTask t) =>
        new(t.Id, t.EmployeeId, t.TemplateId, t.Title, t.IsCompleted, t.CompletedAtUtc?.ToString("o"), t.SortOrder);
}
