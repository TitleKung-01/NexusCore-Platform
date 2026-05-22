using FluentValidation;
using NexusCore.Application.DTOs;

namespace NexusCore.Application.Validators;

/// <summary>
/// ตรวจสอบความถูกต้องของคำขอเข้าสู่ระบบก่อนเรียก use case
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>กำหนดกฎ username และ password</summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(64);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
