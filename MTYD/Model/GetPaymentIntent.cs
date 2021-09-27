using System;
namespace MTYD.Model
{
    public class GetPaymentIntent
    {
        public string currency { get; set; }
        public string customer_uid { get; set; }
        public string business_code { get; set; }
        public string item_uid { get; set; }
        public int num_items { get; set; }
        public int num_deliveries { get; set; }
        public double delivery_discount { get; set; }
        public PaymentSummary payment_summary { get; set; }
    }

    public class PaymentSummary
    {
        public string mealSubPrice { get; set; }
        public string discountAmount { get; set; }
        public string addOns { get; set; }
        public string tip { get; set; }
        public string serviceFee { get; set; }
        public string deliveryFee { get; set; }
        public double taxRate { get; set; }
        public string taxAmount { get; set; }
        public string ambassadorDiscount { get; set; }
        public string total { get; set; }
        public string subtotal { get; set; }
    }
}
