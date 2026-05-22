using NexusCore.Application.Common;

namespace NexusCore.Application.Expenses;

/// <summary>
/// บริการเบิกค่าใช้จ่าย: สร้าง ส่ง อนุมัติ/ปฏิเสธ และยกเลิก
/// </summary>
public interface IExpenseService
{
    /// <summary>ดึงรายการเบิกตาม scope</summary>
    Task<IReadOnlyList<ExpenseClaimResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);

    /// <summary>ดึงรายละเอียดใบเบิก</summary>
    Task<ExpenseClaimResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>สร้างใบเบิกแบบร่างพร้อมรายการ</summary>
    Task<ServiceResult<ExpenseClaimResponse>> CreateAsync(CreateExpenseClaimRequest request, CancellationToken cancellationToken = default);

    /// <summary>ส่งใบเบิกเพื่อรออนุมัติ</summary>
    Task<ServiceResult<ExpenseClaimResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>อนุมัติใบเบิก</summary>
    Task<ServiceResult<ExpenseClaimResponse>> ApproveAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default);

    /// <summary>ปฏิเสธใบเบิก</summary>
    Task<ServiceResult<ExpenseClaimResponse>> RejectAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default);

    /// <summary>ยกเลิกใบเบิกแบบร่างหรือรออนุมัติ</summary>
    Task<ServiceResult<ExpenseClaimResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
