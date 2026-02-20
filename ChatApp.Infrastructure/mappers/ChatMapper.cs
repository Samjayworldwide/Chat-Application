using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class ChatMapper
{
    public static ChatDocument ToDocument(Chat chat)
    {
        return new ChatDocument
        {
            Id = chat.Id,
            ChatType = chat.ChatType,
            GroupId = chat.GroupId,
            GroupName = chat.GroupName,
            ParticipantIds = chat.ParticipantIds,
            LastMessageId = chat.LastMessageId,
            LastMessageContent = chat.LastMessageContent,
            LastMessageSentAt = chat.LastMessageSentAt,
            CreatedAt = chat.CreatedAt,
            UpdatedAt = chat.UpdatedAt,
            IsActive = chat.IsActive
        };
    }
    
    public static Chat ToEntity(ChatDocument chatDocument)
    {
        return new Chat
        {
            Id = chatDocument.Id,
            ChatType = chatDocument.ChatType,
            GroupId = chatDocument.GroupId,
            GroupName = chatDocument.GroupName,
            ParticipantIds = chatDocument.ParticipantIds,
            LastMessageId = chatDocument.LastMessageId,
            LastMessageContent = chatDocument.LastMessageContent,
            LastMessageSentAt = chatDocument.LastMessageSentAt,
            CreatedAt = chatDocument.CreatedAt,
            UpdatedAt = chatDocument.UpdatedAt,
            IsActive = chatDocument.IsActive
        };
    }
}