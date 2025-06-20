

namespace VCIConsumer.Api.Models
{
    public class _CustomerResult: VCIModelbase
    {
       
        public required string name { get; set; }
        public required string email_truncated { get; set; }
        public required string phone_truncated { get; set; }       
        public required BankAccountRequest bank_account { get; set; }
        public bool active { get; set; }              
    }
}
