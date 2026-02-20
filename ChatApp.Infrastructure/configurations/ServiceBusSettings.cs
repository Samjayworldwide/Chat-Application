using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.configurations;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class ServiceBusSettings
{
    public required string ConnectionString { get; set; }
    
    public required string EmailQueueName { get; set; }
}