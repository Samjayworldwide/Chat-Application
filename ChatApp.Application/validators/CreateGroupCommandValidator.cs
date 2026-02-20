using ChatApp.Application.commands;
using FluentValidation;

namespace ChatApp.Application.validators;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Group name is required.")
            .Length(3, 50)
            .WithMessage("Group name must be between 3 and 50 characters.");

        RuleFor(c => c.Description)
            .NotEmpty()
            .NotNull()
            .WithMessage("Group description is required.")
            .Length(10, 100)
            .WithMessage("Group description must be between 10 and 100 characters.");

        RuleFor(c => c.Type)
            .IsInEnum()
            .WithMessage("Select a valid group type.");
    }
}