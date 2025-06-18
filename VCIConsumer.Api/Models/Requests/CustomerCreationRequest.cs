
using VCIConsumer.Api.Models;

namespace VCIConsumer.Api.Models.Requests;

public class CustomerCreationRequest
{
    public required string name { get; set; }
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public BankAccount bank_account { get; set; }
}
