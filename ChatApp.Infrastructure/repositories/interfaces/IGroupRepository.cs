using ChatApp.Domain.entities;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IGroupRepository
{
    Task<bool> GroupNameExistsForUser(string groupName, string userId);

    Task<Group> CreateGroupAsync(Group group);

    Task<Group?> GetGroupByIdAsync(string groupId);
    
    Task UpdateGroupAsync(Group group);
    
    Task<List<Group>> GetAllUserGroupsAsync(string userId);
}