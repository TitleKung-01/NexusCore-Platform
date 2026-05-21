using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Onboarding;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/onboarding")]
[Authorize]
public class OnboardingController(IOnboardingService onboardingService) : ControllerBase
{
    [HttpGet("templates")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> ListTemplates(CancellationToken cancellationToken)
    {
        var list = await onboardingService.ListTemplatesAsync(cancellationToken);
        return Ok(list);
    }

    [HttpPost("templates")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateTemplate([FromBody] SaveOnboardingTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.CreateTemplateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPut("templates/{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] SaveOnboardingTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.UpdateTemplateAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> ListTasks([FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await onboardingService.ListTasksAsync(employeeId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("assign")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Assign([FromBody] AssignOnboardingRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.AssignTemplateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("tasks/{taskId:guid}/complete")]
    public async Task<IActionResult> CompleteTask(Guid taskId, [FromBody] CompleteOnboardingTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.CompleteTaskAsync(taskId, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
