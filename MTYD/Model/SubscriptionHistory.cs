using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class SubHist : INotifyPropertyChanged
    {
        public string Date { get; set; }
        public ObservableCollection<Meals> mealColl { get; set; }
        bool _collVisible;
        string _mealPlanName { get; set; }
        public int mealCollHeight { get; set; }
        public bool mainGridVisible { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public string mealPlanName
        {
            set
            {
                if (_mealPlanName != value)
                {
                    _mealPlanName = value;
                    OnPropertyChanged("mealPlanName");
                }
            }
            get
            {
                return _mealPlanName;
            }

        }

        public bool CollVisible
        {
            set
            {
                if (_collVisible != value)
                {
                    _collVisible = value;
                    OnPropertyChanged("CollVisible");
                }
            }
            get
            {
                return _collVisible;
            }

        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Meals
    {
        public string DelivDate { get; set; }
        public bool DelivDateVisible { get; set; }
        public string mealName { get; set; }
        public string qty { get; set; }
        public string urlLink { get; set; }
    }

    public class PlanName
    {
        public string name { get; set; }
    }

    public class History
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("result")]
        public HistorySel[] Result { get; set; }
    }

    public class HistorySel
    {
        [JsonProperty("purchase_uid")]
        public string PurchUid { get; set; }

        [JsonProperty("purchase_date")]
        public string PurchDate { get; set; }

        [JsonProperty("purchase_id")]
        public string PurchId { get; set; }

        [JsonProperty("purchase_status")]
        public string PurchStatus { get; set; }

        [JsonProperty("pur_customer_uid")]
        public string PurCustUid { get; set; }

        [JsonProperty("pur_business_uid")]
        public string PurBusUid { get; set; }

        [JsonProperty("items")]
        public string Items { get; set; }

        [JsonProperty("ms")]
        public string Ms { get; set; }

        [JsonProperty("meal_uid")]
        public string MealUid { get; set; }

        [JsonProperty("meal_category")]
        public string MealCat { get; set; }

        [JsonProperty("meal_name")]
        public string MealName { get; set; }

        [JsonProperty("meal_qty")]
        public string MealQty { get; set; }

        [JsonProperty("meal_desc")]
        public string MealDesc { get; set; }

        [JsonProperty("meal_photo_URL")]
        public string MealPhotoUrl { get; set; }

        [JsonProperty("payment_time_stamp")]
        public string PayTimeStamp { get; set; }

        [JsonProperty("start_delivery_date")]
        public string StartDelivDate { get; set; }

        [JsonProperty("charge_id")]
        public string ChargeId { get; set; }

        [JsonProperty("last_payment")]
        public string LastPayment { get; set; }

        [JsonProperty("sel_menu_date")]
        public string SelMenuDate { get; set; }
    }
}
