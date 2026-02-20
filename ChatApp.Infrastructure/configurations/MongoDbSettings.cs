using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.configurations;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MongoDbSettings
{
    public string? ConnectionString { get; set; }
    
    public string? DatabaseName { get; set; }
}