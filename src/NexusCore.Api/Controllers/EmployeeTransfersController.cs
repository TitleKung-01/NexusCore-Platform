using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Transfers;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/employee-transfers")]
[Authorize]
public class EmployeeTransfersController(ITransferService transferService) : ControllerBase
{
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
