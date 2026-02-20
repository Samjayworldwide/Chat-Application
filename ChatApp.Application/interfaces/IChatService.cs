using System.Security.Claims;
using ChatApp.Application.commons;
using ChatApp.Domain.enumerations;
using ChatApp.SharedKernel.dtos.request;
using ChatApp.SharedKernel.dtos.response;

namespace ChatApp.Application.interfaces;

public interface IChatService
{
    Task UpdateUserStatusAsync(string userId, bool isOnline);

    Task<Result<PrivateMessageResponseDto>>
        SavePrivateMessageAsync(SavePrivateMessageRequest savePrivateMessageRequest);

    Task<Result<GroupMessageResponseDto>> SaveGroupMessageAsync(SaveGroupMessageRequest saveGroupMessageRequest);

    Task<Result<List<ChatConversationDto>>> GetUserChatsAsync(ClaimsPrincipal claimsPrincipal);

    Task<Result<string>> UpdatePrivateMessageStatus(string messageId, MessageStatus messageStatus);

    Task<Result<string>> UpdateGroupMessageStatus(string messageId, MessageStatus messageStatus);

    Task<Result<PrivateMessageResponseDto>> EditMessageAsync(EditMessageRequest editMessageRequest);

    Task<Result<PagedMessageHistoryDto>> GetChatMessagesAsync(ClaimsPrincipal claimsPrincipal, string chatId,
        int pageNumber, int pageSize);
    
    Task<Result<string>> MarkGroupMessagesAsReadAsync(MarkGroupMessageAsReadRequest markGroupMessageAsReadRequest);
}