using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.configurations;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class BlobStorageSettings
{
    public string? ConnectionString { get; set; }
    
    public string? ContainerName { get; set; }
}