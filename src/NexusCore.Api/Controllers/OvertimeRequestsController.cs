using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Overtime;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/overtime-requests")]
[Authorize]
public class OvertimeRequestsController(IOvertimeService overtimeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string scope = "mine", CancellationToken cancellationToken = default)
    {
        var list = await overtimeService.ListAsync(scope, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await overtimeService.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(new { Message = "Overtime request not found." });
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOvertimeRequest request, CancellationToken cancellationToken)
    {
        var result = await overtimeService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await overtimeService.SubmitAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] DecideOvertimeRequest request, CancellationToken cancellationToken)
    {
        var result = await overtimeService.ApproveAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] DecideOvertimeRequest request, CancellationToken cancellationToken)
    {
        var result = await overtimeService.RejectAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await overtimeService.CancelAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
