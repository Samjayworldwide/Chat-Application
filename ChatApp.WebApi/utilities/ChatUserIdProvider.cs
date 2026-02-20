using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebApi.utilities;

public class ChatUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}