using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;

namespace VCIConsumer.Api.Services;

public class OnboardService 
{
    public OnboardService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService)
    {
    }
}
