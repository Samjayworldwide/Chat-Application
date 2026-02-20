using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GroupDocument
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? AvatarUrl { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = AppExtensions.GetLocalDateTime();

    public List<GroupMemberDocument> Members { get; set; } = [];

    public GroupType Type { get; set; }
}