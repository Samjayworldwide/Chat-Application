using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Application.commands;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UserRegistrationCommand
{
    public string? Firstname { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Username { get; set; }
    
    public string? Email { get; set; } 
    
    public string? Password { get; set; }
}