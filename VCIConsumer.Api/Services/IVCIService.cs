using VCIConsumer.Api.Models.Request;
namespace VCIConsumer.Api.Services;

public interface IVCIService
{
    Task<IResult> GetTokenAsync(AuthenticationRequest request);
}
