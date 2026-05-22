using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงเทมเพลตและงาน onboarding (OnboardingTemplates, EmployeeOnboardingTasks)</summary>
public class OnboardingRepository(AppDbContext db) : IOnboardingRepository
{
    /// <summary>ดึงเทมเพลต onboarding ทั้งหมดพร้อมรายการงาน</summary>
    public async Task<IReadOnlyList<OnboardingTemplate>> ListTemplatesAsync(CancellationToken cancellationToken = default) =>
        await db.OnboardingTemplates
            .AsNoTracking()
            .Include(t => t.Tasks.OrderBy(x => x.SortOrder))
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    /// <summary>ค้นหาเทมเพลตจากรหัส (อ่านอย่างเดียว)</summary>
    public Task<OnboardingTemplate?> FindTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.OnboardingTemplates
            .AsNoTracking()
            .Include(t => t.Tasks.OrderBy(x => x.SortOrder))
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    /// <summary>ค้นหาเทมเพลตจากรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<OnboardingTemplate?> FindTemplateByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.OnboardingTemplates
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    /// <summary>เพิ่มเทมเพลต onboarding ใหม่</summary>
    public async Task AddTemplateAsync(OnboardingTemplate template, CancellationToken cancellationToken = default) =>
        await db.OnboardingTemplates.AddAsync(template, cancellationToken);

    /// <summary>ลบงานในเทมเพลต</summary>
    public void RemoveTemplateTasks(IEnumerable<OnboardingTemplateTask> tasks) =>
        db.OnboardingTemplateTasks.RemoveRange(tasks);

    /// <summary>ดึงงาน onboarding ที่มอบหมายให้พนักงาน</summary>
    public async Task<IReadOnlyList<EmployeeOnboardingTask>> ListTasksForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default) =>
        await db.EmployeeOnboardingTasks
            .AsNoTracking()
            .Where(t => t.EmployeeId == employeeId)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(cancellationToken);

    /// <summary>ค้นหางาน onboarding ของพนักงาน (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<EmployeeOnboardingTask?> FindTaskByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.EmployeeOnboardingTasks.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    /// <summary>เพิ่มงาน onboarding ให้พนักงาน</summary>
    public async Task AddTasksAsync(IEnumerable<EmployeeOnboardingTask> tasks, CancellationToken cancellationToken = default) =>
        await db.EmployeeOnboardingTasks.AddRangeAsync(tasks, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
