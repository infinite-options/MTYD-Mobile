using System;
namespace MTYD.Model
{
    public class NextBilling
    {
        public string purchase_uid { get; set; }
        public string purchase_date { get; set; }
        public string purchase_id { get; set; }
        public string purchase_status { get; set; }
        public string pur_customer_uid { get; set; }
        public string pur_business_uid { get; set; }
        public Item items { get; set; }

    }
}
