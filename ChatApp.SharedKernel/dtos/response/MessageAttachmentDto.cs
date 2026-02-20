using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MessageAttachmentDto
{
    public string? FileName { get; set; }
    
    public string? FileUrl { get; set; }
    
    public string? FileType { get; set; }
    
    public long FileSize { get; set; }
}