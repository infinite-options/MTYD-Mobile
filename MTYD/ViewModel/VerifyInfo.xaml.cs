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

using Stripe;

using PayPalHttp;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;

using MTYD.Constants;

using Item = MTYD.Model.Item;
using System.Diagnostics;
using Application = Xamarin.Forms.Application;
using System.Xml.Linq;
using System.Net;
using Xamarin.Forms.Maps;
using System.Security.Cryptography;

namespace MTYD.ViewModel
{
    public partial class VerifyInfo : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string AptEntry, FNameEntry, LNameEntry, emailEntry, PhoneEntry, AddressEntry, CityEntry, StateEntry, ZipEntry, DeliveryEntry, CCEntry, CVVEntry, ZipCCEntry;
        public bool isSocialLogin;
        public bool isAddessValidated = false;
        double deviceWidth, deviceHeight;
        string hashedPassword = "";
        string billingEmail = "billing_email" + (string)Application.Current.Properties["user_id"];
        string billingName = "billing_name" + (string)Application.Current.Properties["user_id"];
        string billingNum = "billing_num" + (string)Application.Current.Properties["user_id"];
        string billingMonth = "billing_month" + (string)Application.Current.Properties["user_id"];
        string billingYear = "billing_year" + (string)Application.Current.Properties["user_id"];
        string billingCVV = "billing_cvv" + (string)Application.Current.Properties["user_id"];
        string billingAddress = "billing_address" + (string)Application.Current.Properties["user_id"];
        string billingUnit = "billing_unit" + (string)Application.Current.Properties["user_id"];
        string billingCity = "billing_city" + (string)Application.Current.Properties["user_id"];
        string billingState = "billing_state" + (string)Application.Current.Properties["user_id"];
        string billingZip = "billing_zip" + (string)Application.Current.Properties["user_id"];
        string purchaseDescription = "purchase_descr" + (string)Application.Current.Properties["user_id"];
        Zones[] passingZones;
        double tax_rate;
        double service_fee;
        double delivery_fee;
        double checkoutTotal;
        string paymentMethod;
        bool paypalPaymentDone = false;


        // CREDENTIALS CLASS
        public class Credentials
        {
            public string key { get; set; }
        }

        // PAYPAL CREDENTIALS
        private static string clientId = Constant.TestClientId;
        private static string secret = Constant.TestSecret;
        private string payPalOrderId = "";
        public static string mode = "";

