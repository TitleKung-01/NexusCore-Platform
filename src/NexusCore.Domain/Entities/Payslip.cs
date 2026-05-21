namespace NexusCore.Domain.Entities;

public class Payslip
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public DateTime PublishedAtUtc { get; set; }
    public Guid PublishedById { get; set; }
}
