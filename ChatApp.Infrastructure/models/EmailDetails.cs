using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.models;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class EmailDetails
{
    public required string RecipientEmail { get; set; }
    
    public required string Subject { get; set; }
    
    public required string Body { get; set; }
}