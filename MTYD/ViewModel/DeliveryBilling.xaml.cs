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
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using Xamarin.Forms.Maps;

namespace MTYD.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeliveryBilling : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string salt;
        string fullName; string emailAddress;
        public bool isAddessValidated = false;
        bool withinZones = false;
        WebClient client = new WebClient();
        Zones[] passingZones;
        double tax;
        double serviceFee;
        double deliveryFee;
        string savedFirstName = "firstName" + (string)Application.Current.Properties["user_id"];
        string savedLastName = "lastName" + (string)Application.Current.Properties["user_id"];
        string savedEmail = "email" + (string)Application.Current.Properties["user_id"];
        string savedAdd = "address" + (string)Application.Current.Properties["user_id"];
        string savedApt = "apt" + (string)Application.Current.Properties["user_id"];
        string savedCity = "city" + (string)Application.Current.Properties["user_id"];
        string savedState = "state" + (string)Application.Current.Properties["user_id"];
        string savedZip = "zip" + (string)Application.Current.Properties["user_id"];
        string savedPhone = "phone" + (string)Application.Current.Properties["user_id"];
        string savedInstr = "instructions" + (string)Application.Current.Properties["user_id"];


        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();
            //need to add item_business_id
            Item item1 = new Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = "1";
            item1.item_uid = Preferences.Get("item_uid", "");
            item1.itm_business_uid = "200-000001"; 
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry.Text);

            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("YOUR userID is " + userID);
            newPayment.customer_uid = userID;
            //newPayment.customer_uid = "100-000082";
            //newPayment.business_uid = "200-000002";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            newPayment.salt = "";
            newPayment.delivery_first_name = FNameEntry.Text;
            newPayment.delivery_last_name = LNameEntry.Text;
            newPayment.delivery_email = emailEntry.Text;
            newPayment.delivery_phone = PhoneEntry.Text;
            newPayment.delivery_address = AddressEntry.Text;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
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
            // newPayment.cc_num = CCEntry.Text;
            newPayment.cc_num = "";
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            newPayment.cc_exp_month = "11";
            // newPayment.cc_cvv = CVVEntry.Text;
            newPayment.cc_cvv = "";
            // newPayment.cc_zip = ZipCCEntry.Text;
            newPayment.cc_zip = "";

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

        //auto-populate the delivery info if the user has already previously entered it
        public async void fillEntries()
        {
            //if there is no saved info
            if (Preferences.Get(savedFirstName, "") == "")
            {
                Console.WriteLine("no info");
                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                Debug.WriteLine("getProfileInfo url: " + url);
                var request3 = new HttpRequestMessage();
                request3.RequestUri = new Uri(url);
                request3.Method = HttpMethod.Get;
                var client2 = new HttpClient();
                HttpResponseMessage response2 = await client2.SendAsync(request3);
                HttpContent content2 = response2.Content;
                Console.WriteLine("content: " + content2.ToString());
                var userString2 = await content2.ReadAsStringAsync();
                Debug.WriteLine("userString: " + userString2);
                JObject info_obj3 = JObject.Parse(userString2);

                FNameEntry.Text = (info_obj3["result"])[0]["customer_first_name"].ToString();
                LNameEntry.Text = (info_obj3["result"])[0]["customer_last_name"].ToString();
                emailEntry.Text = info_obj3["result"][0]["customer_email"].ToString();
                AddressEntry.Text = info_obj3["result"][0]["customer_address"].ToString();
                if (info_obj3["result"][0]["customer_unit"].ToString() == null || info_obj3["result"][0]["customer_unit"].ToString() == "")
                    AptEntry.Placeholder = "Unit";
                else AptEntry.Text = info_obj3["result"][0]["customer_unit"].ToString();
                CityEntry.Text = info_obj3["result"][0]["customer_city"].ToString();
                StateEntry.Text = info_obj3["result"][0]["customer_state"].ToString();
                ZipEntry.Text = info_obj3["result"][0]["customer_zip"].ToString();
                PhoneEntry.Text = info_obj3["result"][0]["customer_phone_num"].ToString();


                DeliveryEntry.Placeholder = "Delivery Instructions";
                CCEntry.Placeholder = "Credit Card Number*";
                CVVEntry.Placeholder = "CVC/CVV*";
                ZipCCEntry.Placeholder = "Zip*";

                return;
            }
            else
            {
                FNameEntry.Text = Preferences.Get(savedFirstName, "");
                LNameEntry.Text = Preferences.Get(savedLastName, "");
                emailEntry.Text = Preferences.Get(savedEmail, "");
                AddressEntry.Text = Preferences.Get(savedAdd, "");
                CityEntry.Text = Preferences.Get(savedCity, "");
                StateEntry.Text = Preferences.Get(savedState, "");
                ZipEntry.Text = Preferences.Get(savedZip, "");
                PhoneEntry.Text = Preferences.Get(savedPhone, "");

                if (Preferences.Get(savedApt, "") != "")
                    AptEntry.Text = Preferences.Get(savedApt, "");
                else AptEntry.Placeholder = "Unit";
                if (Preferences.Get(savedInstr, "") != "")
                    DeliveryEntry.Text = Preferences.Get(savedInstr, "");
                else DeliveryEntry.Placeholder = "Delivery Instructions";

            }


            /*Console.WriteLine("fillEntries entered");
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
                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                    Debug.WriteLine("getProfileInfo url: " + url);
                    var request3 = new HttpRequestMessage();
                    request3.RequestUri = new Uri(url);
                    request3.Method = HttpMethod.Get;
                    var client2 = new HttpClient();
                    HttpResponseMessage response2 = await client2.SendAsync(request3);
                    HttpContent content2 = response2.Content;
                    Console.WriteLine("content: " + content2.ToString());
                    var userString2 = await content2.ReadAsStringAsync();
                    Debug.WriteLine("userString: " + userString2);
                    JObject info_obj3 = JObject.Parse(userString2);

                    FNameEntry.Text = (info_obj3["result"])[0]["customer_first_name"].ToString();
                    LNameEntry.Text = (info_obj3["result"])[0]["customer_last_name"].ToString();
                    emailEntry.Text = info_obj3["result"][0]["customer_email"].ToString();
                    AddressEntry.Text = info_obj3["result"][0]["customer_address"].ToString();
                    if (info_obj3["result"][0]["customer_unit"].ToString() == null || info_obj3["result"][0]["customer_unit"].ToString() == "")
                        AptEntry.Placeholder = "Unit";
                    else AptEntry.Text = info_obj3["result"][0]["customer_unit"].ToString();
                    CityEntry.Text = info_obj3["result"][0]["customer_city"].ToString();
                    StateEntry.Text = info_obj3["result"][0]["customer_state"].ToString();
                    ZipEntry.Text = info_obj3["result"][0]["customer_zip"].ToString();
                    PhoneEntry.Text = info_obj3["result"][0]["customer_phone_num"].ToString();


                    DeliveryEntry.Placeholder = "Delivery Instructions";
                    CCEntry.Placeholder = "Credit Card Number*";
                    CVVEntry.Placeholder = "CVC/CVV*";
                    ZipCCEntry.Placeholder = "Zip*";

                    return;
                }
                Debug.WriteLine("after empty filled reached");

                //Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["selection_uid"]);
                FNameEntry.Text = (info_obj["result"])[0]["delivery_first_name"].ToString();
                if (FNameEntry.Text == "")
                    FNameEntry.Placeholder = "First Name*";

                LNameEntry.Text = (info_obj["result"])[0]["delivery_last_name"].ToString();
                if (LNameEntry.Text == "")
                    LNameEntry.Placeholder = "Last Name*";

                emailEntry.Text = (info_obj["result"])[0]["delivery_email"].ToString();
                if (emailEntry.Text == "")
                    emailEntry.Placeholder = "Email*";

                AddressEntry.Text = (info_obj["result"])[(info_obj["result"])]["delivery_address"].ToString();
                if (AddressEntry.Text == "")
                    AddressEntry.Placeholder = "Street*";

                if ((info_obj["result"])[0]["delivery_unit"].ToString() == "" || (info_obj["result"])[0]["delivery_unit"].ToString() == "NULL")
                    AptEntry.Placeholder = "Unit";
                else AptEntry.Text = (info_obj["result"])[0]["delivery_unit"].ToString();

                CityEntry.Text = (info_obj["result"])[0]["delivery_city"].ToString();
                if (CityEntry.Text == "")
                    CityEntry.Placeholder = "City*";

                StateEntry.Text = (info_obj["result"])[0]["delivery_state"].ToString();
                if (StateEntry.Text == "")
                    StateEntry.Placeholder = "State*";

                ZipEntry.Text = (info_obj["result"])[0]["delivery_zip"].ToString();
                if (ZipEntry.Text == "")
                    ZipEntry.Placeholder = "Zip*";

                PhoneEntry.Text = (info_obj["result"])[0]["delivery_phone_num"].ToString();
                if (PhoneEntry.Text == "")
                    PhoneEntry.Placeholder = "Phone Number*";

                if ((info_obj["result"])[0]["delivery_instructions"].ToString() == "" || (info_obj["result"])[0]["delivery_instructions"].ToString() == "NULL")
                    DeliveryEntry.Placeholder = "Delivery Instructions";
                else DeliveryEntry.Text = (info_obj["result"])[0]["delivery_instructions"].ToString();

                CCEntry.Text = (info_obj["result"])[0]["cc_num"].ToString();
                if (CCEntry.Text == "")
                    CCEntry.Placeholder = "Credit Card Number*";

                CVVEntry.Text = (info_obj["result"])[0]["cc_cvv"].ToString();
                if (CVVEntry.Text == "")
                    CVVEntry.Placeholder = "CVC/CVV*";

                ZipCCEntry.Text = (info_obj["result"])[0]["cc_zip"].ToString();
                if (ZipCCEntry.Text == "")
                    ZipCCEntry.Placeholder = "Zip*";

                int chosenMonth = int.Parse(((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(5, 2));
                if (chosenMonth == 1)
                    MonthPicker.SelectedIndex = 0;
                else if (chosenMonth == 2)
                    MonthPicker.SelectedIndex = 1;
                else if (chosenMonth == 3)
                    MonthPicker.SelectedIndex = 2;
                else if (chosenMonth == 4)
                    MonthPicker.SelectedIndex = 3;
                else if (chosenMonth == 5)
                    MonthPicker.SelectedIndex = 4;
                else if (chosenMonth == 6)
                    MonthPicker.SelectedIndex = 5;
                else if (chosenMonth == 7)
                    MonthPicker.SelectedIndex = 6;
                else if (chosenMonth == 8)
                    MonthPicker.SelectedIndex = 7;
                else if (chosenMonth == 9)
                    MonthPicker.SelectedIndex = 8;
                else if (chosenMonth == 10)
                    MonthPicker.SelectedIndex = 9;
                else if (chosenMonth == 11)
                    MonthPicker.SelectedIndex = 10;
                else MonthPicker.SelectedIndex = 11;

                int chosenYear = int.Parse(((info_obj["result"])[0]["cc_exp_date"].ToString()).Substring(0, 4));
                if (chosenYear == 2020)
                    YearPicker.SelectedIndex = 0;
                else if (chosenYear == 2021)
                    YearPicker.SelectedIndex = 1;
                else if (chosenYear == 2022)
                    YearPicker.SelectedIndex = 2;
                else if (chosenYear == 2023)
                    YearPicker.SelectedIndex = 3;
                else if (chosenYear == 2024)
                    YearPicker.SelectedIndex = 4;
                else if (chosenYear == 2025)
                    YearPicker.SelectedIndex = 5;
                else if (chosenYear == 2026)
                    YearPicker.SelectedIndex = 6;
                else if (chosenYear == 2027)
                    YearPicker.SelectedIndex = 7;
                else if (chosenYear == 2028)
                    YearPicker.SelectedIndex = 8;
                else if (chosenYear == 2029)
                    YearPicker.SelectedIndex = 9;
                else if (chosenYear == 2030)
                    YearPicker.SelectedIndex = 10;
                else if (chosenYear == 2031)
                    YearPicker.SelectedIndex = 11;
                else if (chosenYear == 2032)
                    YearPicker.SelectedIndex = 12;
                else if (chosenYear == 2033)
                    YearPicker.SelectedIndex = 13;
                else if (chosenYear == 2034)
                    YearPicker.SelectedIndex = 14;
                else if (chosenYear == 2035)
                    YearPicker.SelectedIndex = 15;
                else if (chosenYear == 2036)
                    YearPicker.SelectedIndex = 16;
                else if (chosenYear == 2037)
                    YearPicker.SelectedIndex = 17;
                else if (chosenYear == 2038)
                    YearPicker.SelectedIndex = 18;
                else if (chosenYear == 2039)
                    YearPicker.SelectedIndex = 19;
                else if (chosenYear == 2040)
                    YearPicker.SelectedIndex = 20;
                else if (chosenYear == 2041)
                    YearPicker.SelectedIndex = 21;
                else if (chosenYear == 2042)
                    YearPicker.SelectedIndex = 22;
                else YearPicker.SelectedIndex = 23;
            }
            else
            {

            }*/
        }

        public DeliveryBilling(string Fname, string Lname, string email)
        {
            cust_firstName = Fname;
            cust_lastName = Lname;
            cust_email = email;
            InitializeComponent();
            Console.WriteLine("hashed password: " + Preferences.Get("hashed_password", ""));
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 32;
                heading.Margin = new Thickness(0, 0, 0, 30);
                pfp.HeightRequest = width / 20;
                pfp.WidthRequest = width / 20;
                pfp.CornerRadius = (int)(width / 40);
                //pfp.Margin = new Thickness(0, 0, 23, 27);
                innerGrid.Margin = new Thickness(0, 0, 23, 27);


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

                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;
                menu.Margin = new Thickness(25, 0, 0, 30);

                firstName.CornerRadius = 22;
                firstName.HeightRequest = 35;
                lastName.CornerRadius = 22;
                lastName.HeightRequest = 35;

                emailAdd.CornerRadius = 22;
                emailAdd.HeightRequest = 35;

                street.CornerRadius = 22;
                street.HeightRequest = 35;

                unit.CornerRadius = 22;
                unit.HeightRequest = 35;
                city.CornerRadius = 22;
                city.HeightRequest = 35;
                state.CornerRadius = 22;
                state.HeightRequest = 35;

                zipCode.CornerRadius = 22;
                zipCode.HeightRequest = 35;
                phoneNum.CornerRadius = 22;
                phoneNum.HeightRequest = 35;

                deliveryInstr.CornerRadius = 22;

                creditCard.CornerRadius = 22;
                creditCard.HeightRequest = 35;

                cvv.CornerRadius = 22;
                cvv.HeightRequest = 35;
                zipCode2.CornerRadius = 22;
                zipCode2.HeightRequest = 35;

                month.CornerRadius = 22;
                year.CornerRadius = 22;
                SignUpButton.CornerRadius = 25;
            }
            else //android
            {
                orangeBox.CornerRadius = 35;
                pfp.CornerRadius = 20;

                firstName.CornerRadius = 24;
                lastName.CornerRadius = 24;

                emailAdd.CornerRadius = 24;

                street.CornerRadius = 24;

                unit.CornerRadius = 24;
                city.CornerRadius = 24;
                state.CornerRadius = 24;

                zipCode.CornerRadius = 24;
                phoneNum.CornerRadius = 24;

                deliveryInstr.CornerRadius = 24;

                creditCard.CornerRadius = 24;

                cvv.CornerRadius = 24;
                zipCode2.CornerRadius = 24;

                month.CornerRadius = 24;
                year.CornerRadius = 24;
                SignUpButton.CornerRadius = 25;
            }

            fillEntries();
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
            string platform = Application.Current.Properties["platform"].ToString();
            //string passwordSalt = Preferences.Get("password_salt", "");
            // Console.WriteLine("Clicked done: The Salt is: " + passwordSalt);
            //setPaymentInfo();
            //if (string.IsNullOrEmpty(passwordSalt)){  //If social login (salt is NULL)

            //-----------validate address start

            if (AddressEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your address", "OK");
            }

            if (CityEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your city", "OK");
            }

            if (StateEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your state", "OK");
            }

            if (ZipEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your zipcode", "OK");
            }

            //if (PhoneEntry.Text == null && PhoneEntry.Text.Length == 10)
            //{
            //    await DisplayAlert("Error", "Please enter your phone number", "OK");
            //}
            if (AptEntry.Text == null)
            {
                AptEntry.Text = "";
            }

            // Setting request for USPS API
            XDocument requestDoc = new XDocument(
                new XElement("AddressValidateRequest",
                new XAttribute("USERID", "400INFIN1745"),
                new XElement("Revision", "1"),
                new XElement("Address",
                new XAttribute("ID", "0"),
                new XElement("Address1", AddressEntry.Text.Trim()),
                new XElement("Address2", AptEntry.Text.Trim()),
                new XElement("City", CityEntry.Text.Trim()),
                new XElement("State", StateEntry.Text.Trim()),
                new XElement("Zip5", ZipEntry.Text.Trim()),
                new XElement("Zip4", "")
                     )
                 )
             );
            var url = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
            Console.WriteLine(url);
            var client2 = new WebClient();
            var response2 = client2.DownloadString(url);

            var xdoc = XDocument.Parse(response2.ToString());
            Console.WriteLine("xdoc begin");
            Console.WriteLine(xdoc);


            string latitude = "0";
            string longitude = "0";
            foreach (XElement element in xdoc.Descendants("Address"))
            {
                if (GetXMLElement(element, "Error").Equals(""))
                {
                    if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())) // Best case
                    {
                        // Get longitude and latitide because we can make a deliver here. Move on to next page.
                        // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                        //GetAddressLatitudeLongitude();
                        Geocoder geoCoder = new Geocoder();

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

                        //directSignUp.latitude = latitude;
                        //directSignUp.longitude = longitude;
                        //map.MapType = MapType.Street;
                        //var mapSpan = new MapSpan(position, 0.001, 0.001);

                        //Pin address = new Pin();
                        //address.Label = "Delivery Address";
                        //address.Type = PinType.SearchResult;
                        //address.Position = position;

                        //map.MoveToRegion(mapSpan);
                        //map.Pins.Add(address);

                        //https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/-121.8866517,37.2270928 long,lat
                        //var request3 = new HttpRequestMessage();
                        //Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                        //Debug.WriteLine("latitude: " + latitude + ", longitude: " + longitude);
                        string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;
                        //request3.RequestUri = new Uri(url3);
                        //request3.Method = HttpMethod.Get;
                        //var client3 = new HttpClient();
                        //HttpResponseMessage response3 = await client3.SendAsync(request3);
                        Debug.WriteLine("categorical options url: " + url3);

                        var content = client.DownloadString(url3);
                        var obj = JsonConvert.DeserializeObject<ZonesDto>(content);

                        //HttpContent content3 = response3.Content;
                        //Console.WriteLine("content: " + content3);
                        //var userString3 = await content3.ReadAsStringAsync();
                        //Debug.WriteLine("userString3: " + userString3);
                        //JObject info_obj3 = JObject.Parse(userString3);
                        if (obj.Result.Length == 0)
                        {
                            withinZones = false;
                        }
                        else
                        {
                            Debug.WriteLine("first business: " + obj.Result[0].business_name);
                            tax = obj.Result[0].tax_rate;
                            deliveryFee = obj.Result[0].delivery_fee;
                            serviceFee = obj.Result[0].service_fee;
                            passingZones = obj.Result;
                            withinZones = true;
                        }

                        break;
                    }
                    else if (GetXMLElement(element, "DPVConfirmation").Equals("D"))
                    {
                        //await DisplayAlert("Alert!", "Address is missing information like 'Apartment number'.", "Ok");
                        //return;
                    }
                    else
                    {
                        //await DisplayAlert("Alert!", "Seems like your address is invalid.", "Ok");
                        //return;
                    }
                }
                else
                {   // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
                    //Console.WriteLine("Seems like your address is invalid.");
                    //await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
                    //return;
                }
            }
            if (latitude == "0" || longitude == "0")
            {
                await DisplayAlert("We couldn't find your address", "Please check for errors.", "Ok");
            }
            else if (withinZones == false)
            {
                await DisplayAlert("Invalid Address", "Address is not within any of our delivery zones.", "OK");
            }
            else
            {
                int startIndex = xdoc.ToString().IndexOf("<Address2>") + 10;
                int length = xdoc.ToString().IndexOf("</Address2>") - startIndex;

                string xdocAddress = xdoc.ToString().Substring(startIndex, length);
                //Console.WriteLine("xdoc address: " + xdoc.ToString().Substring(startIndex, length));
                //Console.WriteLine("xdoc end");

                if (xdocAddress != AddressEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing address", "ok");
                    AddressEntry.Text = xdocAddress;
                }

                startIndex = xdoc.ToString().IndexOf("<State>") + 7;
                length = xdoc.ToString().IndexOf("</State>") - startIndex;
                string xdocState = xdoc.ToString().Substring(startIndex, length);

                if (xdocAddress != StateEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing state", "ok");
                    StateEntry.Text = xdocState;
                }

                isAddessValidated = true;
                await DisplayAlert("We validated your address", "Please click on the Sign up button to create your account!", "OK");
                await Application.Current.SavePropertiesAsync();
                //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());

                saveInfo();
                Debug.WriteLine("passed in tax, service fee, and delivery fee: " + tax.ToString() + ", " + serviceFee.ToString() + ", " + deliveryFee.ToString());
                await Navigation.PushAsync(new VerifyInfo(passingZones, tax, serviceFee, deliveryFee, cust_firstName, cust_lastName, cust_email, AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, "", "", "", salt));

            }

            //-------------validate address end


            //old spot for navigation
            //Navigation.PushAsync(new VerifyInfo(cust_firstName, cust_lastName, cust_email, AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, "", "", "", salt));


            //if (platform != "DIRECT")
            //{
            //    // Navigation.PushAsync(new VerifyInfo(AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, CCEntry.Text, CVVEntry.Text, ZipCCEntry.Text, salt));
            //    Debug.WriteLine("AMOUNT TO PAY: " + Preferences.Get("price", "00.00"));
            //    Navigation.PushAsync(new VerifyInfo(cust_firstName, cust_lastName, cust_email, AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, "", "", "", salt));
            //}
            //else //If direct login (salt != NULL)
            //{
            //    // Navigation.PushAsync(new VerifyInfoDirectLogin(AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, CCEntry.Text, CVVEntry.Text, ZipCCEntry.Text, salt));
            //    Navigation.PushAsync(new VerifyInfoDirectLogin(cust_firstName, cust_lastName, cust_email, AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, "", "", "", salt));
            //}
            //MainPage = PaymentPage();
        }

        void saveInfo()
        {
            if (FNameEntry.Text != null)
                Preferences.Set(savedFirstName, FNameEntry.Text);

            if (LNameEntry.Text != null)
                Preferences.Set(savedLastName, LNameEntry.Text);

            if (emailEntry.Text != null)
                Preferences.Set(savedEmail, emailEntry.Text);

            if (AddressEntry.Text != null)
                Preferences.Set(savedAdd, AddressEntry.Text);

            if (AptEntry.Text != null)
                Preferences.Set(savedApt, AptEntry.Text);

            if (CityEntry.Text != null)
                Preferences.Set(savedCity, CityEntry.Text);

            if (StateEntry.Text != null)
                Preferences.Set(savedState, StateEntry.Text);

            if (ZipEntry.Text != null)
                Preferences.Set(savedZip, ZipEntry.Text);

            if (PhoneEntry.Text != null)
                Preferences.Set(savedPhone, PhoneEntry.Text);

            if (DeliveryEntry.Text != null)
                Preferences.Set(savedInstr, DeliveryEntry.Text);

        }

        public static string GetXMLElement(XElement element, string name)
        {
            var el = element.Element(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
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
            if (FNameEntry.Text == null || FNameEntry.Text == "")
            {
                DisplayAlert("Warning!", "first name required", "okay");
                return;
            }

            if (LNameEntry.Text == null || LNameEntry.ToString() == "")
            {
                DisplayAlert("Warning!", "last name required", "okay");
                return;
            }

            if (emailEntry.Text == null || emailEntry.Text == "")
            {
                DisplayAlert("Warning!", "email required", "okay");
                return;
            }

            if (AddressEntry.Text == null || AddressEntry.Text == "")
            {
                DisplayAlert("Warning!", "address required", "okay");
                return;
            }

            if (CityEntry.Text == null || CityEntry.Text == "")
            {
                DisplayAlert("Warning!", "city required", "okay");
                return;
            }

            if (StateEntry.Text == null || StateEntry.Text == "")
            {
                DisplayAlert("Warning!", "state required", "okay");
                return;
            }

            if (ZipEntry.Text == null || ZipEntry.Text == "")
            {
                DisplayAlert("Warning!", "address zip code required", "okay");
                return;
            }


            if (PhoneEntry.Text == null || PhoneEntry.Text == "")
            {
                DisplayAlert("Warning!", "phone number required", "okay");
                return;
            }

            //if (CCEntry.Text == null || CCEntry.Text == "")
            //{
            //    DisplayAlert("Warning!", "credit card number required", "okay");
            //    return;
            //}

            //if (CVVEntry.Text == null || CVVEntry.Text == "")
            //{
            //    DisplayAlert("Warning!", "CVV required", "okay");
            //    return;
            //}

            //if (ZipCCEntry.Text == null || ZipCCEntry.Text == "")
            //{
            //    DisplayAlert("Warning!", "credit card zip code required", "okay");
            //    return;
            //}

            //if (MonthPicker.SelectedIndex == -1)
            //{
            //    DisplayAlert("Warning!", "select a month", "okay");
            //    return;
            //}

            //if (YearPicker.SelectedIndex == -1)
            //{
            //    DisplayAlert("Warning!", "select a year", "okay");
            //    return;
            //}

            clickedDone(sender, e);
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