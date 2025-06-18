using System.Net;
using System.Text;
using System.Text.Json;
using VCIConsumer.Api.Models.Responses;

public class MockHttpClientHandler : HttpMessageHandler // Define the missing MockHttpClientHandler class  
{
    private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? _sendAsyncFunc;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_sendAsyncFunc != null)
        {
            return _sendAsyncFunc(request, cancellationToken);
        }

        throw new InvalidOperationException("SendAsync function is not set up.");
    }

    public Task<HttpResponseMessage> SendAsyncWithToken(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new AuthenticationResponse
        {
            AccessToken = "mock-access-token",
            TokenType = "Bearer",
            ExpiresIn = DateTime.UtcNow.AddHours(1)
        };

        var jsonResponse = JsonSerializer.Serialize(response);

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        });
    }
    
    public void SetupSendAsync(HttpResponseMessage responseMessage)
    {
        _sendAsyncFunc = (request, cancellationToken) => Task.FromResult(responseMessage);
    }
}