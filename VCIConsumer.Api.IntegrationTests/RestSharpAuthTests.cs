using RestSharp;

namespace VCIConsumer.Api.IntegrationTests;

public class RestSharpAuthTests
{
    [Fact]
    public async Task PostAuthentication_ReturnsTokenOrError()
    {
        var options = new RestClientOptions("https://api.sandbox.vericheck.com/authentication");
        var client = new RestClient(options);

        var request = new RestRequest("", Method.Post);
        request.AddHeader("accept", "application/json");
        request.AddHeader("Idempotency-Key", Guid.NewGuid().ToString());
        request.AddHeader("VeriCheck-Version", "1.13");
        request.AddJsonBody(new
        {
            client_id = "cf98c85e-9309-4bae-8c94-f9bc24565ce0",
            client_secret = "j-T8Q~afY7~b8wUqnLH963pkyC1GSMjNfBbmVbbR"
        });

        var response = await client.ExecuteAsync(request);

        // Log response for debugging
        Console.WriteLine(response.Content);

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Content));
        Assert.Contains("access_token", response.Content ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}