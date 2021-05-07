using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    public partial class VerifyInfoDirectLogin : ContentPage
    {
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string AptEntry, FNameEntry, LNameEntry, emailEntry, PhoneEntry, AddressEntry, CityEntry, StateEntry, ZipEntry, DeliveryEntry, CCEntry, CVVEntry, ZipCCEntry;
        public bool isSocialLogin;
        string cust_firstName; string cust_lastName; string cust_email;
        public string passwordSalt, passwordAlgo, hashedPassword;
        public bool isAddessValidated = false;
        public VerifyInfoDirectLogin(string firstName, string lastName, string email, string AptEntry1, string FNameEntry1, string LNameEntry1, string emailEntry1, string PhoneEntry1, string AddressEntry1, string CityEntry1, string StateEntry1, string ZipEntry1, string DeliveryEntry1, string CCEntry1, string CVVEntry1, string ZipCCEntry1, string salt1)
        {
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


            System.Diagnostics.Debug.WriteLine("password algo stored: " + Preferences.Get("password_algorithm", ""));
            passwordAlgo = Preferences.Get("password_algorithm", "");
            System.Diagnostics.Debug.WriteLine("password salt stored: " + Preferences.Get("password_salt", ""));
            passwordSalt = Preferences.Get("password_salt", "");


            AptEntry = AptEntry1; FNameEntry = FNameEntry1; LNameEntry = LNameEntry1; emailEntry = emailEntry1; PhoneEntry = PhoneEntry1; AddressEntry = AddressEntry1; CityEntry = CityEntry1; StateEntry = StateEntry1; ZipEntry = ZipEntry1; DeliveryEntry = DeliveryEntry1; CCEntry = CCEntry1; CVVEntry = CVVEntry1; ZipCCEntry = ZipCCEntry1;
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
                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;
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

                menu.WidthRequest = 40;
                menu.Margin = new Thickness(25, 0, 0, 30);
                password.CornerRadius = 22;
                checkoutButton.CornerRadius = 24;
            }
        }

        protected async Task setPaymentInfo()
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
            Preferences.Set("unitNum", AptEntry);

            Console.WriteLine("In set payment info: Hashing Password!");
            SHA512 sHA512 = new SHA512Managed();
            byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text + passwordSalt));
            hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
            Console.WriteLine("In set payment info:  Password Hashed!");

            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("YOUR userID is " + userID);
            newPayment.customer_uid = userID;
            //newPayment.customer_uid = "100-000082";
            //newPayment.business_uid = "200-000002";
            newPayment.items = itemsList;
            //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
            //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
            //newPayment.salt = "2020-09-22 21:55:17";
            //newPayment.salt = "";
            newPayment.salt = hashedPassword;
            newPayment.delivery_first_name = FNameEntry;
            newPayment.delivery_last_name = LNameEntry;
            newPayment.delivery_email = emailEntry;
            newPayment.delivery_phone = PhoneEntry;
            newPayment.delivery_address = AddressEntry;
            newPayment.delivery_unit = Preferences.Get("unitNum", "");
            newPayment.delivery_city = CityEntry;
            newPayment.delivery_state = StateEntry;
            newPayment.delivery_zip = ZipEntry;
            newPayment.delivery_instructions = DeliveryEntry;
            newPayment.delivery_longitude = "";
            newPayment.delivery_latitude = "";
            newPayment.order_instructions = "slow";
            newPayment.purchase_notes = "new purch";
            newPayment.amount_due = Preferences.Get("price", "00.00");
            newPayment.amount_discount = "00.00";
            newPayment.amount_paid = Preferences.Get("price", "00.00");
            newPayment.cc_num = CCEntry;
            //newPayment.cc_exp_year = YearPicker.Items[YearPicker.SelectedIndex];
            newPayment.cc_exp_year = "2022";
            //newPayment.cc_exp_month = MonthPicker.Items[MonthPicker.SelectedIndex];
            newPayment.cc_exp_month = "11";
            newPayment.cc_cvv = CVVEntry;
            newPayment.cc_zip = ZipCCEntry;

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

        async private void checkoutButton_Clicked(object sender, EventArgs e)
        {
            //-----------validate address start

            /*if (cardHolderAddress.Text == null)
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
                    DisplayAlert("heading", "changing address", "ok");
                    cardHolderAddress.Text = xdocAddress;
                }

                startIndex = xdoc.ToString().IndexOf("<State>") + 7;
                length = xdoc.ToString().IndexOf("</State>") - startIndex;
                string xdocState = xdoc.ToString().Substring(startIndex, length);

                if (xdocAddress != cardState.Text.ToUpper().Trim())
                {
                    DisplayAlert("heading", "changing state", "ok");
                    cardState.Text = xdocState;
                }

                isAddessValidated = true;
                await DisplayAlert("We validated your address", "Please click on the Sign up button to create your account!", "OK");
                await Application.Current.SavePropertiesAsync();
                //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());
            }

            //-------------validate address end*/

            setPaymentInfo();
            //Navigation.PushAsync(new Select(cust_firstName, cust_lastName, cust_email));
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
    }
}