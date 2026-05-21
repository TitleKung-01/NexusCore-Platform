namespace NexusCore.Application.Transfers;

public interface ITransferService
{
    Task<IReadOnlyList<EmployeeTransferResponse>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default);
}
