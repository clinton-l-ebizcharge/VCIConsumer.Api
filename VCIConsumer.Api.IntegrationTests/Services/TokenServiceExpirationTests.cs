using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.IntegrationTests.Setup;
using VCIConsumer.Api.IntegrationTests.Support;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Services;
using Xunit;

namespace VCIConsumer.Api.IntegrationTests.Services;

public class TokenServiceExpirationTests : IClassFixture<TestServerSetup>
{
    private readonly ITokenService _tokenService;
    private readonly Mock<ILogger<TokenService>> _mockLogger = new();
    private readonly Mock<ITimeProvider> _clockMock;
    private readonly ExpiringTokenHandler _ExpiringTokenhandler;

    public TokenServiceExpirationTests(TestServerSetup factory)
    {
        _clockMock = factory.ClockMock;

        // Inject a known expiring token
        _ExpiringTokenhandler = new ExpiringTokenHandler("initial-token", DateTime.UtcNow.AddSeconds(1));
        var client = new HttpClient(_ExpiringTokenhandler) { BaseAddress = new Uri("https://mock-api.com") };

        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                   .Returns(() => new HttpClient(Handler)
                   {
                       BaseAddress = new Uri(baseUrl ?? "https://mock-api.com")
                   });

        var settings = Options.Create(new ApiSettings
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            BaseUrl = "https://mock-api.com"
        });

        _tokenService = new TokenService(settings, factoryMock.Object, NullLogger<TokenService>.Instance, _clockMock.Object);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsCachedToken_WhenNotExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var factory = new TokenServiceTestFactory(initialToken: "cached-token");

        factory.ClockMock.SetupSequence(c => c.UtcNow)
                         .Returns(now)
                         .Returns(now.AddSeconds(30)); // Within expiration window

        // Act
        var firstToken = await factory.TokenService.GetTokenAsync();
        var secondToken = await factory.TokenService.GetTokenAsync();

        // Assert
        Assert.Equal("cached-token", firstToken);
        Assert.Equal(firstToken, secondToken);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsNewToken_WhenExpired()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var factory = new TokenServiceTestFactory(initialToken: "initial-token");

        factory.ClockMock.SetupSequence(c => c.UtcNow)
                         .Returns(now)                 // Token issued
                         .Returns(now.AddSeconds(70));  // Simulate token expiry

        factory.Handler.NextToken = "refreshed-token"; // Prepare next token for refresh

        // Act
        var token1 = await factory.TokenService.GetTokenAsync();
        var token2 = await factory.TokenService.GetTokenAsync();

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.Equal("refreshed-token", token2);
    }


    [Fact]
    public async Task TokenExpires_WhenClockMovesForward()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var factory = new TokenServiceTestFactory(initialToken: "initial-token");

        factory.ClockMock.SetupSequence(c => c.UtcNow)
                         .Returns(now)                  // Token created
                         .Returns(now.AddSeconds(70));  // Simulated passage of time

        factory.Handler.NextToken = "refreshed-token"; // Simulate refreshable token downstream

        // Act
        var token1 = await factory.TokenService.GetTokenAsync();
        var token2 = await factory.TokenService.GetTokenAsync();

        // Assert
        Assert.NotEqual(token1, token2);
        Assert.Equal("refreshed-token", token2);
    }
}
