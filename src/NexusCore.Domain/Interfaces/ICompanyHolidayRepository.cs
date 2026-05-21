using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface ICompanyHolidayRepository
{
    Task<IReadOnlyList<CompanyHoliday>> ListAsync(int? year, CancellationToken cancellationToken = default);
    Task<CompanyHoliday?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CompanyHoliday?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(CompanyHoliday holiday, CancellationToken cancellationToken = default);
    void Remove(CompanyHoliday holiday);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
