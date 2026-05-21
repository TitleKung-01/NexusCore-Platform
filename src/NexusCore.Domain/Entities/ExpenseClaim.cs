using NexusCore.Domain.Enums;

namespace NexusCore.Domain.Entities;

public class ExpenseClaim
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
    public Guid? DecidedById { get; set; }
    public string? ManagerComment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public ICollection<ExpenseLineItem> LineItems { get; set; } = [];
}

public class ExpenseLineItem
{
    public Guid Id { get; set; }
    public Guid ExpenseClaimId { get; set; }
    public ExpenseClaim ExpenseClaim { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class ExpenseAttachment
{
    public Guid Id { get; set; }
    public Guid ExpenseClaimId { get; set; }
    public ExpenseClaim ExpenseClaim { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long SizeBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }
}
