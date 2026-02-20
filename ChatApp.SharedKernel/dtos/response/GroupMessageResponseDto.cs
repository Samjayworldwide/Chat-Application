using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class GroupMessageResponseDto
{
    public string? GroupId { get; set; }

    public string? GroupMessageId { get; set; }

    public string? GroupName { get; set; }

    public string? Message { get; set; }

    public string? SenderUsername { get; set; }

    public DateTime SentAt { get; set; }

    public static GroupMessageResponseDto Create(string groupId, string groupMessageId, string groupName,
        string message, string senderUsername, DateTime sentAt)
    {
        return new GroupMessageResponseDto
        {
            GroupId = groupId,
            GroupMessageId = groupMessageId,
            GroupName = groupName,
            Message = message,
            SenderUsername = senderUsername,
            SentAt = sentAt
        };
    }
}