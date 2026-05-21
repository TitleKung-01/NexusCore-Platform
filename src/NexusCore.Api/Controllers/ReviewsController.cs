using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Reviews;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController(IReviewService reviewService) : ControllerBase
{
    [HttpGet("cycles")]
    public async Task<IActionResult> ListCycles(CancellationToken cancellationToken)
    {
        var list = await reviewService.ListCyclesAsync(cancellationToken);
        return Ok(list);
    }

    [HttpPost("cycles")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateCycle([FromBody] CreateReviewCycleRequest request, CancellationToken cancellationToken)
    {
        var result = await reviewService.CreateCycleAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string scope = "mine", CancellationToken cancellationToken = default)
    {
        var list = await reviewService.ListReviewsAsync(scope, cancellationToken);
        return Ok(list);
    }

    [HttpPost("{id:guid}/self")]
    public async Task<IActionResult> SubmitSelf(Guid id, [FromBody] SubmitSelfReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await reviewService.SubmitSelfAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("{id:guid}/manager")]
    [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> SubmitManager(Guid id, [FromBody] SubmitManagerReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await reviewService.SubmitManagerAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
