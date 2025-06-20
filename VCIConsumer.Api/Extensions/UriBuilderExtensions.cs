using System.Net;

namespace VCIConsumer.Api.Extensions;

public static class UriBuilderExtensions
{
    public static void AddQueryParameters(this UriBuilder builder, Dictionary<string, string?> parameters)
    {
        builder.Query = string.Join("&", parameters
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
    }
}

