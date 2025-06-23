using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses
{
    public class CustomerSummary
    {
        // Extend with actual properties returned in the "customer" object
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("customer_id")]
        public string? CustomerId { get; set; }
    }
}
