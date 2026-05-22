using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Leave;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API ยอดคงเหลือลา — โควต้าและวันลาที่ใช้แล้ว</summary>
[ApiController]
[Route("api/leave-balances")]
[Authorize]
public class LeaveBalancesController(ILeaveService leaveService) : ControllerBase
{
    /// <summary>ดึงยอดลาตามปีและพนักงาน (ถ้าไม่ระบุ employeeId ใช้ผู้ใช้ปัจจุบัน)</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? year, [FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await leaveService.GetBalancesAsync(year, employeeId, cancellationToken);
        return Ok(list);
    }
}
