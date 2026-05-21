using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IPayslipRepository
{
    Task<Payslip?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payslip>> ListForEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    Task AddAsync(Payslip payslip, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
