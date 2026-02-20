using System.Diagnostics.CodeAnalysis;
using ChatApp.Domain.entities;
using ChatApp.Infrastructure.documents;
using ChatApp.Infrastructure.mappers;
using ChatApp.Infrastructure.repositories.interfaces;
using ChatApp.SharedKernel.extensions;
using MongoDB.Driver;

namespace ChatApp.Infrastructure.repositories.implementations;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
public class GroupRepository : IGroupRepository
{
    private readonly IMongoCollection<GroupDocument> _groups;

    public GroupRepository(IMongoDatabase database)
    {
        _groups = database.GetCollection<GroupDocument>(AppExtensions.GroupDocumentName);
    }

    public async Task<bool> GroupNameExistsForUser(string groupName, string userId)
    {
        var filter = Builders<GroupDocument>.Filter.And(
            Builders<GroupDocument>.Filter.Eq(g => g.Name, groupName),
            Builders<GroupDocument>.Filter.ElemMatch(
                g => g.Members,
                m => m.UserId == userId
            )
        );

        var options = new FindOptions
        {
            Collation = new Collation("en", strength: CollationStrength.Secondary) // case-insensitive
        };

        return await _groups.Find(filter, options).AnyAsync();
    }

    public async Task<Group> CreateGroupAsync(Group group)
    {
        var groupDocument = GroupMapper.ToDocument(group);

        await _groups.InsertOneAsync(groupDocument);

        return GroupMapper.ToEntity(groupDocument);
    }

    public async Task<Group?> GetGroupByIdAsync(string groupId)
    {
        var filter = Builders<GroupDocument>.Filter.Eq(g => g.Id, groupId);

        var groupDocument = await _groups.Find(filter).FirstOrDefaultAsync();

        return groupDocument != null ? GroupMapper.ToEntity(groupDocument) : null;
    }

    public async Task UpdateGroupAsync(Group group)
    {
        var filter = Builders<GroupDocument>.Filter.Eq(g => g.Id, group.Id);

        var groupDocument = GroupMapper.ToDocument(group);

        await _groups.ReplaceOneAsync(filter, groupDocument);
    }

    public async Task<List<Group>> GetAllUserGroupsAsync(string userId)
    {
        var filter = Builders<GroupDocument>.Filter.ElemMatch(
            g => g.Members,
            m => m.UserId == userId
        );

        var groupDocuments = await _groups.Find(filter).ToListAsync();

        return groupDocuments.Select(GroupMapper.ToEntity).ToList();
    }
}