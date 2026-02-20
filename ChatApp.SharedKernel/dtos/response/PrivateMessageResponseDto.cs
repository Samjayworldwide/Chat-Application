using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class PrivateMessageResponseDto
{
    public string? ChatId { get; set; }
    
    public string? PrivateMessageId { get; set; }

    public string? SenderId { get; set; }

    public string? SenderUsername { get; set; }

    public string? ReceiverId { get; set; }

    public string? ReceiverUsername { get; set; }

    public string? Message { get; set; }

    public DateTime? DateSent { get; set; }

    public static PrivateMessageResponseDto Create(string chatId, string privateMessageId,
        string senderId, string senderName, string receiverId, string receiverName, string message, DateTime? dateSent)
    {
        return new PrivateMessageResponseDto
        {
            ChatId = chatId,
            PrivateMessageId = privateMessageId,
            SenderId = senderId,
            SenderUsername = senderName,
            ReceiverId = receiverId,
            ReceiverUsername = receiverName,
            Message = message,
            DateSent = dateSent
        };
    }
}