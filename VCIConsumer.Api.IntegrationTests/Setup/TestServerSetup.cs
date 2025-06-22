using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Net.Http;
using VCIConsumer.Api;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.IntegrationTests.Setup;
using VCIConsumer.Api.Models;
using VCIConsumer.Api.Services;

public class TestServerSetup : WebApplicationFactory<VCIConsumer.Api.Program>
{
    public Mock<ITimeProvider> ClockMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHttpClientFactory>();
            services.RemoveAll<ITimeProvider>();

            // Configure test clock
            ClockMock.Setup(c => c.UtcNow).Returns(() => DateTime.UtcNow);
            services.AddSingleton<ITimeProvider>(ClockMock.Object);

            // Mock HTTP response
            var handler = new ExpiringTokenHandler("test-token", DateTime.UtcNow.AddMinutes(1));
            var mockClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://mock-api.com")
            };

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockClient);

            services.AddSingleton(mockFactory.Object);
        });
    }
}

