using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Payslips;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API สลิปเงินเดือน — ดูรายการ ดาวน์โหลด และอัปโหลด (Hr/Admin)</summary>
[ApiController]
[Route("api/payslips")]
[Authorize]
public class PayslipsController(IPayslipService payslipService) : ControllerBase
{
    /// <summary>ดึงรายการสลิป (กรองตามพนักงานได้)</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? employeeId, CancellationToken cancellationToken)
    {
        var list = await payslipService.ListAsync(employeeId, cancellationToken);
        return Ok(list);
    }

    /// <summary>ดาวน์โหลดไฟล์สลิป</summary>
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var file = await payslipService.DownloadAsync(id, cancellationToken);
        if (file is null)
            return NotFound();
        return File(file.Value.Stream, file.Value.ContentType, file.Value.FileName);
    }

    /// <summary>อัปโหลดสลิปให้พนักงาน (Hr/Admin)</summary>
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] UploadPayslipRequest request, IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { Message = "File is required." });

        await using var stream = file.OpenReadStream();
        var result = await payslipService.UploadAsync(request, file.FileName, stream, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
