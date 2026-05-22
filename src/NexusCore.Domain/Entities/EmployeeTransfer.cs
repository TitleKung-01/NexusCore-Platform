namespace NexusCore.Domain.Entities;

/// <summary>
/// ประวัติการโอนย้ายแผนกของพนักงาน — เก็บแผนกเดิม/ใหม่และวันมีผลเพื่อ audit
/// </summary>
public class EmployeeTransfer
{
    /// <summary>รหัสรายการโอน</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานที่ถูกโอน</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์พนักงาน</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>รหัสแผนกก่อนโอน</summary>
    public Guid FromDepartmentId { get; set; }
    /// <summary>แผนกก่อนโอน</summary>
    public Department FromDepartment { get; set; } = null!;
    /// <summary>รหัสแผนกหลังโอน</summary>
    public Guid ToDepartmentId { get; set; }
    /// <summary>แผนกหลังโอน</summary>
    public Department ToDepartment { get; set; } = null!;
    /// <summary>วันที่โอนมีผล</summary>
    public DateOnly EffectiveDate { get; set; }
    /// <summary>หมายเหตุเพิ่มเติม (ไม่บังคับ)</summary>
    public string? Note { get; set; }
    /// <summary>วันเวลาบันทึกรายการ (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
}
