using FluentValidation;
using NexusCore.Application.Expenses;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอเบิกค่าใช้จ่ายและรายการย่อย
/// </summary>
public class CreateExpenseClaimRequestValidator : AbstractValidator<CreateExpenseClaimRequest>
{
    /// <summary>กำหนดกฎหัวข้อและรายการค่าใช้จ่าย</summary>
    public CreateExpenseClaimRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.LineItems).NotEmpty();
        RuleForEach(x => x.LineItems).ChildRules(item =>
        {
            item.RuleFor(l => l.Description).NotEmpty().MaximumLength(500);
            item.RuleFor(l => l.Amount).GreaterThan(0);
        });
    }
}
