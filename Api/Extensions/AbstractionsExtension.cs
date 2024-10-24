using Api.Clients;
using Api.Managers;
using Api.Services;

namespace Api.Extensions;

public static class AbstractionsExtension
{
    public static void ConfigureClients(this IServiceCollection services)
    {
        services.AddHttpClient<DiscordClient>();
        services.AddSingleton<DbClient>();
    }
    
    public static void ConfigureManagers(this IServiceCollection services)
    {
        services.AddSingleton<TransactionManager>();
        services.AddScoped<UserManager>();
        services.AddScoped<IdentityManager>();
        services.AddScoped<RoleManager>();
        // services.AddSingleton<MongoLoggerManager>();
    }
    
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<RoleService>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<UserService>();
        // services.AddScoped<GuiErrorsService>();
    }
}

