using System.Text;
using System.Text.Json;
using Api.Settings;
using Microsoft.Extensions.Options;

namespace Api.Clients;

public class DiscordClient
{
    private readonly ILogger<DiscordClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _logWebhookUrl;
    private readonly string _errorWebhookUrl;

    public DiscordClient(ILogger<DiscordClient> logger, HttpClient httpClient, IOptions<DiscordSettings> settings)
    {
        _logger = logger;
        _httpClient = httpClient;

        _logWebhookUrl = settings.Value.Logs;
        _errorWebhookUrl = settings.Value.Errors;
            
        _logger.LogDebug("DiscordClient instantiated");
    }

    private async Task<HttpResponseMessage> SendMessageAsync(string title, string error, string webhookUrl)
    {
        var requestBody = CreateDiscordMessage(title, error);

        if (requestBody.Length > 2000)
        {
            error = error.Substring(0, requestBody.Length - (requestBody.Length - 2000));
        }
            
        requestBody = CreateDiscordMessage(title, error);

        var stringContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(webhookUrl, stringContent);
    }

    private string CreateDiscordMessage(string title, string error)
    {
        var data = new
        {
            username = "GUI Error",
            content = title,
            embeds = new[]
            {
                new
                {
                    title = "Hello, Embed!",
                    description = error
                }
            }
        };

        return JsonSerializer.Serialize(data);
    }
    public async Task SendDiscordLogMessage(string title, string error)
    {
        // TODO: predelat
        try
        {
            var response = await SendMessageAsync(title, error, _logWebhookUrl);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Discord LOG message sent successfully");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                await SendDiscordErrorMessage(title, error);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Exception in SendDiscordLogMessage: \n {e}");
        }
    }
    public async Task SendDiscordErrorMessage(string title, string error)
    {
        try
        {
            var response = await SendMessageAsync(title, error, _errorWebhookUrl);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Discord ERROR message sent successfully");
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogCritical($"Status code was not success in SendDiscordErrorMessage: \n {errorMessage}");
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Exception in SendDiscordErrorMessage: \n {e}");
        }
    }
}