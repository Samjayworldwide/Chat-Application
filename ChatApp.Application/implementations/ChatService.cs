using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using ChatApp.Application.commons;
using ChatApp.Application.interfaces;
using ChatApp.Application.validators;
using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.dtos.request;
using ChatApp.SharedKernel.dtos.response;
using ChatApp.SharedKernel.enums;
using ChatApp.SharedKernel.extensions;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class ChatService : IChatService
{
    private readonly IUserRepository _userRepository;

    private readonly IPrivateMessageRepository _privateMessageRepository;

    private readonly IChatRepository _chatRepository;

    private readonly IGroupRepository _groupRepository;

    private readonly IGroupMessageRepository _groupMessageRepository;

    private readonly ILogger<ChatService> _logger;

    public ChatService(IUserRepository userRepository, IPrivateMessageRepository privateMessageRepository,
        IChatRepository chatRepository,
        IGroupRepository groupRepository, IGroupMessageRepository groupMessageRepository, ILogger<ChatService> logger)
    {
        _userRepository = userRepository;

        _privateMessageRepository = privateMessageRepository;

        _chatRepository = chatRepository;

        _groupRepository = groupRepository;

        _groupMessageRepository = groupMessageRepository;

        _logger = logger;
    }

    public async Task UpdateUserStatusAsync(string userId, bool isOnline)
    {
        await _userRepository.UpdateUserStatusAsync(userId, isOnline);
    }

    public async Task<Result<PrivateMessageResponseDto>> SavePrivateMessageAsync(
        SavePrivateMessageRequest savePrivateMessageRequest)
    {
        try
        {
            var validationResult =
                await new SavePrivateMessageRequestValidator().ValidateAsync(savePrivateMessageRequest);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                _logger.LogInformation("Validation failed for SavePrivateMessageRequest with errors {Errors}",
                    string.Join(", ", errorMessages));

                return Result<PrivateMessageResponseDto>.ValidationFailure("Validation error", errorMessages);
            }

            var sender = await _userRepository.GetByIdAsync(savePrivateMessageRequest.SenderId!);

            if (sender == null)
            {
                _logger.LogInformation("Unable to find sender with given Id {SenderId}",
                    savePrivateMessageRequest.SenderId);

                return Result<PrivateMessageResponseDto>.Failure("Unable to save message");
            }

            var receiver = await _userRepository.GetByIdAsync(savePrivateMessageRequest.ReceiverId!);

            if (receiver == null)
            {
                _logger.LogInformation("Unable to find receiver with given Id {ReceiverId}",
                    savePrivateMessageRequest.ReceiverId);

                return Result<PrivateMessageResponseDto>.Failure("Unable to save message");
            }

            var chat = await _chatRepository.GetPrivateChatAsync(sender.Id!, receiver.Id!);

            if (chat == null)
            {
                var participantIds = new List<string> { sender.Id!, receiver.Id! };

                chat = Chat.Create(ChatType.Private, participantIds);

                chat = await _chatRepository.CreateAsync(chat);

                if (string.IsNullOrEmpty(chat.Id))
                {
                    _logger.LogInformation(
                        "Failed to create chat for sender with Id {SenderId} and receiver with Id {ReceiverId}",
                        savePrivateMessageRequest.SenderId, savePrivateMessageRequest.ReceiverId);

                    return Result<PrivateMessageResponseDto>.Failure("Unable to save message");
                }
            }

            var privateMessage = PrivateMessage.Create(chat.Id!, sender.Id!,
                sender.Username!, receiver.Id!, receiver.Username!, savePrivateMessageRequest.Message!,
                savePrivateMessageRequest.MessageType);

            var savedPrivateMessage = await _privateMessageRepository.CreatePrivateMessageAsync(privateMessage);

            await _chatRepository.UpdateLastMessageAsync(chat.Id!, savedPrivateMessage.Id!,
                savedPrivateMessage.Content!, savedPrivateMessage.CreatedAt);

            var privateMessageResponse = PrivateMessageResponseDto.Create(savedPrivateMessage.ChatId!,
                savedPrivateMessage.Id!,
                savedPrivateMessage.SenderId!, savedPrivateMessage.SenderUsername!, savedPrivateMessage.ReceiverId!,
                savedPrivateMessage.ReceiverUsername!, savedPrivateMessage.Content!, savedPrivateMessage.CreatedAt);

            return Result<PrivateMessageResponseDto>.Success(privateMessageResponse, "");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred saving private message {exception}", e.Message);

            return Result<PrivateMessageResponseDto>.Failure("Unable to save message");
        }
    }

    public async Task<Result<GroupMessageResponseDto>> SaveGroupMessageAsync(
        SaveGroupMessageRequest saveGroupMessageRequest)
    {
        try
        {
            var validationResult = await new SaveGroupMessageRequestValidator().ValidateAsync(saveGroupMessageRequest);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                _logger.LogInformation("Validation failed for SaveGroupMessageRequest with errors {Errors}",
                    string.Join(", ", errorMessages));

                return Result<GroupMessageResponseDto>.ValidationFailure("Validation error", errorMessages);
            }

            var sender = await _userRepository.GetByIdAsync(saveGroupMessageRequest.SenderId!);

            if (sender == null)
            {
                _logger.LogInformation("Unable to find sender with given Id {SenderId}",
                    saveGroupMessageRequest.SenderId);

                return Result<GroupMessageResponseDto>.Failure("Unable to save message");
            }

            var group = await _groupRepository.GetGroupByIdAsync(saveGroupMessageRequest.GroupId!);

            if (group == null)
            {
                _logger.LogInformation("Unable to find group with given Id {GroupId}",
                    saveGroupMessageRequest.GroupId);

                return Result<GroupMessageResponseDto>.Failure("Unable to save message");
            }

            var isMemberOfGroup = group.Members.Any(m => m.UserId == sender.Id);

            if (!isMemberOfGroup)
            {
                _logger.LogInformation("User with Id {SenderId} is not a member of group with Id {GroupId}",
                    saveGroupMessageRequest.SenderId, saveGroupMessageRequest.GroupId);

                return Result<GroupMessageResponseDto>.Failure("Unable to save message");
            }

            var chat = await _chatRepository.GetGroupChatAsync(group.Id!);

            if (chat == null)
            {
                var participantIds = group.Members.Select(m => m.UserId!).ToList();

                chat = Chat.Create(ChatType.Group, participantIds, group.Id!, group.Name!);

                chat = await _chatRepository.CreateAsync(chat);

                if (string.IsNullOrEmpty(chat.Id))
                {
                    _logger.LogInformation("Failed to create chat for group with Id {GroupId}",
                        saveGroupMessageRequest.GroupId);

                    return Result<GroupMessageResponseDto>.Failure("Unable to save message");
                }
            }

            var groupMessage = GroupMessage.Create(chat.Id!, group.Id!, group.Name!, sender.Id!, sender.Username!,
                saveGroupMessageRequest.Message!, saveGroupMessageRequest.MessageType,
                replyToMessageId: saveGroupMessageRequest.ReplyToMessageId,
                mentionedUserIds: saveGroupMessageRequest.MentionedUserIds);

            var savedGroupMessage = await _groupMessageRepository.CreateGroupMessageAsync(groupMessage);

            if (string.IsNullOrEmpty(savedGroupMessage.Id))
            {
                _logger.LogInformation(
                    "Failed to save group message for group with Id {GroupId} and sender with Id {SenderId}",
                    saveGroupMessageRequest.GroupId, saveGroupMessageRequest.SenderId);

                return Result<GroupMessageResponseDto>.Failure("Unable to save message");
            }

            await _chatRepository.UpdateLastMessageAsync(chat.Id!, savedGroupMessage.Id!,
                savedGroupMessage.Content!, savedGroupMessage.CreatedAt);

            var groupMessageResponse = GroupMessageResponseDto.Create(group.Id!, savedGroupMessage.Id!, group.Name!,
                savedGroupMessage.Content!, sender.Username!, savedGroupMessage.CreatedAt);

            return Result<GroupMessageResponseDto>.Success(groupMessageResponse, "Message saved successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred saving group message {exception}", e.Message);

            return Result<GroupMessageResponseDto>.Failure("Unable to save message");
        }
    }

    public async Task<Result<List<ChatConversationDto>>> GetUserChatsAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("Unable to find user Id in claims");

                return Result<List<ChatConversationDto>>.Failure("Unable to retrieve chats");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogInformation("Unable to find user with given Id {UserId}", userId);

                return Result<List<ChatConversationDto>>.Failure("Unable to retrieve chats");
            }

            var chats = await _chatRepository.GetUserChatsAsync(userId);

            if (chats.Count == 0)
                return Result<List<ChatConversationDto>>.Success([], "No chats found");

            var chatConversations = new List<ChatConversationDto>();

            foreach (var chat in chats)
            {
                if (chat.ChatType == ChatType.Private)
                {
                    var otherParticipantId = chat.ParticipantIds.First(id => id != userId);

                    var otherParticipant = await _userRepository.GetByIdAsync(otherParticipantId);

                    if (otherParticipant == null) continue;

                    var unreadCount = await _privateMessageRepository.GetUnreadCountAsync(chat.Id!, userId);

                    chatConversations.Add(new ChatConversationDto
                    {
                        Id = chat.Id,
                        Name = otherParticipant.Username,
                        Avatar = otherParticipant.AvatarUrl,
                        IsGroup = false,
                        LastMessage = chat.LastMessageContent,
                        LastMessageAt = chat.LastMessageSentAt,
                        UnreadCount = unreadCount
                    });
                }
                else if (chat.ChatType == ChatType.Group)
                {
                    var unreadCount = await _groupMessageRepository.GetUnreadCountAsync(chat.Id!, userId);

                    var group = await _groupRepository.GetGroupByIdAsync(chat.GroupId!);

                    if (group == null) continue;

                    chatConversations.Add(new ChatConversationDto
                    {
                        Id = chat.Id,
                        Name = group.Name,
                        Avatar = group.AvatarUrl,
                        IsGroup = true,
                        LastMessage = chat.LastMessageContent,
                        LastMessageAt = chat.LastMessageSentAt,
                        UnreadCount = unreadCount
                    });
                }
            }

            return Result<List<ChatConversationDto>>.Success(chatConversations, "Chat retrieved successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred retrieving user chats {exception}", e.Message);

            return Result<List<ChatConversationDto>>.Failure("Unable to retrieve chats");
        }
    }

    public async Task<Result<string>> UpdatePrivateMessageStatus(string messageId, MessageStatus messageStatus)
    {
        try
        {
            await _privateMessageRepository.UpdateMessageStatus(messageId, messageStatus);

            return Result<string>.Success("Message status updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred updating message status {exception}", e.Message);

            return Result<string>.Failure("Unable to update message status");
        }
    }

    public async Task<Result<string>> UpdateGroupMessageStatus(string messageId, MessageStatus messageStatus)
    {
        try
        {
            await _groupMessageRepository.UpdateMessageStatus(messageId, messageStatus);

            return Result<string>.Success("Message status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred updating group message status {exception}", ex.Message);

            return Result<string>.Failure("Unable to update message status");
        }
    }

    public async Task<Result<PrivateMessageResponseDto>> EditMessageAsync(EditMessageRequest editMessageRequest)
    {
        var validationResult = await new EditMessageRequestValidator().ValidateAsync(editMessageRequest);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            _logger.LogInformation("Validation failed for EditMessageRequest with errors {Errors}",
                string.Join(", ", errorMessages));

            return Result<PrivateMessageResponseDto>.ValidationFailure("Validation error", errorMessages);
        }

        var privateMessage = await _privateMessageRepository.EditPrivateMessageAsync(editMessageRequest.MessageId!,
            editMessageRequest.UserId!, editMessageRequest.NewContent!);

        if (privateMessage == null)
            return Result<PrivateMessageResponseDto>.Failure("Unable to edit message");

        var privateMessageResponse = PrivateMessageResponseDto.Create(privateMessage.ChatId!, privateMessage.Id!,
            privateMessage.SenderId!, privateMessage.SenderUsername!, privateMessage.ReceiverId!,
            privateMessage.ReceiverUsername!, privateMessage.Content!, privateMessage.CreatedAt);

        return Result<PrivateMessageResponseDto>.Success(privateMessageResponse, "Message edited successfully");
    }

    public async Task<Result<PagedMessageHistoryDto>> GetChatMessagesAsync(ClaimsPrincipal claimsPrincipal,
        string chatId, int pageNumber, int pageSize)
    {
        try
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("Unable to find user Id in claims");

                return Result<PagedMessageHistoryDto>.Failure("Unable to retrieve messages");
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogInformation("Unable to find user with given Id {UserId}", userId);

                return Result<PagedMessageHistoryDto>.Failure("Unable to retrieve messages");
            }

            var chat = await _chatRepository.GetChatByIdAsync(chatId);

            if (chat == null)
            {
                _logger.LogInformation("Unable to find chat with given Id {ChatId}", chatId);

                return Result<PagedMessageHistoryDto>.Failure("Unable to retrieve messages");
            }

            var isParticipant = chat.ParticipantIds.Contains(userId);

            if (!isParticipant)
            {
                _logger.LogInformation("User with Id {UserId} is not a participant of chat with Id {ChatId}", userId,
                    chatId);

                return Result<PagedMessageHistoryDto>.Failure("Unable to retrieve messages");
            }

            List<MessageHistoryDto> messages;

            if (chat.ChatType == ChatType.Private)
            {
                var privateMessages =
                    await _privateMessageRepository.GetChatMessagesByChatIdAsync(chatId, pageNumber, pageSize);

                messages = privateMessages.Select(m => new MessageHistoryDto()
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    SenderUsername = m.SenderUsername,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    IsEdited = m.IsEdited,
                    IsOwn = m.SenderId == userId,
                    ReplyToMessageId = m.ReplyToMessageId,
                    Attachments = m.Attachments?.Select(a => new MessageAttachmentDto
                    {
                        FileUrl = a.FileUrl,
                        FileName = a.FileName,
                        FileSize = a.FileSize,
                        FileType = a.FileType
                    }).ToList()
                }).ToList();
            }
            else
            {
                var groupMessages =
                    await _groupMessageRepository.GetChatMessagesByChatIdAsync(chatId, pageNumber, pageSize);

                messages = groupMessages.Select(m => new MessageHistoryDto()
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    SenderUsername = m.SenderUsername,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    IsEdited = m.IsEdited,
                    IsOwn = m.SenderId == userId,
                    ReplyToMessageId = m.ReplyToMessageId,
                    MentionedUserIds = m.MentionedUserIds,
                    Attachments = m.Attachments?.Select(a => new MessageAttachmentDto
                    {
                        FileUrl = a.FileUrl,
                        FileName = a.FileName,
                        FileSize = a.FileSize,
                        FileType = a.FileType
                    }).ToList()
                }).ToList();
            }

            var result = new PagedMessageHistoryDto()
            {
                Messages = messages,
                Page = pageNumber,
                PageSize = pageSize,
                HasMore = messages.Count == pageSize
            };

            return Result<PagedMessageHistoryDto>.Success(result, "");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred retrieving chat messages {exception}", e.Message);

            return Result<PagedMessageHistoryDto>.Failure("Unable to retrieve messages");
        }
    }

    public async Task<Result<string>> MarkGroupMessagesAsReadAsync(
        MarkGroupMessageAsReadRequest markGroupMessageAsReadRequest)
    {
        try
        {
            var validationResult =
                await new MarkGroupMessageAsReadRequestValidator().ValidateAsync(markGroupMessageAsReadRequest);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

                _logger.LogInformation("Validation failed for MarkGroupMessageAsReadRequest with errors {Errors}",
                    string.Join(", ", errorMessages));

                return Result<string>.ValidationFailure("Validation error", errorMessages);
            }

            var groupMessage =
                await _groupMessageRepository.GetMessageByIdAsync(markGroupMessageAsReadRequest.MessageId!);

            if (groupMessage == null)
            {
                _logger.LogInformation("Unable to find group message with given Id {MessageId}",
                    markGroupMessageAsReadRequest.MessageId);

                return Result<string>.Failure("Unable to mark messages as read");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupMessage.GroupId!);

            if (group == null)
            {
                _logger.LogInformation("Unable to find group with given Id {GroupId}",
                    groupMessage.GroupId);

                return Result<string>.Failure("Unable to mark messages as read");
            }

            var isMemberOfGroup = group.Members.Any(m => m.UserId == markGroupMessageAsReadRequest.UserId);

            if (!isMemberOfGroup)
            {
                _logger.LogInformation("User with Id {UserId} is not a member of group with Id {GroupId}",
                    markGroupMessageAsReadRequest.UserId, group.Id);

                return Result<string>.Failure("Unable to mark messages as read");
            }

            var readReceipt = new MessageReadReceiptDocument()
            {
                UserId = markGroupMessageAsReadRequest.UserId!,
                ReadAt = AppExtensions.GetLocalDateTime()
            };

            var messageReadReceipts = groupMessage.ReadBy;

            var messageReadReceiptDocument = messageReadReceipts
                .Select(m => new MessageReadReceiptDocument
                {
                    UserId = m.UserId,
                    ReadAt = m.ReadAt
                })
                .ToList();

            var messageAlreadyRead =
                messageReadReceiptDocument.Any(r => r.UserId == markGroupMessageAsReadRequest.UserId);

            if (messageAlreadyRead)
            {
                _logger.LogInformation("Message with Id {MessageId} is already marked as read by user with Id {UserId}",
                    markGroupMessageAsReadRequest.MessageId, markGroupMessageAsReadRequest.UserId);

                return Result<string>.Success("Message already marked as read");
            }

            messageReadReceiptDocument.Add(readReceipt);

            await _groupMessageRepository.UpdateMessageStatus(groupMessage.Id!, MessageStatus.Read,
                messageReadReceiptDocument);

            return Result<string>.Success("Message marked as read successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An unexpected error occurred marking group messages as read {exception}", e.Message);

            return Result<string>.Failure("Unable to mark messages as read");
        }
    }
}