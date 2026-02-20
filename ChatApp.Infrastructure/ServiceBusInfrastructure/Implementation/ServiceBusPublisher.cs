using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;
using Microsoft.Extensions.Logging;

namespace ChatApp.Infrastructure.ServiceBusInfrastructure.Implementation;

[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class ServiceBusPublisher : IServiceBusPublisher
{
    private readonly ServiceBusClient _serviceBusClient;

    private readonly ILogger<ServiceBusPublisher> _logger;

    private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

    public ServiceBusPublisher(ServiceBusClient serviceBusClient, ILogger<ServiceBusPublisher> logger)
    {
        _serviceBusClient = serviceBusClient;

        _logger = logger;
    }

    public async Task<bool> PushToServiceBusQueueAsync<T>(T message, string queueName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var sender = _senders.GetOrAdd(queueName, _serviceBusClient.CreateSender);

            var body = JsonSerializer.Serialize(message);

            var serviceBusMessage = new ServiceBusMessage(body);

            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogInformation("Message sent to queue {Queue} using provided connection string", queueName);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error occurred while pushing message to Service Bus queue {QueueName}", queueName);
            
            return false;
        }
    }
}