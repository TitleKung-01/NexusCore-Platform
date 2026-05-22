using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Organization;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API บทบาท/สิทธิ์ — ดูและสร้างนิยามบทบาท</summary>
[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController(IOrganizationService organizationService) : ControllerBase
{
    /// <summary>ดึงรายการบทบาททั้งหมด</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var list = await organizationService.ListRolesAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>สร้างบทบาทใหม่ (Hr/Admin)</summary>
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await organizationService.CreateRoleAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
