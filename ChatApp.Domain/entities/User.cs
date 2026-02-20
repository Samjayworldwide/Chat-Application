using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.enumerations;

namespace ChatApp.Domain.entities;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class User
{
    public string? Id { get; set; }
    
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
    
    public string? AvatarUrl { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Offline;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static User Create(string firstname, string lastname, string username, string email, string password)
    {
        return new User
        {
            Firstname = firstname,
            Lastname = lastname,
            Username = username,
            Email = email,
            Password = password,
            Status = UserStatus.Offline,
            CreatedAt = DateTime.UtcNow
        };
    }
}