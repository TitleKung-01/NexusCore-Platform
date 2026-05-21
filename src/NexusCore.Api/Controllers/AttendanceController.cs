using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Attendance;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/attendance")]
[Authorize]
public class AttendanceController(IAttendanceService attendanceService) : ControllerBase
{
    [HttpGet("today")]
    public async Task<IActionResult> Today(CancellationToken cancellationToken)
    {
        var record = await attendanceService.GetTodayAsync(cancellationToken);
        return Ok(record);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string from, [FromQuery] string to, [FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await attendanceService.ListAsync(from, to, employeeId, cancellationToken);
        return Ok(list);
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request, CancellationToken cancellationToken)
    {
        var result = await attendanceService.CheckInAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request, CancellationToken cancellationToken)
    {
        var result = await attendanceService.CheckOutAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
