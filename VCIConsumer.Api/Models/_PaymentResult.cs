namespace VCIConsumer.Api.Models;

public class _PaymentResult : PaymentBase
{
   
   // public ErrorMessage[] Warnings { get; set; }
    public required CustomerResult customer { get; set; }
    public required bool velocity_breached { get; set; }
    public required string correlation_id { get; set; }
    public required string company_id { get; set; }
    public required double amount { get; set; }
    public required string standard_entry_class { get; set; }
    public required string description { get; set; }
    public required string status { get; set; }
    public required APIRate api_rate_limits { get; set; }
    public bool Error { get { return status == "Error"; } }
    public override string ToString()
    {
        return $"Id: {uuid} | Status: {status} - {description}  | Date {created_at}";
    }

}
