using NexusCore.Application.Common;

namespace NexusCore.Application.Expenses;

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseClaimResponse>> ListAsync(string scope, CancellationToken cancellationToken = default);
    Task<ExpenseClaimResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<ExpenseClaimResponse>> CreateAsync(CreateExpenseClaimRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<ExpenseClaimResponse>> SubmitAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceResult<ExpenseClaimResponse>> ApproveAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<ExpenseClaimResponse>> RejectAsync(Guid id, DecideExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ServiceResult<ExpenseClaimResponse>> CancelAsync(Guid id, CancellationToken cancellationToken = default);
}
