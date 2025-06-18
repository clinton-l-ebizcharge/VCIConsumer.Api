using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.IntegrationTests.Setup;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.IntegrationTests.Services;

public class TokenServiceExpirationTests
{
    private readonly Mock<ILogger<TokenService>> _mockLogger = new();

    private readonly ApiSettings _mockSettings = new()
    {
        ClientId = "test-client-id",
        ClientSecret = "test-client-secret",
        BaseUrl = "https://mock-api.com",
    };

    [Fact]
    public async Task GetTokenAsync_ReturnsCachedToken_WhenNotExpired()
    {
        // Arrange: Create a handler that returns a mock token
        var handler = new ExpiringTokenHandler("cached-token", expiresInSeconds: 3600);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(_mockSettings.BaseUrl)
        };

        var service = new TokenService(
            Options.Create(_mockSettings), client, _mockLogger.Object);

        // Act: Call twice and expect the second one to use the cached token
        var firstToken = await service.GetTokenAsync();
        var secondToken = await service.GetTokenAsync();

        // Assert
        Assert.Equal(firstToken, secondToken);
        Assert.Equal("cached-token", firstToken);
        Assert.Equal(1, handler.CallCount); // Ensure token was fetched only once
    }

    [Fact]
    public async Task GetTokenAsync_FetchesNewToken_WhenExpired()
    {
        // Arrange: Simulate short-lived token by using a 1-second expiration
        var handler = new ExpiringTokenHandler("initial-token", expiresInSeconds: 1);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(_mockSettings.BaseUrl)
        };

        var service = new TokenService(
            Options.Create(_mockSettings), client, _mockLogger.Object);

        // Act
        var token1 = await service.GetTokenAsync(); // This will be cached
        await Task.Delay(2000);                    // Wait until token expires
        handler.NextToken = "refreshed-token";     // Simulate backend giving a new one
        var token2 = await service.GetTokenAsync();

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.Equal("refreshed-token", token2);
        Assert.Equal(2, handler.CallCount); // Fetched token twice
    }
}

