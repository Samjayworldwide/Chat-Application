using ChatApp.Application.commons;
using ChatApp.SharedKernel.dtos.request;

namespace ChatApp.Application.interfaces;

public interface IEmailVerificationService
{
    Task<Result<string>> SendVerificationCodeToEmailAsync(string email);
    
    Task<Result<string>> VerifyCodeAsync(EmailVerificationRequest emailVerificationRequest);
    
    Task<bool> IsEmailVerifiedAsync(string email);
}