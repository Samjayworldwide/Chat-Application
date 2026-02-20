using ChatApp.Domain.entities;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username);
    
    Task CreateUserAsync(User user);
    
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
    
    Task UpdateUserStatusAsync(string userId, bool isOnline);
    
    Task<User?> GetByIdAsync(string userId);
    
    Task UpdateUserAsync(User user);
}