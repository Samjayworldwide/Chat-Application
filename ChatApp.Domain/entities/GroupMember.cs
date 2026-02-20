using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GroupMember
{
    public string? UserId { get; set; }
    
    public string? Username { get; set; }
    
    public GroupRole Role { get; set; }
    
    public DateTime JoinedAt { get; set; }
    
    public DateTime? LastSeenAt { get; set; }
}