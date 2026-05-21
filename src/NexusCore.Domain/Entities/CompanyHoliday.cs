namespace NexusCore.Domain.Entities;

public class CompanyHoliday
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public string Name { get; set; } = string.Empty;
}
