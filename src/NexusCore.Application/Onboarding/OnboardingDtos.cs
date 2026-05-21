namespace NexusCore.Application.Onboarding;

public record OnboardingTemplateResponse(Guid Id, string Name, IReadOnlyList<OnboardingTemplateTaskResponse> Tasks);

public record OnboardingTemplateTaskResponse(Guid Id, string Title, int SortOrder);

public record EmployeeOnboardingTaskResponse(
    Guid Id,
    Guid EmployeeId,
    Guid TemplateId,
    string Title,
    bool IsCompleted,
    string? CompletedAtUtc,
    int SortOrder);

public record AssignOnboardingRequest(Guid TemplateId, Guid EmployeeId);

public record CompleteOnboardingTaskRequest(bool IsCompleted);
