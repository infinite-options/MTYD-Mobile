using System;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class nextDelivDate
    {
        [JsonProperty("menu_date")]
        public string MenuDate { get; set; }

        [JsonProperty("taxes")]
        public double Taxes { get; set; }

        [JsonProperty("delivery_fee")]
        public double DeliveryFee { get; set; }

        [JsonProperty("service_fee")]
        public double ServiceFee { get; set; }

        [JsonProperty("driver_tip")]
        public double DriverTip { get; set; }

        [JsonProperty("base_amount")]
        public double BaseAmount { get; set; }

        [JsonProperty("discount")]
        public double Discount { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }
}
