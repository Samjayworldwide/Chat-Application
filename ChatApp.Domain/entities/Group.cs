using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Group
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AvatarUrl { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<GroupMember> Members { get; set; } = [];

    public GroupType Type { get; set; }

    public static Group Create(string name, string username, string description, string createdBy,
        GroupType type)
    {
        var groupMember = new GroupMember
        {
            UserId = createdBy,
            Username = username,
            Role = GroupRole.Admin,
        };

        return new Group
        {
            Name = name,
            Description = description,
            CreatedBy = createdBy,
            Type = type,
            Members = [groupMember],
            CreatedAt = AppExtensions.GetLocalDateTime()
        };
    }
}