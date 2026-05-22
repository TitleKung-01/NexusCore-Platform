using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Transfers;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API ประวัติย้ายแผนก — ดูรายการการโยกย้ายพนักงาน</summary>
[ApiController]
[Route("api/employee-transfers")]
[Authorize]
public class EmployeeTransfersController(ITransferService transferService) : ControllerBase
{
    /// <summary>ดึงประวัติการย้ายแผนก (กรองพนักงานและจำกัดจำนวนได้)</summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] Guid? employeeId,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var list = await transferService.ListAsync(employeeId, limit, cancellationToken);
        return Ok(list);
    }
}
