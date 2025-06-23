using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class WarningCustomer
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("uuid")]
    public string UUId { get; set; }
}

