using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึง onboarding — จัดการแม่แบบและงานที่มอบให้พนักงานใหม่
/// </summary>
public interface IOnboardingRepository
{
    /// <summary>รายการแม่แบบ onboarding ทั้งหมด</summary>
    Task<IReadOnlyList<OnboardingTemplate>> ListTemplatesAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาแม่แบบตามรหัส (อ่านอย่างเดียว)</summary>
    Task<OnboardingTemplate?> FindTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาแม่แบบพร้อมติดตามการแก้ไข (รวมงานในแม่แบบ)</summary>
    Task<OnboardingTemplate?> FindTemplateByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มแม่แบบใหม่</summary>
    Task AddTemplateAsync(OnboardingTemplate template, CancellationToken cancellationToken = default);
    /// <summary>ลบงานในแม่แบบออกจาก context ก่อนบันทึก</summary>
    void RemoveTemplateTasks(IEnumerable<OnboardingTemplateTask> tasks);
    /// <summary>รายการงาน onboarding ของพนักงานคนหนึ่ง</summary>
    Task<IReadOnlyList<EmployeeOnboardingTask>> ListTasksForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    /// <summary>ค้นหางานของพนักงานพร้อมติดตามการแก้ไข (ทำเครื่องหมายเสร็จ)</summary>
    Task<EmployeeOnboardingTask?> FindTaskByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มงาน onboarding ให้พนักงานหลายรายการพร้อมกัน</summary>
    Task AddTasksAsync(IEnumerable<EmployeeOnboardingTask> tasks, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
