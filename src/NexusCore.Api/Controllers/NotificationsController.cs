using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Notifications;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API การแจ้งเตือน — รายการ นับยังไม่อ่าน และทำเครื่องหมายอ่านแล้ว</summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    /// <summary>ดึงการแจ้งเตือนล่าสุดของผู้ใช้ปัจจุบัน</summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var list = await notificationService.ListAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>นับจำนวนการแจ้งเตือนที่ยังไม่อ่าน</summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken cancellationToken)
    {
        var count = await notificationService.GetUnreadCountAsync(cancellationToken);
        return Ok(count);
    }

    /// <summary>ทำเครื่องหมายการแจ้งเตือนหนึ่งรายการว่าอ่านแล้ว</summary>
    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var ok = await notificationService.MarkReadAsync(id, cancellationToken);
        if (!ok)
            return NotFound();
        return NoContent();
    }

    /// <summary>ทำเครื่องหมายว่าอ่านทั้งหมดแล้ว</summary>
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        await notificationService.MarkAllReadAsync(cancellationToken);
        return NoContent();
    }
}
