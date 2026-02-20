using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using ChatApp.Application.interfaces;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.RedisInfrastructure.Interfaces;
using ChatApp.SharedKernel.dtos.request;
using ChatApp.SharedKernel.extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.WebApi.Controllers;

[Authorize]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    private readonly IGroupService _groupService;

    private readonly IRedisPubSubService _redisPubSubService;

    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, IGroupService groupService, IRedisPubSubService redisPubSubService,
        ILogger<ChatHub> logger)
    {
        _chatService = chatService;

        _groupService = groupService;

        _redisPubSubService = redisPubSubService;

        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (string.IsNullOrEmpty(userId))
            return;

        UserConnectionMapping.AddConnection(userId, Context.ConnectionId);

        await _chatService.UpdateUserStatusAsync(userId, true);

        var result = await _groupService.GetUserGroupsAsync(userId);

        if (result.IsSuccessful)
        {
            var groups = result.Value!;

            foreach (var group in groups)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, group.Id!);
            }
        }

        await _redisPubSubService.SubscribeToPrivateMessagesAsync(userId);

        await _redisPubSubService.SubscribeToPrivateMessageEditsAsync(userId);

        _logger.LogInformation("User {UserId} connected with connection ID {ConnectionId} connected successfully",
            userId, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public async Task SendPrivateMessage(SavePrivateMessageRequest savePrivateMessageRequest)
    {
        var senderId = Context.UserIdentifier;

        savePrivateMessageRequest.SenderId = senderId;

        var result = await _chatService.SavePrivateMessageAsync(savePrivateMessageRequest);

        if (!result.IsSuccessful)
            await Clients.Caller.SendAsync("MessageFailed",
                new { TempId = Guid.NewGuid(), Reason = "Unable to send message" });

        await _redisPubSubService.PublishPrivateMessageAsync(
            savePrivateMessageRequest.ReceiverId!, result.Value!);

        await _chatService.UpdatePrivateMessageStatus(result.Value!.PrivateMessageId!, MessageStatus.Delivered);
    }

    public async Task SendGroupMessage(SaveGroupMessageRequest saveGroupMessageRequest)
    {
        var senderId = Context.UserIdentifier;

        saveGroupMessageRequest.SenderId = senderId;

        var result = await _chatService.SaveGroupMessageAsync(saveGroupMessageRequest);

        if (!result.IsSuccessful)
            await Clients.Caller.SendAsync("MessageFailed",
                new { TempId = Guid.NewGuid(), Reason = "Unable to send message" });

        var senderConnections = UserConnectionMapping.GetConnections(senderId!).ToList();

        await Clients.GroupExcept(saveGroupMessageRequest.GroupId!, senderConnections)
            .SendAsync(AppExtensions.ReceiveGroupMessage, result.Value);

        await _chatService.UpdateGroupMessageStatus(result.Value!.GroupMessageId!, MessageStatus.Delivered);
    }

    public async Task MarkPrivateMessageAsRead(string messageId)
    {
        var result = await _chatService.UpdatePrivateMessageStatus(messageId, MessageStatus.Read);

        if (!result.IsSuccessful)
        {
            _logger.LogInformation("Failed to update message status to read for message with Id {MessageId}",
                messageId);

            return;
        }

        await Clients.Caller.SendAsync(AppExtensions.MessageRead, new { MessageId = messageId });
    }

    public async Task MarkGroupMessagesAsRead(MarkGroupMessageAsReadRequest markGroupMessageAsReadRequest)
    {
        var userId = Context.UserIdentifier;

        markGroupMessageAsReadRequest.UserId = userId;

        var result = await _chatService.MarkGroupMessagesAsReadAsync(markGroupMessageAsReadRequest);

        if (!result.IsSuccessful)
        {
            _logger.LogInformation(
                "Failed to mark group messages as read for group message with Id {MessageId} and user with Id {UserId}",
                markGroupMessageAsReadRequest.MessageId, markGroupMessageAsReadRequest.UserId);

            return;
        }

        await Clients.Caller.SendAsync(AppExtensions.GroupMessagesRead,
            new { GroupId = markGroupMessageAsReadRequest.MessageId });
    }

    public async Task EditMessage(EditMessageRequest editMessageRequest)
    {
        var userId = Context.UserIdentifier;

        editMessageRequest.UserId = userId;

        var result = await _chatService.EditMessageAsync(editMessageRequest);

        if (!result.IsSuccessful)
        {
            _logger.LogInformation("Failed to edit message with Id {MessageId} for user with Id {UserId}",
                editMessageRequest.MessageId, editMessageRequest.UserId);

            await Clients.Caller.SendAsync("EditFailed",
                new { editMessageRequest.MessageId, Reason = "Unable to edit message" });

            return;
        }

        var editedMessage = result.Value!;

        await _redisPubSubService.PublishPrivateMessageEditedAsync(editedMessage.ReceiverId!, editedMessage);
    }

    public async Task LeaveGroup(string groupId)
    {
        var user = Context.User;

        if (user == null)
            return;

        var result = await _groupService.ExitGroupAsync(user, groupId);

        if (!result.IsSuccessful)
        {
            _logger.LogInformation("Failed to leave group with Id {GroupId} for user with Id {UserId}",
                groupId, user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            await Clients.Caller.SendAsync("LeaveGroupFailed",
                new { GroupId = groupId, Reason = "Unable to leave group" });

            return;
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);

        await Clients.Group(groupId).SendAsync(AppExtensions.ExitGroup, result.Message);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (userId != null)
        {
            UserConnectionMapping.RemoveConnection(userId, Context.ConnectionId);

            await _chatService.UpdateUserStatusAsync(userId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }
}