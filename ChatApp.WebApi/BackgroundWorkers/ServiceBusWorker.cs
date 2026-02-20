using System.Diagnostics.CodeAnalysis;
using ChatApp.Infrastructure.ServiceBusInfrastructure.Interface;

namespace ChatApp.WebApi.BackgroundWorkers;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class ServiceBusWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ServiceBusWorker(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var processors = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IServiceBusProcessorBase>>();

        foreach (var processor in processors)
        {
            await processor.StartAsync(stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var processors = scope.ServiceProvider
            .GetRequiredService<IEnumerable<IServiceBusProcessorBase>>();

        foreach (var processor in processors)
        {
            await processor.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}