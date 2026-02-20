using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MessageHistoryDto
{
    public string? Id { get; set; }

    public string? ChatId { get; set; }

    public string? SenderId { get; set; }

    public string? SenderUsername { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsEdited { get; set; }

    public bool IsOwn { get; set; }

    public string? ReplyToMessageId { get; set; }

    public List<string>? MentionedUserIds { get; set; }

    public List<MessageAttachmentDto>? Attachments { get; set; }
}