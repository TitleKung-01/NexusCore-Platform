using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Holidays;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API วันหยุดบริษัท — ดู สร้าง แก้ไข ลบ (Hr/Admin สำหรับเขียน)</summary>
[ApiController]
[Route("api/holidays")]
[Authorize]
public class HolidaysController(IHolidayService holidayService) : ControllerBase
{
    /// <summary>ดึงรายการวันหยุด (กรองตามปีได้)</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? year, CancellationToken cancellationToken)
    {
        var list = await holidayService.ListAsync(year, cancellationToken);
        return Ok(list);
    }

    /// <summary>เพิ่มวันหยุดใหม่</summary>
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] CreateHolidayRequest request, CancellationToken cancellationToken)
    {
        var result = await holidayService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(List), result.Data);
    }

    /// <summary>แก้ไขวันหยุด</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHolidayRequest request, CancellationToken cancellationToken)
    {
        var result = await holidayService.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>ลบวันหยุด</summary>
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
