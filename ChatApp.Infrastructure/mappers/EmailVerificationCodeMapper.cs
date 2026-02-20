using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;

namespace ChatApp.Infrastructure.mappers;

public static class EmailVerificationCodeMapper
{
    public static EmailVerificationCodeDocument ToDocument(EmailVerificationCode emailVerificationCode)
    {
        return new EmailVerificationCodeDocument
        {
            Id = emailVerificationCode.Id,
            Email = emailVerificationCode.Email,
            VerificationCode = emailVerificationCode.VerificationCode,
            IsVerified = emailVerificationCode.IsVerified,
            CreatedAt = emailVerificationCode.CreatedAt,
            ExpiresAt = emailVerificationCode.ExpiresAt
        };
    }
    
    public static EmailVerificationCode ToEntity(EmailVerificationCodeDocument emailVerificationCodeDocument)
    {
        return new EmailVerificationCode
        {
            Id = emailVerificationCodeDocument.Id,
            Email = emailVerificationCodeDocument.Email,
            VerificationCode = emailVerificationCodeDocument.VerificationCode,
            IsVerified = emailVerificationCodeDocument.IsVerified
        };
    }
}