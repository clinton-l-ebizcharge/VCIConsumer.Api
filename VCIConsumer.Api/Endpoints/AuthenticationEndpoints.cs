using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VCIConsumer.Api.Models.Requests;
using VCIConsumer.Api.Models.Responses;
using VCIConsumer.Api.Services;

namespace VCIConsumer.Api.Endpoints;

public static class AuthenticationEndpoints
{
    public static void ConfigureAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/authentication", GetAccessTokenAsync)
              .WithName("Get Access Token")
              .Accepts<AuthenticationRequest>("application/json")
              .Produces<ApiResponse>(StatusCodes.Status200OK)
              .Produces<ApiResponse>(StatusCodes.Status400BadRequest)
              .Produces<ApiResponse>(StatusCodes.Status500InternalServerError);
    }

    [Tags("Authentication")]
    private static async Task<IResult> GetAccessTokenAsync(
        [FromServices] IAuthenticationService service, 
        [FromServices] IHttpClientFactory httpClientFactory, 
        [FromBody] AuthenticationRequest request)
    {
        var response = await service.GetAccessTokenAsync(request);
        return Results.Ok(response);
    }

    private static string GetDebuggerDisplay(this Program program) => $"{program.GetType().Name}";
}
