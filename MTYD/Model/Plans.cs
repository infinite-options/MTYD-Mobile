using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MTYD.Model
{
    public class Plans
    {
        public string item_uid { get; set; }
        public string itm_business_uid { get; set; }
        public string item_name { get; set; }
        public string item_desc { get; set; }
        public double item_price { get; set; }
        public string item_sizes { get; set; }
        public int num_items { get; set; }
        public string item_photo { get; set; }
        public string created_at { get; set; }
        public string favorite { get; set; }
        public string auto_renew { get; set; }
        public int num_issues { get; set; }
        public int deliveries_per_week { get; set; }
        public string info_headline { get; set; }
        public string info_footer { get; set; }
        public double info_weekly_price { get; set; }
        public string payment_day { get; set; }
        public int payment_frequency { get; set; }
        public double shipping { get; set; }
    }
}
