using FluentValidation;
using NexusCore.Application.Employees;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอแก้ไขโปรไฟล์ส่วนตัว (ชื่อ อีเมล โทรศัพท์)
/// </summary>
public class UpdateMeRequestValidator : AbstractValidator<UpdateMeRequest>
{
    /// <summary>กำหนดกฎฟิลด์โปรไฟล์</summary>
    public UpdateMeRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Phone).MaximumLength(32).When(x => x.Phone is not null);
    }
}
