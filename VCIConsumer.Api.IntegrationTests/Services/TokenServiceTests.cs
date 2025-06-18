using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.IntegrationTests.Services;

public class TokenServiceTests : IClassFixture<TestServerSetup>
{
    private readonly ITokenService _tokenService;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<TokenService>> _mockLogger;

    public TokenServiceTests(TestServerSetup setup)
    {
        _httpClient = setup.CreateClient();
        _mockLogger = new Mock<ILogger<TokenService>>();

        var mockSettings = Options.Create(new ApiSettings
        {
            BaseUrl = "https://mock-api.com",            
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        });

        _tokenService = new TokenService(mockSettings, _httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsValidToken()
    {
        // Act
        var token = await _tokenService.GetTokenAsync();

        // Assert
        Assert.NotNull(token);
        Assert.Equal("mock-access-token", token);
    }

    [Fact]
    public async Task GetTokenAsync_ThrowsException_WhenApiFails()
    {
        // Arrange: Simulate failed request
        var handler = new MockHttpClientHandler();
        handler.SetupSendAsync(new HttpResponseMessage(HttpStatusCode.BadRequest));

        var failingHttpClient = new HttpClient(handler);
        var failingService = new TokenService(Options.Create(new ApiSettings()), failingHttpClient, _mockLogger.Object);

        // Act & Assert: Expect exception due to failed API call
        await Assert.ThrowsAsync<HttpRequestException>(() => failingService.GetTokenAsync());
    }
}

