using NexusCore.Application.Common;

namespace NexusCore.Application.Onboarding;

public interface IOnboardingService
{
    Task<IReadOnlyList<OnboardingTemplateResponse>> ListTemplatesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeOnboardingTaskResponse>> ListTasksAsync(Guid? employeeId, CancellationToken cancellationToken = default);
    Task<ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>> AssignTemplateAsync(AssignOnboardingRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<EmployeeOnboardingTaskResponse>> CompleteTaskAsync(Guid taskId, CompleteOnboardingTaskRequest request, CancellationToken cancellationToken = default);
}
