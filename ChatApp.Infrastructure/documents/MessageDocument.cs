using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.enumerations;
using ChatApp.SharedKernel.enums;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class MessageDocument
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? SenderId { get; set; }
    
    public string? SenderUsername { get; set; }
    
    public string? Content { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(1);
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsEdited { get; set; }

    public bool IsDeleted { get; set; }
    
    public MessageType Type { get; set; }
    
    public List<MessageAttachmentDocument>? Attachments { get; set; }
    
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
}