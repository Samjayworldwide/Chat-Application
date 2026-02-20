using ChatApp.SharedKernel.dtos.request;
using FluentValidation;

namespace ChatApp.Application.validators;

public class SaveGroupMessageRequestValidator : AbstractValidator<SaveGroupMessageRequest>
{
    public SaveGroupMessageRequestValidator()
    {
        RuleFor(request => request.SenderId)
            .NotNull()
            .NotEmpty()
            .WithMessage("SenderId is required.");

        RuleFor(request => request.GroupId)
            .NotNull()
            .NotEmpty()
            .WithMessage("GroupId is required.");

        RuleFor(request => request.Message)
            .NotNull()
            .NotEmpty()
            .WithMessage("Message is required.");
        
        RuleFor(request => request.MessageType)
            .IsInEnum()
            .WithMessage("Invalid MessageType.");
    }
}