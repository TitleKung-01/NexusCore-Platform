namespace NexusCore.Domain.Entities;

/// <summary>
/// หน่วยงาน/แผนกในองค์กร — ใช้จัดกลุ่มพนักงาน ปฏิทินลา และการโอนย้าย
/// </summary>
public class Department
{
    /// <summary>รหัสแผนก</summary>
    public Guid Id { get; set; }
    /// <summary>ชื่อแผนก</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>รหัสย่อแผนก (ไม่ซ้ำ)</summary>
    public string Code { get; set; } = string.Empty;
}
