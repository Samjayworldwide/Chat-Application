using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatDocument> _chats;

    public ChatRepository(IMongoDatabase database)
    {
        _chats = database.GetCollection<ChatDocument>(AppExtensions.ChatDocumentName);
    }

    public async Task<Chat?> GetPrivateChatAsync(string senderId, string receiverId)
    {
        var filter = Builders<ChatDocument>.Filter.And(
            Builders<ChatDocument>.Filter.Eq(c => c.ChatType, ChatType.Private),
            Builders<ChatDocument>.Filter.All(c => c.ParticipantIds, new[] { senderId, receiverId })
        );

        var chatDocument = await _chats.Find(filter).FirstOrDefaultAsync();

        return chatDocument == null ? null : ChatMapper.ToEntity(chatDocument);
    }

    public async Task<Chat?> GetGroupChatAsync(string groupId)
    {
        var filter = Builders<ChatDocument>.Filter.And(
            Builders<ChatDocument>.Filter.Eq(c => c.ChatType, ChatType.Group),
            Builders<ChatDocument>.Filter.Eq(c => c.GroupId, groupId)
        );

        var chatDocument = await _chats.Find(filter).FirstOrDefaultAsync();

        return chatDocument == null ? null : ChatMapper.ToEntity(chatDocument);
    }

    public async Task<Chat> CreateAsync(Chat chat)
    {
        var document = ChatMapper.ToDocument(chat);

        await _chats.InsertOneAsync(document);

        return ChatMapper.ToEntity(document);
    }

    public async Task UpdateLastMessageAsync(string chatId, string messageId, string content, DateTime sentAt)
    {
        var filter = Builders<ChatDocument>.Filter.Eq(c => c.Id, chatId);

        var update = Builders<ChatDocument>.Update
            .Set(c => c.LastMessageId, messageId)
            .Set(c => c.LastMessageContent, content)
            .Set(c => c.LastMessageSentAt, sentAt)
            .Set(c => c.UpdatedAt, AppExtensions.GetLocalDateTime());

        await _chats.UpdateOneAsync(filter, update);
    }

    public async Task UpdateGroupChatMembersAsync(string chatId, List<string> participantIds)
    {
        var filter = Builders<ChatDocument>.Filter.Eq(c => c.Id, chatId);

        var update = Builders<ChatDocument>.Update
            .Set(c => c.ParticipantIds, participantIds)
            .Set(c => c.UpdatedAt, AppExtensions.GetLocalDateTime());

        await _chats.UpdateOneAsync(filter, update);
    }

    public async Task<List<Chat>> GetUserChatsAsync(string userId)
    {
        var filter = Builders<ChatDocument>.Filter.And(
            Builders<ChatDocument>.Filter.AnyEq(c => c.ParticipantIds, userId),
            Builders<ChatDocument>.Filter.Eq(c => c.IsActive, true)
        );

        var sort = Builders<ChatDocument>.Sort.Descending(c => c.LastMessageSentAt);

        var chatDocuments = await _chats.Find(filter).Sort(sort).ToListAsync();

        return chatDocuments.Select(ChatMapper.ToEntity).ToList();
    }

    public async Task<Chat?> GetChatByIdAsync(string chatId)
    {
        var filter = Builders<ChatDocument>.Filter.Eq(c => c.Id, chatId);

        var chatDocument = await _chats.Find(filter).FirstOrDefaultAsync();

        return chatDocument == null ? null : ChatMapper.ToEntity(chatDocument);
    }
}