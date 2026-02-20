using System.Diagnostics.CodeAnalysis;

namespace ChatApp.SharedKernel.dtos.response;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UserLoginResponse
{
    public required string Id { get; set; }
    
    public required string Username { get; set; }
    
    public required string JwtToken { get; set; }
}