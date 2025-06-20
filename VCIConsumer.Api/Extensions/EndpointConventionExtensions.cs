using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Extensions;

public static class EndpointProducesExtensions
{
    public static RouteHandlerBuilder WithStandardApiResponses(this RouteHandlerBuilder builder)
    {
        return builder
            .Produces<ApiResponse>(StatusCodes.Status200OK)
            .Produces<ApiResponse>(StatusCodes.Status400BadRequest, "application/json")
            .Produces<ApiResponse>(StatusCodes.Status401Unauthorized, "application/json")
            .Produces<ApiResponse>(StatusCodes.Status500InternalServerError, "application/json");
    }
}
