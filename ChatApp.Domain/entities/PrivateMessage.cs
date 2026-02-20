using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.enumerations;
using ChatApp.SharedKernel.enums;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PrivateMessage
{
    public string? Id { get; set; }

    public string? SenderId { get; set; }
    
    public string? ChatId { get; set; }

    public string? SenderUsername { get; set; }

    public string? ReceiverId { get; set; }

    public string? ReceiverUsername { get; set; }

    public List<string> ParticipantIds { get; set; } = [];

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsEdited { get; set; }

    public bool IsDeleted { get; set; }

    public MessageType Type { get; set; }

    public List<MessageAttachment>? Attachments { get; set; }

    public MessageStatus Status { get; set; } = MessageStatus.Sent;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public string? ReplyToMessageId { get; set; }

    public static PrivateMessage Create(string chatId, string senderId, string senderUsername, string receiverId, string receiverUsername,
        string content, MessageType type, List<MessageAttachment>? attachments = null)
    {
        return new PrivateMessage
        {
            ChatId = chatId,
            SenderId = senderId,
            SenderUsername = senderUsername,
            ReceiverId = receiverId,
            ReceiverUsername = receiverUsername,
            ParticipantIds = [senderId, receiverId],
            Content = content,
            Type = type,
            Attachments = attachments,
            IsEdited = false,
            IsDeleted = false,
            IsRead = false
        };
    }
}