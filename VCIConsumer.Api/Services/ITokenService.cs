
namespace VCIConsumer.Api.Services;

public interface ITokenService
{
    Task<string> GetTokenAsync();
}