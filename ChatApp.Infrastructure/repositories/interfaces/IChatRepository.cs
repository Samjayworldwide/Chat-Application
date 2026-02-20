using ChatApp.Domain.entities;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IChatRepository
{
    Task<Chat?> GetPrivateChatAsync(string senderId, string receiverId);
    
    Task<Chat?> GetGroupChatAsync(string groupId);
    
    Task<Chat> CreateAsync(Chat chat);
    
    Task UpdateLastMessageAsync(string chatId, string messageId, string content, DateTime sentAt);
    
    Task UpdateGroupChatMembersAsync(string chatId, List<string> participantIds);
    
    Task<List<Chat>> GetUserChatsAsync(string userId);
    
    Task<Chat?> GetChatByIdAsync(string chatId);
}