using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Onboarding;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API onboarding — เทมเพลต มอบหมายงาน และทำเครื่องหมายงานเสร็จ</summary>
[ApiController]
[Route("api/onboarding")]
[Authorize]
public class OnboardingController(IOnboardingService onboardingService) : ControllerBase
{
    /// <summary>ดึงเทมเพลต onboarding ทั้งหมด (Hr/Admin)</summary>
    [HttpGet("templates")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> ListTemplates(CancellationToken cancellationToken)
    {
        var list = await onboardingService.ListTemplatesAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>สร้างเทมเพลต onboarding (Hr/Admin)</summary>
    [HttpPost("templates")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> CreateTemplate([FromBody] SaveOnboardingTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.CreateTemplateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>แก้ไขเทมเพลต onboarding (Hr/Admin)</summary>
    [HttpPut("templates/{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] SaveOnboardingTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.UpdateTemplateAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>ดึงงาน onboarding ของพนักงาน</summary>
    [HttpGet("tasks")]
    public async Task<IActionResult> ListTasks([FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await onboardingService.ListTasksAsync(employeeId, cancellationToken);
        return Ok(list);
    }

    /// <summary>มอบหมายเทมเพลตให้พนักงาน (Hr/Admin)</summary>
    [HttpPost("assign")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Assign([FromBody] AssignOnboardingRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.AssignTemplateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>ทำเครื่องหมายงาน onboarding ว่าเสร็จแล้ว</summary>
    [HttpPost("tasks/{taskId:guid}/complete")]
    public async Task<IActionResult> CompleteTask(Guid taskId, [FromBody] CompleteOnboardingTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await onboardingService.CompleteTaskAsync(taskId, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
