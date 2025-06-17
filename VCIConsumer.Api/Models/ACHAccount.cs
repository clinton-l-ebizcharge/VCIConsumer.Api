namespace VCIConsumer.Api.Models;

public class ACHAccount
{
    public required string AccountNumber { get; set; }
    public required string RoutingNumber { get; set; }
    public required string CheckNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsChecking { get; set; }

    // Default constructor initializing all required properties
    public ACHAccount()
    {
        AccountNumber = string.Empty;
        RoutingNumber = string.Empty;
        CheckNumber = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        IsChecking = true;
    }

    // Constructor with specific parameters
    public ACHAccount(string accountNumber, string routingNumber, string checkNumber)
        : this()
    {
        AccountNumber = accountNumber;
        RoutingNumber = routingNumber;
        CheckNumber = checkNumber;
    }

    public static ACHAccount CreateTestCheck()
    {
        string[] TestRountings = new string[] { "130000006", "140000009", "150000002", "160000005", "170000008", "180000001", "190000004" };
        string[] TestAccounts = new string[] { "10102233", "10103344", "10104455", "10105566", "10106677", "10107788", "10108899" };
        var r = new Random();
        ACHAccount ret = new()
        {
            AccountNumber = TestAccounts[r.Next(0, TestAccounts.Length)],
            RoutingNumber = TestRountings[r.Next(0, TestRountings.Length)],
            CheckNumber = r.Next(100, 999).ToString(),
            FirstName = "TestFirstName", // Added to fix CS9035
            LastName = "TestLastName"   // Added to fix CS9035
        };
        return ret;
    }
}
