using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Application.commands;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UserLoginCommand
{
    public string? EmailOrUsername { get; set; }
    
    public string? Password { get; set; }
}