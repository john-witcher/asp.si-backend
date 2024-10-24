using Api.Settings;

namespace Api.Extensions;

public static class SettingsExtension
{
    public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));
        services.Configure<DbSettings>(configuration.GetSection("MongoDb"));
        services.Configure<DiscordSettings>(configuration.GetSection("Discord"));
    }
}

