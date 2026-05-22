using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.DTOs;
using NexusCore.Application.Services;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API จัดการบัญชีผู้ใช้ระบบ — ดู สร้าง แก้ไข ลบ (ตามสิทธิ์บทบาท)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>ดึงรายการผู้ใช้ทั้งหมด</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>ดึงผู้ใช้ตามรหัส</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);
        if (user is null)
            return NotFound(new { Message = "User not found." });

        return Ok(user);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.CreateAsync(request, cancellationToken);
        if (user is null)
            return Conflict(new { Message = "Username already exists." });

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>อัปเดตผู้ใช้ (Hr หรือ Admin)</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Hr,Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.UpdateAsync(id, request, cancellationToken);
        if (user is null)
            return NotFound(new { Message = "User not found or invalid role." });
        return Ok(user);
    }

    /// <summary>ลบผู้ใช้ (Admin เท่านั้น)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await userService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(new { Message = "User not found." });
        return NoContent();
    }
}
