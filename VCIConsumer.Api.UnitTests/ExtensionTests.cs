using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VCIConsumer.Api.Extensions;
using VCIConsumer.Api.Models.Responses;
using Xunit;

// Fix the ambiguity by removing the duplicate method in TestHelpers class.
namespace VCIConsumer.Api.UnitTests;

public static class TestHelpers
{
    public static async Task<ApiResponse> ExtractResponseAsync(IResult result)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        await result.ExecuteAsync(context);
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return (await JsonSerializer.DeserializeAsync<ApiResponse>(context.Response.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }))!;
    }
}

// Test classes
public class DateTimeExtensionsTests
{
    [Fact]
    public void AsUtc_AlreadyUtc_DoesNothing()
    {
        var utcNow = DateTime.UtcNow;
        var result = utcNow.AsUtc();

        Assert.Equal(utcNow, result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void AsUtc_NonUtc_ConvertsKindOnly()
    {
        var local = DateTime.Now;
        var result = local.AsUtc();

        Assert.Equal(local.Ticks, result.Ticks);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }
}

public class HttpExtensionsTests
{
    [Fact]
    public void ToFormattedString_JoinsAllHeaders()
    {
        var request = new HttpRequestMessage();
        request.Headers.Add("X-Correlation-Id", "abc123");
        request.Headers.Add("Accept", new[] { "application/json", "text/plain" });

        var result = request.Headers.ToFormattedString();
        Assert.Contains("X-Correlation-Id: abc123", result);
        Assert.Contains("Accept: application/json, text/plain", result);
    }
}

public class LoggingExtensionsTests
{
    private class InMemoryLogger : ILogger
    {
        public string? Captured { get; private set; }
        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel level, EventId id, TState state, Exception? ex, Func<TState, Exception?, string> formatter)
        {
            Captured = formatter(state, ex);
        }
    }

    [Fact]
    public void LogApiRequest_OutputsExpectedContent()
    {
        var logger = new InMemoryLogger();
        var payload = new { Name = "Foo", Id = 123 };
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-abc";

        logger.LogApiRequest("FetchCustomer", payload, context);

        Assert.Contains("FetchCustomer", logger.Captured);
        Assert.Contains("trace-abc", logger.Captured);
        Assert.Contains("Name", logger.Captured);
        Assert.Contains("123", logger.Captured);
    }
}

public class UriBuilderExtensionsTests
{
    [Fact]
    public void AddQueryParameters_EncodesProperly()
    {
        var builder = new UriBuilder("https://test.com/resource");
        var queryParams = new Dictionary<string, string?>
        {
            ["sort"] = "name asc",
            ["limit"] = "10",
            ["skip"] = null,
            ["empty"] = ""
        };

        builder.AddQueryParameters(queryParams);

        var result = builder.Query;
        Assert.Contains("sort=name+asc", result);
        Assert.Contains("limit=10", result);
        Assert.DoesNotContain("skip", result);
        Assert.DoesNotContain("empty", result);
    }
}
