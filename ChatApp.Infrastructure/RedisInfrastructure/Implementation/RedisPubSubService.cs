using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ChatApp.Infrastructure.RedisInfrastructure.Interfaces;
using ChatApp.SharedKernel.dtos.response;
using ChatApp.SharedKernel.extensions;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ChatApp.Infrastructure.RedisInfrastructure.Implementation;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
[SuppressMessage("ReSharper", "AsyncVoidLambda")]
public class RedisPubSubService : IRedisPubSubService
{
    private readonly ISubscriber _subscriber;

    private readonly IChatNotifier _chatNotifier;

    private readonly ILogger<RedisPubSubService> _logger;

    private static string PrivateMessageChannel(string userId) => $"private:message:{userId}";

    private static string PrivateMessageEditedChannel(string userId) => $"private:edited:{userId}";

    public RedisPubSubService(IConnectionMultiplexer redis, IChatNotifier chatNotifier,
        ILogger<RedisPubSubService> logger)
    {
        _subscriber = redis.GetSubscriber();

        _chatNotifier = chatNotifier;

        _logger = logger;
    }

    public async Task PublishPrivateMessageAsync(string receiverId, PrivateMessageResponseDto message)
    {
        var channel = PrivateMessageChannel(receiverId);

        var payload = JsonSerializer.Serialize(message);

        await _subscriber.PublishAsync(RedisChannel.Literal(channel), payload);

        _logger.LogInformation("Published private message to channel {Channel}", channel);
    }

    public async Task PublishPrivateMessageEditedAsync(string receiverId, PrivateMessageResponseDto message)
    {
        var channel = PrivateMessageEditedChannel(receiverId);

        var payload = JsonSerializer.Serialize(message);

        await _subscriber.PublishAsync(RedisChannel.Literal(channel), payload);

        _logger.LogInformation("Published private message to channel {Channel}", channel);
    }

    public async Task SubscribeToPrivateMessagesAsync(string userId)
    {
        var channel = PrivateMessageChannel(userId);

        await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), async (_, value) =>
        {
            if (value.IsNullOrEmpty) return;

            var message = JsonSerializer.Deserialize<PrivateMessageResponseDto>((string)value!);

            if (message != null)
                await _chatNotifier.SendPrivateMessageAsync(userId, message);
        });

        _logger.LogInformation("Subscribed to private message channel {Channel}", channel);
    }

    public async Task SubscribeToPrivateMessageEditsAsync(string userId)
    {
        var channel = PrivateMessageEditedChannel(userId);

        await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), async (_, value) =>
        {
            if (value.IsNullOrEmpty) return;

            var message = JsonSerializer.Deserialize<PrivateMessageResponseDto>((string)value!);

            if (message != null)
                await _chatNotifier.SendMessageEditedAsync(userId, message);
        });

        _logger.LogInformation("Subscribed to private message edit channel {Channel}", channel);
    }
}