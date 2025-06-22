using System.Net;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.IntegrationTests.Setup;
public class ExpiringTokenHandler : HttpMessageHandler
{
    private readonly DateTime _expiresAt;
    public string CurrentToken { get; private set; }
    public string? NextToken { get; set; }
    public int CallCount { get; private set; } = 0;

    public ExpiringTokenHandler(string initialToken, DateTime expiresAt)
    {
        CurrentToken = initialToken;
        _expiresAt = expiresAt; 
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        CallCount++;
        var response = new AuthenticationResponse
        {
            AccessToken = NextToken ?? CurrentToken,
            TokenType = "Bearer",
            ExpirationDateTime = _expiresAt // Corrected property name and logic
        };

        var json = JsonSerializer.Serialize(response);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}

