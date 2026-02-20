using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public class PrivateMessageRepository(IMongoDatabase database) : IPrivateMessageRepository
{
    private readonly IMongoCollection<PrivateMessageDocument> _privateMessages =
        database.GetCollection<PrivateMessageDocument>(AppExtensions.PrivateMessageDocumentName);

    public async Task<PrivateMessage> CreatePrivateMessageAsync(PrivateMessage privateMessage)
    {
        var privateMessageDocument = PrivateMessageMapper.ToDocument(privateMessage);

        await _privateMessages.InsertOneAsync(privateMessageDocument);

        return PrivateMessageMapper.ToEntity(privateMessageDocument);
    }

    public async Task UpdateMessageStatus(string messageId, MessageStatus messageStatus)
    {
        var filter = Builders<PrivateMessageDocument>.Filter.Eq(m => m.Id, messageId);

        UpdateDefinition<PrivateMessageDocument>? update = messageStatus switch
        {
            MessageStatus.Delivered => Builders<PrivateMessageDocument>.Update.Set(m => m.Status, messageStatus)
                .Set(m => m.UpdatedAt, AppExtensions.GetLocalDateTime()),
            MessageStatus.Read => Builders<PrivateMessageDocument>.Update.Set(m => m.Status, messageStatus)
                .Set(m => m.IsRead, true)
                .Set(m => m.ReadAt, AppExtensions.GetLocalDateTime())
                .Set(m => m.UpdatedAt, AppExtensions.GetLocalDateTime()),
            _ => null
        };

        await _privateMessages.UpdateOneAsync(filter, update);
    }

    public async Task<PrivateMessage?> EditPrivateMessageAsync(string messageId, string userId, string newContent)
    {
        var filter = Builders<PrivateMessageDocument>.Filter.And(
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.Id, messageId),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.SenderId, userId));

        var update = Builders<PrivateMessageDocument>.Update
            .Set(m => m.Content, newContent)
            .Set(m => m.IsEdited, true)
            .Set(m => m.UpdatedAt, AppExtensions.GetLocalDateTime());

        var options = new FindOneAndUpdateOptions<PrivateMessageDocument>
        {
            ReturnDocument = ReturnDocument.After
        };

        var updated = await _privateMessages.FindOneAndUpdateAsync(filter, update, options);

        return updated == null ? null : PrivateMessageMapper.ToEntity(updated);
    }

    public async Task<int> GetUnreadCountAsync(string chatId, string receiverId)
    {
        var filter = Builders<PrivateMessageDocument>.Filter.And(
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.ChatId, chatId),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.ReceiverId, receiverId),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.IsRead, false),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.Status, MessageStatus.Delivered),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.IsDeleted, false)
        );

        return (int)await _privateMessages.CountDocumentsAsync(filter);
    }

    public async Task<List<PrivateMessage>> GetChatMessagesByChatIdAsync(string chatId, int pageNumber, int pageSize)
    {
        var filter = Builders<PrivateMessageDocument>.Filter.And(
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.ChatId, chatId),
            Builders<PrivateMessageDocument>.Filter.Eq(m => m.IsDeleted, false)
        );

        var messages = await _privateMessages
            .Find(filter)
            .SortByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return messages.Select(PrivateMessageMapper.ToEntity).ToList();
    }
}