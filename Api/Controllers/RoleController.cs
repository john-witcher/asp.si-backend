using Api.Models.ApiRoutes;
using Api.Services;
using Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize(Policy = AuthPolicy.Admin)]
[Route(BaseRoutes.RoleController)]
public class RoleController : ControllerBase
{
    private readonly ILogger<RoleController> _logger;
    private readonly RoleService _roleService;

    public RoleController(ILogger<RoleController> logger, RoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;

        _logger.LogDebug("RoleController instantiated.");
    }
        
    [HttpGet]
    public IActionResult GetAllRoles()
    {
        var result = _roleService.GetAllRoles();

        return Ok(result);
    }
        
    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetRoleByName(string roleName)
    {
        var result = await _roleService.GetRoleByNameAsync(roleName);
    
        return Ok(result);
    }
        
    [HttpPost]
    public async Task<IActionResult> AddRole([FromBody] string roleName)
    {
        var result = await _roleService.AddRoleAsync(roleName);

        return Ok(result);
    }
        
    [HttpDelete("{roleName}")]
    public async Task<IActionResult> RemoveRole(string roleName)
    {
        var result = await _roleService.RemoveRoleAsync(roleName);
    
        return Ok(result);
    }  
}