namespace VCIConsumer.Api.Models
{
    public class _Check
    {
       
        public required int check_number { get; set; }
        public required string check_image_front { 
            get;
            set; 
        }
        public required string check_image_back { get; set; }
        public _Check()
        {

        }
        public _Check(int checkNumber)
        {
            check_number = checkNumber;
        }
        public _Check(int checkNumber, bool testImage) : this(checkNumber)
        {

            check_image_front = Convert.ToBase64String(new byte[50 * 30]);
            check_image_back = Convert.ToBase64String(new byte[50 * 29]);

        }
    }
}
