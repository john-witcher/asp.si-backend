using Api.Managers;
using Api.Models.Db;
using Api.Models.Responses;

namespace Api.Services;

public class RoleService
{
    private readonly ILogger<RoleService> _logger;
    private readonly RoleManager _roleManager;

    public RoleService(ILogger<RoleService> logger, RoleManager roleManager)
    {
        _logger = logger;
        _roleManager = roleManager;

        _logger.LogDebug("RoleService instantiated");
    }

    public List<IdentityRole> GetAllRoles()
    {
        return _roleManager.GetAllRoles();
    }
    
    public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
    {
        return await _roleManager.GetRoleByNameAsync(roleName);
    }
    
    public async Task<ApiResponse> AddRoleAsync(string roleName)
    {
        await _roleManager.ThrowIfRoleExistAsync(roleName);
        var newRole = await _roleManager.CreateRoleAsync(roleName);
        
        return ApiResponse.Succeeded($"Role '{roleName}' created successfully");
    }
    
    public async Task<ApiResponse> RemoveRoleAsync(string roleName)
    {
        var role = await _roleManager.GetRoleByNameAsync(roleName);
        await _roleManager.ThrowIfAnyUserHasRoleAsync(roleName);
        await _roleManager.DeleteRoleAsync(role);

        return ApiResponse.Succeeded($"Role '{roleName}' deleted successfully.");
    }
}