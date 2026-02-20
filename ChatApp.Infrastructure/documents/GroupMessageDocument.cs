using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class GroupMessageDocument : MessageDocument
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? GroupId { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ChatId { get; set; }
    
    public string? GroupName { get; set; }
    
    public List<MessageReadReceiptDocument>? ReadBy { get; set; } = [];
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ReplyToMessageId { get; set; }
    
    public List<string>? MentionedUserIds { get; set; }
}