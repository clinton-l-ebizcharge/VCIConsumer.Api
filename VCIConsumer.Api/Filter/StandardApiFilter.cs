using System.Net;
using System.Reflection;
using VCIConsumer.Api.Models.Responses;

namespace VCIConsumer.Api.Filter;

public class StandardApiFilter<T> : IEndpointFilter
{
    private readonly ILogger _logger;

    public StandardApiFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<StandardApiFilter<T>>();
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {            
            var endpointMethod = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<MethodInfo>();
            if (endpointMethod != null &&
               (endpointMethod.IsDefined(typeof(SkipStandardFilterAttribute), inherit: true) ||
                endpointMethod.DeclaringType?.IsDefined(typeof(SkipStandardFilterAttribute), inherit: true) == true))
            {
                return await next(context);
            }

            var result = await next(context);

            // If the endpoint already returned IResult, don't wrap it again
            if (result is IResult existing) return existing;

            var correlationId = context.HttpContext.TraceIdentifier;

            // Ensure 'Result' is set in the object initializer
            var apiResponse = new ApiResponse
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Result = result ?? new object(),
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            };

            return Results.Ok(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error");

            var correlationId = context.HttpContext.TraceIdentifier;

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Result = new object(),
                Errors = new List<ErrorResponse>
                                {
                                    new ErrorResponse
                                    {
                                        Code = "500",
                                        Message = ex.Message,
                                        Type = "Exception"
                                    }
                                },
                CorrelationId = correlationId,
                Timestamp = DateTime.UtcNow
            };

            return Results.Json(apiResponse, statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}
