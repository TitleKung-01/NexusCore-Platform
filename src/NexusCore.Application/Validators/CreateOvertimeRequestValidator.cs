using FluentValidation;
using NexusCore.Application.Overtime;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอทำงานล่วงเวลา (วันที่ ชั่วโมง เหตุผล)
/// </summary>
public class CreateOvertimeRequestValidator : AbstractValidator<CreateOvertimeRequest>
{
    /// <summary>กำหนดกฎฟิลด์ OT</summary>
    public CreateOvertimeRequestValidator()
    {
        RuleFor(x => x.WorkDate).NotEmpty();
        RuleFor(x => x.Hours).GreaterThan(0).LessThanOrEqualTo(24);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}
