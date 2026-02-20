using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MessageReadReceiptDocument
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? UserId { get; set; }
    
    public DateTime ReadAt { get; set; }
}