using System.Diagnostics.CodeAnalysis;
using ChatApp.SharedKernel.enums;

namespace ChatApp.Application.commands;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CreateGroupCommand
{
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public GroupType Type { get; set; }
}