using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;

namespace ChatApp.SharedKernel.dtos.request;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class SavePrivateMessageRequest
{
    public string? SenderId { get; set; }
    
    public string? ReceiverId { get; set; }
    
    public string? Message { get; set; }
    
    public MessageType MessageType { get; set; }
}