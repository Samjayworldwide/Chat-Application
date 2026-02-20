using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.SharedKernel.dtos.response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebApi.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    [Route("all-chats")]
    [Produces(typeof(Result<List<ChatConversationDto>>))]
    public async Task<IActionResult> GetAllChatConversationsForUser()
    {
        var result = await _chatService.GetUserChatsAsync(User);
        
        if (!result.IsSuccessful)
            return BadRequest(result);
        
        return Ok(result);
    }
    
    [HttpGet]
    [Route("chats/{chatId}/messages")]
    [Produces(typeof(PagedMessageHistoryDto))]
    public async Task<IActionResult> GetChatMessages(string chatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _chatService.GetChatMessagesAsync(User, chatId, page, pageSize);

        if (!result.IsSuccessful)
            return BadRequest(result);
        
        return Ok(result);
    }
}