using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var me = await employeeService.GetMeAsync(cancellationToken);
        if (me is null)
            return NotFound(new { Message = "Profile not found." });
        return Ok(me);
    }

    [HttpPut]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdateMeRequest request, CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateMeAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
