using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.extensions;
using ChatApp.WebApi.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebApi.utilities;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class ChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendPrivateMessageAsync(string userId, object message)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(AppExtensions.ReceivePrivateMessage, message);
    }

    public async Task SendMessageEditedAsync(string userId, object message)
    {
        await _hubContext.Clients.User(userId)
            .SendAsync(AppExtensions.MessageEdited, message);
    }
}