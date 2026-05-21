using FluentValidation;
using NexusCore.Application.Overtime;

namespace NexusCore.Application.Validators;

public class CreateOvertimeRequestValidator : AbstractValidator<CreateOvertimeRequest>
{
    public CreateOvertimeRequestValidator()
    {
        RuleFor(x => x.WorkDate).NotEmpty();
        RuleFor(x => x.Hours).GreaterThan(0).LessThanOrEqualTo(24);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
