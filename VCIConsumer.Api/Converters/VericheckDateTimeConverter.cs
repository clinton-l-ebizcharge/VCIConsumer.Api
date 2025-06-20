using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Converters;

public class VericheckDateTimeConverter : JsonConverter<DateTime>
{
    private const string ExpectedFormat = "MMM d, yyyy, h:mm:ss tt";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (DateTime.TryParseExact(
                value,
                ExpectedFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            return parsed.ToUniversalTime();
        }

        throw new JsonException($"Unable to parse '{value}' to DateTime with expected format '{ExpectedFormat}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(ExpectedFormat, CultureInfo.InvariantCulture));
    }
}
