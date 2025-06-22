namespace VCIConsumer.Api.Extensions;

public static class DateTimeExtensions
{
    public static DateTime AsUtc(this DateTime value)
    {
        return value.Kind == DateTimeKind.Utc
            ? value
            : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}

