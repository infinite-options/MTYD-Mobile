using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace MTYD.Model.Database
{
    public partial class MealInformation
    {
        [JsonProperty("qty")]
        public string Qty { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("item_uid")]
        public string ItemUid { get; set; }
    }


}
