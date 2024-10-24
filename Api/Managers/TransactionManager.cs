using Api.Clients;
using MongoDB.Driver;

namespace Api.Managers;

public class TransactionManager
{
    private readonly ILogger<TransactionManager> _logger;

    //TODO: put this in middleware
    private readonly DbClient _dbClient;

    public TransactionManager(ILogger<TransactionManager> logger, DbClient dbClient)
    {
        _logger = logger;
        _dbClient = dbClient;
        
        _logger.LogDebug("TransactionManager instantiated.");
    }
    
    public async Task ExecuteInTransactionAsync(Func<IClientSessionHandle, Task> operation)
    {
        using var session = await _dbClient.DatabaseClient.StartSessionAsync();
        
        session.StartTransaction();
        try
        {
            await operation(session);

            await session.CommitTransactionAsync();
        }
        catch
        {
            // TODO: test če passa naprej eror
            await session.AbortTransactionAsync();
            throw;
        }
    }
}