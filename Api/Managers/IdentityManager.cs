using Api.Exceptions;
using Api.Exceptions.Auth;
using Api.Helpers;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Api.Models.Db.IdentityUser;

namespace Api.Managers;

public class IdentityManager
{
    private readonly ILogger<IdentityManager> _logger;
    private readonly UserManager<IdentityUser> _userIdentityManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityManager(ILogger<IdentityManager> logger, UserManager<IdentityUser> userIdentityManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _userIdentityManager = userIdentityManager;
        _signInManager = signInManager;

        _logger.LogDebug("IdentityManager instantiated.");
    }
    
    public async Task LoginAsync(IdentityUser user, string password)
    {
        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!signInResult.Succeeded)
            throw new InvalidCredentialsException();
    }
    
    public async Task ResetPasswordAsync(IdentityUser user, string token, string password)
    {
        string decodedToken = Encryption.DecodeToken(token);
        
        var result = await _userIdentityManager.ResetPasswordAsync(user, decodedToken, password);
        if (!result.Succeeded)
            throw new DatabaseErrorException($"Password reset failed. {result}");
    }
}