using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IPrivateMessageRepository
{
    Task<PrivateMessage> CreatePrivateMessageAsync(PrivateMessage privateMessage);
    
    Task UpdateMessageStatus(string messageId, MessageStatus messageStatus);
    
    Task <PrivateMessage?> EditPrivateMessageAsync(string messageId, string userId, string newContent);
    
    Task<int> GetUnreadCountAsync(string chatId, string receiverId);
    
    Task<List<PrivateMessage>> GetChatMessagesByChatIdAsync(string chatId, int pageNumber, int pageSize);
}