using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusCore.Application.Expenses;

namespace NexusCore.Api.Controllers;

/// <summary>กลุ่ม API คำเบิกค่าใช้จ่าย — สร้าง ส่ง อนุมัติ และยกเลิก</summary>
[ApiController]
[Route("api/expense-claims")]
[Authorize]
public class ExpenseClaimsController(IExpenseService expenseService) : ControllerBase
{
    /// <summary>ดึงรายการคำเบิก (scope: mine / team / all)</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string scope = "mine", CancellationToken cancellationToken = default)
    {
        var list = await expenseService.ListAsync(scope, cancellationToken);
        return Ok(list);
    }

    /// <summary>ดึงคำเบิกตามรหัส</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await expenseService.GetByIdAsync(id, cancellationToken);
        if (item is null)
            return NotFound(new { Message = "Expense claim not found." });
        return Ok(item);
    }

    /// <summary>สร้างคำเบิกใหม่</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseClaimRequest request, CancellationToken cancellationToken)
    {
        var result = await expenseService.CreateAsync(request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>ส่งคำเบิกเพื่อรออนุมัติ</summary>
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken cancellationToken)
    {
        var result = await expenseService.SubmitAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>อนุมัติคำเบิก</summary>
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] DecideExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await expenseService.ApproveAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>ปฏิเสธคำเบิก</summary>
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] DecideExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await expenseService.RejectAsync(id, request, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }

    /// <summary>ยกเลิกคำเบิก</summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await expenseService.CancelAsync(id, cancellationToken);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { Message = result.Error });
        return Ok(result.Data);
    }
}
