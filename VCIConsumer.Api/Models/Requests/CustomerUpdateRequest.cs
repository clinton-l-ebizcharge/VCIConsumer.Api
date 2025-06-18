
namespace VCIConsumer.Api.Models.Requests;

public class CustomerUpdateRequest
{
    public required string uuid { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
    public bool active { get; set; }
    BankAccount bank_account { get; set; }
}
