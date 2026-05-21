using NexusCore.Application.Common;
using NexusCore.Domain.Constants;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Payslips;

public class PayslipService(
    ICurrentUserService currentUser,
    IPayslipRepository payslips,
    IEmployeeProfileRepository profiles,
    IFileStorage fileStorage) : IPayslipService
{
    public async Task<IReadOnlyList<PayslipResponse>> ListAsync(Guid? employeeId, CancellationToken cancellationToken = default)
    {
        var target = employeeId ?? currentUser.UserId;
        if (target is null)
            return [];

        if (target != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return [];

        var list = await payslips.ListForEmployeeAsync(target.Value, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payslip = await payslips.FindByIdAsync(id, cancellationToken);
        if (payslip is null)
            return null;

        if (payslip.EmployeeId != currentUser.UserId && !currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return null;

        var stream = await fileStorage.OpenReadAsync(payslip.StoragePath, cancellationToken);
        if (stream is null)
            return null;

        return (stream, "application/pdf", payslip.FileName);
    }

    public async Task<ServiceResult<PayslipResponse>> UploadAsync(UploadPayslipRequest request, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsInAnyRole(UserRoles.Hr, UserRoles.Admin))
            return ServiceResult<PayslipResponse>.Fail("Forbidden.", 403);

        if (currentUser.UserId is null)
            return ServiceResult<PayslipResponse>.Fail("Unauthorized.", 401);

        var employee = await profiles.FindByUserIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
            return ServiceResult<PayslipResponse>.Fail("Employee not found.", 404);

        var stored = await fileStorage.SaveAsync("payslips", fileName, content, cancellationToken);
        var entity = new Payslip
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            Year = request.Year,
            Month = request.Month,
            FileName = Path.GetFileName(fileName),
            StoragePath = stored.StoragePath,
            PublishedAtUtc = DateTime.UtcNow,
            PublishedById = currentUser.UserId.Value
        };

        await payslips.AddAsync(entity, cancellationToken);
        await payslips.SaveChangesAsync(cancellationToken);
        return ServiceResult<PayslipResponse>.Ok(Map(entity));
    }

    private static PayslipResponse Map(Payslip p) =>
        new(p.Id, p.EmployeeId, p.Year, p.Month, p.FileName, p.PublishedAtUtc.ToString("o"));
}
