
using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Models.Responses;

public class CustomerDetailResponse
{    
    public required string uuid { get; set; }
    public string hrid { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string email_truncated { get; set; } = string.Empty;
    public string phone_truncated { get; set; } = string.Empty;
    public string created_at { get; set; } = string.Empty;
    public string modified_at { get; set; } = string.Empty;
    public BankAccount bank_account { get; set; }
    public string active { get; set; } = string.Empty;
}

