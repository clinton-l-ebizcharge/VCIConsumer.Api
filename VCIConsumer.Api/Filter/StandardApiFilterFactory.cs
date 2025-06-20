namespace VCIConsumer.Api.Filter;

public class StandardApiFilterFactory : IEndpointFilterFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public StandardApiFilterFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public EndpointFilterDelegate CreateFilter(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var method = context.MethodInfo;

        // Skip if the method or declaring type has [SkipStandardFilter]
        if (method.IsDefined(typeof(SkipStandardFilterAttribute), inherit: true) ||
            method.DeclaringType?.IsDefined(typeof(SkipStandardFilterAttribute), inherit: true) == true)
        {
            return next;
        }

        // Determine response type from method signature
        var returnType = context.MethodInfo.ReturnType;

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = returnType.GetGenericArguments()[0];

            var filterType = typeof(StandardApiFilter<>).MakeGenericType(resultType);
            var filterInstance = Activator.CreateInstance(filterType, _loggerFactory)!;
            var invokeMethod = filterType.GetMethod("InvokeAsync")!;

            return async invocationContext =>
                await (ValueTask<object?>)invokeMethod.Invoke(filterInstance, new object[] { invocationContext, next })!;
        }

        // Fallback if return type isn’t generic
        return next;
    }
}

