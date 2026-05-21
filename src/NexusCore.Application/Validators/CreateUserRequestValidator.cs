using FluentValidation;
using NexusCore.Application.DTOs;

namespace NexusCore.Application.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private static readonly string[] AllowedRoles = ["Employee", "Manager", "Hr", "Admin"];

    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(64)
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Username may only contain letters, numbers, dot, underscore, and hyphen.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.Role)
            .Must(r => AllowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Role must be Employee, Manager, Hr, or Admin.");
    }
}
