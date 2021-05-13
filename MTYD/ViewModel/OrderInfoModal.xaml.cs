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
    public partial class OrderInfoModal : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string salt;
        string fullName; string emailAddress;

        string password; string refresh_token; string cc_num; string cc_exp_year; string cc_exp_month; string cc_cvv; string purchase_id;
        string new_item_id; string customer_id; string zip; string mealSelected; string mealPrice; string itm_business_id;

        protected async Task setPaymentInfo()
        {
            try
            {
                Console.WriteLine("SetPaymentInfo Func Started!");
                PaymentInfo newPayment = new PaymentInfo();
                //need to add item_business_id
                Item item1 = new Item();
                item1.name = Preferences.Get("item_name", "");
                item1.price = Preferences.Get("price", "00.00");
                item1.qty = Preferences.Get("freqSelected", "");
                item1.item_uid = Preferences.Get("item_uid", "");
                item1.itm_business_uid = "200-000002";
                List<Item> itemsList = new List<Item> { item1 };
                //Preferences.Set("unitNum", AptEntry.Text);

                string userID = (string)Application.Current.Properties["user_id"];
                Console.WriteLine("YOUR userID is " + userID);
                newPayment.customer_uid = userID;
                //newPayment.customer_uid = "100-000082";
                //newPayment.business_uid = "200-000002";
                //newPayment.items = itemsList;
                //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
                //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
                //newPayment.salt = "2020-09-22 21:55:17";
                //newPayment.salt = "";
                //newPayment.delivery_first_name = FNameEntry.Text;
                //newPayment.delivery_last_name = LNameEntry.Text;
                //newPayment.delivery_email = emailEntry.Text;
                //newPayment.delivery_phone = PhoneEntry.Text;
                //newPayment.delivery_address = AddressEntry.Text;
                //newPayment.delivery_unit = Preferences.Get("unitNum", "");
                //newPayment.delivery_city = CityEntry.Text;
                //newPayment.delivery_state = StateEntry.Text;
                //newPayment.delivery_zip = ZipEntry.Text;
                //newPayment.delivery_instructions = DeliveryEntry.Text;
                //newPayment.delivery_longitude = "";
                //newPayment.delivery_latitude = "";
                //newPayment.order_instructions = "slow";
                //newPayment.purchase_notes = "new purch";
                //newPayment.amount_due = Preferences.Get("price", "00.00");
                //newPayment.amount_discount = "00.00";
                //newPayment.amount_paid = Preferences.Get("price", "00.00");
                newPayment.cc_num = CCEntry.Text;
                //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
                newPayment.cc_exp_year = "20" + cardExpYear.Text;
                //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
                newPayment.cc_exp_month = cardExpMonth.Text;
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
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        //auto-populate the delivery info if the user has already previously entered it
        public async void fillEntries()
        {
            try
            {
                Console.WriteLine("fillEntries entered");
                var request = new HttpRequestMessage();
                Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                request.RequestUri = new Uri(url);
                //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                request.Method = HttpMethod.Get;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    HttpContent content = response.Content;
                    Console.WriteLine("content: " + content);
                    var userString = await content.ReadAsStringAsync();
                    //Console.WriteLine(userString);
                    JObject info_obj = JObject.Parse(userString);
                    this.NewDeliveryInfo.Clear();

                    //ArrayList item_price = new ArrayList();
                    //ArrayList num_items = new ArrayList();
                    //ArrayList payment_frequency = new ArrayList();
                    //ArrayList groupArray = new ArrayList();

                    //Console.WriteLine("string: " + (info_obj["result"]).ToString());
                    //check if the user hasn't entered any info before, if so put in the placeholders
                    if ((info_obj["result"]).ToString() == "[]")
                    {
                        Console.WriteLine("no info");

                        //FNameEntry.Placeholder = "First Name*";
                        //LNameEntry.Placeholder = "Last Name*";
                        //emailEntry.Placeholder = "Email*";
                        //AddressEntry.Placeholder = "Street*";
                        //AptEntry.Placeholder = "Unit";
                        //CityEntry.Placeholder = "City*";
                        //StateEntry.Placeholder = "State*";
                        //ZipEntry.Placeholder = "Zip*";
                        //PhoneEntry.Placeholder = "Phone Number*";
                        //DeliveryEntry.Placeholder = "Delivery Instructions";
                        CCEntry.Placeholder = "Credit Card Number*";
                        CVVEntry.Placeholder = "CVC/CVV*";
                        ZipCCEntry.Placeholder = "Zip*";

                        return;
                    }


                    //Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["selection_uid"]);
                    //FNameEntry.Text = (info_obj["result"])[0]["delivery_first_name"].ToString();
                    //if (FNameEntry.Text == "")
                    //    FNameEntry.Placeholder = "First Name*";

                    //LNameEntry.Text = (info_obj["result"])[0]["delivery_last_name"].ToString();
                    //if (LNameEntry.Text == "")
                    //    LNameEntry.Placeholder = "Last Name*";

                    //emailEntry.Text = (info_obj["result"])[0]["delivery_email"].ToString();
                    //if (emailEntry.Text == "")
                    //    emailEntry.Placeholder = "Email*";

                    //AddressEntry.Text = (info_obj["result"])[0]["delivery_address"].ToString();
                    //if (AddressEntry.Text == "")
                    //    AddressEntry.Placeholder = "Street*";

                    //AptEntry.Text = (info_obj["result"])[0]["delivery_unit"].ToString();
                    //if (AptEntry.Text == "")
                    //    AptEntry.Placeholder = "Unit";

                    //CityEntry.Text = (info_obj["result"])[0]["delivery_city"].ToString();
                    //if (CityEntry.Text == "")
                    //    CityEntry.Placeholder = "City*";

                    //StateEntry.Text = (info_obj["result"])[0]["delivery_state"].ToString();
                    //if (StateEntry.Text == "")
                    //    StateEntry.Placeholder = "State*";

                    //ZipEntry.Text = (info_obj["result"])[0]["delivery_zip"].ToString();
                    //if (ZipEntry.Text == "")
                    //    ZipEntry.Placeholder = "Zip*";

                    //PhoneEntry.Text = (info_obj["result"])[0]["delivery_phone_num"].ToString();
                    //if (PhoneEntry.Text == "")
                    //    PhoneEntry.Placeholder = "Phone Number*";

                    //DeliveryEntry.Text = (info_obj["result"])[0]["delivery_instructions"].ToString();
                    //if (DeliveryEntry.Text == "")
                    //    DeliveryEntry.Placeholder = "Delivery Instructions";

                    CCEntry.Text = (info_obj["result"])[0]["cc_num"].ToString();
                    if (CCEntry.Text == "")
                        CCEntry.Placeholder = "Credit Card Number*";

                    CVVEntry.Text = (info_obj["result"])[0]["cc_cvv"].ToString();
                    if (CVVEntry.Text == "")
                        CVVEntry.Placeholder = "CVC/CVV*";

                    ZipCCEntry.Text = (info_obj["result"])[0]["cc_zip"].ToString();
                    if (ZipCCEntry.Text == "")
                        ZipCCEntry.Placeholder = "Zip*";

                    cardExpMonth.Text = ((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(5, 2);
                    //int chosenMonth = int.Parse(((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(5, 2));
                    //if (chosenMonth == 1)
                    //    MonthPicker.SelectedIndex = 0;
                    //else if (chosenMonth == 2)
                    //    MonthPicker.SelectedIndex = 1;
                    //else if (chosenMonth == 3)
                    //    MonthPicker.SelectedIndex = 2;
                    //else if (chosenMonth == 4)
                    //    MonthPicker.SelectedIndex = 3;
                    //else if (chosenMonth == 5)
                    //    MonthPicker.SelectedIndex = 4;
                    //else if (chosenMonth == 6)
                    //    MonthPicker.SelectedIndex = 5;
                    //else if (chosenMonth == 7)
                    //    MonthPicker.SelectedIndex = 6;
                    //else if (chosenMonth == 8)
                    //    MonthPicker.SelectedIndex = 7;
                    //else if (chosenMonth == 9)
                    //    MonthPicker.SelectedIndex = 8;
                    //else if (chosenMonth == 10)
                    //    MonthPicker.SelectedIndex = 9;
                    //else if (chosenMonth == 11)
                    //    MonthPicker.SelectedIndex = 10;
                    //else MonthPicker.SelectedIndex = 11;

                    cardExpYear.Text = ((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(2, 2);

                    //int chosenYear = int.Parse(((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(0, 4));
                    //if (chosenYear == 2020)
                    //    YearPicker.SelectedIndex = 0;
                    //else if (chosenYear == 2021)
                    //    YearPicker.SelectedIndex = 1;
                    //else if (chosenYear == 2022)
                    //    YearPicker.SelectedIndex = 2;
                    //else if (chosenYear == 2023)
                    //    YearPicker.SelectedIndex = 3;
                    //else if (chosenYear == 2024)
                    //    YearPicker.SelectedIndex = 4;
                    //else if (chosenYear == 2025)
                    //    YearPicker.SelectedIndex = 5;
                    //else if (chosenYear == 2026)
                    //    YearPicker.SelectedIndex = 6;
                    //else if (chosenYear == 2027)
                    //    YearPicker.SelectedIndex = 7;
                    //else if (chosenYear == 2028)
                    //    YearPicker.SelectedIndex = 8;
                    //else if (chosenYear == 2029)
                    //    YearPicker.SelectedIndex = 9;
                    //else if (chosenYear == 2030)
                    //    YearPicker.SelectedIndex = 10;
                    //else if (chosenYear == 2031)
                    //    YearPicker.SelectedIndex = 11;
                    //else if (chosenYear == 2032)
                    //    YearPicker.SelectedIndex = 12;
                    //else if (chosenYear == 2033)
                    //    YearPicker.SelectedIndex = 13;
                    //else if (chosenYear == 2034)
                    //    YearPicker.SelectedIndex = 14;
                    //else if (chosenYear == 2035)
                    //    YearPicker.SelectedIndex = 15;
                    //else if (chosenYear == 2036)
                    //    YearPicker.SelectedIndex = 16;
                    //else if (chosenYear == 2037)
                    //    YearPicker.SelectedIndex = 17;
                    //else if (chosenYear == 2038)
                    //    YearPicker.SelectedIndex = 18;
                    //else if (chosenYear == 2039)
                    //    YearPicker.SelectedIndex = 19;
                    //else if (chosenYear == 2040)
                    //    YearPicker.SelectedIndex = 20;
                    //else if (chosenYear == 2041)
                    //    YearPicker.SelectedIndex = 21;
                    //else if (chosenYear == 2042)
                    //    YearPicker.SelectedIndex = 22;
                    //else YearPicker.SelectedIndex = 23;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public OrderInfoModal(string firstName, string lastName, string email, string pass, string token, string num, string year, string month, string cvv, string zip, string purchaseID, string businessID, string mealPlan, string price, string itemID, string customerID)
        {
            try
            {
                cust_firstName = firstName;
                cust_lastName = lastName;
                cust_email = email;
                InitializeComponent();
                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;

                //MonthPicker.SelectedIndex = Int32.Parse(month) - 1;
                cardExpMonth.Text = month;
                //YearPicker.SelectedIndex = Int32.Parse(year) - 2020;
                cardExpYear.Text = year.Substring(2);
                password = pass; refresh_token = token; CCEntry.Text = num; CVVEntry.Text = cvv; ZipCCEntry.Text = zip; purchase_id = purchaseID;
                new_item_id = itemID; customer_id = customerID; mealSelected = mealPlan; mealPrice = price; itm_business_id = businessID;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    orangeBox.HeightRequest = height / 2;
                    orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                    orangeBox.CornerRadius = height / 40;
                    heading.FontSize = width / 32;
                    heading.Margin = new Thickness(0, 0, 0, 30);
                    pfp.HeightRequest = 40;
                    pfp.WidthRequest = 40;
                    pfp.CornerRadius = 20;
                    //pfp.Margin = new Thickness(0, 0, 23, 27);
                    innerGrid.Margin = new Thickness(0, 0, 23, 27);
                    heading.WidthRequest = 140;

                    if (Preferences.Get("profilePicLink", "") == "")
                    {
                        string userInitials = "";
                        if (cust_firstName != "" && cust_firstName != null)
                        {
                            userInitials += cust_firstName.Substring(0, 1);
                        }
                        if (cust_lastName != "" && cust_lastName != null)
                        {
                            userInitials += cust_lastName.Substring(0, 1);
                        }
                        initials.Text = userInitials.ToUpper();
                        initials.FontSize = width / 38;
                    }
                    else pfp.Source = Preferences.Get("profilePicLink", "");

                    menu.Margin = new Thickness(25, 0, 0, 30);

                    //menu.Margin = new Thickness(25, 0, 0, 30);

                    //firstName.CornerRadius = 22;
                    //firstName.HeightRequest = 35;
                    //lastName.CornerRadius = 22;
                    //lastName.HeightRequest = 35;

                    //emailAdd.CornerRadius = 22;
                    //emailAdd.HeightRequest = 35;

                    //street.CornerRadius = 22;
                    //street.HeightRequest = 35;

                    //unit.CornerRadius = 22;
                    //unit.HeightRequest = 35;
                    //city.CornerRadius = 22;
                    //city.HeightRequest = 35;
                    //state.CornerRadius = 22;
                    //state.HeightRequest = 35;

                    //zipCode.CornerRadius = 22;
                    //zipCode.HeightRequest = 35;
                    //phoneNum.CornerRadius = 22;
                    //phoneNum.HeightRequest = 35;

                    //deliveryInstr.CornerRadius = 22;

                    creditCard.CornerRadius = 22;
                    creditCard.HeightRequest = 35;

                    cvvFrame.CornerRadius = 22;
                    cvvFrame.HeightRequest = 35;
                    zipCode2.CornerRadius = 22;
                    zipCode2.HeightRequest = 35;

                    //monthFrame.CornerRadius = 22;
                    //yearFrame.CornerRadius = 22;
                    SignUpButton.CornerRadius = 25;
                }
                else //android
                {
                    orangeBox.CornerRadius = 35;
                    pfp.CornerRadius = 20;

                    //firstName.CornerRadius = 24;
                    //lastName.CornerRadius = 24;

                    //emailAdd.CornerRadius = 24;

                    //street.CornerRadius = 24;

                    //unit.CornerRadius = 24;
                    //city.CornerRadius = 24;
                    //state.CornerRadius = 24;

                    //zipCode.CornerRadius = 24;
                    //phoneNum.CornerRadius = 24;

                    //deliveryInstr.CornerRadius = 24;

                    creditCard.CornerRadius = 24;

                    cvvFrame.CornerRadius = 24;
                    zipCode2.CornerRadius = 24;

                    //monthFrame.CornerRadius = 24;
                    //yearFrame.CornerRadius = 24;
                    SignUpButton.CornerRadius = 25;
                }

                fillEntries();
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        /*
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
        }*/

        private async void clickedDone(object sender, EventArgs e)
        {
            try
            {
                string platform = Application.Current.Properties["platform"].ToString();
                //string passwordSalt = Preferences.Get("password_salt", "");
                // Console.WriteLine("Clicked done: The Salt is: " + passwordSalt);
                //setPaymentInfo();
                //if (string.IsNullOrEmpty(passwordSalt)){  //If social login (salt is NULL)

                PurchaseInfo updated = new PurchaseInfo();
                updated.password = password;
                updated.refresh_token = refresh_token;
                updated.cc_num = CCEntry.Text;
                //updated.cc_exp_year = YearPicker.SelectedItem.ToString();
                updated.cc_exp_year = "20" + cardExpYear.Text;
                //"cc_exp_year":"2022","cc_exp_month":"11-November"
                //updated.cc_exp_month = MonthPicker.SelectedItem.ToString().Substring(0,2);
                updated.cc_exp_month = cardExpMonth.Text;
                updated.cc_cvv = CVVEntry.Text;
                updated.purchase_id = purchase_id;
                updated.new_item_id = new_item_id;
                updated.customer_id = customer_id;
                updated.cc_zip = ZipCCEntry.Text;

                List<Item2> list1 = new List<Item2>();
                Item2 item1 = new Item2();
                item1.price = mealPrice;
                item1.name = mealSelected;
                item1.qty = Preferences.Get("freqSelected", "");
                item1.itm_business_uid = itm_business_id;
                item1.item_uid = new_item_id;
                list1.Add(item1);
                updated.items = list1;

                var newPaymentJSONString = JsonConvert.SerializeObject(updated);
                // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
                var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("Content: " + content);
                /*var request = new HttpRequestMessage();
                request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                request.Method = HttpMethod.Post;
                request.Content = content;*/
                var client = new HttpClient();
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                // HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedDone Func ENDED!");

                Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
                //if (platform != "DIRECT")
                //{
                //    Navigation.PushAsync(new VerifyInfo(AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, CCEntry.Text, CVVEntry.Text, ZipCCEntry.Text, salt));
                //}
                //else //If direct login (salt != NULL)
                //{
                //    Navigation.PushAsync(new VerifyInfoDirectLogin(AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, CCEntry.Text, CVVEntry.Text, ZipCCEntry.Text, salt));
                //}
                //MainPage = PaymentPage();
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        void clickedNotDone(object sender, EventArgs e)
        {
            try
            {
                if (CCEntry.Text == null || CCEntry.Text == "")
                {
                    DisplayAlert("Error", "credit card number required", "OK");
                    return;
                }
                else if (CCEntry.Text.Length != 16)
                {
                    DisplayAlert("Error", "invalid credit card number", "OK");
                    return;
                }

                if (CVVEntry.Text == null || CVVEntry.Text == "")
                {
                    DisplayAlert("Error", "CVV required", "OK");
                    return;
                }
                else if (CVVEntry.Text.Length != 3)
                {
                    DisplayAlert("Error", "invalid CVV", "OK");
                    return;
                }

                if (ZipCCEntry.Text == null || ZipCCEntry.Text == "")
                {
                    DisplayAlert("Error", "credit card zip code required", "OK");
                    return;
                }

                //if (MonthPicker.SelectedIndex == -1)
                //{
                //    DisplayAlert("Error", "select a month", "OK");
                //    return;
                //}
                if (cardExpMonth.Text == null || cardExpMonth.Text == "")
                {
                    DisplayAlert("Error", "invalid expiration date", "OK");
                    return;
                }

                if (cardExpYear.Text == null || cardExpYear.Text == "")
                {
                    DisplayAlert("Error", "invalid expiration date", "OK");
                    return;
                }

                //if (YearPicker.SelectedIndex == -1)
                //{
                //    DisplayAlert("Error", "select a year", "OK");
                //    return;
                //}

                clickedDone(sender, e);
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        /*//Saving CC and Address Info
        private void clickedSaveCC(object sender, EventArgs e)
        {
            if (SaveCC.BackgroundColor != Color.Green)
            {
                SaveCC.BackgroundColor = Color.Green;
            }
            else
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
        }*/
    }
}