namespace VCIConsumer.Api.Models;

public class PaymentBase: VCIModelbase
{
    public enum PaymentEntries { PPD, CCD, BOC, WEB, POP, TEL }
    public required string originated_at { get; set; }
    public required string settled_at { get; set; }
    public required string origination_scheduled_for { get; set; }
    public required string settlement_scheduled_for { get; set; }
    public DateTime originated_atUTC
    {
        get
        {
            return ET2UTC(originated_at);
        }
    }
    public DateTime settled_atUTC
    {
        get
        {
          
            return ET2UTC(settled_at);
        }
    }
    public DateTime origination_scheduled_forUTC
    {
        get
        {
          
            return ET2UTC(origination_scheduled_for);
        }
    }
    public DateTime settlement_scheduled_forUTC
    {
        get
        {

            return ET2UTC(settlement_scheduled_for);
        }
    }
    public required string JSON { get; set; }
}
