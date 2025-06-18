
using System.Text.Json.Serialization;
using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Models.Requests;

public class CustomerCreationRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("bank_account")]
    public BankAccount BankAccount { get; set; }
}
