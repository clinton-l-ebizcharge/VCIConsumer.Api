using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class CustomerListResponse
{
    [JsonPropertyName("hrid")]
    public string HRId { get; set; }

    [JsonPropertyName("uuid")]
    public string UUId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email_truncated")]
    public string EmailTruncated { get; set; }

    [JsonPropertyName("phone_truncated")]
    public string PhoneTruncated { get; set; }

    [JsonPropertyName("bank_account")]
    public BankAccountResponse BankAccount { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("modified_at")]
    public string ModifiedAt { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}

