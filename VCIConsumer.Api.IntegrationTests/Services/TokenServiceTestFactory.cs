using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.IntegrationTests.Setup;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.IntegrationTests.Support;

public class TokenServiceTestFactory
{
    public Mock<ITimeProvider> ClockMock { get; } = new();
    public ExpiringTokenHandler Handler { get; }
    public ITokenService TokenService { get; }

    public TokenServiceTestFactory(string initialToken = "initial-token", string? baseUrl = null)
    {
        DateTime now = DateTime.UtcNow;
        var expiration = now.AddSeconds(60);
        Handler = new ExpiringTokenHandler(initialToken, expiration);

        // Instead of creating a single HttpClient, supply a factory that returns a new instance every time.
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
            BaseUrl = (baseUrl ?? "https://mock-api.com")
        });

        // Default clock setup (this can be overridden in individual tests)
        ClockMock.Setup(tp => tp.UtcNow).Returns(now);

        TokenService = new TokenService(settings, mockFactory.Object, new NullLogger<TokenService>(), ClockMock.Object);
    }
}

