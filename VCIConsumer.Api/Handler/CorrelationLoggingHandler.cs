using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Handler;

public class CorrelationLoggingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<CorrelationLoggingHandler> _logger;
    private readonly LoggingOptions _logOptions;

    public CorrelationLoggingHandler(
        IHttpContextAccessor contextAccessor,
        ILogger<CorrelationLoggingHandler> logger,
        IOptions<LoggingOptions> loggingOptions)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
        _logOptions = loggingOptions.Value ?? new LoggingOptions();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId =
            _contextAccessor.HttpContext?.TraceIdentifier ??
            Activity.Current?.Id ??
            Guid.NewGuid().ToString();

        request.Headers.TryAddWithoutValidation("X-Correlation-ID", correlationId);

        if (request.Content is not null)
        {
            var rawBody = await request.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogApiRequest("Outbound HTTP Request", new
            {
                request.Method,
                request.RequestUri,
                request.Headers,
                Body = TryParseJson(rawBody)
            }, _contextAccessor.HttpContext);
        }

        var stopwatch = Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        var responseBody = response.Content != null
            ? await response.Content.ReadAsStringAsync(cancellationToken)
            : null;

        _logger.LogInformation("Received response {StatusCode} in {ElapsedMs}ms\nBody:\n{PrettyBody}",
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            TryFormatJson(responseBody));

        return response;
    }

    private static object TryParseJson(string body)
    {
        try
        {
            return JsonSerializer.Deserialize<object>(body) ?? body;
        }
        catch
        {
            return body;
        }
    }

    private static string? TryFormatJson(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            var parsed = JsonSerializer.Deserialize<object>(body);
            return JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return body;
        }
    }
}

