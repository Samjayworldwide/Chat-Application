using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PrivateMessageDocument : MessageDocument
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ReceiverId { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ChatId { get; set; }

    public string? ReceiverUsername { get; set; }

    public List<string> ParticipantIds { get; set; } = [];

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ReplyToMessageId { get; set; }
}