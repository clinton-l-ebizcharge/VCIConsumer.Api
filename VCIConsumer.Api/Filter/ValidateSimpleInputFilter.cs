using VCIConsumer.Api.Extensions;

namespace VCIConsumer.Api.Filter;

public class ValidateSimpleInputFilter<T> : IEndpointFilter
{
    private readonly Func<T, (bool IsValid, string? ErrorMessage)> _predicate;
    private readonly ILogger<ValidateSimpleInputFilter<T>>? _logger;

    public ValidateSimpleInputFilter(
        Func<T, (bool IsValid, string? ErrorMessage)> predicate,
        ILogger<ValidateSimpleInputFilter<T>>? logger = null)
    {
        _predicate = predicate;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var value = ctx.Arguments.OfType<T>().FirstOrDefault();
        if (value is not null)
        {
            var (isValid, message) = _predicate(value);
            if (!isValid)
            {
                var correlationId = ctx.HttpContext.TraceIdentifier;
                _logger?.LogWarning("Validation failed for input: {Input}. CorrelationId: {CorrelationId}. Reason: {Reason}",
                    value, correlationId, message);

                return (message ?? "Invalid input.").ToValidationFailure();
            }
        }

        return await next(ctx);
    }
}




