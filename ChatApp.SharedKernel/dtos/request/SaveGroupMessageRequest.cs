using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;

namespace ChatApp.SharedKernel.dtos.request;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SaveGroupMessageRequest
{
    public string? SenderId { get; set; }
    
    public string? GroupId { get; set; }
    
    public string? Message { get; set; }
    
    public MessageType MessageType { get; set; }
    
    public string? ReplyToMessageId { get; set; }
    
    public List<string>? MentionedUserIds { get; set; }
}