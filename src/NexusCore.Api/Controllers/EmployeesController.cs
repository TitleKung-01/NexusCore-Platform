using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API จัดการพนักงาน — รายชื่อและแก้ไขโปรไฟล์ (Hr/Admin)</summary>
[ApiController]
[Route("api/employees")]
[Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>ดึงรายชื่อพนักงานทั้งหมด</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmployeeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await employeeService.GetEmployeesAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>อัปเดตโปรไฟล์พนักงานตามรหัสผู้ใช้</summary>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(EmployeeListItem), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateEmployeeAsync(userId, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
