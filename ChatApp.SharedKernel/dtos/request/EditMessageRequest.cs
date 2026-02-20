using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.request;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class EditMessageRequest
{
    public string? UserId { get; set; }
    
    public string? MessageId { get; set; }
    
    public string? NewContent { get; set; }
}