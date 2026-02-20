namespace ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;

public interface IServiceBusPublisher
{
    Task<bool> PushToServiceBusQueueAsync<T>(T message, string queueName,
        CancellationToken cancellationToken = default);
}