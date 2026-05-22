using FluentValidation;
using NexusCore.Application.DTOs;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบคำขอเปลี่ยนรหัสผ่าน (ความยาวรหัสใหม่ ฯลฯ)
/// </summary>
public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    /// <summary>กำหนดกฎรหัสเดิมและรหัสใหม่</summary>
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
