namespace VCIConsumer.Api.Extensions;

public static class QueryBuilderExtensions
{
    public static void AddIfNotNullOrWhiteSpace(this IDictionary<string, string?> dict, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            dict[key] = value;
    }

    public static void AddIfHasValue<T>(this IDictionary<string, string?> dict, string key, T? value)
        where T : struct
    {
        if (value.HasValue)
            dict[key] = value.Value.ToString();
    }
}

