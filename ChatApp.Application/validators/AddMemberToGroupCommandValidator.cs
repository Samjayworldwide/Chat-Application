using ChatApp.Application.commands;
using FluentValidation;

namespace ChatApp.Application.validators;

public class AddMemberToGroupCommandValidator : AbstractValidator<AddMemberToGroupCommand>
{
    public AddMemberToGroupCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty()
            .WithMessage("Username is required");

        RuleFor(x => x.GroupId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Group Id is required");
    }
}