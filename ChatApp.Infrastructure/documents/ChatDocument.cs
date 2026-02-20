using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class ChatDocument
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }

    public ChatType ChatType { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public List<string> ParticipantIds { get; set; } = [];
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? GroupId { get; set; }

    public string? GroupName { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? LastMessageId { get; set; }

    public string? LastMessageContent { get; set; }

    public DateTime? LastMessageSentAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;
}