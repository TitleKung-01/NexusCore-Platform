using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Employees;
using NexusCore.Application.Organization;
using NexusCore.Domain.Constants;

namespace NexusCore.Api.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize]
public class DepartmentsController(
    IEmployeeService employeeService,
    IOrganizationService organizationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DepartmentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var list = await employeeService.GetDepartmentsAsync(cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Hr},{UserRoles.Admin}")]
    [ProducesResponseType(typeof(DepartmentResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await organizationService.CreateDepartmentAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
