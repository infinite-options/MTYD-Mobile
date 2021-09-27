using System;
namespace MTYD.Model
{
    public class StripePayment
    {
        public string currency { get; set; }
        public string customer_uid { get; set; }
        public string business_code { get; set; }
        public PaySummary payment_summary { get; set; }
    }

    public class PaySummary
    {
        public string total { get; set; }
    }
}
