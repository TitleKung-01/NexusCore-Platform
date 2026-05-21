namespace NexusCore.Application.Expenses;

public record ExpenseLineItemDto(Guid Id, string Description, decimal Amount);

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

public record CreateExpenseClaimRequest(string Title, IReadOnlyList<CreateExpenseLineItemRequest> LineItems);

public record CreateExpenseLineItemRequest(string Description, decimal Amount);

public record DecideExpenseRequest(string? Comment);
