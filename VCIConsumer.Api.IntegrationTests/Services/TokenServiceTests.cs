using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.IntegrationTests.Support;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.IntegrationTests.Services;

public class TokenServiceTests : IClassFixture<TestServerSetup>
{
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Mock<ILogger<TokenService>> _mockLogger;
    private readonly Mock<ITimeProvider> _clockMock;

    public TokenServiceTests(TestServerSetup factory)
    {
        var scope = factory.Services.CreateScope();
        _clockMock = factory.ClockMock;
        _tokenService = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<ITokenService>();

        var services = new ServiceCollection();
        services.AddHttpClient();
        var serviceProvider = services.BuildServiceProvider();
        _httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

        _mockLogger = new Mock<ILogger<TokenService>>();

        var mockSettings = Options.Create(new ApiSettings
        {
            BaseUrl = "https://mock-api.com",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        });

        _clockMock.SetupSequence(c => c.UtcNow)
             .Returns(DateTime.UtcNow)               // token creation
             .Returns(DateTime.UtcNow.AddSeconds(70));

        _tokenService = new TokenService(mockSettings, _httpClientFactory, _mockLogger.Object, _clockMock.Object);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsValidToken()
    {
        // Arrange
        var factory = new TokenServiceTestFactory(initialToken: "mock-access-token");

        // Act
        var token = await factory.TokenService.GetTokenAsync();

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

        var failingHttpClientFactory = new Mock<IHttpClientFactory>();
        failingHttpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(handler));

        var failingService = new TokenService(Options.Create(new ApiSettings()), failingHttpClientFactory.Object,_mockLogger.Object, _clockMock.Object);
        
        await Assert.ThrowsAsync<HttpRequestException>(() => failingService.GetTokenAsync());
    }
}

