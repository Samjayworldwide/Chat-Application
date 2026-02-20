using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GroupMemberDocument
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? UserId { get; set; }
    
    public string? Username { get; set; }
    
    public GroupRole Role { get; set; } = GroupRole.Admin;
    
    public DateTime JoinedAt { get; set; } = AppExtensions.GetLocalDateTime();
    
    public DateTime? LastSeenAt { get; set; }
}