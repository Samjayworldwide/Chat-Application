using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class Chat
{
    public string? Id { get; set; }

    public ChatType ChatType { get; set; }

    public List<string> ParticipantIds { get; set; } = [];

    public string? GroupId { get; set; }

    public string? GroupName { get; set; }

    public string? LastMessageId { get; set; }

    public string? LastMessageContent { get; set; }

    public DateTime? LastMessageSentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    public static Chat Create(ChatType chatType, List<string> participantIds, string? groupId = null,
        string? groupName = null)
    {
        return new Chat
        {
            ChatType = chatType,
            ParticipantIds = participantIds,
            GroupId = groupId,
            GroupName = groupName,
            CreatedAt = AppExtensions.GetLocalDateTime()
        };
    }
}