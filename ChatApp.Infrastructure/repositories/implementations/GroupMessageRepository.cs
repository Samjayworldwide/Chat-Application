using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
public class GroupMessageRepository : IGroupMessageRepository
{
    private readonly IMongoCollection<GroupMessageDocument> _groupMessages;

    public GroupMessageRepository(IMongoDatabase database)
    {
        _groupMessages = database.GetCollection<GroupMessageDocument>(AppExtensions.GroupMessageDocumentName);
    }

    public async Task<GroupMessage> CreateGroupMessageAsync(GroupMessage groupMessage)
    {
        var document = GroupMessageMapper.ToDocument(groupMessage);

        await _groupMessages.InsertOneAsync(document);

        return GroupMessageMapper.ToEntity(document);
    }

    public async Task UpdateMessageStatus(string messageId, MessageStatus messageStatus,
        List<MessageReadReceiptDocument>? readReceipt = null)
    {
        var filter = Builders<GroupMessageDocument>.Filter.Eq(m => m.Id, messageId);

        var update = messageStatus switch
        {
            MessageStatus.Delivered => Builders<GroupMessageDocument>.Update.Set(m => m.Status, messageStatus)
                .Set(m => m.UpdatedAt, AppExtensions.GetLocalDateTime()),
            MessageStatus.Read => Builders<GroupMessageDocument>.Update.Set(m => m.Status, messageStatus)
                .Set(m => m.UpdatedAt, AppExtensions.GetLocalDateTime())
                .Set(m => m.ReadBy, readReceipt),
            _ => null
        };

        await _groupMessages.UpdateOneAsync(filter, update);
    }

    public async Task<int> GetUnreadCountAsync(string chatId, string userId)
    {
        var filter = Builders<GroupMessageDocument>.Filter.And(
            Builders<GroupMessageDocument>.Filter.Eq(m => m.ChatId, chatId),
            Builders<GroupMessageDocument>.Filter.Eq(m => m.Status, MessageStatus.Delivered),
            Builders<GroupMessageDocument>.Filter.Eq(m => m.IsDeleted, false),
            Builders<GroupMessageDocument>.Filter.Not(
                Builders<GroupMessageDocument>.Filter.ElemMatch(
                    m => m.ReadBy,
                    Builders<MessageReadReceiptDocument>.Filter.Eq(r => r.UserId, userId)
                )
            )
        );

        return (int)await _groupMessages.CountDocumentsAsync(filter);
    }

    public async Task<List<GroupMessage>> GetChatMessagesByChatIdAsync(string chatId, int pageNumber, int pageSize)
    {
        var filter = Builders<GroupMessageDocument>.Filter.And(
            Builders<GroupMessageDocument>.Filter.Eq(m => m.ChatId, chatId),
            Builders<GroupMessageDocument>.Filter.Eq(m => m.IsDeleted, false)
        );

        var messages = await _groupMessages
            .Find(filter)
            .SortByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return messages.Select(GroupMessageMapper.ToEntity).ToList();
    }

    public async Task<GroupMessage?> GetMessageByIdAsync(string messageId)
    {
        var filter = Builders<GroupMessageDocument>.Filter.Eq(m => m.Id, messageId);

        var document = await _groupMessages.Find(filter).FirstOrDefaultAsync();

        return document == null ? null : GroupMessageMapper.ToEntity(document);
    }
}