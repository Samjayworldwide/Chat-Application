using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

public class EmailVerificationRepository(IMongoDatabase mongoDatabase) : IEmailVerificationRepository
{
    private readonly IMongoCollection<EmailVerificationCodeDocument> _emailVerifications =
        mongoDatabase.GetCollection<EmailVerificationCodeDocument>(AppExtensions.EmailVerificationDocumentName);

    public async Task CreateVerificationTokenAsync(EmailVerificationCode emailVerificationCode)
    {
        var emailVerificationCodeDocument = EmailVerificationCodeMapper.ToDocument(emailVerificationCode);

        await _emailVerifications.InsertOneAsync(emailVerificationCodeDocument);
    }

    public async Task<EmailVerificationCode?> GetEmailVerificationAsync(string email)
    {
        var filter = Builders<EmailVerificationCodeDocument>.Filter.Eq(doc => doc.Email, email);

        var emailVerificationCode = await _emailVerifications.Find(filter).FirstOrDefaultAsync();

        return emailVerificationCode == null ? null : EmailVerificationCodeMapper.ToEntity(emailVerificationCode);
    }

    public async Task UpdateVerificationTokenAsync(EmailVerificationCode emailVerificationCode)
    {
        var emailVerificationCodeDocument = EmailVerificationCodeMapper.ToDocument(emailVerificationCode);

        var filter =
            Builders<EmailVerificationCodeDocument>.Filter.Eq(doc => doc.Email, emailVerificationCodeDocument.Email);

        await _emailVerifications.ReplaceOneAsync(filter, emailVerificationCodeDocument);
    }
}