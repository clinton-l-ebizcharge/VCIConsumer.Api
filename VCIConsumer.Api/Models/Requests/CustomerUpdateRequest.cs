
using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Requests;

public class CustomerUpdateRequest
{
    public required string Uuid { get; set; } 
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("bank_account")]
    public BankAccountRequest BankAccount { get; set; }
}
