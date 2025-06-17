namespace VCIConsumer.Api.Models;

public class _BankAccount
{

    public enum BankAccountTypes { Checking = 1, Savings = 2, General_Ledger = 3, Loan = 4 }
    public required string routing_number { get; set; }
    public required string account_number { get; set; }
   
    public string account_type
    {
        get
        {
            return AccountType.ToString();
        }
    }
    public required BankAccountTypes AccountType { get; set; }
    public _BankAccount()
    {
       
    }
    public override string ToString()
    {
        return $"Routing#: {routing_number} | Account#: {account_number} |  Account Type: {AccountType.ToString()}";
    }
}
