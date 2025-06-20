using VCIConsumer.Api.Filter;

namespace VCIConsumer.Api.Extensions;

public static class SimpleInputValidationExtensions
{
    public static RouteHandlerBuilder AddSimpleInputValidation<T>(
        this RouteHandlerBuilder builder,
        Func<T, (bool IsValid, string? ErrorMessage)> predicate)
    {
        var filter = new ValidateSimpleInputFilter<T>(predicate);
        return builder.AddEndpointFilter(filter);
    }
}
