using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class UserMapper
{
    public static UserDocument ToDocument(User user)
    {
        return new UserDocument
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Username = user.Username,
            Email = user.Email,
            Password = user.Password,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
    }
    
    public static User ToEntity(UserDocument userDocument)
    {
        return new User
        {
            Id = userDocument.Id,
            Firstname = userDocument.Firstname,
            Lastname = userDocument.Lastname,
            Username = userDocument.Username,
            Email = userDocument.Email,
            Password = userDocument.Password,
            AvatarUrl = userDocument.AvatarUrl,
            Status = userDocument.Status,
            CreatedAt = userDocument.CreatedAt
        };
    }
}