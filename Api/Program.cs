using System.Text.Json.Serialization;
using Api.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

Console.WriteLine("Start");

var builder = WebApplication.CreateBuilder(args);

// If we want to run on custom port, default is 5098
// builder.WebHost.ConfigureKestrel(serverOptions =>
// {
//     serverOptions.ListenAnyIP(5009); 
// });

// Add custom logger
// builder.Logging.AddLoggerExtension(builder.Configuration);


// TODO: rate limiting, ip limit?


// Auth extensions
builder.Services.ConfigureIdentity(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
AuthExtensions.AddAuthorization(builder.Services);

// My extensions
builder.Services.ConfigureCors();
builder.Services.ConfigureIisIntegration();

// Default controllers
builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });

// My settings - from appsettings.json
builder.Services.ConfigureSettings(builder.Configuration);

// My abstraction wrappers
builder.Services.ConfigureClients();
builder.Services.ConfigureManagers();
builder.Services.ConfigureServices();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// APP -----------------------------------------------------------------------------------------------------------------
var app  = builder.Build();

// My middleware, order je pomemben
// app.UseMiddleware<RequestLoggingMiddleware>();
app.ConfigureExceptionHandler();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: ne vem kaj dela
app.UseStaticFiles();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();