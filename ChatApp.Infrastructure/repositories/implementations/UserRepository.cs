using ChatApp.Domain.entities;
using ChatApp.Domain.enumerations;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _users =
        database.GetCollection<UserDocument>(AppExtensions.UserDocumentName);

    public async Task<bool> ExistsByEmailOrUsernameAsync(string email, string username)
    {
        var filter = Builders<UserDocument>.Filter.Or(Builders<UserDocument>.Filter.Eq(u => u.Email, email),
            Builders<UserDocument>.Filter.Eq(u => u.Username, username)
        );

        var options = new FindOptions
        {
            Collation = new Collation("en", strength: CollationStrength.Secondary) // case-insensitive
        };

        return await _users.Find(filter, options).AnyAsync();
    }

    public async Task CreateUserAsync(User user)
    {
        var userDocument = UserMapper.ToDocument(user);

        await _users.InsertOneAsync(userDocument);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var filter = Builders<UserDocument>.Filter.Or(Builders<UserDocument>.Filter.Eq(u => u.Email, emailOrUsername),
            Builders<UserDocument>.Filter.Eq(u => u.Username, emailOrUsername)
        );

        var options = new FindOptions
        {
            Collation = new Collation("en", strength: CollationStrength.Secondary) // case-insensitive
        };

        var userDocument = await _users.Find(filter, options).FirstOrDefaultAsync();

        return userDocument != null ? UserMapper.ToEntity(userDocument) : null;
    }

    public async Task UpdateUserStatusAsync(string userId, bool isOnline)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);

        var update = Builders<UserDocument>.Update
            .Set(u => u.Status, isOnline ? UserStatus.Online : UserStatus.Offline)
            .Set(u => u.LastSeenAt, isOnline ? null : AppExtensions.GetLocalDateTime());

        await _users.UpdateOneAsync(filter, update);
    }

    public async Task<User?> GetByIdAsync(string userId)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, userId);

        var userDocument = await _users.Find(filter).FirstOrDefaultAsync();

        return userDocument != null ? UserMapper.ToEntity(userDocument) : null;
    }

    public Task UpdateUserAsync(User user)
    {
        var filter = Builders<UserDocument>.Filter.Eq(u => u.Id, user.Id);

        var userDocument = UserMapper.ToDocument(user);

        return _users.ReplaceOneAsync(filter, userDocument);
    }
}