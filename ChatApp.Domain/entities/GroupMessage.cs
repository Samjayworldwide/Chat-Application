using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.enumerations;
using ChatApp.SharedKernel.enums;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GroupMessage
{
    public string? Id { get; set; }

    public string? GroupId { get; set; }
    
    public string? ChatId { get; set; }

    public string? GroupName { get; set; }

    public string? SenderId { get; set; }

    public string? SenderUsername { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsEdited { get; set; }

    public bool IsDeleted { get; set; }

    public MessageType Type { get; set; }

    public List<MessageAttachment>? Attachments { get; set; }

    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public List<MessageReadReceipt> ReadBy { get; set; } = [];

    public string? ReplyToMessageId { get; set; }

    public List<string>? MentionedUserIds { get; set; }

    public static GroupMessage Create(string chatId, string groupId, string groupName, string senderId, string senderUsername,
        string content, MessageType type, List<MessageAttachment>? attachments = null,
        string? replyToMessageId = null, List<string>? mentionedUserIds = null)
    {
        return new GroupMessage
        {
            ChatId = chatId,
            GroupId = groupId,
            GroupName = groupName,
            SenderId = senderId,
            SenderUsername = senderUsername,
            Content = content,
            Type = type,
            Attachments = attachments,
            ReplyToMessageId = replyToMessageId,
            MentionedUserIds = mentionedUserIds,
            IsEdited = false,
            IsDeleted = false,
        };
    }
}