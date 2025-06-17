namespace VCIConsumer.Api.Models;

public class VCIModelbase
{
    public required string uuid { get; set; }
    public required string hrid { get; set; }
    public required string created_at { get; set; }
    public required string modified_at { get; set; }
    public required string idempotency_key { get; set; }
    public DateTime created_atUTC => ET2UTC(created_at);
    public DateTime modified_atUTC => ET2UTC(modified_at);
    public DateTime ET2UTC(string datetime)
    {
        if (string.IsNullOrEmpty(datetime))
            return DateTime.Parse("01/01/1970");
        DateTime dte = DateTime.Parse(created_at.Replace("ET", "").Trim());
        return dte.AddHours(3).ToUniversalTime();
    }
}
