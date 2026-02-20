using ChatApp.Domain.entities;

namespace ChatApp.Infrastructure.repositories.interfaces;

public interface IEmailVerificationRepository
{
    Task CreateVerificationTokenAsync(EmailVerificationCode emailVerificationCode);
    
    Task<EmailVerificationCode?> GetEmailVerificationAsync(string email);
    
    Task UpdateVerificationTokenAsync(EmailVerificationCode emailVerificationCode);
}