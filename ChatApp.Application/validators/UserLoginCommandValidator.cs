using ChatApp.Application.commands;
using FluentValidation;

namespace ChatApp.Application.validators;

public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
{
    public UserLoginCommandValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
            .NotNull()
            .WithMessage("Email or Username is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("Password is required");
    }
}