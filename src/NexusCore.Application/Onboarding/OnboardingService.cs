using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Onboarding;

/// <summary>
/// เทมเพลตและงาน onboarding ของพนักงานใหม่
/// </summary>
public class OnboardingService(
    ICurrentUserService currentUser,
    IOnboardingRepository onboarding,
    IEmployeeProfileRepository profiles) : IOnboardingService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<OnboardingTemplateResponse>> ListTemplatesAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return [];

        var templates = await onboarding.ListTemplatesAsync(cancellationToken);
        return templates.Select(MapTemplate).ToList();
    }

    /// <inheritdoc />
    public async Task<ServiceResult<OnboardingTemplateResponse>> CreateTemplateAsync(
        SaveOnboardingTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<OnboardingTemplateResponse>.Fail("Forbidden.", 403);

        var normalized = NormalizeTemplateTasks(request.Tasks);
        if (string.IsNullOrWhiteSpace(request.Name))
            return ServiceResult<OnboardingTemplateResponse>.Fail("Template name is required.", 400);
        if (normalized.Count == 0)
            return ServiceResult<OnboardingTemplateResponse>.Fail("At least one task is required.", 400);

        var templateId = Guid.NewGuid();
        var template = new OnboardingTemplate
        {
            Id = templateId,
            Name = request.Name.Trim(),
            Tasks = normalized.Select((t, index) => new OnboardingTemplateTask
            {
                Id = Guid.NewGuid(),
                TemplateId = templateId,
                Title = t.Title,
                SortOrder = index + 1
            }).ToList()
        };

        await onboarding.AddTemplateAsync(template, cancellationToken);
        await onboarding.SaveChangesAsync(cancellationToken);
        return ServiceResult<OnboardingTemplateResponse>.Ok(MapTemplate(template));
    }

    /// <inheritdoc />
    public async Task<ServiceResult<OnboardingTemplateResponse>> UpdateTemplateAsync(
        Guid id,
        SaveOnboardingTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<OnboardingTemplateResponse>.Fail("Forbidden.", 403);

        var normalized = NormalizeTemplateTasks(request.Tasks);
        if (string.IsNullOrWhiteSpace(request.Name))
            return ServiceResult<OnboardingTemplateResponse>.Fail("Template name is required.", 400);
        if (normalized.Count == 0)
            return ServiceResult<OnboardingTemplateResponse>.Fail("At least one task is required.", 400);

        var template = await onboarding.FindTemplateByIdTrackedAsync(id, cancellationToken);
        if (template is null)
            return ServiceResult<OnboardingTemplateResponse>.Fail("Template not found.", 404);

        template.Name = request.Name.Trim();

        var incomingIds = normalized.Where(t => t.Id.HasValue).Select(t => t.Id!.Value).ToHashSet();
        var toRemove = template.Tasks.Where(t => !incomingIds.Contains(t.Id)).ToList();
        if (toRemove.Count > 0)
            onboarding.RemoveTemplateTasks(toRemove);

        for (var i = 0; i < normalized.Count; i++)
        {
            var item = normalized[i];
            var sortOrder = i + 1;
            if (item.Id is Guid taskId)
            {
                var existing = template.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (existing is not null)
                {
                    existing.Title = item.Title;
                    existing.SortOrder = sortOrder;
                }
                else
                {
                    template.Tasks.Add(new OnboardingTemplateTask
                    {
                        Id = Guid.NewGuid(),
                        TemplateId = template.Id,
                        Title = item.Title,
                        SortOrder = sortOrder
                    });
                }
            }
            else
            {
                template.Tasks.Add(new OnboardingTemplateTask
                {
                    Id = Guid.NewGuid(),
                    TemplateId = template.Id,
                    Title = item.Title,
                    SortOrder = sortOrder
                });
            }
        }

        await onboarding.SaveChangesAsync(cancellationToken);
        var updated = await onboarding.FindTemplateByIdAsync(id, cancellationToken);
        return ServiceResult<OnboardingTemplateResponse>.Ok(MapTemplate(updated!));
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    private static OnboardingTemplateResponse MapTemplate(OnboardingTemplate t) =>
        new(
            t.Id,
            t.Name,
            t.Tasks
                .OrderBy(x => x.SortOrder)
                .Select(x => new OnboardingTemplateTaskResponse(x.Id, x.Title, x.SortOrder))
                .ToList());

    private static List<(Guid? Id, string Title)> NormalizeTemplateTasks(
        IReadOnlyList<UpsertOnboardingTemplateTaskRequest> tasks) =>
        tasks
            .Select(t => (t.Id, Title: t.Title.Trim()))
            .Where(t => !string.IsNullOrWhiteSpace(t.Title))
            .ToList();
}