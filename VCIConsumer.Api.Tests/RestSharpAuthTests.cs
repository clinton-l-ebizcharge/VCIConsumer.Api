using RestSharp;

namespace VCIConsumer.Api.Tests
{
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
                client_id = "62b35185-7adb-4ba0-8abf-1877c9a6786a",
                client_secret = "NDK7Q~Xro8IVPny7TpqLw6ucIPbq7qr_-mdwa"
            });

            var response = await client.ExecuteAsync(request);

            // Log response for debugging
            Console.WriteLine(response.Content);

            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response.Content));
            Assert.Contains("access_token", response.Content ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}