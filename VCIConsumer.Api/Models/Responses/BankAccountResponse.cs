using System.Text.Json.Serialization;

namespace VCIConsumer.Api.Models.Responses;

public class BankAccountResponse : BaseBankAccountResponse
{
    [JsonPropertyName("account_validated")]
    public bool AccountValidated { get; set; }
}
