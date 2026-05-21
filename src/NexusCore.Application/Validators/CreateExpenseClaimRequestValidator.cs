using FluentValidation;
using NexusCore.Application.Expenses;

namespace NexusCore.Application.Validators;

public class CreateExpenseClaimRequestValidator : AbstractValidator<CreateExpenseClaimRequest>
{
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
