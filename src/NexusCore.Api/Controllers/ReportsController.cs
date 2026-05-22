using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Reports;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API รายงาน HR — ส่งออก CSV สรุปลาและการเข้างาน (Hr/Admin)</summary>
[ApiController]
[Route("api/reports")]
[Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    /// <summary>ส่งออก CSV สรุปการลาตามปี</summary>
    [HttpGet("leave-summary")]
    public async Task<IActionResult> LeaveSummary([FromQuery] int year, CancellationToken cancellationToken)
    {
        var csv = await reportService.ExportLeaveSummaryCsvAsync(year, cancellationToken);
        return File(csv, "text/csv; charset=utf-8", $"leave-summary-{year}.csv");
    }

    /// <summary>ส่งออก CSV บันทึกการเข้างานในช่วงวันที่</summary>
    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] Guid? employeeId,
        CancellationToken cancellationToken)
    {
        var csv = await reportService.ExportAttendanceCsvAsync(from, to, employeeId, cancellationToken);
        var fileName = employeeId is null
            ? $"attendance-{from}-to-{to}.csv"
            : $"attendance-{employeeId}-{from}-to-{to}.csv";
        return File(csv, "text/csv; charset=utf-8", fileName);
    }
}
