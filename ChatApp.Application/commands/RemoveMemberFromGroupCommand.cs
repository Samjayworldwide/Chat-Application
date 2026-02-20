using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Application.commands;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RemoveMemberFromGroupCommand
{
    public string? UserId { get; set; }
    
    public string? GroupId { get; set; }
}