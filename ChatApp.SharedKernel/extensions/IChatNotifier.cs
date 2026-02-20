namespace ChatApp.SharedKernel.extensions;

public interface IChatNotifier
{
    Task SendPrivateMessageAsync(string userId, object message);
    
    Task SendMessageEditedAsync(string userId, object message);
}