using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentsController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await employeeService.GetDepartmentsAsync(cancellationToken);
        return Ok(list);
    }
}
