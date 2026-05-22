namespace NexusCore.Domain.Entities;

/// <summary>
/// ประเภทการลาในองค์กร — ใช้จัดกลุ่มคำขอและโควตาวันลา
/// </summary>
public class LeaveType
{
    /// <summary>รหัสประเภทลา</summary>
    public Guid Id { get; set; }
    /// <summary>ชื่อแสดง (เช่น ลาป่วย)</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>รหัสย่อสำหรับอ้างอิงในระบบและรายงาน</summary>
    public string Code { get; set; } = string.Empty;
}
