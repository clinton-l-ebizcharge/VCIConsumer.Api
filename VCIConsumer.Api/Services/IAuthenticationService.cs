
using VCIConsumer.Api.Models.Requests;

namespace VCIConsumer.Api.Services;

public interface IAuthenticationService
{
    Task<string> GetAccessTokenAsync(AuthenticationRequest request);
}