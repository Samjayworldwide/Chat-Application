using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Application.commands;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class AddMemberToGroupCommand
{
    public string? Username { get; set; }
    
    public string? GroupId { get; set; }
}