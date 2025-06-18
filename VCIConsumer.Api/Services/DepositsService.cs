using Microsoft.Extensions.Options;
using VCIConsumer.Api.Configuration;

namespace VCIConsumer.Api.Services;

public class DepositsService : ServiceBase
{
    public DepositsService(IOptions<ApiSettings> apiSettings, IHttpClientFactory httpClientFactory, TokenService tokenService) : base(apiSettings, httpClientFactory, tokenService)
    {
    }
}
