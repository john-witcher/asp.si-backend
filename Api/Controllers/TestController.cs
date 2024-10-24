

using System.Security.Claims;
using Api.Clients;
using Api.Managers;
using Api.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Api.Controllers;


[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly DbClient _dbClient;
    private readonly RoleManager _roleManager;
    private readonly IdentityManager _identityManager;
    private readonly TransactionManager _transactionManager;

    public TestController(ILogger<TestController> logger, RoleManager roleManager, DbClient dbClient, 
        IdentityManager identityManager, TransactionManager transactionManager)
    {
        _logger = logger;
        _roleManager = roleManager;
        _dbClient = dbClient;
        _identityManager = identityManager;
        _transactionManager = transactionManager;

        _logger.LogDebug("TestController instantiated.");
    }
    

    
    [HttpGet("transaction-session-example")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        await _transactionManager.ExecuteInTransactionAsync(async session =>
        {
            // Primer: await _companyManager.UpdateOneAsync(companyName, company, session);
        });
        
        return Ok();
    }
}