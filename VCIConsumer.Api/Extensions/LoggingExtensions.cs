using System.Text.Json;

namespace VCIConsumer.Api.Extensions;

public static class LoggingExtensions
{
    public static void LogApiRequest<T>(this ILogger logger, string operation, T payload, HttpContext? httpContext = null)
    {
        var correlationId = httpContext?.TraceIdentifier ?? "N/A";
        var prettyJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });

        logger.LogInformation("API call: {Operation}\nPayload={PayloadType}\nCorrelationId={CorrelationId}\nPayload:\n{PrettyPayload}",
            operation, typeof(T).Name, correlationId, prettyJson);

    }

}
