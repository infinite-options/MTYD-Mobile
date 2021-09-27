using System;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class ZonesDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("result")]
        public Zones[] Result { get; set; }
    }

    public class Zones
    {
        public string zone_uid { get; set; }
        public string zone { get; set; }
        public string zone_name { get; set; }
        public string z_id { get; set; }
        public string z_biz_id { get; set; }
        public string business_name { get; set; }
        public string z_delivery_day { get; set; }
        public string z_delivery_time { get; set; }
        public string z_accepting_day { get; set; }
        public string z_accepting_time { get; set; }
        public double LB_long { get; set; }
        public double LB_lat { get; set; }
        public double LT_long { get; set; }
        public double LT_lat { get; set; }
        public double RT_long { get; set; }
        public double RT_lat { get; set; }
        public double RB_long { get; set; }
        public double RB_lat { get; set; }
        public string business_type { get; set; }
        public string business_image { get; set; }
        public string business_accepting_hours { get; set; }
        public double tax_rate { get; set; }
        public double service_fee { get; set; }
        public double delivery_fee { get; set; }
    }
}
