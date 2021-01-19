using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTYD.Model.Database
{
    public partial class MealsSelected
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public Result[] Result { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("selection_uid")]
        public string SelectionUid { get; set; }

        [JsonProperty("sel_purchase_id")]
        public string SelPurchaseId { get; set; }

        [JsonProperty("selection_time")]
        public string SelectionTime { get; set; }

        [JsonProperty("sel_menu_date")]
        public string SelMenuDate { get; set; }

        [JsonProperty("meal_selection")]
        public string MealSelection { get; set; }

        [JsonProperty("delivery_day")]
        public string DeliveryDay { get; set; }

        [JsonProperty("last_sel_pur_id")]
        public string LastSelPurId { get; set; }

        [JsonProperty("last_menu_date")]
        public string LastMenuDate { get; set; }

        [JsonProperty("last_selection")]
        public string LastSelection { get; set; }

        [JsonProperty("addon_selection")]
        public string AddonSelection { get; set; }

        [JsonProperty("combined_selection")]
        public string CombinedSelection { get; set; }

        [JsonProperty("purchase_uid")]
        public string PurchaseUid { get; set; }

        [JsonProperty("purchase_date")]
        public string PurchaseDate { get; set; }

        [JsonProperty("purchase_id")]
        public string PurchaseId { get; set; }

        [JsonProperty("purchase_status")]
        public string PurchaseStatus { get; set; }

        [JsonProperty("pur_customer_uid")]
        public string PurCustomerUid { get; set; }

        [JsonProperty("pur_business_uid")]
        public string PurBusinessUid { get; set; }

        [JsonProperty("items")]
        public string Items { get; set; }

        [JsonProperty("order_instructions")]
        public string OrderInstructions { get; set; }

        [JsonProperty("delivery_instructions")]
        public string DeliveryInstructions { get; set; }

        [JsonProperty("order_type")]
        public string OrderType { get; set; }

        [JsonProperty("delivery_first_name")]
        public string DeliveryFirstName { get; set; }

        [JsonProperty("delivery_last_name")]
        public string DeliveryLastName { get; set; }

        [JsonProperty("delivery_phone_num")]
        public string DeliveryPhoneNum { get; set; }

        [JsonProperty("delivery_email")]
        public string DeliveryEmail { get; set; }

        [JsonProperty("delivery_address")]
        public string DeliveryAddress { get; set; }

        [JsonProperty("delivery_unit")]
        public string DeliveryUnit { get; set; }

        [JsonProperty("delivery_city")]
        public string DeliveryCity { get; set; }

        [JsonProperty("delivery_state")]
        public string DeliveryState { get; set; }

        [JsonProperty("delivery_zip")]
        public long DeliveryZip { get; set; }

        [JsonProperty("delivery_latitude")]
        public string DeliveryLatitude { get; set; }

        [JsonProperty("delivery_longitude")]
        public string DeliveryLongitude { get; set; }

        [JsonProperty("purchase_notes")]
        public string PurchaseNotes { get; set; }

        [JsonProperty("payment_uid")]
        public string PaymentUid { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("pay_purchase_id")]
        public string PayPurchaseId { get; set; }

        [JsonProperty("payment_time_stamp")]
        public string PaymentTimeStamp { get; set; }

        [JsonProperty("start_delivery_date")]
        public string StartDeliveryDate { get; set; }

        [JsonProperty("pay_coupon_id")]
        public string PayCouponId { get; set; }

        [JsonProperty("amount_due")]
        public double AmountDue { get; set; }

        [JsonProperty("amount_discount")]
        public long AmountDiscount { get; set; }

        [JsonProperty("amount_paid")]
        public double AmountPaid { get; set; }

        [JsonProperty("info_is_Addon")]
        public string InfoIsAddon { get; set; }

        [JsonProperty("cc_num")]
        public long CcNum { get; set; }

        [JsonProperty("cc_exp_date")]
        public string CcExpDate { get; set; }

        [JsonProperty("cc_cvv")]
        public long CcCvv { get; set; }

        [JsonProperty("cc_zip")]
        public long CcZip { get; set; }

        [JsonProperty("charge_id")]
        public string ChargeId { get; set; }

        [JsonProperty("payment_type")]
        public string PaymentType { get; set; }
    }
}
