using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ChatConversationDto
{
    public string? Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Avatar { get; set; }

    public bool IsGroup { get; set; }
    
    public string? LastMessage { get; set; }
    
    public DateTime? LastMessageAt { get; set; }

    public int UnreadCount { get; set; }
}