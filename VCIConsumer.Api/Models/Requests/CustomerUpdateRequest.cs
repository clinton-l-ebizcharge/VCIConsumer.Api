
namespace VCIConsumer.Api.Models.Requests;

public class CustomerUpdateRequest
{
    public required string uuid { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public bool active { get; set; }
    BankAccount bank_account { get; set; }
}
