using ChatApp.SharedKernel.dtos.request;
using FluentValidation;

namespace ChatApp.Application.validators;

public class MarkGroupMessageAsReadRequestValidator : AbstractValidator<MarkGroupMessageAsReadRequest>
{
    public MarkGroupMessageAsReadRequestValidator()
    {
        RuleFor(x => x.MessageId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Message Id is required.");

        RuleFor(x => x.UserId)
            .NotNull()
            .NotEmpty()
            .WithMessage("User Id is required.");
    }
}