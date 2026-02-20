using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class GroupMessageMapper
{
    public static GroupMessageDocument ToDocument(GroupMessage groupMessage)
    {
        return new GroupMessageDocument
        {
            Id = groupMessage.Id,
            GroupId = groupMessage.GroupId,
            ChatId = groupMessage.ChatId,
            GroupName = groupMessage.GroupName,
            SenderId = groupMessage.SenderId,
            SenderUsername = groupMessage.SenderUsername,
            Content = groupMessage.Content,
            Type = groupMessage.Type,
            CreatedAt = groupMessage.CreatedAt,
            UpdatedAt = groupMessage.UpdatedAt,
            IsEdited = groupMessage.IsEdited,
            IsDeleted = groupMessage.IsDeleted,
            Attachments = groupMessage.Attachments == null
                ? null
                :
                [
                    ..groupMessage.Attachments.Select(a => new MessageAttachmentDocument
                    {
                        FileName = a.FileName,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        FileUrl = a.FileUrl
                    })
                ],
            Status = groupMessage.Status,
            ReadBy =
                [
                    ..groupMessage.ReadBy.Select(r => new MessageReadReceiptDocument
                    {
                        UserId = r.UserId,
                        ReadAt = r.ReadAt
                    })
                ],
            ReplyToMessageId = groupMessage.ReplyToMessageId,
            MentionedUserIds = groupMessage.MentionedUserIds
        };
    }
    
    public static GroupMessage ToEntity(GroupMessageDocument document)
    {
        return new GroupMessage
        {
            Id = document.Id,
            ChatId = document.ChatId,
            GroupId = document.GroupId,
            GroupName = document.GroupName,
            SenderId = document.SenderId,
            SenderUsername = document.SenderUsername,
            Content = document.Content,
            Type = document.Type,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            IsEdited = document.IsEdited,
            IsDeleted = document.IsDeleted,
            Attachments = document.Attachments == null
                ? null
                :
                [
                    ..document.Attachments.Select(a => new MessageAttachment
                    {
                        FileName = a.FileName,
                        FileType = a.FileType,
                        FileSize = a.FileSize,
                        FileUrl = a.FileUrl
                    })
                ],
            Status = document.Status,
            ReadBy = document.ReadBy == null
                ? []
                :
                [
                    ..document.ReadBy.Select(r => new MessageReadReceipt
                    {
                        UserId = r.UserId,
                        ReadAt = r.ReadAt
                    })
                ],
            ReplyToMessageId = document.ReplyToMessageId,
            MentionedUserIds = document.MentionedUserIds
        };
    }
}