using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.request;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MarkGroupMessageAsReadRequest
{
    public string? MessageId { get; set; }
    
    public string? UserId { get; set; }
}