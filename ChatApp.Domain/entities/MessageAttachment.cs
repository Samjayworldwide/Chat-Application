using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MessageAttachment
{
    public string? FileName { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
}