namespace NexusCore.Domain.Entities;

public class OnboardingTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<OnboardingTemplateTask> Tasks { get; set; } = [];
}

public class OnboardingTemplateTask
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public OnboardingTemplate Template { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class EmployeeOnboardingTask
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public Guid TemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int SortOrder { get; set; }
}
