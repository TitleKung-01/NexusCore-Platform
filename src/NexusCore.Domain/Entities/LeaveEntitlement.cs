namespace NexusCore.Domain.Entities;

/// <summary>
/// โควตาวันลาที่กำหนดให้พนักงานต่อประเภทลาและปี — ใช้ตรวจสอบก่อนอนุมัติคำขอลา
/// </summary>
public class LeaveEntitlement
{
    /// <summary>รหัสโควตา</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานที่ได้รับโควตา</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์พนักงาน</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>รหัสประเภทลา</summary>
    public Guid LeaveTypeId { get; set; }
    /// <summary>ประเภทลา (เช่น ลาป่วย ลาพักร้อน)</summary>
    public LeaveType LeaveType { get; set; } = null!;
    /// <summary>ปีที่โควตานี้มีผล (ค.ศ.)</summary>
    public int Year { get; set; }
    /// <summary>จำนวนวันลาที่อนุญาตในปีนั้น</summary>
    public decimal DaysAllowed { get; set; }
}
