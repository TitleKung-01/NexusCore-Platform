using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Leave;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/leave-requests")]
[Authorize]
public class LeaveRequestsController(ILeaveService leaveService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeaveRequestResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] string scope = "mine", CancellationToken cancellationToken = default)
    {
        var list = await leaveService.ListAsync(scope, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LeaveRequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await leaveService.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(new { Message = "Leave request not found." });
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(typeof(LeaveRequestResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequest request, CancellationToken cancellationToken)
    {
        var result = await leaveService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await leaveService.SubmitAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] DecideLeaveRequest request, CancellationToken cancellationToken)
    {
        var result = await leaveService.ApproveAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] DecideLeaveRequest request, CancellationToken cancellationToken)
    {
        var result = await leaveService.RejectAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await leaveService.CancelAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> Calendar([FromQuery] string from, [FromQuery] string to, [FromQuery] Guid? departmentId, CancellationToken cancellationToken)
    {
        var list = await leaveService.GetCalendarAsync(from, to, departmentId, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}/attachments")]
    public async Task<IActionResult> ListAttachments(Guid id, CancellationToken cancellationToken)
    {
        var list = await leaveService.ListAttachmentsAsync(id, cancellationToken);
        return Ok(list);
    }

    [HttpPost("{id:guid}/attachments")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadAttachment(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { Message = "File is required." });

        await using var stream = file.OpenReadStream();
        var result = await leaveService.UploadAttachmentAsync(id, file.FileName, stream, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("attachments/{attachmentId:guid}/download")]
    public async Task<IActionResult> DownloadAttachment(Guid attachmentId, CancellationToken cancellationToken)
    {
        var file = await leaveService.DownloadAttachmentAsync(attachmentId, cancellationToken);
        if (file is null)
            return NotFound();
        return File(file.Value.Stream, file.Value.ContentType, file.Value.FileName);
    }
}

