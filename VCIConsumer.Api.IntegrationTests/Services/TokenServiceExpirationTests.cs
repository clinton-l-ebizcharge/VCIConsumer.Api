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

    private readonly IHttpClientFactory _mockHttpClientFactory;

    public TokenServiceExpirationTests()
    {
        // Setup IHttpClientFactory mock
        var handler = new ExpiringTokenHandler("cached-token", expiresInSeconds: 3600);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(_mockSettings.BaseUrl)
        };

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        _mockHttpClientFactory = mockFactory.Object;
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsCachedToken_WhenNotExpired()
    {
        // Arrange
        var service = new TokenService(
            Options.Create(_mockSettings), _mockHttpClientFactory, _mockLogger.Object);

        // Act
        var firstToken = await service.GetTokenAsync();
        var secondToken = await service.GetTokenAsync();

        // Assert
        Assert.Equal(firstToken, secondToken);
        Assert.Equal("cached-token", firstToken);
    }

    [Fact]
    public async Task GetTokenAsync_FetchesNewToken_WhenExpired()
    {
        // Arrange
        var handler = new ExpiringTokenHandler("initial-token", expiresInSeconds: 1);
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(_mockSettings.BaseUrl)
        };

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);
        var service = new TokenService(
            Options.Create(_mockSettings), mockFactory.Object, _mockLogger.Object);

        // Act
        var token1 = await service.GetTokenAsync();
        await Task.Delay(2000);
        handler.NextToken = "refreshed-token";
        var token2 = await service.GetTokenAsync();

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.Equal("refreshed-token", token2);
    }
}

