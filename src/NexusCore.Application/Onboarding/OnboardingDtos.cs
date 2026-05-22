namespace NexusCore.Application.Onboarding;

/// <summary>เทมเพลตงาน onboarding พร้อมรายการงาน</summary>
public record OnboardingTemplateResponse(Guid Id, string Name, IReadOnlyList<OnboardingTemplateTaskResponse> Tasks);

/// <summary>งานในเทมเพลต onboarding</summary>
public record OnboardingTemplateTaskResponse(Guid Id, string Title, int SortOrder);

/// <summary>งาน onboarding ที่มอบหมายให้พนักงาน</summary>
public record EmployeeOnboardingTaskResponse(
    Guid Id,
    Guid EmployeeId,
    Guid TemplateId,
    string Title,
    bool IsCompleted,
    string? CompletedAtUtc,
    int SortOrder);

/// <summary>คำขอมอบหมายเทมเพลต onboarding ให้พนักงาน</summary>
public record AssignOnboardingRequest(Guid TemplateId, Guid EmployeeId);

/// <summary>คำขอทำเครื่องหมายงาน onboarding ว่าเสร็จหรือยังไม่เสร็จ</summary>
public record CompleteOnboardingTaskRequest(bool IsCompleted);

/// <summary>งานในเทมเพลตเมื่อบันทึก/แก้ไขเทมเพลต</summary>
public record UpsertOnboardingTemplateTaskRequest(Guid? Id, string Title, int SortOrder);

/// <summary>คำขอบันทึกเทมเพลต onboarding และรายการงาน</summary>
public record SaveOnboardingTemplateRequest(string Name, IReadOnlyList<UpsertOnboardingTemplateTaskRequest> Tasks);
