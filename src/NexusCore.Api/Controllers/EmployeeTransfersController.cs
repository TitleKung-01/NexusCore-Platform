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
    public async Task<IActionResult> List([FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await transferService.ListAsync(employeeId, cancellationToken);
        return Ok(list);
    }
}
