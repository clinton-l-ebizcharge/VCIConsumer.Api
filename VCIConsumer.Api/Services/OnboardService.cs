using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;

namespace VCIConsumer.Api.Services;

public class OnboardService : ServiceBase
{
    public OnboardService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService) : base(apiSettings, httpClientFactory, tokenService)
    {
    }
}
