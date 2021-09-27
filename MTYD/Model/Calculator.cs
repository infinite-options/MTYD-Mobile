using System;
using Newtonsoft.Json;

namespace MTYD.Model
{
    public class Calculator
    {

        [JsonProperty("purchase_uid")]
        public string PurchUid { get; set; }

        [JsonProperty("purchase_id")]
        public string PurchId { get; set; }

        [JsonProperty("payment_id")]
        public string PaymentId { get; set; }

        [JsonProperty("completed_deliveries")]
        public double CompletedDelivs { get; set; }

        [JsonProperty("customer_uid")]
        public string CustUid { get; set; }

        [JsonProperty("meal_refund")]
        public double MealRefund { get; set; }

        [JsonProperty("amount_discount")]
        public double AmtDiscount { get; set; }

        [JsonProperty("service_fee")]
        public double ServiceFee { get; set; }

        [JsonProperty("delivery_fee")]
        public double DelivFee { get; set; }

        [JsonProperty("driver_tip")]
        public double DriverTip { get; set; }

        [JsonProperty("taxes")]
        public double Taxes { get; set; }

        [JsonProperty("ambassador_code")]
        public double AmbCode { get; set; }

        [JsonProperty("amount_due")]
        public double AmtDue { get; set; }

        [JsonProperty("amount_paid")]
        public double AmtPaid { get; set; }

        [JsonProperty("charge_id")]
        public string ChargeId { get; set; }

        [JsonProperty("delivery_instructions")]
        public string DelivInstr { get; set; }
    }
}
