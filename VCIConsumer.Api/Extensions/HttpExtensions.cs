using System.Net.Http.Headers;

namespace VCIConsumer.Api.Extensions;

public static class HttpExtensions
{
    public static string ToFormattedString(this HttpHeaders headers) =>
        string.Join("\n", headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"));
}

