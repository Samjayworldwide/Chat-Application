using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.Application.validators;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
[SuppressMessage("ReSharper", "InvertIf")]
public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;

    private readonly IUserRepository _userRepository;

    private readonly IChatRepository _chatRepository;

    private readonly ILogger<GroupService> _logger;

    public GroupService(IGroupRepository groupRepository, IUserRepository userRepository,
        IChatRepository chatRepository, ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;

        _userRepository = userRepository;

        _chatRepository = chatRepository;

        _logger = logger;
    }

    public async Task<Result<string>> CreateGroupAsync(ClaimsPrincipal claimsPrincipal,
        CreateGroupCommand createGroupCommand)
    {
        try
        {
            var validationResult = await new CreateGroupCommandValidator().ValidateAsync(createGroupCommand);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                return Result<string>.ValidationFailure("Validation error", errors);
            }

            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Result<string>.Failure("User not authenticated");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return Result<string>.Failure("User not found");

            var groupNameExists = await _groupRepository.GroupNameExistsForUser(createGroupCommand.Name!, userId);

            if (groupNameExists)
                return Result<string>.Failure("Group name already exists for this user");

            var group = Group.Create(createGroupCommand.Name!, user.Username!, createGroupCommand.Description!, userId,
                createGroupCommand.Type);

            group = await _groupRepository.CreateGroupAsync(group);

            var participantIds = group.Members.Select(m => m.UserId!).ToList();

            var chat = Chat.Create(ChatType.Group, participantIds, group.Id!, group.Name);

            chat = await _chatRepository.CreateAsync(chat);

            if (string.IsNullOrEmpty(chat.Id))
            {
                _logger.LogError("Failed to create chat for the group with ID: {GroupId}", group.Id);

                return Result<string>.Failure("Failed to create chat for the group");
            }

            return Result<string>.Success("Group created successfully", group.Id!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the group");

            return Result<string>.Failure("An error occurred while creating the group");
        }
    }

    public async Task<Result<string>> AddMemberToGroupAsync(ClaimsPrincipal claimsPrincipal,
        AddMemberToGroupCommand addMemberToGroupCommand)
    {
        var validationResult = await new AddMemberToGroupCommandValidator().ValidateAsync(addMemberToGroupCommand);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return Result<string>.ValidationFailure("Validation error", errors);
        }

        var adminId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(adminId))
            return Result<string>.Failure("User not authenticated");

        var user = await _userRepository.GetByEmailOrUsernameAsync(addMemberToGroupCommand.Username!);

        if (user == null)
            return Result<string>.Failure("User with given username not found");

        var group = await _groupRepository.GetGroupByIdAsync(addMemberToGroupCommand.GroupId!);

        if (group == null)
            return Result<string>.Failure("Group not found");

        var isAdmin = group.Members.Any(m => m.UserId == adminId && m.Role == GroupRole.Admin);

        if (!isAdmin)
            return Result<string>.Failure("Only group admins can add members");

        var isExistingMember = group.Members.Any(m => m.UserId == user.Id);

        if (isExistingMember)
            return Result<string>.Failure("User is already a member of the group");

        var newMember = new GroupMember
        {
            UserId = user.Id,
            Username = user.Username!,
            Role = GroupRole.Member,
            JoinedAt = AppExtensions.GetLocalDateTime(),
        };

        group.Members.Add(newMember);

        await _groupRepository.UpdateGroupAsync(group);

        var chat = await _chatRepository.GetGroupChatAsync(group.Id!);

        if (chat == null)
        {
            _logger.LogError("Chat not found for the group with ID: {GroupId}", group.Id);

            return Result<string>.Failure("Chat not found for the group");
        }

        var participantIds = group.Members.Select(m => m.UserId!).ToList();

        await _chatRepository.UpdateGroupChatMembersAsync(chat.Id!, participantIds);

        return Result<string>.Success("Member added to group successfully");
    }

    public async Task<Result<List<Group>>> GetUserGroupsAsync(string userId)
    {
        try
        {
            var groups = await _groupRepository.GetAllUserGroupsAsync(userId);

            return Result<List<Group>>.Success(groups, "Groups retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving user groups");

            return Result<List<Group>>.Failure("An error occurred while retrieving user groups");
        }
    }

    public async Task<Result<string>> RemoveMemberFromGroupAsync(ClaimsPrincipal claimsPrincipal,
        RemoveMemberFromGroupCommand removeMemberFromGroupCommand)
    {
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Result<string>.Failure("User not authenticated");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return Result<string>.Failure("User not found");

        var group = await _groupRepository.GetGroupByIdAsync(removeMemberFromGroupCommand.GroupId!);

        if (group == null)
            return Result<string>.Failure("Group not found");

        var isAdmin = group.Members.Any(m => m.UserId == userId && m.Role == GroupRole.Admin);

        if (!isAdmin)
            return Result<string>.Failure("Only group admins can remove members");

        var memberToRemove = group.Members.FirstOrDefault(m => m.UserId == removeMemberFromGroupCommand.UserId);

        if (memberToRemove == null)
            return Result<string>.Failure("User to remove is not a member of the group");

        group.Members.Remove(memberToRemove);

        await _groupRepository.UpdateGroupAsync(group);

        var chat = await _chatRepository.GetGroupChatAsync(group.Id!);

        if (chat == null)
        {
            _logger.LogError("Chat not found for the group with ID: {GroupId}", group.Id);

            return Result<string>.Failure("Chat not found for the group");
        }

        var participantIds = group.Members.Select(m => m.UserId!).ToList();

        await _chatRepository.UpdateGroupChatMembersAsync(chat.Id!, participantIds);

        return Result<string>.Success($"{user.Username} has been removed from group.");
    }

    public async Task<Result<string>> ExitGroupAsync(ClaimsPrincipal claimsPrincipal, string groupId)
    {
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Result<string>.Failure("User not authenticated");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return Result<string>.Failure("User not found");

        var group = await _groupRepository.GetGroupByIdAsync(groupId);

        if (group == null)
            return Result<string>.Failure("Group not found");

        var memberToRemove = group.Members.FirstOrDefault(m => m.UserId == userId);

        if (memberToRemove == null)
            return Result<string>.Failure("User is not a member of the group");

        group.Members.Remove(memberToRemove);

        await _groupRepository.UpdateGroupAsync(group);

        var chat = await _chatRepository.GetGroupChatAsync(group.Id!);

        if (chat == null)
        {
            _logger.LogError("Chat not found for the group with ID: {GroupId}", group.Id);

            return Result<string>.Failure("Chat not found for the group");
        }

        var participantIds = group.Members.Select(m => m.UserId!).ToList();

        await _chatRepository.UpdateGroupChatMembersAsync(chat.Id!, participantIds);

        return Result<string>.Success($"{user.Username} has exited group.");
    }
}