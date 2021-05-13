using System;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class AmbassCodePost
    {
        public string code { get; set; }
        public string info { get; set; }
        public string IsGuest { get; set; }

    }

    public class AmbassadorCouponDto
    {
        public string message { get; set; }
        public string code { get; set; }
        public double discount { get; set; }
        public string[] uids { get; set; }
        public AmbassadorCoupon sub { get; set; }
    }


    public class AmbassadorCoupon
    {
        public string coupon_uid { get; set; }
        public string coupon_id { get; set; }
        public string valid { get; set; }
        public double threshold { get; set; }
        public double discount_percent { get; set; }
        public double discount_amount { get; set; }
        public double discount_shipping { get; set; }
        public string expire_date { get; set; }
        public int limits { get; set; }
        public string notes { get; set; }
        public int num_used { get; set; }
        public string recurring { get; set; }
        public string email_id { get; set; }
        public string cup_business_uid { get; set; }
    }

    public class createAmb
    {
        public string code { get; set; }
    }
}
