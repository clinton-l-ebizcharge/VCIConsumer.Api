using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses
{
    public class Warning
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
