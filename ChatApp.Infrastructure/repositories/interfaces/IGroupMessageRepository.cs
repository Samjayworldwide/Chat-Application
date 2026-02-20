using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IGroupMessageRepository
{
    Task<GroupMessage> CreateGroupMessageAsync(GroupMessage groupMessage);
    
    Task UpdateMessageStatus(string messageId, MessageStatus messageStatus, List<MessageReadReceiptDocument>? readReceipt = null);
    
    Task<int> GetUnreadCountAsync(string chatId, string userId);
    
    Task<List<GroupMessage>> GetChatMessagesByChatIdAsync(string chatId, int pageNumber, int pageSize);
    
    Task<GroupMessage?> GetMessageByIdAsync(string messageId);
}