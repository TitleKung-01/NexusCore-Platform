namespace NexusCore.Domain.Entities;

/// <summary>
/// แม่แบบงานต้อนรับพนักงานใหม่ — HR กำหนดรายการมาตรฐานแล้วคัดลอกให้แต่ละคน
/// </summary>
public class OnboardingTemplate
{
    /// <summary>รหัสแม่แบบ</summary>
    public Guid Id { get; set; }
    /// <summary>ชื่อแม่แบบ (เช่น พนักงานทั่วไป)</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>รายการงานในแม่แบบนี้</summary>
    public ICollection<OnboardingTemplateTask> Tasks { get; set; } = [];
}

/// <summary>
/// งานหนึ่งรายการในแม่แบบ onboarding — กำหนดลำดับและชื่องาน
/// </summary>
public class OnboardingTemplateTask
{
    /// <summary>รหัสงานในแม่แบบ</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสแม่แบบที่สังกัด</summary>
    public Guid TemplateId { get; set; }
    /// <summary>แม่แบบที่เป็นเจ้าของงานนี้</summary>
    public OnboardingTemplate Template { get; set; } = null!;
    /// <summary>ชื่องานที่ต้องทำ</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>ลำดับการแสดงผล</summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// งาน onboarding ที่มอบให้พนักงานคนหนึ่ง — คัดลอกจากแม่แบบและติดตามว่าทำเสร็จหรือยัง
/// </summary>
public class EmployeeOnboardingTask
{
    /// <summary>รหัสงานของพนักงาน</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานผู้รับผิดชอบ</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์พนักงาน</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>รหัสแม่แบบที่อ้างอิง (ถ้ามี)</summary>
    public Guid TemplateId { get; set; }
    /// <summary>ชื่องาน</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>ทำเสร็จแล้วหรือยัง</summary>
    public bool IsCompleted { get; set; }
    /// <summary>วันเวลาที่ทำเสร็จ (UTC) ถ้ามี</summary>
    public DateTime? CompletedAtUtc { get; set; }
    /// <summary>ลำดับการแสดงผล</summary>
    public int SortOrder { get; set; }
}
