using ChatApp.Application.commands;
using FluentValidation;

namespace ChatApp.Application.validators;

public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
{
    public UserRegistrationCommandValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty()
            .NotNull()
            .WithMessage("Firstname is required");

        RuleFor(x => x.Lastname)
            .NotEmpty()
            .NotNull()
            .WithMessage("Lastname is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Enter a valid email address");

        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .WithMessage("Username is required")
            .Length(5, 30)
            .WithMessage("Username length must be between 5 and 15 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("Password is required")
            .Length(8, 20)
            .WithMessage("Password length must be between 8 and 20 characters");
    }
}