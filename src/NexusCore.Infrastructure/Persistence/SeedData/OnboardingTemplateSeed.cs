using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;

namespace NexusCore.Infrastructure.Persistence.SeedData;

/// <summary>Demo onboarding templates for HR assign dropdown.</summary>
internal static class OnboardingTemplateSeed
{
    private sealed record TemplateDef(string Name, string[] Tasks);

    private static readonly TemplateDef[] Templates =
    [
        new(
            "พนักงานใหม่ (ทั่วไป)",
            [
                "ลงทะเบียนอีเมลและระบบ HR-Lite",
                "อ่านนโยบายบริษัทและ PDPA",
                "กรอกข้อมูลโปรไฟล์ให้ครบ",
                "พบหัวหน้าและทีมงาน",
                "รับอุปกรณ์การทำงาน (คอม/บัตรพนักงาน)",
            ]),
        new(
            "แผนกวิศวกร",
            [
                "ตั้งค่า Git และ repo ของทีม",
                "เข้าร่วม Slack/Teams ช่อง Engineering",
                "อ่านมาตรฐาน code review",
                "ตั้งค่าเครื่อง dev (SDK, VPN)",
                "คู่กับ buddy สัปดาห์แรก",
            ]),
        new(
            "แผนกขาย",
            [
                "อบรมผลิตภัณฑ์และราคา",
                "เข้าระบบ CRM",
                "ฝึก pitch และ role-play",
                "พบลูกค้าตัวอย่างกับหัวหน้า",
                "ตั้งเป้ายอดขายไตรมาสแรก",
            ]),
        new(
            "แผนก HR",
            [
                "ทำความเข้าใจ workflow ลา/OT",
                "ฝึกใช้หน้ารายงานและ CSV",
                "ทบทวนกฎหมายแรงงานพื้นฐาน",
                "พบทีม payroll",
            ]),
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.OnboardingTemplates.AnyAsync())
            return;

        foreach (var def in Templates)
        {
            var templateId = Guid.NewGuid();
            db.OnboardingTemplates.Add(new OnboardingTemplate
            {
                Id = templateId,
                Name = def.Name
            });

            for (var i = 0; i < def.Tasks.Length; i++)
            {
                db.OnboardingTemplateTasks.Add(new OnboardingTemplateTask
                {
                    Id = Guid.NewGuid(),
                    TemplateId = templateId,
                    Title = def.Tasks[i],
                    SortOrder = i + 1
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
