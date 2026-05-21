namespace NexusCore.Application.Transfers;

public interface ITransferService
{
    Task<IReadOnlyList<EmployeeTransferResponse>> ListAsync(
        Guid? employeeId,
        int? limit,
        CancellationToken cancellationToken = default);
}
