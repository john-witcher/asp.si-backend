using Api.Clients;
using Api.Exceptions;
using Api.Exceptions.Users;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using IdentityUser = Api.Models.Db.IdentityUser;

namespace Api.Managers;

public class UserManager
{
    private readonly ILogger<UserManager> _logger;
    private readonly DbClient _dbClient;
    private readonly UserManager<IdentityUser> _userIdentityManager;

    public UserManager(ILogger<UserManager> logger, DbClient dbClient, UserManager<IdentityUser> userIdentityManager)
    {
        _logger = logger;
        _dbClient = dbClient;
        _userIdentityManager = userIdentityManager;
        
        _logger.LogDebug("UserManager instantiated.");
    }

    private ProjectionDefinition<IdentityUser> GetProjectionForUser()
    {
        return Builders<IdentityUser>.Projection
            .Exclude(u => u.PasswordHash)
            .Exclude(u => u.SecurityStamp)
            .Exclude(u => u.ConcurrencyStamp)
            .Exclude(u => u.PhoneNumberConfirmed)
            .Exclude(u => u.TwoFactorEnabled)
            .Exclude(u => u.LockoutEnd)
            .Exclude(u => u.LockoutEnabled)
            .Exclude(u => u.AccessFailedCount)
            .Exclude(u => u.Claims)
            .Exclude(u => u.Roles)
            .Exclude(u => u.Logins)
            .Exclude(u => u.Tokens);
    }

    public async Task<List<IdentityUser>> GetAllAsync()
    {
        var filter = Builders<IdentityUser>.Filter.Empty;
        var projection = GetProjectionForUser();

        return await _dbClient.UserCollection.Find(filter).Project<IdentityUser>(projection).ToListAsync();
    }
    
    public async Task<IdentityUser> GetByIdAsync(Guid userId)
    {
        var filter = Builders<IdentityUser>.Filter.Eq(user => user.Id, userId);
        var projection = GetProjectionForUser();

        var user = await _dbClient.UserCollection.Find(filter).Project<IdentityUser>(projection).FirstOrDefaultAsync();
        if (user == null)
            throw new UserByIdNotFoundException(userId);

        return user;
    }
    
    public async Task<IdentityUser> GetByUsernameAsync(string username)
    {
        var filter = Builders<IdentityUser>.Filter.Eq(user => user.UserName, username);
        var projection = GetProjectionForUser();

        var user = await _dbClient.UserCollection.Find(filter).Project<IdentityUser>(projection).FirstOrDefaultAsync();
        if (user == null)
            throw new UserByEmailNotFoundException(username);

        return user;
    }
    
    public async Task CreateOneAsync(IdentityUser newUser, string password)
    {
        var result = await _userIdentityManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
            throw new DatabaseErrorException(
                $"Failed to create user.", result.Errors.Select(error => error.Description).ToList());
    }
    
    public async Task RemoveOneAsync(string username, IClientSessionHandle session)
    {
        var result = await _dbClient.UserCollection.DeleteOneAsync(session, user => user.UserName == username);
        if (!result.IsAcknowledged || result.DeletedCount != 1)
        {
            var errorMessage = result.IsAcknowledged 
                ? $"No user found with username: {username} to delete. DeletedCount: {result.DeletedCount}."
                : $"Delete operation was not acknowledged by DB. DeletedCount: {result.DeletedCount}.";

            throw new DatabaseErrorException(errorMessage);
        }
    }
    
    public async Task ThrowIfUserByNameExistsAsync(string username)
    {
        var filter = Builders<IdentityUser>.Filter.Eq(user => user.UserName, username);
        var projection = GetProjectionForUser();
        
        var user = await _dbClient.UserCollection.Find(filter).Project<IdentityUser>(projection).FirstOrDefaultAsync();
        if (user != null)
            throw new UserNameIdNotFoundException(username);
    }
    
    public async Task UpdateUserAsync(IdentityUser updatedUser, IClientSessionHandle session)
    {
        var existingUser = await GetByUsernameAsync(username);


        var result = await _dbClient.UserCollection.UpdateOneAsync(session, updatedUser);
        if (!result.IsAcknowledged || result.ModifiedCount != 1)
        {
            throw new DatabaseErrorException(
                message: $"Update failed or no changes were made. MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}"
            );
        }
    }
}