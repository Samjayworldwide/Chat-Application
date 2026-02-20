using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class GroupMapper
{
    public static GroupDocument ToDocument(Group group)
    {
        return new GroupDocument
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            AvatarUrl = group.AvatarUrl,
            CreatedBy = group.CreatedBy,
            CreatedAt = group.CreatedAt,
            Members =
            [
                ..group.Members.Select(m => new GroupMemberDocument
                {
                    UserId = m.UserId,
                    Username = m.Username,
                    Role = m.Role,
                })
            ],
            Type = group.Type
        };
    }
    
    public static Group ToEntity(GroupDocument groupDocument)
    {
        return new Group
        {
            Id = groupDocument.Id,
            Name = groupDocument.Name,
            Description = groupDocument.Description,
            AvatarUrl = groupDocument.AvatarUrl,
            CreatedBy = groupDocument.CreatedBy,
            CreatedAt = groupDocument.CreatedAt,
            Members =
            [
                ..groupDocument.Members.Select(m => new GroupMember
                {
                    UserId = m.UserId,
                    Username = m.Username,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt,
                    LastSeenAt = m.LastSeenAt
                })
            ],
            Type = groupDocument.Type
        };
    }
}