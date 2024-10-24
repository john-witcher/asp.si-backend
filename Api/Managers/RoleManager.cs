using Api.Exceptions;
using Api.Exceptions.Auth;
using Microsoft.AspNetCore.Identity;
using IdentityRole = Api.Models.Db.IdentityRole;
using IdentityUser = Api.Models.Db.IdentityUser;

namespace Api.Managers;

public class RoleManager
{
    private readonly ILogger<RoleManager> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userIdentityManager;

    public RoleManager(ILogger<RoleManager> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userIdentityManager)
    {
        _logger = logger;
        _roleManager = roleManager;
        _userIdentityManager = userIdentityManager;
        
        _logger.LogDebug("RoleManager instantiated.");
    }

    // ROLES -----------------------------------------------------------------------------------------------------------
    public List<IdentityRole> GetAllRoles()
    {
        _logger.LogInformation("Fetching all roles.");
        return _roleManager.Roles.ToList();
    }
    
    public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
    {
        _logger.LogInformation($"Fetching role by name: {roleName}");
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            _logger.LogWarning($"Role '{roleName}' not found.");
            throw new RoleNotFoundException(roleName);
        }

        _logger.LogInformation($"Role '{roleName}' found.");
        return role;
    }
    
    public async Task<IdentityRole> CreateRoleAsync(string roleName)
    {
        _logger.LogInformation($"Attempting to create role: {roleName}");
        var newRole = new IdentityRole(roleName);
        var createResult = await _roleManager.CreateAsync(newRole);
        if (!createResult.Succeeded)
        {
            var errorMessages = createResult.Errors.Select(error => error.Description).ToList();
            _logger.LogError($"Failed to create role '{roleName}': {string.Join(", ", errorMessages)}");
            throw new DatabaseErrorException($"Failed to create role '{roleName}'.", errorMessages);
        }

        _logger.LogInformation($"Role '{roleName}' created successfully.");
        return newRole;
    }
    
    public async Task DeleteRoleAsync(IdentityRole role)
    {
        _logger.LogInformation($"Attempting to delete role: {role.Name}");
        var deleteResult = await _roleManager.DeleteAsync(role);
        if (!deleteResult.Succeeded)
        {
            var errorMessages = deleteResult.Errors.Select(error => error.Description).ToList();
            _logger.LogError($"Failed to delete role '{role.Name}': {string.Join(", ", errorMessages)}");
            throw new DatabaseErrorException($"Failed to delete role '{role.Name}'.", errorMessages);
        }

        _logger.LogInformation($"Role '{role.Name}' deleted successfully.");
    }
    
    public async Task ThrowIfRoleExistAsync(string roleName)
    {
        _logger.LogInformation($"Checking if role '{roleName}' exists.");
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            _logger.LogWarning($"Role '{roleName}' already exists.");
            throw new RoleAlreadyExistsException(roleName);
        }

        _logger.LogInformation($"Role '{roleName}' does not exist.");
    }
    
    
    // USER ROLES ------------------------------------------------------------------------------------------------------
    public async Task<IList<string>> GetUserRoles(IdentityUser user)
    {
        _logger.LogInformation($"Fetching roles for user: {user.UserName}");
        return await _userIdentityManager.GetRolesAsync(user);
    }
    
    public async Task AddUserToRole(IdentityUser newUser, string roleName)
    {
        _logger.LogInformation($"Attempting to add user '{newUser.UserName}' to role '{roleName}'.");
        var result = await _userIdentityManager.AddToRoleAsync(newUser, roleName);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(error => error.Description).ToList();
            _logger.LogError($"Failed to add user '{newUser.UserName}' to role '{roleName}': {string.Join(", ", errorMessages)}");
            throw new DatabaseErrorException($"Failed to add user to role '{roleName}'.", errorMessages);
        }

        _logger.LogInformation($"User '{newUser.UserName}' added to role '{roleName}' successfully.");
    }
    
    public async Task RemoveUserFromRoles(IdentityUser user, IList<string> roles)
    {
        _logger.LogInformation($"Attempting to remove user '{user.UserName}' from roles: {string.Join(", ", roles)}.");
        var result = await _userIdentityManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(error => error.Description).ToList();
            _logger.LogError($"Failed to remove user '{user.UserName}' from roles: {string.Join(", ", errorMessages)}");
            throw new DatabaseErrorException($"Failed to remove user from roles.", errorMessages);
        }

        _logger.LogInformation($"User '{user.UserName}' removed from roles successfully.");
    }

    public async Task ThrowIfAnyUserHasRoleAsync(string roleName)
    {
        _logger.LogInformation($"Checking if any users are assigned to role '{roleName}'.");
        var users = await _userIdentityManager.GetUsersInRoleAsync(roleName);
        if (users.Any())
        {
            _logger.LogWarning($"Cannot delete role '{roleName}' because users are assigned to it.");
            throw new CantDeleteRoleException(roleName);
        }

        _logger.LogInformation($"No users are assigned to role '{roleName}', role can be deleted.");
    }
}
