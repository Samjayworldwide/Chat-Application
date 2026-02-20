using ChatApp.SharedKernel.dtos.request;
using FluentValidation;

namespace ChatApp.Application.validators;

public class SavePrivateMessageRequestValidator : AbstractValidator<SavePrivateMessageRequest>
{
    public SavePrivateMessageRequestValidator()
    {
        RuleFor(x => x.SenderId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Sender Id is required.");

        RuleFor(x => x.ReceiverId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Receiver Id is required.");

        RuleFor(x => x.Message)
            .NotNull()
            .NotEmpty()
            .WithMessage("Message content is required.");

        RuleFor(y => y.MessageType)
            .IsInEnum()
            .WithMessage("Message Type is required.");
    }
}