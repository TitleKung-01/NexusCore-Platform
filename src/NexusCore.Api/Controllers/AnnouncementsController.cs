using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Announcements;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API ประกาศบริษัท — ดูและจัดการประกาศ (Hr/Admin สำหรับเขียน)</summary>
[ApiController]
[Route("api/announcements")]
[Authorize]
public class AnnouncementsController(IAnnouncementService announcementService) : ControllerBase
{
    /// <summary>ดึงประกาศที่เปิดใช้งาน</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var list = await announcementService.ListAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>ดึงประกาศตามรหัส</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await announcementService.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound();
        return Ok(item);
    }

    /// <summary>สร้างประกาศใหม่</summary>
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request, CancellationToken cancellationToken)
    {
        var result = await announcementService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>แก้ไขประกาศ</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAnnouncementRequest request, CancellationToken cancellationToken)
    {
        var result = await announcementService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