        public VerifyInfo(Zones[] zones, double taxRate, double service, double delivery, string firstName, string lastName, string email, string AptEntry1, string FNameEntry1, string LNameEntry1, string emailEntry1, string PhoneEntry1, string AddressEntry1, string CityEntry1, string StateEntry1, string ZipEntry1, string DeliveryEntry1, string CCEntry1, string CVVEntry1, string ZipCCEntry1, string salt1)
        {
            tax_rate = taxRate;
            service_fee = service;
            delivery_fee = delivery;
            passingZones = zones;
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;

            

            InitializeComponent();
            if (salt1 == "")
            {
                isSocialLogin = true;
            }
            else
            {
                isSocialLogin = false;
            }

            AptEntry = AptEntry1; FNameEntry = FNameEntry1; LNameEntry = LNameEntry1; emailEntry = emailEntry1; PhoneEntry = PhoneEntry1; AddressEntry = AddressEntry1; CityEntry = CityEntry1; StateEntry = StateEntry1; ZipEntry = ZipEntry1; DeliveryEntry = DeliveryEntry1; CCEntry = CCEntry1; CVVEntry = CVVEntry1; ZipCCEntry = ZipCCEntry1;

            Preferences.Set("subtotal", Preferences.Get("price", "00.00"));
            double payment = Double.Parse(Preferences.Get("price", "00.00")) + (Double.Parse(Preferences.Get("price", "00.00")) * tax_rate);
            payment += service_fee;
            payment += delivery_fee;
            Math.Round(payment, 2);
            Debug.WriteLine("payment after tax and fees: " + payment.ToString());
            Preferences.Set("price", payment.ToString());

            //make sure price is formatted correctly
            var total = Preferences.Get("price", "00.00");
            if (total.Contains(".") == false)
                total = total + ".00";
            else if (total.Substring(total.IndexOf(".") + 1).Length == 1)
                total = total + "0";
            else if (total.Substring(total.IndexOf(".") + 1).Length == 0)
                total = total + "00";
            Preferences.Set("price", total);

            grandTotal.Text = "$" + payment.ToString();


            subtotal.Text = "$" + Preferences.Get("subtotal", "00.00").ToString();
            tax.Text = "$" + taxRate.ToString();
            serviceFee.Text = "$" + service.ToString();
            deliveryFee.Text = "$" + delivery.ToString();
            driverTip.Text = "$0";
            discount.Text = "$0";

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            deviceHeight = height;
            deviceWidth = width;

            fillEntries();


            //cardNum.Text = "**************" + CCEntry1.Substring(CCEntry1.Length - 2);
            name.Text = FNameEntry1 + " " + LNameEntry1;
            if (AptEntry1 == "NULL" || AptEntry1 == "")
            {
                apt.IsVisible = false;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }
            else
            {
                apt.Text = "Apt #" + AptEntry1;
                street.Text = AddressEntry1;
                cityStateZip.Text = CityEntry1 + ", " + StateEntry1 + " " + ZipEntry1;
            }


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

                menu.HeightRequest = width / 20;
                menu.HeightRequest = width / 20;
                menu.Margin = new Thickness(25, 0, 0, 30);
                //password.CornerRadius = 22;
                checkoutButton.CornerRadius = 24;
                password.CornerRadius = 22;
                password2.CornerRadius = 22;
            }
            SetPayPalCredentials();
        }

        public void fillEntries()
        {

            //Debug.WriteLine("billing email: " + Preferences.Get(billingAddress, ""));
            if (Preferences.Get(billingEmail, "") != "")
                cardHolderEmail.Text = Preferences.Get(billingEmail, "");
            else cardHolderEmail.Text = emailEntry;

            if (Preferences.Get(billingName, "") != "")
                cardHolderName.Text = Preferences.Get(billingName, "");
            else cardHolderName.Text = FNameEntry + " " + LNameEntry;

            if (Preferences.Get(billingNum, "") != "")
                cardHolderNumber.Text = Preferences.Get(billingNum, "");

            if (Preferences.Get(billingMonth, "") != "")
                cardExpMonth.Text = Preferences.Get(billingMonth, "");

            if (Preferences.Get(billingYear, "") != "")
                cardExpYear.Text = Preferences.Get(billingYear, "");

            if (Preferences.Get(billingCVV, "") != "")
                cardCVV.Text = Preferences.Get(billingCVV, "");

            if (Preferences.Get(billingAddress, "") != "")
                cardHolderAddress.Text = Preferences.Get(billingAddress, "");
            else cardHolderAddress.Text = AddressEntry;

            if (Preferences.Get(billingUnit, "") != "")
                cardHolderUnit.Text = Preferences.Get(billingUnit, "");
            else cardHolderUnit.Text = AptEntry;

            if (Preferences.Get(billingCity, "") != "")
                cardCity.Text = Preferences.Get(billingCity, "");
            else cardCity.Text = CityEntry;

            if (Preferences.Get(billingState, "") != "")
                cardState.Text = Preferences.Get(billingState, "");
            else cardState.Text = StateEntry;

            if (Preferences.Get(billingZip, "") != "")
                cardZip.Text = Preferences.Get(billingZip, "");
            else cardZip.Text = ZipEntry;

            if (Preferences.Get(purchaseDescription, "") != "")
                cardDescription.Text = Preferences.Get(purchaseDescription, "");

        }

        protected async Task setPaymentInfo()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();
            //need to add item_business_id
            Item item1 = new Model.Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = Preferences.Get("freqSelected", "");
            item1.item_uid = Preferences.Get("item_uid", "");
            item1.itm_business_uid = "200-000002";
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry);

            //if ((string)Application.Current.Properties["platform"] == "DIRECT")
            //{
            //    Console.WriteLine("In set payment info: Hashing Password!");
            //    SHA512 sHA512 = new SHA512Managed();
            //    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text + Preferences.Get("password_salt", "")));
            //    string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
            //    Debug.WriteLine("hashedPassword: " + hashedPassword);
            //    byte[] data2 = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text));
            //    string hashedPassword2 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
            //    Debug.WriteLine("hashedPassword solo: " + hashedPassword2);
            //    Debug.WriteLine("password_hashed: " + Preferences.Get("password_hashed", ""));
            //    Console.WriteLine("In set payment info:  Password Hashed!");
            //    if (Preferences.Get("password_hashed", "") != hashedPassword)
            //    {
            //        DisplayAlert("Error", "Wrong password entered.", "OK");
            //        return;
            //    }
            //    newPayment.salt = hashedPassword;
            //}
            //else newPayment.salt = "";

            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("YOUR userID is " + userID);
            newPayment.customer_uid = userID;
            //newPayment.customer_uid = "100-000082";
            //newPayment.business_uid = "200-000002";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            //newPayment.salt = hashedPassword;
            //testing for paypal
            if ((string)Application.Current.Properties["platform"] == "DIRECT")
                newPayment.salt = hashedPassword;
            else newPayment.salt = "";

            newPayment.delivery_first_name = FNameEntry;
            newPayment.delivery_last_name = LNameEntry;
            newPayment.delivery_email = emailEntry;
            newPayment.delivery_phone = PhoneEntry;
            newPayment.delivery_address = AddressEntry;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
            newPayment.delivery_city = CityEntry;
            newPayment.delivery_state = StateEntry;
            newPayment.delivery_zip = ZipEntry;
            //newPayment.delivery_instructions = DeliveryEntry;
            newPayment.delivery_instructions = DeliveryEntry;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = "00.00";//Preferences.Get("price", "00.00");
            

            if (paymentMethod == "stripe")
            {
                newPayment.purchase_notes = cardDescription.Text;
                newPayment.cc_num = cardHolderNumber.Text;
                newPayment.cc_exp_year = "20" + cardExpYear.Text;
                newPayment.cc_exp_month = cardExpMonth.Text;
                newPayment.cc_cvv = cardCVV.Text;
                newPayment.cc_zip = cardZip.Text;
            }
            else
            {
                newPayment.purchase_notes = "n/a";
                newPayment.cc_num = "4242424242424242";
                newPayment.cc_exp_year = "2050";
                newPayment.cc_exp_month = "08";
                newPayment.cc_cvv = "222";
                newPayment.cc_zip = "95132";
            }
            
            // OLD IMPLEMENTATION
            //==================================
            //newPayment.cc_num = CCEntry;
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            //newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            //newPayment.cc_exp_month = "11";
            //newPayment.cc_cvv = CVVEntry;
            //newPayment.cc_zip = ZipCCEntry;
            //==================================

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
            Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new System.Net.Http.HttpClient();
            var response = await client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content);
            // HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("RESPONSE TO CHECKOUT           : " + response.IsSuccessStatusCode);
                Debug.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Debug.WriteLine("SetPaymentInfo Func ENDED!");
            }
            else
            {
                await DisplayAlert("Ooops", "Our system is down. We were able to process your request. We are currently working to solve this issue", "OK");
            }
        }
        
        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        // CHECKOUT FUNCTION MOVES TO SELECT PAGE IN SUCCESSFUL PAYMENTS ONLY
        private async void checkoutButton_Clicked(object sender, EventArgs e)
        {
            if (checkoutButton.Text == "CONTINUE")
            {
                if (paymentMethod == "paypal" && (string)Application.Current.Properties["platform"] == "DIRECT")
                {
                    Console.WriteLine("In set payment info: Hashing Password!");
                    SHA512 sHA512 = new SHA512Managed();
                    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry2.Text + Preferences.Get("password_salt", "")));
                    string hashedPassword2 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    Debug.WriteLine("hashedPassword: " + hashedPassword2);
                    //byte[] data2 = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text));
                    //string hashedPassword3 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    //Debug.WriteLine("hashedPassword solo: " + hashedPassword2);
                    //Debug.WriteLine("password_hashed: " + Preferences.Get("password_hashed", ""));
                    Console.WriteLine("In set payment info:  Password Hashed!");
                    if (Preferences.Get("password_hashed", "") != hashedPassword2)
                    {
                        Debug.WriteLine("wrong password entered");
                        DisplayAlert("Error", "Wrong password entered.", "OK");
                        return;
                    }
                    else
                    {
                        Debug.WriteLine("hash finished and ready");
                        hashedPassword = hashedPassword2;
                    }
                }

                //if (true)
                //{

                Preferences.Set(billingEmail, cardHolderEmail.Text);
                Preferences.Set(billingName, cardHolderName.Text);
                Preferences.Set(billingNum, cardHolderNumber.Text);
                Preferences.Set(billingMonth, cardExpMonth.Text);
                Preferences.Set(billingYear, cardExpYear.Text);
                Preferences.Set(billingCVV, cardCVV.Text);
                Preferences.Set(billingAddress, cardHolderAddress.Text);
                Preferences.Set(billingUnit, cardHolderUnit.Text);
                Preferences.Set(billingCity, cardCity.Text);
                Preferences.Set(billingState, cardState.Text);
                Preferences.Set(billingZip, cardZip.Text);
                Preferences.Set(purchaseDescription, cardDescription.Text);

                await setPaymentInfo();
                Preferences.Set("canChooseSelect", true);
                await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                

            }
            else
            {
                await DisplayAlert("Oops", "Our records show that you still have to process your payment before moving on the meals selection. Please complete your payment via PayPal or Stripe.", "OK");
            }
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

        // STRIPE FUNCTIONS

        // FUNCTION  1:
        public async void CheckouWithStripe(System.Object sender, System.EventArgs e)
        {
            paymentMethod = "stripe";

            var total = Preferences.Get("price", "00.00");
            if (total.Contains(".") == false)
                total = total + ".00";
            else if (total.Substring(total.IndexOf(".") + 1).Length == 1)
                total = total + "0";
            else if (total.Substring(total.IndexOf(".") + 1).Length == 0)
                total = total + "00";
            Preferences.Set("price", total);


                Debug.WriteLine("STRIPE AMOUNT TO PAY: " + total);
            if (total != "00.00")
            {
                //applying tax, service and delivery fees
                //double payment = Double.Parse(total) + (Double.Parse(total) * tax_rate);
                //payment += service_fee;
                //payment += delivery_fee;
                //Math.Round(payment, 2);
                //Debug.WriteLine("payment after tax and fees: " + payment.ToString());
                //Preferences.Set("price", payment.ToString());
                //total = payment.ToString();


                //headingGrid.IsVisible = false;

                //checkoutButton.IsVisible = false;
                //backButton.IsVisible = false;
                //PaymentScreen.HeightRequest = this.Height;
                ////PaymentScreen.HeightRequest = 0;
                //PaymentScreen.Margin = new Thickness(0, -PaymentScreen.HeightRequest / 2, 0, 0);
                //PayPalScreen.Height = this.Height - (this.Height / 8);

                headingGrid.IsVisible = false;
                originalStack.IsVisible = false;
                checkoutButton.IsVisible = false;
                backButton.IsVisible = false;
                PaymentScreen.HeightRequest = deviceHeight;
                PayPalScreen.Height = deviceHeight - (deviceHeight / 8);


                //PayPalScreen.Height = ;
                StripeScreen.Height = 0;
                orangeBox.HeightRequest = 0;
                if ((string)Application.Current.Properties["platform"] == "DIRECT")
                {
                    PaymentScreen.HeightRequest = this.Height * 1.5;
                    PayPalScreen.Height = (this.Height - (this.Height / 8)) * 1.5;
                    spacer6.IsVisible = true;
                    passLabel.IsVisible = true;
                    spacer7.IsVisible = true;
                    password.IsVisible = true;
                    passwordEntry.IsVisible = true;
                    password.WidthRequest = purchDescFrame.Width;
                    //passwordEntry.WidthRequest = purchDescFrame.Width;
                    spacer8.IsVisible = true;
                    spacer9.IsVisible = true;
                }
            }
            else
            {
                await DisplayAlert("Ooops", "The amount to pay is zero. It must be greater than zero to process a payment", "OK");
            }
        }

        // FUNCTION  2:
        public async void PayViaStripe(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("PayViaStripe entered");
            try
            {
                //-----------validate address start
                    if (cardHolderName.Text == null)
                    {
                        await DisplayAlert("Error", "Please enter your name", "OK");
                        return;
                    }

                if (cardHolderEmail.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your email", "OK");
                    return;
                }

                if (cardHolderNumber.Text == null || cardHolderNumber.Text.Length != 16)
                {
                    await DisplayAlert("Error", "invalid credit card number", "OK");
                    return;
                }

                if (cardExpMonth.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your cc expiration month", "OK");
                    return;
                }
                else if (cardExpMonth.Text.Length < 2)
                {
                    await DisplayAlert("Error", "invalid month", "OK");
                    return;
                }

                if (cardExpYear.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your cc expiration year", "OK");
                    return;
                }
                else if (cardExpYear.Text.Length < 2)
                {
                    await DisplayAlert("Error", "invalid year", "OK");
                    return;
                }

                if (cardCVV.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your CVV", "OK");
                    return;
                }
                else if (cardCVV.Text.Length < 3)
                {
                    await DisplayAlert("Error", "invalid CVV", "OK");
                    return;
                }

                if (cardHolderAddress.Text == null)
                    {
                        await DisplayAlert("Error", "Please enter your address", "OK");
                    }

                    if (cardCity.Text == null)
                    {
                        await DisplayAlert("Error", "Please enter your city", "OK");
                    }

                    if (cardState.Text == null)
                    {
                        await DisplayAlert("Error", "Please enter your state", "OK");
                    }

                    if (cardZip.Text == null)
                    {
                        await DisplayAlert("Error", "Please enter your zipcode", "OK");
                    }

                    if (passwordEntry.Text == null && (string)Application.Current.Properties["platform"] == "DIRECT")
                    {
                        await DisplayAlert("Error", "Please enter your password", "OK");
                    }

                    //if (PhoneEntry.Text == null && PhoneEntry.Text.Length == 10)
                    //{
                    //    await DisplayAlert("Error", "Please enter your phone number", "OK");
                    //}

                    if (cardHolderUnit.Text == null)
                    {
                        cardHolderUnit.Text = "";
                    }
        
                    // Setting request for USPS API
                    XDocument requestDoc = new XDocument(
                        new XElement("AddressValidateRequest",
                        new XAttribute("USERID", "400INFIN1745"),
                        new XElement("Revision", "1"),
                        new XElement("Address",
                        new XAttribute("ID", "0"),
                        new XElement("Address1", cardHolderAddress.Text.Trim()),
                        new XElement("Address2", cardHolderUnit.Text.Trim()),
                        new XElement("City", cardCity.Text.Trim()),
                        new XElement("State", cardState.Text.Trim()),
                        new XElement("Zip5", cardZip.Text.Trim()),
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
                            if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(cardZip.Text.Trim()) && GetXMLElement(element, "City").Equals(cardCity.Text.ToUpper().Trim())) // Best case
                            {
                                // Get longitude and latitide because we can make a deliver here. Move on to next page.
                                // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                                //GetAddressLatitudeLongitude();
                                Geocoder geoCoder = new Geocoder();

                                IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(cardHolderAddress.Text.Trim() + "," + cardCity.Text.Trim() + "," + cardState.Text.Trim());
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
                        return;
                    }
                    else
                    {
                        int startIndex = xdoc.ToString().IndexOf("<Address2>") + 10;
                        int length = xdoc.ToString().IndexOf("</Address2>") - startIndex;

                        string xdocAddress = xdoc.ToString().Substring(startIndex, length);
                        //Console.WriteLine("xdoc address: " + xdoc.ToString().Substring(startIndex, length));
                        //Console.WriteLine("xdoc end");

                        if (xdocAddress != cardHolderAddress.Text.ToUpper().Trim())
                        {
                            //DisplayAlert("heading", "changing address", "ok");
                            cardHolderAddress.Text = xdocAddress;
                        }

                        startIndex = xdoc.ToString().IndexOf("<State>") + 7;
                        length = xdoc.ToString().IndexOf("</State>") - startIndex;
                        string xdocState = xdoc.ToString().Substring(startIndex, length);

                        if (xdocAddress != cardState.Text.ToUpper().Trim())
                        {
                            //DisplayAlert("heading", "changing state", "ok");
                            cardState.Text = xdocState;
                        }

                        isAddessValidated = true;
                        Debug.WriteLine("we validated your address");
                        //await DisplayAlert("We validated your address", "Please click on the Sign up button to create your account!", "OK");
                        await Application.Current.SavePropertiesAsync();
                        //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());
                    }
                //-------------validate address end

                if ((string)Application.Current.Properties["platform"] == "DIRECT")
                {
                    Console.WriteLine("In set payment info: Hashing Password!");
                    SHA512 sHA512 = new SHA512Managed();
                    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text + Preferences.Get("password_salt", "")));
                    string hashedPassword2 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    Debug.WriteLine("hashedPassword: " + hashedPassword2);
                    //byte[] data2 = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text));
                    //string hashedPassword3 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    //Debug.WriteLine("hashedPassword solo: " + hashedPassword2);
                    //Debug.WriteLine("password_hashed: " + Preferences.Get("password_hashed", ""));
                    Console.WriteLine("In set payment info:  Password Hashed!");
                    if (Preferences.Get("password_hashed", "") != hashedPassword2)
                    {
                        Debug.WriteLine("wrong password entered");
                        DisplayAlert("Error", "Wrong password entered.", "OK");
                        return;
                    }
                    else
                    {
                        Debug.WriteLine("hash finished and ready");
                        hashedPassword = hashedPassword2;
                    }
                }

        //start commenting here?

                //var total = Preferences.Get("price", "00.00");
                //var clientHttp = new System.Net.Http.HttpClient();
                //var stripe = new Credentials();
                //    stripe.key = Constant.TestPK;
                //    //stripe.key = Constant.LivePK;

                //var stripeObj = JsonConvert.SerializeObject(stripe);
                //var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
                //var RDSResponse = await clientHttp.PostAsync(Constant.StripeModeUrl, stripeContent);
                //var content = await RDSResponse.Content.ReadAsStringAsync();

                //Debug.WriteLine("key to send JSON: " + stripeObj);
                //Debug.WriteLine("Response from key: " + content);

                //if (RDSResponse.IsSuccessStatusCode)
                //{
                //    //Carlos original code
                //    //if (content != "200")
                //    if (content.Contains("200"))
                //    {
                //        //Debug.WriteLine("error encountered");
                //        string SK = "";
                //        string mode = "";

                //        if (stripeObj.Contains("test"))
                //        {
                //            mode = "TEST";
                //            SK = Constant.TestSK;
                //        }
                //        else if (stripeObj.Contains("live"))
                //        {
                //            mode = "LIVE";
                //            SK = Constant.LiveSK;
                //        }
                //        //Carlos original code
                //        //if (content.Contains("Test"))
                //        //{
                //        //    mode = "TEST";
                //        //    SK = Constant.TestSK;
                //        //}
                //        //else if (content.Contains("Live"))
                //        //{
                //        //    mode = "LIVE";
                //        //    SK = Constant.LiveSK;
                //        //}

                //        Debug.WriteLine("MODE          : " + mode);
                //        Debug.WriteLine("STRIPE SECRET : " + SK);
                       
                //        //Debug.WriteLine("SK" + SK);
                //        StripeConfiguration.ApiKey = SK;

                //        string CardNo = cardHolderNumber.Text.Trim();
                //        string expMonth = cardExpMonth.Text.Trim();
                //        string expYear = cardExpYear.Text.Trim();
                //        string cardCvv = cardCVV.Text.Trim();

                //        Debug.WriteLine("step 1 reached");
                //        // Step 1: Create Card
                //        TokenCardOptions stripeOption = new TokenCardOptions();
                //        stripeOption.Number = CardNo;
                //        stripeOption.ExpMonth = Convert.ToInt64(expMonth);
                //        stripeOption.ExpYear = Convert.ToInt64(expYear);
                //        stripeOption.Cvc = cardCvv;

                //        Debug.WriteLine("step 2 reached");
                //        // Step 2: Assign card to token object
                //        TokenCreateOptions stripeCard = new TokenCreateOptions();
                //        stripeCard.Card = stripeOption;

                //        TokenService service = new TokenService();
                //        Stripe.Token newToken = service.Create(stripeCard);

                //        Debug.WriteLine("step 3 reached");
                //        // Step 3: Assign the token to the soruce 
                //        var option = new SourceCreateOptions();
                //        option.Type = SourceType.Card;
                //        option.Currency = "usd";
                //        option.Token = newToken.Id;

                //        var sourceService = new SourceService();
                //        Source source = sourceService.Create(option);

                //        Debug.WriteLine("step 4 reached");
                //        // Step 4: Create customer
                //        CustomerCreateOptions customer = new CustomerCreateOptions();
                //        customer.Name = cardHolderName.Text.Trim();
                //        customer.Email = cardHolderEmail.Text.ToLower().Trim();
                //        if (cardDescription.Text == "" || cardDescription.Text == null)
                //            customer.Description = "";
                //        else customer.Description = cardDescription.Text.Trim();
                //        if (cardHolderUnit.Text == null)
                //        {
                //            cardHolderUnit.Text = "";
                //        }
                //        customer.Address = new AddressOptions { City = cardCity.Text.Trim(), Country = Constant.Contry, Line1 = cardHolderAddress.Text.Trim(), Line2 = cardHolderUnit.Text.Trim(), PostalCode = cardZip.Text.Trim(), State = cardState.Text.Trim() };

                //        var customerService = new CustomerService();
                //        var cust = customerService.Create(customer);

                //        Debug.WriteLine("step 5 reached");
                //        // Step 5: Charge option
                //        var chargeOption = new ChargeCreateOptions();
                //        chargeOption.Amount = (long)RemoveDecimalFromTotalAmount(total);
                        
                //        Debug.WriteLine("hopefully correct total: " + total);
                //        chargeOption.Currency = "usd";
                //        chargeOption.ReceiptEmail = cardHolderEmail.Text.ToLower().Trim();
                //        chargeOption.Customer = cust.Id;
                //        chargeOption.Source = source.Id;
                //        if (cardDescription.Text == "" || cardDescription.Text == null)
                //            chargeOption.Description = "";
                //        else chargeOption.Description = cardDescription.Text.Trim();

                //        //chargeOption.Description = cardDescription.Text.Trim();

                //        Debug.WriteLine("step 6 reached");
                        // Step 6: charge the customer COMMENTED OUT FOR TESTING, backend already charges stripe so we don't have to do it here
                        //var chargeService = new ChargeService();
                        //Charge charge = chargeService.Create(chargeOption);
                        //Debug.WriteLine("charge: " + charge.ToString());
                        //if (charge.Status == "succeeded")
                        //{
                            await Navigation.PushAsync(new Loading());

                            PaymentScreen.HeightRequest = 0;
                            PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                            StripeScreen.Height = 0;
                            PayPalScreen.Height = 0;
                            orangeBox.HeightRequest = deviceHeight / 2;

                            Debug.WriteLine("STRIPE PAYMENT WAS SUCCESSFUL");
        //stop commenting here?
                            //Preferences.Set("price", "00.00");
                            //DisplayAlert("Payment Completed", "Your payment was successful. Press 'CONTINUE' to select your meals!", "OK");
                            //when purchasing a first meal, might not send to endpoint on time when clicking the continue button as fast as possible, resulting in error on select pg
                            //wait half a second
                            Task.Delay(500).Wait();
                            if ((string)Application.Current.Properties["platform"] == "DIRECT")
                            {
                                spacer6.IsVisible = true;
                                passLabel.IsVisible = true;
                                spacer7.IsVisible = true;
                                password.IsVisible = true;
                                passwordEntry.IsVisible = true;
                                spacer8.IsVisible = true;
                            }
                            checkoutButton.Text = "CONTINUE";
                            headingGrid.IsVisible = true;
                            checkoutButton.IsVisible = true;
                            backButton.IsVisible = true;

                            //checkout button clicked functionality added below vv
                            Preferences.Set(billingEmail, cardHolderEmail.Text);
                            Preferences.Set(billingName, cardHolderName.Text);
                            Preferences.Set(billingNum, cardHolderNumber.Text);
                            Preferences.Set(billingMonth, cardExpMonth.Text);
                            Preferences.Set(billingYear, cardExpYear.Text);
                            Preferences.Set(billingCVV, cardCVV.Text);
                            Preferences.Set(billingAddress, cardHolderAddress.Text);
                            Preferences.Set(billingUnit, cardHolderUnit.Text);
                            Preferences.Set(billingCity, cardCity.Text);
                            Preferences.Set(billingState, cardState.Text);
                            Preferences.Set(billingZip, cardZip.Text);
                            Preferences.Set(purchaseDescription, cardDescription.Text);

                            await setPaymentInfo();
                            Preferences.Set("canChooseSelect", true);
                            await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                            //done from checkout button clicked
                        //}
                        //else
                        //{
                        //    // Fail
                        //    await DisplayAlert("Ooops", "Payment was not succesfull. Please try again", "OK");
                        //}
                    //}
                //}
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert!", ex.Message, "OK");
            }
        }

        // FUNCTION  3:
        public int RemoveDecimalFromTotalAmount(string amount)
        {
            var stringAmount = "";
            var arrayAmount = amount.ToCharArray();
            for (int i = 0; i < arrayAmount.Length; i++)
            {
                if ((int)arrayAmount[i] != (int)'.')
                {
                    stringAmount += arrayAmount[i];
                }
            }
            System.Diagnostics.Debug.WriteLine(stringAmount);
            return Int32.Parse(stringAmount);
        }

        // FUNCTION  4:
        public void CancelViaStripe(System.Object sender, System.EventArgs e)
        {
            headingGrid.IsVisible = true;
            checkoutButton.IsVisible = true;
            backButton.IsVisible = true;
            originalStack.IsVisible = true;

            PaymentScreen.HeightRequest = 0;
            PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
            StripeScreen.Height = 0;
            PayPalScreen.Height = 0;
            orangeBox.HeightRequest = deviceHeight / 2;
        }

        // PAYPAL FUNCTIONS

        // FUNCTION  1: SET BROWSER AND PAYPAL SCREEN TO PROCESS PAYMENT
        //step from carlos' notes: purchase an order via PayPal use the following credentials:
        //Card Type: Visa.
        //Card Number: 4032031027352565
        //Expiration Date: 02/2024
        //CVV: 154
        public async void CheckouWithPayPayl(System.Object sender, System.EventArgs e)
        {
            paymentMethod = "paypal";

            Debug.WriteLine("paypal CheckouWithPayPayl called 1");

            var total = Preferences.Get("price", "00.00");
            Debug.WriteLine("PAYPAL AMOUNT TO PAY: " + total);
            if (total != "00.00")
            {
                //applying tax, service and delivery fees
                //double payment = Double.Parse(total) + (Double.Parse(total) * tax_rate);
                //payment += service_fee;
                //payment += delivery_fee;
                //Math.Round(payment, 2);
                //Debug.WriteLine("payment after tax and fees: " + payment.ToString());
                //Preferences.Set("price", payment.ToString());
                //total = payment.ToString();

                headingGrid.IsVisible = false;
                checkoutButton.IsVisible = false;
                backButton.IsVisible = false;
                PaymentScreen.HeightRequest = this.Height;
                //PaymentScreen.Margin = new Thickness(0, -PaymentScreen.HeightRequest / 2, 0, 0);
                PaymentScreen.Margin = new Thickness(0, -originalStack.Height, 0, 0);
                PayPalScreen.Height = 0;
                StripeScreen.Height = this.Height;
                Browser.HeightRequest = this.Height - (this.Height / 8);
                orangeBox.HeightRequest = 0;

                //new?
                //headingGrid.IsVisible = false;
                //originalStack.IsVisible = false;
                //checkoutButton.IsVisible = false;
                //backButton.IsVisible = false;
                //PaymentScreen.HeightRequest = deviceHeight;
                //StripeScreen.Height = 0;
                //Browser.HeightRequest = deviceHeight - (deviceHeight / 8);
                //orangeBox.HeightRequest = 0;


                //if ((string)Application.Current.Properties["platform"] == "DIRECT")
                //{
                //    spacer6.IsVisible = true;
                //    passLabel.IsVisible = true;
                //    spacer7.IsVisible = true;
                //    password.IsVisible = true;
                //    passwordEntry.IsVisible = true;
                //    spacer8.IsVisible = true;
                //}
                PayViaPayPal(sender, e);
            }
            else
            {
                await DisplayAlert("Ooops", "The amount to pay is zero. It must be greater than zero to process a payment", "OK");
            }
        }

        // FUNCTION  2: CREATES A PAYMENT REQUEST
        public async void PayViaPayPal(System.Object sender, System.EventArgs e)
        {
            Debug.WriteLine("paypal PayViaPayPal called 2");

            var response = await createOrder(Preferences.Get("price", "00.00"));
            var content = response.Result<PayPalCheckoutSdk.Orders.Order>();
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            Debug.WriteLine("Status: {0}", result.Status);
            Debug.WriteLine("Order Id: {0}", result.Id);
            Debug.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
            Debug.WriteLine("Links:");

            foreach (PayPalCheckoutSdk.Orders.LinkDescription link in result.Links)
            {
                Debug.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                if (link.Rel == "approve")
                {
                    Browser.Source = link.Href;
                }
            }

            Browser.Navigated += Browser_Navigated;
            payPalOrderId = result.Id;
        }

        // FUNCTION  3: SET BROWSER SOURCE WITH PROPER URL TO PROCESS PAYMENT
        private void Browser_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Debug.WriteLine("paypal Browser_Navigated called 3");

            var source = Browser.Source as UrlWebViewSource;
            Debug.WriteLine("BROWSER CURRENT SOURCE: " + source.Url);
            //old link used to check: https://servingfresh.me/
            //m4me link: https://mealtoyourdoor.netlify.app/home
            //new m4me link: https://mealsfor.me
            //paypal check info: Card Type: Visa. Card Number: 4032031027352565 Expiration Date: 02/2024 CVV: 154
            if (source.Url == "https://mealsfor.me/home")
            {
                //Navigation.PushAsync(new Loading());
                if (paypalPaymentDone == true)
                {
                    headingGrid.IsVisible = true;
                    checkoutButton.IsVisible = true;
                    backButton.IsVisible = true;
                    originalStack.IsVisible = false;
                }
                headingGrid.IsVisible = true;
                checkoutButton.IsVisible = true;
                backButton.IsVisible = true;
                PaymentScreen.HeightRequest = 0;
                PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                PayPalScreen.Height = 0;
                StripeScreen.Height = 0;

                if (checkoutButton.Text == "CONTINUE")
                    Debug.WriteLine("checkout was changed");
                
                _ = captureOrder(payPalOrderId);

                //Navigation.PopAsync();
            }
        }

        // FUNCTION  4: PAYPAL CLIENT
        public static PayPalHttp.HttpClient client()
        {
            Debug.WriteLine("paypal client called 4");

            Debug.WriteLine("PAYPAL CLIENT ID MTYD: " + clientId);
            Debug.WriteLine("PAYPAL SECRET MTYD   : " + secret);

            if (mode == "TEST")
            {
                PayPalEnvironment enviroment = new SandboxEnvironment(clientId, secret);
                PayPalHttpClient payPalClient = new PayPalHttpClient(enviroment);
                return payPalClient;
            }
            else if (mode == "LIVE")
            {
                PayPalEnvironment enviroment = new LiveEnvironment(clientId, secret);
                PayPalHttpClient payPalClient = new PayPalHttpClient(enviroment);
                return payPalClient;
            }
            return null;
        }

        // FUNCTION  5: SET PAYPAL CREDENTIALS
        public async void SetPayPalCredentials()
        {
            Debug.WriteLine("paypal SetPayPalCredentials called 5");

            var clientHttp = new System.Net.Http.HttpClient();
            var paypal = new Credentials();
                paypal.key = Constant.LiveClientId;

            var stripeObj = JsonConvert.SerializeObject(paypal);
            var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
            var RDSResponse = await clientHttp.PostAsync(Constant.PayPalModeUrl, stripeContent);
            var content = await RDSResponse.Content.ReadAsStringAsync();

            Debug.WriteLine("CREDENTIALS JSON OBJECT TO SEND: " + stripeObj);
            Debug.WriteLine("RESPONE FROM PAYPAL ENDPOINT   : " + content);

            if (RDSResponse.IsSuccessStatusCode)
            {
                if (!content.Contains("200"))
                {
                    if (content.Contains("Test"))
                    {
                        mode = "TEST";
                        clientId = Constant.TestClientId;
                        secret = Constant.TestSecret;
                    }
                    else if (content.Contains("Live"))
                    {
                        mode = "LIVE";
                        clientId = Constant.LiveClientId;
                        secret = Constant.LiveSecret;
                    }
                    Debug.WriteLine("MODE:             " + mode);
                    Debug.WriteLine("PAYPAL CLIENT ID: " + clientId);
                    Debug.WriteLine("PAYPAL SECRENT:   " + secret);
                }
                else
                {
                    Debug.WriteLine("ERROR");
                    await DisplayAlert("Oops", "We can't not process your request at this moment.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Oops", "We can't not process your request at this moment.", "OK");
            }
        }

        // FUNCTION  6: CREATE ORDER REQUEST
        public async static Task<HttpResponse> createOrder(string amount)
        {
            Debug.WriteLine("paypal createOrder called 6");

            HttpResponse response;
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            var order = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = amount
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "https://mealsfor.me/home",
                    CancelUrl = "https://mealsfor.me/home"
                }
            };


            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(order);
                response = await client().Execute(request);
            return response;
        }

        // FUNCTION  7: CAPTURE ORDER
        public async Task<HttpResponse> captureOrder(string id)
        {
            
            Debug.WriteLine("paypal captureOrder called 7");
            Debug.WriteLine("passed in id: " + id);
            //await Navigation.PushAsync(new Loading());

            // Construct a request object and set desired parameters
            // Replace ORDER-ID with the approved order id from create order
            var request = new OrdersCaptureRequest(id);
                request.RequestBody(new OrderActionRequest());

            //Navigation.PushAsync(new Loading());
            var response = await client().Execute(request);
            Debug.WriteLine("response: " + response.ToString());
            await Navigation.PushAsync(new Loading());
            Debug.WriteLine("after response");
            var statusCode = response.StatusCode;
            Debug.WriteLine("after statusCode");
            var code = statusCode.ToString();
            Debug.WriteLine("after code");
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            //await Navigation.PushAsync(new Loading());

            Debug.WriteLine("REQUEST STATUS CODE: " + code);
            Debug.WriteLine("PAYPAL STATUS      : " + result.Status);
            Debug.WriteLine("ORDER ID           : " + result.Id);
            Debug.WriteLine("ID                 : " + id);

            if (result.Status == "COMPLETED")
            {
                Debug.WriteLine("PAYPAL PAYMENT WAS SUCCESSFUL");


                //testing with password
                if ((string)Application.Current.Properties["platform"] == "DIRECT")
                {
                    paypalPaymentDone = true;
                    orangeBox.HeightRequest = deviceHeight / 2;
                    passStack.IsVisible = true;
                    originalStack.IsVisible = false;
                    //spacer6.IsVisible = true;
                    //passLabel.IsVisible = true;
                    //spacer7.IsVisible = true;
                    //password.IsVisible = true;
                    //passwordEntry.IsVisible = true;
                    //spacer8.IsVisible = true;

                    checkoutButton.Text = "CONTINUE";
                    await Navigation.PopAsync();
                    return response;

                }
                else
                {
                    //checkout button clicked functionality added below vv
                    Preferences.Set(billingEmail, cardHolderEmail.Text);
                    Preferences.Set(billingName, cardHolderName.Text);
                    Preferences.Set(billingNum, cardHolderNumber.Text);
                    Preferences.Set(billingMonth, cardExpMonth.Text);
                    Preferences.Set(billingYear, cardExpYear.Text);
                    Preferences.Set(billingCVV, cardCVV.Text);
                    Preferences.Set(billingAddress, cardHolderAddress.Text);
                    Preferences.Set(billingUnit, cardHolderUnit.Text);
                    Preferences.Set(billingCity, cardCity.Text);
                    Preferences.Set(billingState, cardState.Text);
                    Preferences.Set(billingZip, cardZip.Text);
                    Preferences.Set(purchaseDescription, cardDescription.Text);

                    await setPaymentInfo();
                    Preferences.Set("canChooseSelect", true);
                    await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                    //done from checkout button clicked

                    //Preferences.Set("price", "00.00");
                    //await DisplayAlert("Payment Completed","Your payment was successful. Press 'CONTINUE' to select your meals!","OK");
                    //orangeBox.HeightRequest = deviceHeight / 2;
                    //if ((string)Application.Current.Properties["platform"] == "DIRECT")
                    //{
                    //    spacer6.IsVisible = true;
                    //    passLabel.IsVisible = true;
                    //    spacer7.IsVisible = true;
                    //    password.IsVisible = true;
                    //    passwordEntry.IsVisible = true;
                    //    spacer8.IsVisible = true;
                    //}
                    //checkoutButton.Text = "CONTINUE";
                }
            }
            else
            {
                await Navigation.PopAsync();
                Debug.WriteLine("didn't work");
                await DisplayAlert("Ooops", "You payment was cancel or not sucessful. Please try again", "OK");
            }

            return response;
        }

        
    }
}
