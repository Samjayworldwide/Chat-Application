using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MessageReadReceipt
{
    public string? UserId { get; set; }
    
    public DateTime ReadAt { get; set; }
}