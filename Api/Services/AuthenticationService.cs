using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Clients;
using Api.Exceptions.Auth;
using Api.Managers;
using Api.Models.Db;
using Api.Models.Requests.Auth;
using Api.Models.Responses;
using Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Services;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AuthSettings _authSettings;
    private readonly UserManager _userManager;
    private readonly IdentityManager _identityManager;
    private readonly RoleManager _roleManager;

    public AuthenticationService(ILogger<AuthenticationService> logger, IOptions<AuthSettings> authSettings, UserManager userManager, IdentityManager identityManager, RoleManager roleManager)
    {
        _logger = logger;
        _authSettings = authSettings.Value;
        _userManager = userManager;
        _identityManager = identityManager;
        _roleManager = roleManager;

        _logger.LogDebug("AuthenticationService instantiated");
    }
    
    
    // AUTH ------------------------------------------------------------------------------------------------------------
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.GetMongoDbUserByEmailAsync(request.Email);

        await _identityManager.ThrowIfEmailNotConfirmed(user);
        await _identityManager.LoginAsync(user, request.Password);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, request.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var roles = await _roleManager.GetUserRoles(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettings.Jwt.IssuerSigningKeySalt));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_authSettings.Jwt.TokenLifetimeMinutes);
        
        var token = new JwtSecurityToken(
            issuer: _authSettings.Jwt.ValidIssuer,
            audience: _authSettings.Jwt.ValidAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );
       
        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
            JwtExpire = expires,
            Email = request.Email,
            Company = user.CompanyName ?? string.Empty,
            GlobalSettings = user.GlobalSettings ?? new GlobalSettings(),
            // WorkLayoutData = user.WorkLayoutData,
            // GlobalSettingsData = user.GlobalSettingsData
        };
    }
    
    public async Task<ApiResponse> LogoutAsync()
    {
        // This does nothing. Da bi to delal je treba shrant cookies, pa trackat sessionse, ni za zdej
        // Zdej je dovolj da zbrišemo JWT na clientu.
        // Mogl bi implementirat blacklist tokens pa to
        
        // await _identityManager.SignOutAsync();

        return ApiResponse.Succeeded("Logout successful");
    }

    
    // EMAIL -----------------------------------------------------------------------------------------------------------
    public async Task<ApiResponse> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.GetMongoDbUserByEmailAsync(email);

        await _identityManager.ConfirmEmailAsync(user, token);

        return ApiResponse.Succeeded("Email was successfully confirmed");
    }
    €
    
    // PASSWORD --------------------------------------------------------------------------------------------------------
    public async Task<ApiResponse> RequestPasswordResetAsync(RequestPasswordResetRequest emailRequest)
    {
        var user = await _userManager.GetMongoDbUserByEmailAsync(emailRequest.Email);

        var resetPasswordLink = await _identityManager.GeneratePasswordResetLinkAsync(user);
        _ = _emailClient.SendResetPasswordEmailAsync(emailRequest.Email, resetPasswordLink);
        
        return ApiResponse.Succeeded("Thank you. If your email address is associated with an account in our system, you will receive an email with instructions to reset your password shortly.");
    }
    
    public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.GetMongoDbUserByEmailAsync(request.Email);
        
        await _identityManager.ResetPasswordAsync(user, request.Token, request.Password);

        return ApiResponse.Succeeded("Password has been reset");
    }
}