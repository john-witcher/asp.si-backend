namespace Api.Settings;

public class AuthSettings
{
    public required JwtSettings Jwt { get; set; }
    public required string AllowedUserNameCharacters { get; set; } 
    public required string AllowedEmailCharacters { get; set; } 
}
    
public class JwtSettings
{
    public required string ValidIssuer { get; set; }
    public required string ValidAudience { get; set; }
    public required string IssuerSigningKeySalt { get; set; }
    public int TokenLifetimeMinutes { get; set; }
}

public static class AuthPolicy
{
    public const string Admin = "Admin";
    public const string User = "User";
}

