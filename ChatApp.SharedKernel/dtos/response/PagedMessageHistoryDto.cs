using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PagedMessageHistoryDto
{
    public List<MessageHistoryDto> Messages { get; set; } = [];
    
    public int Page { get; set; }

    public int PageSize { get; set; }

    public bool HasMore { get; set; }
}