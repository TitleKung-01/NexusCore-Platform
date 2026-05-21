using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Holidays;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/holidays")]
[Authorize]
public class HolidaysController(IHolidayService holidayService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? year, CancellationToken cancellationToken)
    {
        var list = await holidayService.ListAsync(year, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateHolidayRequest request, CancellationToken cancellationToken)
    {
        var result = await holidayService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(List), result.Data);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHolidayRequest request, CancellationToken cancellationToken)
    {
        var result = await holidayService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await holidayService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return NoContent();
    }
}
