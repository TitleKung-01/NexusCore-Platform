using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/leave-types")]
[Authorize]
public class LeaveTypesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeaveTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await employeeService.GetLeaveTypesAsync(cancellationToken);
        return Ok(list);
    }
}
