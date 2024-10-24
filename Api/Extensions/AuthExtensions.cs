using System.Text;
using Api.Helpers;
using Api.Models;
using Api.Settings;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using IdentityRole = Api.Models.Db.IdentityRole;
using IdentityUser = Api.Models.Db.IdentityUser;
using MongoDbSettings = AspNetCore.Identity.MongoDbCore.Infrastructure.MongoDbSettings;


namespace Api.Extensions;

public static class AuthExtensions
{
    public static void ConfigureIdentity(this  IServiceCollection services, IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDb").Get<DbSettings>();
        if (mongoSettings == null)
            throw new NullReferenceException("Could not read MongoDb section of app settings.json");
            
        var authSettings = configuration.GetSection("Auth").Get<AuthSettings>();
        if (authSettings == null)
            throw new NullReferenceException("Could not read Auth section of app settings.json");
        
        var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
        {
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = mongoSettings.Authentication.ConnectionString,
                DatabaseName = mongoSettings.Authentication.DatabaseName
            }, 

            IdentityOptionsAction = options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = authSettings.AllowedEmailCharacters;

                options.SignIn.RequireConfirmedEmail = true;
            }
        };


        services.ConfigureMongoDbIdentity<IdentityUser, IdentityRole, Guid>(mongoDbIdentityConfig)
            .AddUserManager<UserManager<IdentityUser>>()
            .AddSignInManager<SignInManager<IdentityUser>>()
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddDefaultTokenProviders();
    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("Auth").Get<AuthSettings>();
        if (authSettings == null)
            throw new NullReferenceException("Could not read Auth section of app settings.json");
            
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = true;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = authSettings.Jwt.ValidIssuer,
                ValidAudience = authSettings.Jwt.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.Jwt.IssuerSigningKeySalt)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }
    public static void AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicy.Admin, policy => policy.RequireRole(IdentityRole.Admin).RequireAuthenticatedUser());
            options.AddPolicy(AuthPolicy.User, policy => policy.RequireRole(IdentityRole.Admin, IdentityRole.User).RequireAuthenticatedUser());
        });
    }
}