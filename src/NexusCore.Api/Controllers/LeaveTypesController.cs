using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API ประเภทการลา — รายการประเภทลาสำหรับฟอร์มคำขอ</summary>
[ApiController]
[Route("api/leave-types")]
[Authorize]
public class LeaveTypesController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>ดึงประเภทลาทั้งหมด</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeaveTypeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await employeeService.GetLeaveTypesAsync(cancellationToken);
        return Ok(list);
    }
}
