namespace VCIConsumer.Api.Models;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
