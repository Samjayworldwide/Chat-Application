using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.documents;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class MessageAttachmentDocument
{
    public string? FileName { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
}