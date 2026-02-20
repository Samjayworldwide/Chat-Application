using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Infrastructure.configurations;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class JwtSettings
{
    public required string Issuer { get; set; }
    
    public required string Audience { get; set; }
    
    public required string Key { get; set; }
}