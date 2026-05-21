using FluentValidation;
using NexusCore.Application.Leave;

namespace NexusCore.Application.Validators;

public class CreateLeaveRequestValidator : AbstractValidator<CreateLeaveRequest>
{
    public CreateLeaveRequestValidator()
    {
        RuleFor(x => x.LeaveTypeId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
