using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;

        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();            
           
            Item item1 = new Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = "1";
            item1.item_uid = Preferences.Get("item_uid", ""); ;
            List<Item> itemsList = new List<Item> {item1};
            Preferences.Set("unitNum", AptEntry.Text);

            newPayment.customer_uid = "100-000082";
            newPayment.business_uid = "200-000001";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            newPayment.delivery_first_name = FNameEntry.Text;
            newPayment.delivery_last_name = LNameEntry.Text;
            newPayment.delivery_email = "jb@007.org";
            newPayment.delivery_phone = PhoneEntry.Text;
            newPayment.delivery_address = AddressEntry.Text;
            newPayment.delivery_unit = Preferences.Get("unitNum","");
            newPayment.delivery_city = CityEntry.Text;
            newPayment.delivery_state = StateEntry.Text;
            newPayment.delivery_zip = ZipEntry.Text;
            newPayment.delivery_instructions = DeliveryEntry.Text;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            newPayment.purchase_notes = "new purch";
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = Preferences.Get("price", "00.00");
            newPayment.cc_num = CCEntry.Text;
            newPayment.cc_exp_year = YearEntry.Text;
            newPayment.cc_exp_month = MonthEntry.Text;
            newPayment.cc_cvv = CVVEntry.Text;
            newPayment.cc_zip = ZipCCEntry.Text;

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
           // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content);
            // HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
            Console.WriteLine("SetPaymentInfo Func ENDED!");
        }
        public PaymentPage(string firstName, string lastName, string email)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            InitializeComponent();
        }

        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.Green;
            meals2.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
           // TotalPrice.Text = "$05.00";
        }
        private void clickedMeals2(object sender, EventArgs e)
        {
            meals2.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
           // TotalPrice.Text = "$10.00";
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals3.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals2.BackgroundColor = Color.Transparent;
            meals4.BackgroundColor = Color.Transparent;
          //  TotalPrice.Text = "$15.00";
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals4.BackgroundColor = Color.Green;
            meals1.BackgroundColor = Color.Transparent;
            meals3.BackgroundColor = Color.Transparent;
            meals2.BackgroundColor = Color.Transparent;
           // TotalPrice.Text = "$20.00";
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            setPaymentInfo();
            //Navigation.PushAsync(new Select(cust_firstName, cust_lastName, cust_email));
            //MainPage = PaymentPage();
        }

        private void clickedSaveCC(object sender, EventArgs e)
        {
            if(SaveCC.BackgroundColor != Color.Green)
            {
                SaveCC.BackgroundColor = Color.Green;
            } else
            {
                SaveCC.BackgroundColor = Color.Transparent;
            }
        }

        private void clickedSaveAdd(object sender, EventArgs e)
        {
            if (SaveAdd.BackgroundColor != Color.Green)
            {
                SaveAdd.BackgroundColor = Color.Green;
            }
            else
            {
                SaveAdd.BackgroundColor = Color.Transparent;
            }
        }
    }
}