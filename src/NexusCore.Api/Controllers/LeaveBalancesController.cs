using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Leave;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/leave-balances")]
[Authorize]
public class LeaveBalancesController(ILeaveService leaveService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? year, [FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await leaveService.GetBalancesAsync(year, employeeId, cancellationToken);
        return Ok(list);
    }
}
