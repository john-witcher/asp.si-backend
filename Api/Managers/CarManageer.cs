using Api.Clients;
using Api.Exceptions;
using Api.Exceptions.Auth;
using Api.Exceptions.Cars;
using Api.Exceptions.Users;
using Api.Helpers;
using Api.Models.Db;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using IdentityUser = Api.Models.Db.IdentityUser;

namespace Api.Managers;

public class CarManager
{
    private readonly ILogger<CarManager> _logger;
    private readonly DbClient _dbClient;

    public CarManager(ILogger<CarManager> logger, DbClient dbClient)
    {
        _logger = logger;
        _dbClient = dbClient;

        _logger.LogDebug("CarManager instantiated.");
    }
    
    private ProjectionDefinition<Car> GetProjection()
    {
        return Builders<Car>.Projection
            .Exclude(u => u.ChangeLog);
    }
    
    public async Task<Car> GetCarByIdAsync(Guid carId)
    {
        var filter = Builders<Car>.Filter.Eq(car => car.Id, carId);
        var projection = GetProjection();

        var car = await _dbClient.CarsCollection.Find(filter).Project<Car>(projection).FirstOrDefaultAsync();
        if (car == null)
            throw new CarByIdNotFoundException(carId);

        return car;
    }
    
    public async Task<List<IdentityUser>> GetAllAsync()
    {
        var filter = Builders<IdentityUser>.Filter.Empty;
        var projection = GetProjectionForUser();

        return await _dbClient.UserCollection.Find(filter).Project<IdentityUser>(projection).ToListAsync();
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
    
    public async Task RemoveMongoDbUserAsync(string email, IClientSessionHandle session)
    {
        var result = await _dbClient.UserCollection.DeleteOneAsync(session, user => user.Email == email);
        // Check if the operation was acknowledged and if any document was actually deleted
        if (!result.IsAcknowledged || result.DeletedCount != 1)
        {
            var errorMessage = result.IsAcknowledged 
                ? $"No user found with email: {email} to delete. DeletedCount: {result.DeletedCount}."
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
    
    public async Task UpdateUserAsync(Guid userId, IdentityUser updatedUser, IClientSessionHandle session)
    {
        // Step 1: Fetch the current user from the database
        var existingUser = await GetByIdAsync(userId);

        // Step 2: Dynamically build the update definition by comparing the fields
        var updateDefinitions = new List<UpdateDefinition<IdentityUser>>();
        var updateDefinitionBuilder = Builders<IdentityUser>.Update;

        var properties = typeof(IdentityUser).GetProperties();

        foreach (var property in properties)
        {
            var updatedValue = property.GetValue(updatedUser);

            // Update only if the updated value is not null and different from the existing value
            if (updatedValue != null && !Equals(updatedValue, property.GetValue(existingUser)))
            {
                // Add this property to the update definition
                var fieldName = property.Name;
                updateDefinitions.Add(updateDefinitionBuilder.Set(fieldName, updatedValue));
            }
        }

        // Step 3: Combine the update definitions and apply the update operation
        var combinedUpdateDefinition = updateDefinitionBuilder.Combine(updateDefinitions);
        var result = await _dbClient.UserCollection.UpdateOneAsync(session, user => user.Id == userId, combinedUpdateDefinition);

        // Step 4: Handle result and return
        if (!result.IsAcknowledged || result.ModifiedCount != 1)
        {
            throw new DatabaseErrorException(
                message: $"Update failed or no changes were made. MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}"
            );
        }
    }
    

}