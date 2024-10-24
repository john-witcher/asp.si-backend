using Api.Models.ApiRoutes;
using Api.Models.Requests.Auth;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route(BaseRoutes.AuthController)]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly AuthenticationService _authService;

    public AuthenticationController(ILogger<AuthenticationController> logger, AuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;

        _logger.LogDebug("AuthenticationController instantiated.");
    }

    // AUTH ------------------------------------------------------------------------------------------------------------
    [HttpPost(AuthRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }
        
    [HttpGet(AuthRoutes.Logout)]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.LogoutAsync();

        return Ok(result);
    }
    
    // EMAIL -----------------------------------------------------------------------------------------------------------
    [HttpGet(AuthRoutes.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail([FromQuery(Name = AuthRoutes.QueryEmail)]string email, [FromQuery(Name = AuthRoutes.QueryToken)] string token)
    {
        var result = await _authService.ConfirmEmailAsync(email, token);
            
        return Ok(result);
    }

    // [HttpPost(AuthRoutes.ResendConfirmationEmail)]
    // public async Task<IActionResult> ResendConfirmationEmail(ResendEmailRequest request)
    // {
    //     var result = await _authService.ResendConfirmationEmailAsync(request);
    //         
    //     return Ok(result);
    // }
    
    // PASSWORD --------------------------------------------------------------------------------------------------------
    [HttpPost(AuthRoutes.RequestPasswordReset)]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest emailRequest)
    {
        var result = await _authService.RequestPasswordResetAsync(emailRequest);
        
        return Ok(result);
    }
    
    [HttpPost(AuthRoutes.ResetPassword)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        return Ok(result);
    }
}