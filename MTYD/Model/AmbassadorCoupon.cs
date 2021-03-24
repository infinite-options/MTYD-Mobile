using System;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class AmbassadorCouponDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("result")]
        public AmbassadorCoupon[] Result { get; set; }
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
}
