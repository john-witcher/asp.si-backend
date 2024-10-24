using Api.Managers;
using Api.Models.Db;
using Api.Models.Requests.Users;
using Api.Models.Responses;

namespace Api.Services;

public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserManager _userManager;
    private readonly IdentityManager _identityManager;
    private readonly TransactionManager _transactionManager;

    public UserService(ILogger<UserService> logger, TransactionManager transactionManager, 
        UserManager userManager, IdentityManager identityManager)
    {
        _logger = logger;
        _userManager = userManager;
        _identityManager = identityManager;
        _transactionManager = transactionManager;

        _logger.LogDebug("UserService instantiated");
    }
        
    public async Task<List<IdentityUser>> GetAllUsersAsync()
    {
        return await _userManager.GetAllAsync();
    }
    
    public async Task<IdentityUser> GetUserByUsername(string username)
    {
        return await _userManager.GetByUsernameAsync(username);
    }
    
    public async Task<IdentityUser> GetUserById(Guid userId)
    {
        return await _userManager.GetByIdAsync(userId);
    }
    
    public async Task<ApiResponse> AddUserUserAsync(AddUserRequest request)
    {
        const string roleName = IdentityRole.User;

        await _userManager.ThrowIfUserByNameExistsAsync(request.Username);
        await _identityManager.GetRoleByNameAsync(roleName);

        var newUser = IdentityUser.CreateNewUser(request.Email);

        await _userManager.CreateMongoDbUserAsync(newUser, request.Password);
        
        try
        {
            await _roleManager.AddUserToRole(newUser, roleName);
        }
        catch (Exception e)
        {
            await _transactionManager.ExecuteInTransactionAsync(async session =>
            {
                await _userManager.RemoveOneAsync(request.Email, session);
            });
            
            return ApiResponse.Failed("Unexpected error occured. User was not registered. Try again", e.Message);
        }

        var confirmationLink = await _identityManager.GenerateEmailConfirmationLinkAsync(newUser);
        _ = _emailClient.SendRegistrationConfirmEmailAsync(request.Email, confirmationLink);

        return ApiResponse.Succeeded($"User registered successfully and added to {roleName} role");
    }
    
    public async Task<ApiResponse> RemoveUserAsync(string email)
    {
        await _userManager.GetByUsernameAsync(email);
        await _transactionManager.ExecuteInTransactionAsync(async session =>
        {
            await _userManager.RemoveOneAsync(email, session);
        });
            
        return ApiResponse.Succeeded("User deleted successfully");
    }
}