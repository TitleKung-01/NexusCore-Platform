using FluentValidation;
using NexusCore.Application.Holidays;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอเพิ่มวันหยุดบริษัท
/// </summary>
public class CreateHolidayRequestValidator : AbstractValidator<CreateHolidayRequest>
{
    /// <summary>กำหนดกฎวันที่และชื่อวันหยุด</summary>
    public CreateHolidayRequestValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(128);
    }
}
