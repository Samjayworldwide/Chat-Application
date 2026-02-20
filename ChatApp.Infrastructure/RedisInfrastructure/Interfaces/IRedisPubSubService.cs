using ChatApp.SharedKernel.dtos.response;

namespace ChatApp.Infrastructure.RedisInfrastructure.Interfaces;

public interface IRedisPubSubService
{
    Task PublishPrivateMessageAsync(string receiverId, PrivateMessageResponseDto message);
    
    Task PublishPrivateMessageEditedAsync(string receiverId, PrivateMessageResponseDto message);
    
    Task SubscribeToPrivateMessagesAsync(string userId);
    
    Task SubscribeToPrivateMessageEditsAsync(string userId);
}