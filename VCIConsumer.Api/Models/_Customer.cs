namespace VCIConsumer.Api.Models;

public class _Customer
{
    public required string uuid { get; set; }
    public required string name { get; set; }
    public required string email { get; set; }
    public required bool active { get; set; }
    
    public required _BankAccount bank_account { get; set; }
    public _Customer()
    {
      
    }
    public override string ToString()
    {
        return $"{name} - Email: {email} ";
    }
}
