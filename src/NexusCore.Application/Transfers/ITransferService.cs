namespace NexusCore.Application.Transfers;

/// <summary>
/// บริการดูประวัติการย้ายแผนกของพนักงาน (ตามสิทธิ์ผู้ดู)
/// </summary>
public interface ITransferService
{
    /// <summary>ดึงประวัติย้ายแผนก (กรองตามพนักงานและจำกัดจำนวนได้)</summary>
    Task<IReadOnlyList<EmployeeTransferResponse>> ListAsync(
        Guid? employeeId,
        int? limit,
        CancellationToken cancellationToken = default);
}
