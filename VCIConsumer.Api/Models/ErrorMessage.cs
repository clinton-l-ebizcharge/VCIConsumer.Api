namespace VCIConsumer.Api.Models;

public class ErrorMessage
{
    public required string code { get; set; }
    public required string message { get; set; }
    public required string type { get; set; }
}
