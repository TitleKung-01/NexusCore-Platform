using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Attendance;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API การเข้างาน — เช็คอิน/เช็คเอาท์ และดูประวัติ</summary>
[ApiController]
[Route("api/attendance")]
[Authorize]
public class AttendanceController(IAttendanceService attendanceService) : ControllerBase
{
    /// <summary>บันทึกการเข้างานวันนี้ของผู้ใช้ปัจจุบัน</summary>
    [HttpGet("today")]
    public async Task<IActionResult> Today(CancellationToken cancellationToken)
    {
        var record = await attendanceService.GetTodayAsync(cancellationToken);
        return Ok(record);
    }

    /// <summary>ดึงประวัติการเข้างานในช่วงวันที่</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string from, [FromQuery] string to, [FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await attendanceService.ListAsync(from, to, employeeId, cancellationToken);
        return Ok(list);
    }

    /// <summary>บันทึกเวลาเข้างาน</summary>
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request, CancellationToken cancellationToken)
    {
        var result = await attendanceService.CheckInAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>บันทึกเวลาออกงาน</summary>
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request, CancellationToken cancellationToken)
    {
        var result = await attendanceService.CheckOutAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
