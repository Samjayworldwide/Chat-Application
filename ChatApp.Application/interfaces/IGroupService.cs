using System.Security.Claims;
using ChatApp.Application.commands;
using ChatApp.Application.commons;
using ChatApp.Domain.entities;

namespace ChatApp.Application.interfaces;

public interface IGroupService
{
    Task<Result<string>> CreateGroupAsync(ClaimsPrincipal claimsPrincipal, CreateGroupCommand createGroupCommand);

    Task<Result<string>> AddMemberToGroupAsync(ClaimsPrincipal claimsPrincipal,
        AddMemberToGroupCommand addMemberToGroupCommand);
    
    Task<Result<List<Group>>> GetUserGroupsAsync(string userId);
    
    Task<Result<string>> RemoveMemberFromGroupAsync(ClaimsPrincipal claimsPrincipal,
        RemoveMemberFromGroupCommand removeMemberFromGroupCommand);
    
    Task<Result<string>> ExitGroupAsync(ClaimsPrincipal claimsPrincipal, string groupId);
}