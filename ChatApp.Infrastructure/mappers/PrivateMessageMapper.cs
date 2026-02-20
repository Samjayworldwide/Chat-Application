using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class PrivateMessageMapper
{
    public static PrivateMessageDocument ToDocument(PrivateMessage privateMessage)
    {
        return new PrivateMessageDocument
        {
            Id = privateMessage.Id,
            SenderId = privateMessage.SenderId,
            ChatId = privateMessage.ChatId,
            SenderUsername = privateMessage.SenderUsername,
            ReceiverId = privateMessage.ReceiverId,
            ReceiverUsername = privateMessage.ReceiverUsername,
            ParticipantIds = privateMessage.ParticipantIds,
            Content = privateMessage.Content,
            Type = privateMessage.Type,
            IsRead = privateMessage.IsRead,
            ReadAt = privateMessage.ReadAt,
            IsDeleted = privateMessage.IsDeleted,
            CreatedAt = privateMessage.CreatedAt,
            ReplyToMessageId = privateMessage.ReplyToMessageId,
            UpdatedAt = privateMessage.UpdatedAt,
            IsEdited = privateMessage.IsEdited,
            Status = privateMessage.Status,
            Attachments = privateMessage.Attachments == null
                ? null
                :
                [
                    ..privateMessage.Attachments.Select(a => new MessageAttachmentDocument
                    {
                        FileName = a.FileName,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        FileUrl = a.FileUrl
                    })
                ],
        };
    }

    public static PrivateMessage ToEntity(PrivateMessageDocument privateMessageDocument)
    {
        return new PrivateMessage
        {
            Id = privateMessageDocument.Id,
            ChatId = privateMessageDocument.ChatId,
            SenderId = privateMessageDocument.SenderId,
            SenderUsername = privateMessageDocument.SenderUsername,
            ReceiverId = privateMessageDocument.ReceiverId,
            ReceiverUsername = privateMessageDocument.ReceiverUsername,
            ParticipantIds = privateMessageDocument.ParticipantIds,
            Content = privateMessageDocument.Content,
            Type = privateMessageDocument.Type,
            IsRead = privateMessageDocument.IsRead,
            ReadAt = privateMessageDocument.ReadAt,
            IsDeleted = privateMessageDocument.IsDeleted,
            CreatedAt = privateMessageDocument.CreatedAt,
            ReplyToMessageId = privateMessageDocument.ReplyToMessageId,
            UpdatedAt = privateMessageDocument.UpdatedAt,
            IsEdited = privateMessageDocument.IsEdited,
            Status = privateMessageDocument.Status,
            Attachments = privateMessageDocument.Attachments == null
                ? null
                :
                [
                    ..privateMessageDocument.Attachments.Select(a => new MessageAttachment
                    {
                        FileName = a.FileName,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        FileUrl = a.FileUrl
                    })
                ],
        };
    }
}