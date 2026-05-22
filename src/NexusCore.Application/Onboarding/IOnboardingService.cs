using NexusCore.Application.Common;

namespace NexusCore.Application.Onboarding;

/// <summary>
/// บริการ onboarding: เทมเพลต มอบหมายงาน และติดตามความคืบหน้า
/// </summary>
public interface IOnboardingService
{
    /// <summary>รายการเทมเพลต onboarding (HR)</summary>
    Task<IReadOnlyList<OnboardingTemplateResponse>> ListTemplatesAsync(CancellationToken cancellationToken = default);

    /// <summary>สร้างเทมเพลตใหม่</summary>
    Task<ServiceResult<OnboardingTemplateResponse>> CreateTemplateAsync(SaveOnboardingTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>แก้ไขเทมเพลตและรายการงาน</summary>
    Task<ServiceResult<OnboardingTemplateResponse>> UpdateTemplateAsync(Guid id, SaveOnboardingTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>รายการงาน onboarding ของพนักงาน</summary>
    Task<IReadOnlyList<EmployeeOnboardingTaskResponse>> ListTasksAsync(Guid? employeeId, CancellationToken cancellationToken = default);

    /// <summary>มอบหมายเทมเพลตให้พนักงาน</summary>
    Task<ServiceResult<IReadOnlyList<EmployeeOnboardingTaskResponse>>> AssignTemplateAsync(AssignOnboardingRequest request, CancellationToken cancellationToken = default);

    /// <summary>อัปเดตสถานะงาน onboarding ของพนักงาน</summary>
    Task<ServiceResult<EmployeeOnboardingTaskResponse>> CompleteTaskAsync(Guid taskId, CompleteOnboardingTaskRequest request, CancellationToken cancellationToken = default);
}
