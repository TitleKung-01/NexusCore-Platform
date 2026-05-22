namespace NexusCore.Application.Expenses;

/// <summary>รายการค่าใช้จ่ายในคำขอเบิก</summary>
public record ExpenseLineItemDto(Guid Id, string Description, decimal Amount);

/// <summary>คำขอเบิกค่าใช้จ่ายพร้อมรายการและสถานะอนุมัติ</summary>
public record ExpenseClaimResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeUsername,
    string EmployeeEmail,
    string EmployeeDepartment,
    string EmployeeRole,
    string? EmployeeManagerName,
    string Title,
    decimal TotalAmount,
    string Status,
    string? SubmittedAtUtc,
    string? DecidedAtUtc,
    string? ManagerComment,
    IReadOnlyList<ExpenseLineItemDto> LineItems);

/// <summary>คำขอสร้างใบเบิกแบบร่างพร้อมรายการ</summary>
public record CreateExpenseClaimRequest(string Title, IReadOnlyList<CreateExpenseLineItemRequest> LineItems);

/// <summary>รายการค่าใช้จ่ายในคำขอสร้างใหม่</summary>
public record CreateExpenseLineItemRequest(string Description, decimal Amount);

/// <summary>ความเห็นผู้อนุมัติเมื่ออนุมัติ/ปฏิเสธเบิกจ่าย</summary>
public record DecideExpenseRequest(string? Comment);
