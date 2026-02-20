using ChatApp.Infrastructure.models;

namespace ChatApp.Infrastructure.EmailInfrastructure.Interface;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDetails emailDetails);
}