namespace VCIConsumer.Api.Models;

public class payment : VCIModelbase
{
    public enum PaymentStatuses { ACCEPTED, ERROR, ORIGINATED, SETTLED, PARTIAL_SETTLED, VERIFYING, VOID, RETURN, NSF_DECLINED }

    public double amount { get; set; }
    public string city { get; set; }
    public string state { get; set; }
    public string description { get; set; } = string.Empty;
    public string addenda { get; set; } = string.Empty;
    public string standard_entry_class { get; set; } = string.Empty;
    public _CustomerResult customer { get; set; } 

    public string status { get; set; } = string.Empty;
    public double refunded_amount { get; set; }
    public string originated_at_window { get; set; } = string.Empty;
    public string settled_at_window { get; set; } = string.Empty;
    public _Check check { get; set; } = new _Check
    {
        check_number = 0,
        check_image_front = string.Empty,
        check_image_back = string.Empty
    };

    public payment()
    {
        city = "XXXX";
        state = "YY";
    }
}
