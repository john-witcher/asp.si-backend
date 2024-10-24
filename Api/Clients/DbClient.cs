using Api.Models;
using Api.Models.Db;
using Api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Clients;

public class DbClient
{
    private readonly ILogger<DbClient> _logger;

    public readonly MongoClient DatabaseClient;
    public readonly MongoClient LogClient;
    public readonly IMongoCollection<IdentityUser> UserCollection;
    public readonly IMongoCollection<Car> CarsCollection;

    public DbClient(ILogger<DbClient> logger, IOptions<DbSettings> settings)
    {
        _logger = logger;
        DatabaseClient = new MongoClient(settings.Value.Authentication.ConnectionString); //dev or prod glede na contex
        var database = DatabaseClient.GetDatabase(settings.Value.Authentication.DatabaseName);
        
        LogClient = new MongoClient(settings.Value.Logs.ConnectionString);
        var logDatabase = LogClient.GetDatabase(settings.Value.Logs.DatabaseName);

        UserCollection = database.GetCollection<IdentityUser>("Users");
        CarsCollection = database.GetCollection<Car>("Car");
        
        _logger.LogDebug("DbClient instantiated");
    }
}