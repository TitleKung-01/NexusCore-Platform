using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Reports;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("leave-summary")]
    public async Task<IActionResult> LeaveSummary([FromQuery] int year, CancellationToken cancellationToken)
    {
        var csv = await reportService.ExportLeaveSummaryCsvAsync(year, cancellationToken);
        return File(csv, "text/csv", $"leave-summary-{year}.csv");
    }
}
