using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงสลิปเงินเดือน — HR อัปโหลด พนักงานดูรายการของตนเอง
/// </summary>
public interface IPayslipRepository
{
    /// <summary>ค้นหาสลิปตามรหัส</summary>
    Task<Payslip?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>รายการสลิปของพนักงาน (เรียงตามปี/เดือนใน implementation)</summary>
    Task<IReadOnlyList<Payslip>> ListForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มสลิปใหม่หลังอัปโหลดไฟล์</summary>
    Task AddAsync(Payslip payslip, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
