using ChatApp.SharedKernel.dtos.request;
using FluentValidation;

namespace ChatApp.Application.validators;

public class EmailVerificationRequestValidator : AbstractValidator<EmailVerificationRequest>
{
    public EmailVerificationRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .NotNull()
            .WithMessage("Verification code is required.")
            .Length(6)
            .WithMessage("Verification code must be 6 characters long.");
    }
}