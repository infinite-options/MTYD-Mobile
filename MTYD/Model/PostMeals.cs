using System;
using System.Collections.Generic;

using System.Globalization;
using MTYD.Model.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTYD.Model
{
    public partial class PostMeals
    {
        [JsonProperty("is_addon")]
        public bool IsAddon { get; set; }

        [JsonProperty("items")]
        public List<MealInformation> Items { get; set; }

        [JsonProperty("purchase_id")]
        public string PurchaseId { get; set; }

        [JsonProperty("menu_date")]
        public string MenuDate { get; set; }

        [JsonProperty("delivery_day")]
        public string DeliveryDay { get; set; }


    }
}