namespace ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;

public interface IServiceBusProcessorBase : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken);
}