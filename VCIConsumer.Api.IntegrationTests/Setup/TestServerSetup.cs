using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using VCIConsumer.Api;
using VCIConsumer.Api.Configuration;
using VCIConsumer.Api.Services;

public class TestServerSetup : WebApplicationFactory<VCIConsumer.Api.Program> // Correctly reference the 'Program' class  
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace HttpClient with a mock handler for testing  
            var httpClientDescriptors = services.Where(d => d.ServiceType == typeof(HttpClient)).ToList();
            foreach (var descriptor in httpClientDescriptors)
            {
                services.Remove(descriptor);
            }

            // Ensure AddHttpClient is available and ITokenService/TokenService are resolved  
            services.AddHttpClient<ITokenService, TokenService>()
                .ConfigurePrimaryHttpMessageHandler(() => new MockHttpClientHandler());
        });     
    }
}
