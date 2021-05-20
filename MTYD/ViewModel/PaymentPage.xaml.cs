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
using MTYD.Constants;
using System.Security.Cryptography;
using PayPalCheckoutSdk.Core;
using PayPalHttp;
using PayPalCheckoutSdk.Orders;
using MTYD.Model.Login.LoginClasses;
using Stripe;
using System.Windows.Input;

namespace MTYD.ViewModel
{
    /*
    Preferences.Set("prevFName", FNameEntry.Text);
    Preferences.Set("prevLName", LNameEntry.Text);
    Preferences.Set("prevEmail", emailEntry.Text);
    Preferences.Set("prevPhone", PhoneEntry.Text);
    Preferences.Set("prevAdd", AddressEntry.Text);
    Preferences.Set("prevApt", AptEntry.Text);
    Preferences.Set("prevCity", CityEntry.Text);
    Preferences.Set("prevState", StateEntry.Text);
    Preferences.Set("prevZip", ZipEntry.Text);
    Preferences.Set("prevInstr", DeliveryEntry.Text);
    Preferences.Set("prevCardName", cardHolderName.Text);
    Preferences.Set("prevCardNum", cardHolderNumber.Text);
    Preferences.Set("prevExpMonth", cardExpMonth.Text);
    Preferences.Set("prevExpYear", cardExpYear.Text);
    Preferences.Set("prevCVV", cardCVV.Text);
    Preferences.Set("prevCardAdd", cardHolderAddress.Text);
    Preferences.Set("prevCardUnit", cardHolderUnit.Text);
    Preferences.Set("prevCardCity", cardCity.Text);
    Preferences.Set("prevCardState", cardState.Text);
    Preferences.Set("prevCardZip", cardZip.Text);
    Preferences.Set("prevProceed", prevProceed);
    Preferences.Set("prevAddFilled", prevAddFilled);
    Preferences.Set("anyPrev", anyPrev);
     */


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewDeliveryInfo = new ObservableCollection<Plans>();
        public string salt;
        string fullName; string emailAddress;
        public bool isAddessValidated = false;
        bool withinZones = false;
        //WebClient client = new WebClient();
        WebClient client4 = new WebClient();
        Zones[] passingZones;
        double tax;
        double serviceFee;
        double deliveryFee;
        double tax_rate;
        double service_fee;
        double delivery_fee;
        double checkoutTotal;
        string savedFirstName;
        string savedLastName;
        string savedEmail;
        string savedAdd;
        string savedApt;
        string savedCity;
        string savedState;
        string savedZip;
        string savedPhone;
        string savedInstr;

        string hashedPassword;
        string billingEmail;
        string billingName;
        string billingNum;
        string billingMonth;
        string billingYear;
        string billingCVV;
        string billingAddress;
        string billingUnit;
        string billingCity;
        string billingState;
        string billingZip;
        string chargeId = "";
        string purchaseDescription = "";
        string paymentMethod;
        bool paypalPaymentDone = false;
        double deviceWidth, deviceHeight;
        public SignUpPost directSignUp = new SignUpPost();
        bool paymentSucceed = false;
        string new_purchase_id = "";
        string guestSalt = "";
        string checkoutAdd, checkoutUnit, checkoutCity, checkoutState, checkoutZip;
        bool onStripeScreen = false;
        bool onCardAdd = false;
        bool termsChecked = false;

        Model.Address addr;
        
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


        //auto-populate the delivery info if the user has already previously entered it
        public async void fillEntriesDeliv()
        {
            try
            {
                if (Preferences.Get("anyPrev", false) == true)
                {

                    FNameEntry.Text = Preferences.Get("prevFName", "");
                    LNameEntry.Text = Preferences.Get("prevLName", "");
                    emailEntry.Text = Preferences.Get("prevEmail", "");
                    PhoneEntry.Text = Preferences.Get("prevPhone", "");
                    AddressEntry.Text = Preferences.Get("prevAdd", "");
                    AptEntry.Text = Preferences.Get("prevApt", "");
                    CityEntry.Text = Preferences.Get("prevCity", "");
                    StateEntry.Text = Preferences.Get("prevState", "");
                    ZipEntry.Text = Preferences.Get("prevZip", "");
                    DeliveryEntry.Text = Preferences.Get("prevInstr", "");
                    cardHolderName.Text = Preferences.Get("prevCardName", "");
                    cardHolderNumber.Text = Preferences.Get("prevCardNum", "");
                    cardExpMonth.Text = Preferences.Get("prevExpMonth", "");
                    cardExpYear.Text = Preferences.Get("prevExpYear", "");
                    cardCVV.Text = Preferences.Get("prevCVV", "");
                    cardHolderAddress.Text = Preferences.Get("prevCardAdd", "");
                    cardHolderUnit.Text = Preferences.Get("prevCardUnit", "");
                    cardCity.Text = Preferences.Get("prevCardCity", "");
                    cardState.Text = Preferences.Get("prevCardState", "");
                    cardZip.Text = Preferences.Get("prevCardZip", "");

                    if (Preferences.Get("prevProceeed", false) == true)
                    {
                        EventArgs e = new EventArgs();
                        clickedDeliv(proceedButton, e);
                    }
                    else if (Preferences.Get("prevAddFilled", false) == true)
                    {
                        // Setting request for USPS API
                        XDocument requestDoc = new XDocument(
                            new XElement("AddressValidateRequest",
                            new XAttribute("USERID", "400INFIN1745"),
                            new XElement("Revision", "1"),
                            new XElement("Address",
                            new XAttribute("ID", "0"),
                            new XElement("Address1", Preferences.Get("prevAdd", "")),
                            new XElement("Address2", Preferences.Get("prevApt", "")),
                            new XElement("City", Preferences.Get("prevCity", "")),
                            new XElement("State", Preferences.Get("prevState", "")),
                            new XElement("Zip5", Preferences.Get("prevZip", "")),
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
                                //  && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())
                                if (GetXMLElement(element, "DPVConfirmation").Equals("Y")) // Best case
                                {
                                    // Get longitude and latitide because we can make a deliver here. Move on to next page.
                                    // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                                    //GetAddressLatitudeLongitude();
                                    Geocoder geoCoder = new Geocoder();

                                    Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                                    Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                                    Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                                    IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                                    Position position = approximateLocations.FirstOrDefault();

                                    latitude = $"{position.Latitude}";
                                    longitude = $"{position.Longitude}";

                                    map.MapType = MapType.Street;
                                    var mapSpan = new MapSpan(position, 0.001, 0.001);

                                    Pin address = new Pin();
                                    address.Label = "Delivery Address";
                                    address.Type = PinType.SearchResult;
                                    address.Position = position;

                                    map.MoveToRegion(mapSpan);
                                    map.Pins.Add(address);
                                }
                            }
                        }
                        //end address updating
                    }

                }

                if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST" && Preferences.Get("mainPageAdd", "") != "")
                {
                    AddressEntry.Text = Preferences.Get("mainPageAdd", "");
                    CityEntry.Text = Preferences.Get("mainPageCity", "");
                    StateEntry.Text = Preferences.Get("mainPageState", "");
                    ZipEntry.Text = Preferences.Get("mainPageZip", "");

                    // Setting request for USPS API
                    XDocument requestDoc = new XDocument(
                        new XElement("AddressValidateRequest",
                        new XAttribute("USERID", "400INFIN1745"),
                        new XElement("Revision", "1"),
                        new XElement("Address",
                        new XAttribute("ID", "0"),
                        new XElement("Address1", Preferences.Get("mainPageAdd", "")),
                        new XElement("Address2", ""),
                        new XElement("City", Preferences.Get("mainPageCity", "")),
                        new XElement("State", Preferences.Get("mainPageState", "")),
                        new XElement("Zip5", Preferences.Get("mainPageZip", "")),
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
                            //  && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())
                            if (GetXMLElement(element, "DPVConfirmation").Equals("Y")) // Best case
                            {
                                // Get longitude and latitide because we can make a deliver here. Move on to next page.
                                // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                                //GetAddressLatitudeLongitude();
                                Geocoder geoCoder = new Geocoder();

                                Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                                Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                                Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                                IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                                Position position = approximateLocations.FirstOrDefault();

                                latitude = $"{position.Latitude}";
                                longitude = $"{position.Longitude}";

                                map.MapType = MapType.Street;
                                var mapSpan = new MapSpan(position, 0.001, 0.001);

                                Pin address = new Pin();
                                address.Label = "Delivery Address";
                                address.Type = PinType.SearchResult;
                                address.Position = position;

                                map.MoveToRegion(mapSpan);
                                map.Pins.Add(address);
                            }
                        }
                    }
                    //end address updating
                }
                else if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
                {

                }
                else if (Preferences.Get(savedFirstName, "") != "" && (string)Xamarin.Forms.Application.Current.Properties["platform"] != "GUEST")
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
                    else DeliveryEntry.Placeholder = "Delivery Instructions (for example:\n gate code, or where to put\nyour meals if you're not home)";

                    EventArgs e = new EventArgs();
                    clickedDeliv(proceedButton, e);
                }
                //if there is no saved info
                else if (Preferences.Get(savedFirstName, "") == "" && (string)Xamarin.Forms.Application.Current.Properties["platform"] != "GUEST")
                {
                    Console.WriteLine("no info");
                    string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                    Debug.WriteLine("getProfileInfo url: " + url);
                    var request3 = new HttpRequestMessage();
                    request3.RequestUri = new Uri(url);
                    request3.Method = HttpMethod.Get;
                    var client2 = new System.Net.Http.HttpClient();
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


                    DeliveryEntry.Placeholder = "Delivery Instructions (for example:\ngate code, or where to put\nyour meals if you're not home)";

                    return;
                }

                addressList.IsVisible = false;
                UnitCity.IsVisible = true;
                StateZip.IsVisible = true;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public PaymentPage(string Fname, string Lname, string email)
        {
            try
            {
                Preferences.Set("subtotal", Preferences.Get("price", "00.00"));

                savedFirstName = "firstName" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedLastName = "lastName" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedEmail = "email" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedAdd = "address" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedApt = "apt" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedCity = "city" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedState = "state" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedZip = "zip" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedPhone = "phone" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                savedInstr = "instructions" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                hashedPassword = "";
                billingEmail = "billing_email" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingName = "billing_name" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingNum = "billing_num" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingMonth = "billing_month" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingYear = "billing_year" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingCVV = "billing_cvv" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingAddress = "billing_address" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingUnit = "billing_unit" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingCity = "billing_city" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingState = "billing_state" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                billingZip = "billing_zip" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                purchaseDescription = "purchase_descr" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];


                cust_firstName = Fname;
                cust_lastName = Lname;
                cust_email = email;
                addr = new Model.Address();
                InitializeComponent();
                BindingContext = this;

                if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
                {
                    menu.IsVisible = false;
                    back.IsVisible = true;
                    innerGrid.IsVisible = false;
                    //topBackButton.IsVisible = true;
                }

                Console.WriteLine("hashed password: " + Preferences.Get("hashed_password", ""));
                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                deviceHeight = height;
                deviceWidth = width;

                //if no address is saved, use the lat and long of 1408 Dot Ct, San Jose, CA 95120
                if (Preferences.Get("user_latitude", "").ToString() == "" || Preferences.Get("user_latitude", "").ToString() == "0.0")
                {
                    //var zoom = 5; // 1-18
                    //var deg = 360 / (Math.Pow(2, zoom));
                    //map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, deg, deg));
                    Preferences.Set("user_latitude", "37.338207");
                    Preferences.Set("user_longitude", "-121.886330");

                    //initializing the maps tool
                    Position position = new Position(Double.Parse(Preferences.Get("user_latitude", "").ToString()), Double.Parse(Preferences.Get("user_longitude", "").ToString()));
                    map.MapType = MapType.Street;
                    //var mapSpan = new MapSpan(position, 0.001, 0.001);
                    var mapSpan = new MapSpan(position, 360 / (Math.Pow(2, 14)), 360 / (Math.Pow(2, 14)));
                    Pin address = new Pin();
                    address.Label = "Delivery Address";
                    address.Type = PinType.SearchResult;
                    address.Position = position;
                    map.MoveToRegion(mapSpan);
                    map.Pins.Add(address);
                }
                else
                {
                    //initializing the maps tool
                    Position position = new Position(Double.Parse(Preferences.Get("user_latitude", "").ToString()), Double.Parse(Preferences.Get("user_longitude", "").ToString()));
                    map.MapType = MapType.Street;
                    var mapSpan = new MapSpan(position, 0.001, 0.001);
                    Pin address = new Pin();
                    address.Label = "Delivery Address";
                    address.Type = PinType.SearchResult;
                    address.Position = position;
                    map.MoveToRegion(mapSpan);
                    map.Pins.Add(address);
                }


                //if (Preferences.Get("user_latitude", "").ToString() == "" || Preferences.Get("user_latitude", "").ToString() == "0.0")
                //{
                //    var zoom = 1; // 1-18
                //    var deg = 360 / (Math.Pow(2, zoom));
                //    map.MoveToRegion(new MapSpan(map.VisibleRegion.Center, deg, deg));
                //}

                if (Device.RuntimePlatform == Device.iOS)
                {
                    //open menu adjustments
                    orangeBox2.HeightRequest = height / 2;
                    orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                    orangeBox2.CornerRadius = height / 40;
                    heading2.WidthRequest = 140;
                    menu2.Margin = new Thickness(25, 0, 0, 30);
                    menu.WidthRequest = 40;
                    menu2.WidthRequest = 40;
                    //menu2.Margin = new Thickness(25, 0, 0, 30);
                    heading.WidthRequest = 140;
                    //heading adjustments

                    orangeBox.HeightRequest = height / 2;
                    orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                    orangeBox.CornerRadius = height / 40;
                    //heading.WidthRequest = width / 3;
                    //menu.WidthRequest = 40;
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

                    menu.Margin = new Thickness(25, 0, 0, 30);
                    menu.WidthRequest = 40;
                    back.Margin = new Thickness(25, 0, 0, 30);
                    back.HeightRequest = 25;
                    //menu.Margin = new Thickness(25, 0, 0, 30);
                    CheckAddressGrid.Margin = new Thickness(50, 170, 50, 120);
                    //topBackButton.HeightRequest = width / 25;
                    //topBackButton.WidthRequest = width / 25;
                    //topBackButton.Margin = new Thickness(25, 0, 0, 30);

                }
                else //android
                {
                    //open menu adjustments
                    orangeBox2.HeightRequest = height / 2;
                    orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                    orangeBox2.CornerRadius = height / 40;
                    heading2.WidthRequest = 140;
                    menu2.Margin = new Thickness(25, 0, 0, 30);
                    menu.WidthRequest = 40;
                    menu2.WidthRequest = 40;
                    //menu2.Margin = new Thickness(25, 0, 0, 30);
                    menu.WidthRequest = 40;
                    //heading adjustments

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

                    //password.CornerRadius = 22;
                    password2.CornerRadius = 22;
                    checkoutButton.CornerRadius = 24;

                    deliveryInstr.CornerRadius = 24;
                    back.Margin = new Thickness(25, 0, 0, 30);
                    back.HeightRequest = 25;
                    //mapFrame.Margin = new Thickness(width / 50, 0);
                    CheckAddressGrid.Margin = new Thickness(50, 170, 50, 120);
                    mapFrame.Margin = new Thickness(20, 0);
                    //SignUpButton.CornerRadius = 25;
                }

                fillEntriesDeliv();
                //saveContact.IsVisible = false;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }



        private async void clickedDeliv(object sender, EventArgs e)
        {
            try
            {
                checkoutAdd = AddressEntry.Text;
                checkoutUnit = AptEntry.Text;
                checkoutCity = CityEntry.Text;
                checkoutState = StateEntry.Text;
                checkoutZip = ZipEntry.Text;

                //Preferences.Set("price", Preferences.Get("subtotal", "00.00"));
                string platform = Xamarin.Forms.Application.Current.Properties["platform"].ToString();
                //string passwordSalt = Preferences.Get("password_salt", "");
                // Console.WriteLine("Clicked done: The Salt is: " + passwordSalt);
                //setPaymentInfo();
                //if (string.IsNullOrEmpty(passwordSalt)){  //If social login (salt is NULL)

                //-----------validate address start

                if (AddressEntry.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your address", "OK");
                    return;
                }

                if (CityEntry.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your city", "OK");
                    return;
                }

                if (StateEntry.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your state", "OK");
                    return;
                }

                if (ZipEntry.Text == null)
                {
                    await DisplayAlert("Error", "Please enter your zipcode", "OK");
                    return;
                }

                if (FNameEntry.Text == null || FNameEntry.Text == "")
                {
                    await DisplayAlert("Error", "first name required", "okay");
                    return;
                }

                if (LNameEntry.Text == null || LNameEntry.ToString() == "")
                {
                    await DisplayAlert("Error", "last name required", "okay");
                    return;
                }

                if (emailEntry.Text == null || emailEntry.Text == "")
                {
                    await DisplayAlert("Error", "email required", "okay");
                    return;
                }

                if (PhoneEntry.Text == null || PhoneEntry.Text == "")
                {
                    await DisplayAlert("Error", "phone number required", "okay");
                    return;
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
                        //  && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())
                        if (GetXMLElement(element, "DPVConfirmation").Equals("Y")) // Best case
                        {
                            // Get longitude and latitide because we can make a deliver here. Move on to next page.
                            // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                            //GetAddressLatitudeLongitude();
                            Geocoder geoCoder = new Geocoder();

                            Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                            Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                            Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                            Position position = approximateLocations.FirstOrDefault();

                            latitude = $"{position.Latitude}";
                            longitude = $"{position.Longitude}";

                            map.MapType = MapType.Street;
                            var mapSpan = new MapSpan(position, 0.001, 0.001);

                            Pin address = new Pin();
                            address.Label = "Delivery Address";
                            address.Type = PinType.SearchResult;
                            address.Position = position;

                            map.MoveToRegion(mapSpan);
                            map.Pins.Add(address);

                            //used for createaccount endpoint
                            Preferences.Set("user_latitude", latitude);
                            Preferences.Set("user_longitude", longitude);
                            Debug.WriteLine("user latitude: " + latitude);
                            Debug.WriteLine("user longitude: " + longitude);

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

                            var content = client4.DownloadString(url3);
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
                            await DisplayAlert("Alert!", "Please enter your unit number", "Ok");
                            return;
                        }
                        else if (GetXMLElement(element, "DPVConfirmation").Equals("S"))
                        {
                            await DisplayAlert("Alert!", "Invalid unit number", "Ok");
                            return;
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
                    CheckAddressHeading.Text = "Still Growing…";
                    CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your neighborhood yet. Enter your email address and we will let you know as soon as we come to your neighborhood.";
                    EmailFrame.IsVisible = true;
                    OkayButton.Text = "Okay";
                    loginButton2.IsVisible = false;
                    //await DisplayAlert("Invalid Address", "Address is not within any of our delivery zones.", "OK");
                    fade.IsVisible = true;
                    CheckAddressGrid.IsVisible = true;
                    return;
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
                    Button b = (Button)sender;
                    if (b.Text == "Save")
                        await DisplayAlert("We validated your address", "Please click on the proceed button to select a form of payment!", "OK");
                    await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                    //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());

                    saveInfoDeliv();
                    Debug.WriteLine("passed in tax, service fee, and delivery fee: " + tax.ToString() + ", " + serviceFee.ToString() + ", " + deliveryFee.ToString());
                    //await Navigation.PushAsync(new VerifyInfo(passingZones, tax, serviceFee, deliveryFee, cust_firstName, cust_lastName, cust_email, AptEntry.Text, FNameEntry.Text, LNameEntry.Text, emailEntry.Text, PhoneEntry.Text, AddressEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, DeliveryEntry.Text, "", "", "", salt));

                    //only run through the code below if proceed is clicked
                    Button receiving = (Button)sender;
                    if (receiving.Text != "Save")
                    {
                        CheckouWithStripe(receiving, e);

                        //clickedSaveContact(receiving, e);
                        if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
                        {
                            Debug.WriteLine("entered guest in clickedproceed");
                            directSignUp.email = emailEntry.Text;
                            directSignUp.first_name = FNameEntry.Text;
                            directSignUp.last_name = LNameEntry.Text;
                            directSignUp.phone_number = PhoneEntry.Text;
                            directSignUp.address = AddressEntry.Text;
                            directSignUp.unit = AptEntry.Text;
                            directSignUp.city = CityEntry.Text;
                            directSignUp.state = StateEntry.Text;
                            directSignUp.zip_code = ZipEntry.Text;
                            directSignUp.latitude = Preferences.Get("user_latitude", "");
                            directSignUp.longitude = Preferences.Get("user_longitude", "");
                            directSignUp.referral_source = "MOBILE";
                            //directSignUp.role = "CUSTOMER";
                            directSignUp.role = "GUEST";
                            directSignUp.mobile_access_token = "FALSE";
                            directSignUp.mobile_refresh_token = "FALSE";
                            directSignUp.user_access_token = "FALSE";
                            directSignUp.user_refresh_token = "FALSE";
                            directSignUp.social = "FALSE";
                            directSignUp.password = FNameEntry.Text + AddressEntry.Text.Substring(0, AddressEntry.Text.IndexOf(" "));
                            Debug.WriteLine("generated password for guest: " + directSignUp.password);
                            directSignUp.social_id = "NULL";

                            var directSignUpSerializedObject = JsonConvert.SerializeObject(directSignUp);
                            var content2 = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");

                            System.Diagnostics.Debug.WriteLine(directSignUpSerializedObject);

                            var signUpclient = new System.Net.Http.HttpClient();
                            var RDSResponse = await signUpclient.PostAsync(Constant.SignUpUrl, content2);
                            Debug.WriteLine("RDSResponse: " + RDSResponse.ToString());
                            var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                            Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

                            // if Sign up is has successfully ie 200 response code
                            if (RDSResponse.IsSuccessStatusCode)
                            {

                                try
                                {
                                    var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);

                                    Debug.WriteLine("RDSData: " + RDSData.ToString());

                                    // Local Variables in Xamarin that can be used throughout the App
                                    Xamarin.Forms.Application.Current.Properties["user_id"] = RDSData.result.customer_uid;

                                    Debug.WriteLine("starting saving new preference strings");
                                    savedFirstName = "firstName" + RDSData.result.customer_uid;
                                    savedLastName = "lastName" + RDSData.result.customer_uid;
                                    savedEmail = "email" + RDSData.result.customer_uid;
                                    savedAdd = "address" + RDSData.result.customer_uid;
                                    savedApt = "apt" + RDSData.result.customer_uid;
                                    savedCity = "city" + RDSData.result.customer_uid;
                                    savedState = "state" + RDSData.result.customer_uid;
                                    savedZip = "zip" + RDSData.result.customer_uid;
                                    savedPhone = "phone" + RDSData.result.customer_uid;
                                    savedInstr = "instructions" + RDSData.result.customer_uid;
                                    billingEmail = "billing_email" + RDSData.result.customer_uid;
                                    billingName = "billing_name" + RDSData.result.customer_uid;
                                    billingNum = "billing_num" + RDSData.result.customer_uid;
                                    billingMonth = "billing_month" + RDSData.result.customer_uid;
                                    billingYear = "billing_year" + RDSData.result.customer_uid;
                                    billingCVV = "billing_cvv" + RDSData.result.customer_uid;
                                    billingAddress = "billing_address" + RDSData.result.customer_uid;
                                    billingUnit = "billing_unit" + RDSData.result.customer_uid;
                                    billingCity = "billing_city" + RDSData.result.customer_uid;
                                    billingState = "billing_state" + RDSData.result.customer_uid;
                                    billingZip = "billing_zip" + RDSData.result.customer_uid;
                                    purchaseDescription = "purchase_descr" + RDSData.result.customer_uid;
                                    Debug.WriteLine("end saving new preference strings");


                                    EventArgs e2 = new EventArgs();
                                    saveInfoDeliv();
                                    clickedSaveContact(proceedButton, e2);
                                }
                                catch
                                {
                                    CheckAddressHeading.Text = "Hmm...";
                                    CheckAddressBody.Text = "Looks like the email address is already in use by another account. Please login to continue with that user account.";
                                    EmailFrame.IsVisible = false;
                                    OkayButton.Text = "Go Back";
                                    loginButton2.IsVisible = true;
                                    fade.IsVisible = true;
                                    CheckAddressGrid.IsVisible = true;
                                    //await DisplayAlert("Email Address Already In Use", "Please log in with the account that uses this email. If you previously used this email for guest checkout, your password is {first name}{house #}.", "OK");

                                    //Xamarin.Forms.Application.Current.MainPage = new MainPage();
                                    //Xamarin.Forms.Application.Current.MainPage = new MainLogin();
                                    return;
                                }
                                //Debug.WriteLine("RDSData: " + RDSData.ToString());

                                //if (RDSData.message.Contains("taken"))
                                //{
                                //    await DisplayAlert("Email Address Already In Use", "Please log in with the account that uses this email. If you previously used this email for guest checkout, your password is {first name}{house #}.", "OK");

                                //    Xamarin.Forms.Application.Current.MainPage = new MainPage();
                                //    return;
                                //}
                                //else
                                //{

                                //}
                            }

                            var client3 = new System.Net.Http.HttpClient();
                            //newPayment.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                            string url2 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                            var request3 = new HttpRequestMessage();
                            request3.RequestUri = new Uri(url2);
                            request3.Method = HttpMethod.Get;
                            var response3 = await client3.SendAsync(request3);
                            var content3 = response3.Content;
                            Console.WriteLine("content: " + content3);
                            var userString2 = await content3.ReadAsStringAsync();
                            JObject info_obj3 = JObject.Parse(userString2);
                            //Preferences.Set("password_salt", (info_obj3["result"])[0]["password_salt"].ToString());

                            //the salt in the json object for /checkout actually stores the hashed password
                            guestSalt = (info_obj3["result"])[0]["password_hashed"].ToString();
                        }

                        Preferences.Set("price", Preferences.Get("subtotal", "00.00"));
                        subtotalTitle.Text = "Meal Subscription \n" + Preferences.Get("item_name", "").Substring(0, 1) + " meals for " + Preferences.Get("freqSelected", "") + " deliveries";
                        meal_delivery_num.Text = Preferences.Get("item_name", "").Substring(0, 1) + " meals for " + Preferences.Get("freqSelected", "") + " deliveries";
                        deliveryTitle.Text = "Total Delivery Fee For All " + Preferences.Get("freqSelected", "") + " Deliveries: ";


                        //Preferences.Set("subtotal", Preferences.Get("price", "00.00"));
                        Debug.WriteLine("price before tax: " + Preferences.Get("price", "00.00"));
                        double payment = Double.Parse(Preferences.Get("price", "00.00")) + Math.Round((Double.Parse(Preferences.Get("price", "00.00")) * tax / 100), 2);
                        Debug.WriteLine("price before tax: " + (Double.Parse(Preferences.Get("price", "00.00")) * tax / 100).ToString());
                        double totalTax = Math.Round(Double.Parse(Preferences.Get("price", "00.00")) * tax / 100, 2);
                        taxPrice.Text = "$" + totalTax.ToString();
                        Debug.WriteLine("payment: " + payment.ToString());
                        payment += serviceFee;
                        Debug.WriteLine("payment + service fee: " + payment.ToString());
                        payment += deliveryFee;
                        Debug.WriteLine("payment + delivery fee: " + payment.ToString());
                        payment += Double.Parse(tipOpt2.Text.Substring(1));
                        Debug.WriteLine("payment + tip: " + payment.ToString());
                        Math.Round(payment, 2);
                        Debug.WriteLine("payment after tax and fees: " + payment.ToString());
                        Preferences.Set("price", payment.ToString());
                        discountPrice.Text = "- $" + Preferences.Get("discountAmt", "0");
                        //make sure price is formatted correctly
                        var total = Preferences.Get("price", "00.00");
                        if (total.Contains(".") == false)
                            total = total + ".00";
                        else if (total.Substring(total.IndexOf(".") + 1).Length == 1)
                            total = total + "0";
                        else if (total.Substring(total.IndexOf(".") + 1).Length == 0)
                            total = total + "00";
                        Preferences.Set("price", total);




                        subtotalPrice.Text = "$" + Preferences.Get("basePrice", "0.00").ToString();
                        //taxPrice.Text = "$" + tax.ToString();
                        serviceFeePrice.Text = "$" + serviceFee.ToString();
                        deliveryFeePrice.Text = "$" + deliveryFee.ToString();
                        tipPrice.Text = tipOpt2.Text;
                        //addOnsPrice.Text = "$0";
                        ambassDisc.Text = "- $0.00";
                        //discountPrice.Text = "$0";
                        //if (DeliveryEntry.Text == "M4METEST" || DeliveryEntry.Text == "M4ME TEST")
                        //{
                        //    clientId = Constant.LiveClientId;
                        //    secret = Constant.LiveSecret;
                        //}

                        if (subtotalPrice.Text.Contains(".") == false)
                            subtotalPrice.Text = subtotalPrice.Text + ".00";
                        else if (subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf(".") + 1).Length == 1)
                            subtotalPrice.Text = subtotalPrice.Text + "0";
                        else if (subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf(".") + 1).Length == 0)
                            subtotalPrice.Text = subtotalPrice.Text + "00";

                        if (discountPrice.Text.Contains(".") == false)
                            discountPrice.Text = discountPrice.Text + ".00";
                        else if (discountPrice.Text.Substring(discountPrice.Text.IndexOf(".") + 1).Length == 1)
                            discountPrice.Text = discountPrice.Text + "0";
                        else if (discountPrice.Text.Substring(discountPrice.Text.IndexOf(".") + 1).Length == 0)
                            discountPrice.Text = discountPrice.Text + "00";

                        if (taxPrice.Text.Contains(".") == false)
                            taxPrice.Text = taxPrice.Text + ".00";
                        else if (taxPrice.Text.Substring(taxPrice.Text.IndexOf(".") + 1).Length == 1)
                            taxPrice.Text = taxPrice.Text + "0";
                        else if (taxPrice.Text.Substring(taxPrice.Text.IndexOf(".") + 1).Length == 0)
                            taxPrice.Text = taxPrice.Text + "00";

                        if (serviceFeePrice.Text.Contains(".") == false)
                            serviceFeePrice.Text = serviceFeePrice.Text + ".00";
                        else if (serviceFeePrice.Text.Substring(serviceFeePrice.Text.IndexOf(".") + 1).Length == 1)
                            serviceFeePrice.Text = serviceFeePrice.Text + "0";
                        else if (serviceFeePrice.Text.Substring(serviceFeePrice.Text.IndexOf(".") + 1).Length == 0)
                            serviceFeePrice.Text = serviceFeePrice.Text + "00";

                        if (deliveryFeePrice.Text.Contains(".") == false)
                            deliveryFeePrice.Text = deliveryFeePrice.Text + ".00";
                        else if (deliveryFeePrice.Text.Substring(deliveryFeePrice.Text.IndexOf(".") + 1).Length == 1)
                            deliveryFeePrice.Text = deliveryFeePrice.Text + "0";
                        else if (deliveryFeePrice.Text.Substring(deliveryFeePrice.Text.IndexOf(".") + 1).Length == 0)
                            deliveryFeePrice.Text = deliveryFeePrice.Text + "00";

                        if (tipPrice.Text.Contains(".") == false)
                            tipPrice.Text = tipPrice.Text + ".00";
                        else if (tipPrice.Text.Substring(tipPrice.Text.IndexOf(".") + 1).Length == 1)
                            tipPrice.Text = tipPrice.Text + "0";
                        else if (tipPrice.Text.Substring(tipPrice.Text.IndexOf(".") + 1).Length == 0)
                            tipPrice.Text = tipPrice.Text + "00";

                        //if (ambassDisc.Text.Contains(".") == false)
                        //    ambassDisc.Text = ambassDisc.Text + ".00";
                        //else if (ambassDisc.Text.Substring(ambassDisc.Text.IndexOf(".") + 1).Length == 1)
                        //    ambassDisc.Text = ambassDisc.Text + "0";
                        //else if (ambassDisc.Text.Substring(ambassDisc.Text.IndexOf(".") + 1).Length == 0)
                        //    ambassDisc.Text = ambassDisc.Text + "00";

                        if (Double.Parse(total.ToString()) <= 0)
                            total = "0.00";

                        grandTotalPrice.Text = "$" + total.ToString();

                        if (grandTotalPrice.Text.Contains(".") == false)
                            grandTotalPrice.Text = grandTotalPrice.Text + ".00";
                        else if (grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf(".") + 1).Length == 1)
                            grandTotalPrice.Text = grandTotalPrice.Text + "0";
                        else if (grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf(".") + 1).Length == 0)
                            grandTotalPrice.Text = grandTotalPrice.Text + "00";

                        SetPayPalCredentials();
                        //grandTotalPrice.Text = "$" + total.ToString();
                        paymentStack.IsVisible = true;

                        await scroller.ScrollToAsync(0, mainStack.Height + 80, true);

                        Debug.WriteLine("clientId after setpaypalcredentials: " + clientId.ToString());
                        Debug.WriteLine("secret after setpaypalcredentials: " + secret.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        void saveInfoDeliv()
        {
            try
            {
                if (AddressEntry.Text != null)
                    Preferences.Set(savedAdd, AddressEntry.Text);
                Debug.WriteLine("address key used: " + savedAdd);

                if (AptEntry.Text != null)
                    Preferences.Set(savedApt, AptEntry.Text);

                if (CityEntry.Text != null)
                    Preferences.Set(savedCity, CityEntry.Text);

                if (StateEntry.Text != null)
                    Preferences.Set(savedState, StateEntry.Text);

                if (ZipEntry.Text != null)
                    Preferences.Set(savedZip, ZipEntry.Text);

                if (DeliveryEntry.Text != null)
                    Preferences.Set(savedInstr, DeliveryEntry.Text);
                Debug.WriteLine("delivery entry key used: " + savedAdd);

                //DisplayAlert("Success", "delivery info saved.", "OK");
                //saveDeliv.IsVisible = false;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
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

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        private async void DeliveryAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            paymentStack.IsVisible = false;
            await scroller.ScrollToAsync(0, -50, true);
            //saveDeliv.IsVisible = true;
        }

        private void ContactInfo_TextChanged(object sender, TextChangedEventArgs e)
        {
            //saveContact.IsVisible = true;
        }

        async Task clickedProceed(object sender, EventArgs e)
        {
            Preferences.Set("price", Preferences.Get("subtotal", ""));

            checkoutAdd = AddressEntry.Text;
            checkoutUnit = AptEntry.Text;
            checkoutCity = CityEntry.Text;
            checkoutState = StateEntry.Text;
            checkoutZip = ZipEntry.Text;

            clickedSaveContact(sender, e);
            clickedDeliv(sender, e);

            Debug.WriteLine("platform: " + (string)Xamarin.Forms.Application.Current.Properties["platform"]);

            if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
            {
                Debug.WriteLine("entered guest in clickedproceed");
                directSignUp.email = emailEntry.Text;
                directSignUp.first_name = FNameEntry.Text;
                directSignUp.last_name = LNameEntry.Text;
                directSignUp.phone_number = PhoneEntry.Text;
                directSignUp.address = AddressEntry.Text;
                directSignUp.unit = AptEntry.Text;
                directSignUp.city = CityEntry.Text;
                directSignUp.state = StateEntry.Text;
                directSignUp.zip_code = ZipEntry.Text;
                directSignUp.latitude = Preferences.Get("user_latitude", "");
                directSignUp.longitude = Preferences.Get("user_longitude", "");
                directSignUp.referral_source = "MOBILE";
                directSignUp.role = "CUSTOMER";
                directSignUp.mobile_access_token = "FALSE";
                directSignUp.mobile_refresh_token = "FALSE";
                directSignUp.user_access_token = "FALSE";
                directSignUp.user_refresh_token = "FALSE";
                directSignUp.social = "FALSE";
                directSignUp.password = FNameEntry.Text + AddressEntry.Text.Substring(0, AddressEntry.Text.IndexOf(" "));
                Debug.WriteLine("generated password for guest: " + directSignUp.password);
                directSignUp.social_id = "NULL";

                var directSignUpSerializedObject = JsonConvert.SerializeObject(directSignUp);
                var content2 = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(directSignUpSerializedObject);

                var signUpclient = new System.Net.Http.HttpClient();
                var RDSResponse = await signUpclient.PostAsync(Constant.SignUpUrl, content2);
                Debug.WriteLine("RDSResponse: " + RDSResponse.ToString());
                var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

                // if Sign up is has successfully ie 200 response code
                if (RDSResponse.IsSuccessStatusCode)
                {
                    var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);
                    Debug.WriteLine("RDSData: " + RDSData.ToString());

                    if (RDSData.message.Contains("taken"))
                    {
                        await DisplayAlert("Email Address Already In Use", "Please log in with the account that uses this email. If you previously used this email for guest checkout, your password is {first name}{house #}.", "OK");

                        Xamarin.Forms.Application.Current.MainPage = new MainPage();
                        return;
                    }
                    else
                    {
                        // Local Variables in Xamarin that can be used throughout the App
                        Xamarin.Forms.Application.Current.Properties["user_id"] = RDSData.result.customer_uid;

                        Debug.WriteLine("starting saving new preference strings");
                        savedFirstName = "firstName" + RDSData.result.customer_uid;
                        savedLastName = "lastName" + RDSData.result.customer_uid;
                        savedEmail = "email" + RDSData.result.customer_uid;
                        savedAdd = "address" + RDSData.result.customer_uid;
                        savedApt = "apt" + RDSData.result.customer_uid;
                        savedCity = "city" + RDSData.result.customer_uid;
                        savedState = "state" + RDSData.result.customer_uid;
                        savedZip = "zip" + RDSData.result.customer_uid;
                        savedPhone = "phone" + RDSData.result.customer_uid;
                        savedInstr = "instructions" + RDSData.result.customer_uid;
                        billingEmail = "billing_email" + RDSData.result.customer_uid;
                        billingName = "billing_name" + RDSData.result.customer_uid;
                        billingNum = "billing_num" + RDSData.result.customer_uid;
                        billingMonth = "billing_month" + RDSData.result.customer_uid;
                        billingYear = "billing_year" + RDSData.result.customer_uid;
                        billingCVV = "billing_cvv" + RDSData.result.customer_uid;
                        billingAddress = "billing_address" + RDSData.result.customer_uid;
                        billingUnit = "billing_unit" + RDSData.result.customer_uid;
                        billingCity = "billing_city" + RDSData.result.customer_uid;
                        billingState = "billing_state" + RDSData.result.customer_uid;
                        billingZip = "billing_zip" + RDSData.result.customer_uid;
                        purchaseDescription = "purchase_descr" + RDSData.result.customer_uid;
                        Debug.WriteLine("end saving new preference strings");


                        EventArgs e2 = new EventArgs();
                        saveInfoDeliv();
                        clickedSaveContact(proceedButton, e2);
                    }
                }

                var client3 = new System.Net.Http.HttpClient();
                //newPayment.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                var request3 = new HttpRequestMessage();
                request3.RequestUri = new Uri(url);
                request3.Method = HttpMethod.Get;
                var response3 = await client3.SendAsync(request3);
                var content3 = response3.Content;
                Console.WriteLine("content: " + content3);
                var userString2 = await content3.ReadAsStringAsync();
                JObject info_obj3 = JObject.Parse(userString2);
                //Preferences.Set("password_salt", (info_obj3["result"])[0]["password_salt"].ToString());

                //the salt in the json object for /checkout actually stores the hashed password
                guestSalt = (info_obj3["result"])[0]["password_hashed"].ToString();
            }
        }

        void clickedSaveContact(object sender, EventArgs e)
        {
            try
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

                if (PhoneEntry.Text == null || PhoneEntry.Text == "")
                {
                    DisplayAlert("Warning!", "phone number required", "okay");
                    return;
                }

                Preferences.Set(savedFirstName, FNameEntry.Text);
                Preferences.Set(savedLastName, LNameEntry.Text);
                Preferences.Set(savedEmail, emailEntry.Text);
                Preferences.Set(savedPhone, PhoneEntry.Text);

                Button b = (Button)sender;
                if (b.Text == "Save")
                    DisplayAlert("Success", "Contact info saved.", "OK");
                //saveContact.IsVisible = false;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            try
            {
                //save previously entered data when navigating

                bool prevProceed = true;
                bool prevAddFilled = true;
                bool anyPrev = false;

                if (FNameEntry.Text != "" && FNameEntry.Text != null)
                {
                    Preferences.Set("prevFName", FNameEntry.Text);
                    anyPrev = true;
                }
                else prevProceed = false;

                if (LNameEntry.Text != "" && LNameEntry.Text != null)
                {
                    Preferences.Set("prevLName", LNameEntry.Text);
                    anyPrev = true;
                }
                else prevProceed = false;

                if (emailEntry.Text != "" && emailEntry.Text != null)
                {
                    Preferences.Set("prevEmail", emailEntry.Text);
                    anyPrev = true;
                }
                else prevProceed = false;

                if (PhoneEntry.Text != "" && PhoneEntry.Text != null)
                {
                    Preferences.Set("prevPhone", PhoneEntry.Text);
                    anyPrev = true;
                }
                else prevProceed = false;

                if (AddressEntry.Text != "" && AddressEntry.Text != null)
                {
                    Preferences.Set("prevAdd", AddressEntry.Text);
                    anyPrev = true;
                }
                else
                {
                    prevProceed = false;
                    prevAddFilled = false;
                }

                if (AptEntry.Text != "" && AptEntry.Text != null)
                {
                    Preferences.Set("prevApt", AptEntry.Text);
                    anyPrev = true;
                }

                if (CityEntry.Text != "" && CityEntry.Text != null)
                {
                    Preferences.Set("prevCity", CityEntry.Text);
                    anyPrev = true;
                }
                else
                {
                    prevProceed = false;
                    prevAddFilled = false;
                }

                if (StateEntry.Text != "" && StateEntry.Text != null)
                {
                    Preferences.Set("prevState", StateEntry.Text);
                    anyPrev = true;
                }
                else
                {
                    prevProceed = false;
                    prevAddFilled = false;
                }

                if (ZipEntry.Text != "" && ZipEntry.Text != null)
                {
                    Preferences.Set("prevZip", ZipEntry.Text);
                    anyPrev = true;
                }
                else
                {
                    prevProceed = false;
                    prevAddFilled = false;
                }

                if (DeliveryEntry.Text != "" && DeliveryEntry.Text != null)
                {
                    Preferences.Set("prevInstr", DeliveryEntry.Text);
                    anyPrev = true;
                }

                if (cardHolderName.Text != "" && cardHolderName.Text != null)
                    Preferences.Set("prevCardName", cardHolderName.Text);

                if (cardHolderNumber.Text != "" && cardHolderNumber.Text != null)
                    Preferences.Set("prevCardNum", cardHolderNumber.Text);

                if (cardExpMonth.Text != "" && cardExpMonth.Text != null)
                    Preferences.Set("prevExpMonth", cardExpMonth.Text);

                if (cardExpYear.Text != "" && cardExpYear.Text != null)
                    Preferences.Set("prevExpYear", cardExpYear.Text);

                if (cardCVV.Text != "" && cardCVV.Text != null)
                    Preferences.Set("prevCVV", cardCVV.Text);

                if (cardHolderAddress.Text != "" && cardHolderAddress.Text != null)
                    Preferences.Set("prevCardAdd", cardHolderAddress.Text);

                if (cardHolderUnit.Text != "" && cardHolderUnit.Text != null)
                    Preferences.Set("prevCardUnit", cardHolderUnit.Text);

                if (cardCity.Text != "" && cardCity.Text != null)
                    Preferences.Set("prevCardCity", cardCity.Text);

                if (cardState.Text != "" && cardState.Text != null)
                    Preferences.Set("prevCardState", cardState.Text);

                if (cardZip.Text != "" && cardZip.Text != null)
                    Preferences.Set("prevCardZip", cardZip.Text);

                Preferences.Set("prevProceed", prevProceed);
                Preferences.Set("prevAddFilled", prevAddFilled);
                Debug.WriteLine("anyPrev value: " + anyPrev.ToString());
                Preferences.Set("anyPrev", anyPrev);

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }


        private void clickedTip(object sender, EventArgs e)
        {
            try
            {
                Button button1 = (Button)sender;

                double tipValue = Double.Parse(tipPrice.Text.Substring(1));
                Debug.WriteLine("tipValue1: " + tipValue.ToString());
                double grandTotalValue = Double.Parse(grandTotalPrice.Text.Substring(1));

                if (button1.Text == tipOpt1.Text)
                {
                    grandTotalValue -= tipValue;

                    tipOpt1.BackgroundColor = Color.FromHex("#F26522");
                    tipOpt2.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt3.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt4.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt1.TextColor = Color.FromHex("#ffffff");
                    tipOpt2.TextColor = Color.FromHex("#000000");
                    tipOpt3.TextColor = Color.FromHex("#000000");
                    tipOpt4.TextColor = Color.FromHex("#000000");
                    tipPrice.Text = "$0";
                }
                else if (button1.Text == tipOpt2.Text)
                {
                    grandTotalValue -= tipValue;

                    tipOpt1.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt2.BackgroundColor = Color.FromHex("#F26522");
                    tipOpt3.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt4.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt1.TextColor = Color.FromHex("#000000");
                    tipOpt2.TextColor = Color.FromHex("#ffffff");
                    tipOpt3.TextColor = Color.FromHex("#000000");
                    tipOpt4.TextColor = Color.FromHex("#000000");
                    tipPrice.Text = tipOpt2.Text;
                }
                else if (button1.Text == tipOpt3.Text)
                {
                    grandTotalValue -= tipValue;

                    tipOpt1.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt2.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt3.BackgroundColor = Color.FromHex("#F26522");
                    tipOpt4.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt1.TextColor = Color.FromHex("#000000");
                    tipOpt2.TextColor = Color.FromHex("#000000");
                    tipOpt3.TextColor = Color.FromHex("#ffffff");
                    tipOpt4.TextColor = Color.FromHex("#000000");
                    tipPrice.Text = tipOpt3.Text;
                }
                else
                {
                    grandTotalValue -= tipValue;

                    tipOpt1.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt2.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt3.BackgroundColor = Color.FromHex("#ffffff");
                    tipOpt4.BackgroundColor = Color.FromHex("#F26522");
                    tipOpt1.TextColor = Color.FromHex("#000000");
                    tipOpt2.TextColor = Color.FromHex("#000000");
                    tipOpt3.TextColor = Color.FromHex("#000000");
                    tipOpt4.TextColor = Color.FromHex("#ffffff");
                    tipPrice.Text = tipOpt4.Text;
                }


                tipValue = Double.Parse(tipPrice.Text.Substring(1));
                Debug.WriteLine("tipValue2: " + tipValue.ToString());
                //grandTotalValue = Double.Parse(grandTotalPrice.Text.Substring(1));
                grandTotalValue += tipValue;
                string grandTotalString = grandTotalValue.ToString();

                if (tipPrice.Text.Contains(".") == false)
                    tipPrice.Text = tipPrice.Text + ".00";
                else if (tipPrice.Text.Substring(tipPrice.Text.IndexOf(".") + 1).Length == 1)
                    tipPrice.Text = tipPrice.Text + "0";
                else if (tipPrice.Text.Substring(tipPrice.Text.IndexOf(".") + 1).Length == 0)
                    tipPrice.Text = tipPrice.Text + "00";

                if (grandTotalString.Contains(".") == false)
                    grandTotalString = grandTotalString + ".00";
                else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 1)
                    grandTotalString = grandTotalString + "0";
                else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 0)
                    grandTotalString = grandTotalString + "00";
                Preferences.Set("price", grandTotalString);

                grandTotalPrice.Text = "$" + grandTotalString;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void clickedVerifyCode(object sender, EventArgs e)
        {
            try
            {
                AmbassCodePost AmbCode = new AmbassCodePost();
                AmbCode.code = ambassTitle.Text.Trim();
                AmbCode.info = emailEntry.Text.Trim();
                if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
                    AmbCode.IsGuest = "True";
                else AmbCode.IsGuest = "False";
                var AmbSerializedObj = JsonConvert.SerializeObject(AmbCode);
                var content4 = new StringContent(AmbSerializedObj, Encoding.UTF8, "application/json");
                var client3 = new System.Net.Http.HttpClient();
                var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/brandAmbassador/discount_checker", content4);
                var message = await response3.Content.ReadAsStringAsync();
                Debug.WriteLine("RESPONSE TO verifyCode   " + response3.ToString());
                Debug.WriteLine("json object sent:  " + AmbSerializedObj.ToString());
                Debug.WriteLine("message received:  " + message.ToString());

                if (message.Contains("Let the customer use the referral") == true)
                {
                    var data = JsonConvert.DeserializeObject<AmbassadorCouponDto>(message);
                    //Application.Current.Properties["user_id"] = data.result[0].valid;


                    Debug.WriteLine("RESPONSE TO verifyCode   " + response3.ToString());
                    Debug.WriteLine("valid: " + data.sub.valid);

                    //AmbassadorCoupon am = response3.Result;

                    string isValid = data.sub.valid;

                    //string isValid = "yes";

                    if (isValid == "TRUE")
                    {
                        verifyCode.IsVisible = false;
                        double totalDiscount = 0;
                        double grandTotalValue = Double.Parse(grandTotalPrice.Text.Substring(1));

                        //discountPrice.Text = "- $5";

                        //add back the previous discount before calculating for the new discount
                        if (ambassDisc.Text != null && ambassDisc.Text != "")
                        {
                            double codeValue = Double.Parse(ambassDisc.Text.Substring(ambassDisc.Text.IndexOf('$') + 1));
                            grandTotalValue = Double.Parse(grandTotalPrice.Text.Substring(1));
                            grandTotalValue += codeValue;
                        }

                        //totalDiscount += Math.Round(grandTotalValue * data.sub[0].discount_percent, 2);
                        //totalDiscount += data.sub[0].discount_amount;
                        //totalDiscount += data.sub[0].discount_shipping;
                        totalDiscount += data.discount;

                        ambassDisc.Text = "- $" + totalDiscount.ToString();
                        grandTotalValue -= totalDiscount;

                        if (grandTotalValue <= 0)
                            grandTotalValue = 0.00;

                        string grandTotalString = grandTotalValue.ToString();

                        if (ambassDisc.Text.Contains(".") == false)
                            ambassDisc.Text = ambassDisc.Text + ".00";
                        else if (ambassDisc.Text.Substring(ambassDisc.Text.IndexOf(".") + 1).Length == 1)
                            ambassDisc.Text = ambassDisc.Text + "0";
                        else if (ambassDisc.Text.Substring(ambassDisc.Text.IndexOf(".") + 1).Length == 0)
                            ambassDisc.Text = ambassDisc.Text + "00";



                        if (grandTotalString.Contains(".") == false)
                            grandTotalString = grandTotalString + ".00";
                        else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 1)
                            grandTotalString = grandTotalString + "0";
                        else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 0)
                            grandTotalString = grandTotalString + "00";
                        Preferences.Set("price", grandTotalString);

                        grandTotalPrice.Text = "$" + grandTotalString;
                    }
                }
                else
                {
                    if (ambassDisc.Text != null && ambassDisc.Text != "")
                    {
                        double codeValue = Double.Parse(ambassDisc.Text.Substring(ambassDisc.Text.IndexOf('$') + 1));
                        double grandTotalValue = Double.Parse(grandTotalPrice.Text.Substring(1));
                        grandTotalValue += codeValue;

                        string grandTotalString = grandTotalValue.ToString();


                        if (grandTotalString.Contains(".") == false)
                            grandTotalString = grandTotalString + ".00";
                        else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 1)
                            grandTotalString = grandTotalString + "0";
                        else if (grandTotalString.Substring(grandTotalString.IndexOf(".") + 1).Length == 0)
                            grandTotalString = grandTotalString + "00";
                        Preferences.Set("price", grandTotalString);

                        grandTotalPrice.Text = "$" + grandTotalString;

                        DisplayAlert("Error", "invalid ambassador code", "OK");
                        ambassDisc.Text = "$0";
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        //VERIFY INFO FUNCTIONS


        public void fillEntries()
        {
            try
            {
                if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "GUEST")
                {
                    cardHolderEmail.Text = emailEntry.Text;
                    cardHolderName.Text = FNameEntry.Text + " " + LNameEntry.Text;
                    cardHolderAddress.Text = AddressEntry.Text;
                    if (AptEntry.Text != "" && AptEntry.Text != null)
                        cardHolderUnit.Text = AptEntry.Text;
                    cardCity.Text = CityEntry.Text;
                    cardState.Text = StateEntry.Text;
                    cardZip.Text = ZipEntry.Text;
                }
                else
                {
                    //Debug.WriteLine("billing email: " + Preferences.Get(billingAddress, ""));
                    if (Preferences.Get(billingEmail, "") != "")
                        cardHolderEmail.Text = Preferences.Get(billingEmail, "");
                    else cardHolderEmail.Text = emailEntry.Text;

                    if (Preferences.Get(billingName, "") != "")
                        cardHolderName.Text = Preferences.Get(billingName, "");
                    else cardHolderName.Text = FNameEntry.Text + " " + LNameEntry.Text;

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
                    else cardHolderAddress.Text = AddressEntry.Text;

                    if (Preferences.Get(billingUnit, "") != "")
                        cardHolderUnit.Text = Preferences.Get(billingUnit, "");
                    else cardHolderUnit.Text = AptEntry.Text;

                    if (Preferences.Get(billingCity, "") != "")
                        cardCity.Text = Preferences.Get(billingCity, "");
                    else cardCity.Text = CityEntry.Text;

                    if (Preferences.Get(billingState, "") != "")
                        cardState.Text = Preferences.Get(billingState, "");
                    else cardState.Text = StateEntry.Text;

                    if (Preferences.Get(billingZip, "") != "")
                        cardZip.Text = Preferences.Get(billingZip, "");
                    else cardZip.Text = ZipEntry.Text;

                    //if (Preferences.Get(purchaseDescription, "") != "")
                    //    cardDescription.Text = Preferences.Get(purchaseDescription, "");
                }
                addressListFrame.IsVisible = false;
                addressList2.IsVisible = false;
                UnitGrid.IsVisible = true;
                CityStateZip.IsVisible = true;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        protected async Task setPaymentInfo()
        {
            try
            {
                Console.WriteLine("SetPaymentInfo Func Started!");
                PaymentInfo newPayment = new PaymentInfo();
                //need to add item_business_id
                Model.Item item1 = new Model.Item();
                item1.name = Preferences.Get("item_name", "");
                item1.price = Preferences.Get("itemPrice", "00.00");
                item1.qty = Preferences.Get("freqSelected", "");
                item1.item_uid = Preferences.Get("item_uid", "");
                item1.itm_business_uid = "200-000002";
                List<Model.Item> itemsList = new List<Model.Item> { item1 };
                Preferences.Set("unitNum", AptEntry.Text);

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

                string userID = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                Console.WriteLine("YOUR userID is " + userID);
                newPayment.customer_uid = userID;
                //newPayment.customer_uid = "100-000082";
                if (Device.RuntimePlatform == Device.Android)
                    newPayment.business_uid = "MOBILE ANDROID";
                else if (Device.RuntimePlatform == Device.iOS)
                    newPayment.business_uid = "MOBILE IOS";
                else newPayment.business_uid = "MOBILE";

                newPayment.items = itemsList;
                //newPayment.salt = "64a7f1fb0df93d8f5b9df14077948afa1b75b4c5028d58326fb801d825c9cd24412f88c8b121c50ad5c62073c75d69f14557255da1a21e24b9183bc584efef71";
                //newPayment.salt = "cec35d4fc0c5e83527f462aeff579b0c6f098e45b01c8b82e311f87dc6361d752c30293e27027653adbb251dff5d03242c8bec68a3af1abd4e91c5adb799a01b";
                //newPayment.salt = "2020-09-22 21:55:17";
                //newPayment.salt = hashedPassword;
                //testing for paypal
                //if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                //    newPayment.salt = hashedPassword;
                //else newPayment.salt = "";
                newPayment.salt = "";

                newPayment.delivery_first_name = FNameEntry.Text;
                newPayment.delivery_last_name = LNameEntry.Text;
                newPayment.delivery_email = emailEntry.Text;
                newPayment.delivery_phone = PhoneEntry.Text;
                //newPayment.delivery_address = AddressEntry.Text;
                //newPayment.delivery_unit = Preferences.Get("unitNum", "");
                //newPayment.delivery_city = CityEntry.Text;
                //newPayment.delivery_state = StateEntry.Text;
                //newPayment.delivery_zip = ZipEntry.Text;
                newPayment.delivery_address = checkoutAdd;
                newPayment.delivery_unit = checkoutUnit;
                newPayment.delivery_city = checkoutCity;
                newPayment.delivery_state = checkoutState;
                newPayment.delivery_zip = checkoutZip;
                //newPayment.delivery_instructions = DeliveryEntry;
                newPayment.delivery_instructions = DeliveryEntry.Text;
                newPayment.delivery_longitude = "";
                newPayment.delivery_latitude = "";
                newPayment.order_instructions = "slow";

                newPayment.amount_due = Preferences.Get("price", "00.00");
                newPayment.amount_discount = Preferences.Get("discountAmt", "0.00");
                newPayment.amount_paid = grandTotalPrice.Text.Substring(1);//Preferences.Get("price", "00.00");
                newPayment.tax = taxPrice.Text.Substring(taxPrice.Text.IndexOf("$") + 1);
                newPayment.tip = tipPrice.Text.Substring(tipPrice.Text.IndexOf("$") + 1);
                newPayment.amb = ambassDisc.Text.Substring(ambassDisc.Text.IndexOf("$") + 1);
                newPayment.service_fee = serviceFeePrice.Text.Substring(serviceFeePrice.Text.IndexOf("$") + 1);
                newPayment.delivery_fee = deliveryFeePrice.Text.Substring(deliveryFeePrice.Text.IndexOf("$") + 1);
                newPayment.subtotal = subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf("$") + 1);
                //new items in json object
                newPayment.payment_type = paymentMethod;
                newPayment.charge_id = chargeId;


                if (paymentMethod == "STRIPE")
                {
                    //newPayment.purchase_notes = cardDescription.Text;
                    //newPayment.purchase_notes = 
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

                newPayment.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];


                if (guestSalt != "")
                    newPayment.salt = guestSalt;
                //newPayment.salt = (info_obj3["result"])[0]["password_hashed"].ToString();

                //var StripeIntentJSONString = JsonConvert.SerializeObject(newPayment);
                //Console.WriteLine("StripeIntentJSONString" + StripeIntentJSONString);
                //var content2 = new StringContent(StripeIntentJSONString, Encoding.UTF8, "application/json");
                //Console.WriteLine("Content: " + content2);
                //var client2 = new System.Net.Http.HttpClient();
                //var response2 = await client2.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content2);


                //var StripeIntentJSONString = JsonConvert.SerializeObject(newPayment);
                //Console.WriteLine("StripeIntentJSONString" + StripeIntentJSONString);
                //var content2 = new StringContent(StripeIntentJSONString, Encoding.UTF8, "application/json");
                //Console.WriteLine("Content: " + content2);
                //var client2 = new System.Net.Http.HttpClient();
                //var response2 = await client2.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout", content2);


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
                var message2 = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<checkoutDto>(message2);
                string newPurchId = data.purchase_id;
                new_purchase_id = newPurchId;
                //Preferences.Set("new_purch_id", newPurchId);

                //add_surprise
                //filler fill = new filler();
                //var fillerJSONString = JsonConvert.SerializeObject(fill);
                //var content5 = new StringContent(fillerJSONString, Encoding.UTF8, "application/json");
                //var client5 = new System.Net.Http.HttpClient();
                //var response5 = await client5.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/add_surprise/" + newPurchId, content5);
                //var message5 = await response5.Content.ReadAsStringAsync();

                //Debug.WriteLine("response from add_surprise: " + response5.ToString());
                //Debug.WriteLine("json object being sent: " + content5.ToString());

                Debug.WriteLine("response from checkout: " + response.ToString());
                // HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Preferences.Set("anyPrev", false);
                    Preferences.Set("canChooseSelect", true);
                    Debug.WriteLine("RESPONSE TO CHECKOUT           : " + response.IsSuccessStatusCode);
                    Debug.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                    Debug.WriteLine("SetPaymentInfo Func ENDED!");
                    paymentSucceed = true;
                }
                else
                {
                    if (paymentMethod == "STRIPE")
                    {
                        headingGrid.IsVisible = true;
                        checkoutButton.IsVisible = true;
                        backButton.IsVisible = true;
                        mainStack.IsVisible = true;
                        paymentStack.IsVisible = true;
                        await scroller.ScrollToAsync(0, mainStack.Height + 80, true);
                        PaymentScreen.HeightRequest = 0;
                        PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                        StripeScreen.Height = 0;
                        PayPalScreen.Height = 0;
                        orangeBox.HeightRequest = deviceHeight / 2;
                        await Navigation.PopAsync(false);
                    }
                    await DisplayAlert("Ooops", "Our system is down. We were able to process your request. We are currently working to solve this issue", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }


        // CHECKOUT FUNCTION MOVES TO SELECT PAGE IN SUCCESSFUL PAYMENTS ONLY
        private async void checkoutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (checkoutButton.Text == "CONTINUE")
                {
                    if (paymentMethod == "PAYPAL" && (string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
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
                    //Preferences.Set(purchaseDescription, cardDescription.Text);

                    await setPaymentInfo();
                    Preferences.Set("canChooseSelect", true);
                    //await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                    if (paymentSucceed)
                    {
                        await Navigation.PushAsync(new CongratsPage(passingZones, cust_firstName, cust_lastName, cust_email, new_purchase_id));
                    }
                }
                else
                {
                    await DisplayAlert("Oops", "Our records show that you still have to process your payment before moving on the meals selection. Please complete your payment via PayPal or Stripe.", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // STRIPE FUNCTIONS

        // FUNCTION  1:
        public async void CheckouWithStripe(System.Object sender, System.EventArgs e)
        {
            try
            {
                onStripeScreen = true;


                paymentMethod = "STRIPE";
                fillEntries();

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

                    //headingGrid.IsVisible = false;
                    //mainStack.IsVisible = false;
                    //paymentStack.IsVisible = false;
                    //checkoutButton.IsVisible = false;
                    //backButton.IsVisible = false;
                    //PaymentScreen.HeightRequest = deviceHeight;
                    //PayPalScreen.Height = deviceHeight - (deviceHeight / 8);

                    //PayPalScreen.Height = ;
                    //StripeScreen.Height = 0;
                    //orangeBox.HeightRequest = 0;
                    if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                    {
                        //PaymentScreen.HeightRequest = this.Height * 1.5;
                        //PayPalScreen.Height = (this.Height - (this.Height / 8)) * 1.5;
                        //spacer6.IsVisible = true;
                        //passLabel.IsVisible = true;
                        ////spacer7.IsVisible = true;
                        //password.IsVisible = true;
                        //passwordEntry.IsVisible = true;
                        //password.WidthRequest = cardAddFrame.Width;
                        ////passwordEntry.WidthRequest = purchDescFrame.Width;
                        ////spacer8.IsVisible = true;
                        //spacer9.IsVisible = true;
                    }
                    //await scroller.ScrollToAsync(0, -40, false);
                }
                else
                {
                    await DisplayAlert("Ooops", "The amount to pay is zero. It must be greater than zero to process a payment", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // FUNCTION  2:
        public async void PayViaStripe(System.Object sender, System.EventArgs e)
        {
            try
            {
                Debug.WriteLine("PayViaStripe entered");
                try
                {
                    if (termsChecked == false)
                    {
                        await DisplayAlert("Error", "Check the terms & conditions before continuing", "OK");
                        return;
                    }
                    //await Navigation.PushAsync(new Loading());
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

                    //if (passwordEntry.Text == null && (string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                    //{
                    //    await DisplayAlert("Error", "Please enter your password", "OK");
                    //}

                    await Navigation.PushAsync(new Loading());

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
                            // && GetXMLElement(element, "Zip5").Equals(cardZip.Text.Trim()) && GetXMLElement(element, "City").Equals(cardCity.Text.ToUpper().Trim())
                            if (GetXMLElement(element, "DPVConfirmation").Equals("Y")) // Best case
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
                                await Navigation.PopAsync(false);
                                await DisplayAlert("Alert!", "Please enter your unit number", "Ok");
                                return;
                            }
                            else if (GetXMLElement(element, "DPVConfirmation").Equals("S"))
                            {
                                await Navigation.PopAsync(false);
                                await DisplayAlert("Alert!", "Invalid unit number", "Ok");
                                return;
                            }
                            else
                            {
                                await Navigation.PopAsync(false);
                                await DisplayAlert("Alert!", "Seems like your address is invalid.", "Ok");
                                return;
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
                        await Navigation.PopAsync(false);
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
                        await Xamarin.Forms.Application.Current.SavePropertiesAsync();
                        //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());
                    }
                    //-------------validate address end

                    //if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                    //{
                    //    Console.WriteLine("In set payment info: Hashing Password!");
                    //    SHA512 sHA512 = new SHA512Managed();
                    //    byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text + Preferences.Get("password_salt", "")));
                    //    string hashedPassword2 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    //    Debug.WriteLine("hashedPassword: " + hashedPassword2);
                    //    //byte[] data2 = sHA512.ComputeHash(Encoding.UTF8.GetBytes(passwordEntry.Text));
                    //    //string hashedPassword3 = BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
                    //    //Debug.WriteLine("hashedPassword solo: " + hashedPassword2);
                    //    //Debug.WriteLine("password_hashed: " + Preferences.Get("password_hashed", ""));
                    //    Console.WriteLine("In set payment info:  Password Hashed!");
                    //    if (Preferences.Get("password_hashed", "") != hashedPassword2)
                    //    {
                    //        await Navigation.PopAsync(false);
                    //        Debug.WriteLine("wrong password entered");
                    //        DisplayAlert("Error", "Wrong password entered.", "OK");
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        Debug.WriteLine("hash finished and ready");
                    //        hashedPassword = hashedPassword2;
                    //    }
                    //}

                    //stripe frontend processing start

                    var total = Preferences.Get("price", "00.00");
                    var clientHttp = new System.Net.Http.HttpClient();
                    var stripe = new Credentials();
                    if (DeliveryEntry.Text == "M4METEST" || DeliveryEntry.Text == "M4ME TEST")
                        stripe.key = Constant.TestPK;
                    else stripe.key = Constant.LivePK;

                    var stripeObj = JsonConvert.SerializeObject(stripe);
                    var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
                    var RDSResponse = await clientHttp.PostAsync(Constant.StripeModeUrl, stripeContent);
                    var content = await RDSResponse.Content.ReadAsStringAsync();

                    Debug.WriteLine("key to send JSON: " + stripeObj);
                    Debug.WriteLine("Response from key: " + content);
                    Debug.WriteLine("RDSResponse" + RDSResponse.IsSuccessStatusCode.ToString());
                    if (RDSResponse.IsSuccessStatusCode)
                    {
                        //Carlos original code
                        //if (content != "200")
                        //if (content.Contains("200"))
                        //{
                        //Debug.WriteLine("error encountered");
                        string SK = "";
                        string mode = "";

                        if (stripeObj.Contains("test"))
                        {
                            mode = "TEST";
                            SK = Constant.TestSK;
                        }
                        else if (stripeObj.Contains("live"))
                        {
                            mode = "LIVE";
                            SK = Constant.LiveSK;
                        }
                        //Carlos original code
                        //if (content.Contains("Test"))
                        //{
                        //    mode = "TEST";
                        //    SK = Constant.TestSK;
                        //}
                        //else if (content.Contains("Live"))
                        //{
                        //    mode = "LIVE";
                        //    SK = Constant.LiveSK;
                        //}

                        //stripe payment intent url: https://huo8rhh76i.execute-api.us-west-1.amazonaws.com/dev/api/v2/createPaymentIntent
                        //the endpoint returns the payment intent, doesn't make any charges
                        //assemble a STPPaymentIntentParams object, put payment details and the PaymentIntent client secret
                        //complete the payment by calling the STPPaymentHandler confirmPayment function
                        /*

                         {   
                            "currency": "usd",   
                            "customer_uid": "100-000142",
                            "business_code": "M4METEST",
                            "item_uid": "320-000054",
                            "num_items": 5,
                            "num_deliveries": 9,
                            "delivery_discount": 13,
                            "payment_summary": {     
                                "mealSubPrice": "45.00",     
                                "discountAmount": "5.85",    
                                "addOns": "0.00",     
                                "tip": "2.00",     
                                "serviceFee": "0.00",     
                                "deliveryFee": "0.00",     
                                "taxRate": 0,     
                                "taxAmount": "0.00",
                                "ambassadorDiscount": "0.00",     
                                "total": "41.15",     
                                "subtotal": "41.15"   
                            } 
                        }
                         */

                        //trying to implement stripe with payment intent and payment method
                        GetPaymentIntent newPaymentIntent = new GetPaymentIntent();
                        newPaymentIntent.currency = "usd";
                        newPaymentIntent.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                        newPaymentIntent.business_code = DeliveryEntry.Text.Trim();
                        newPaymentIntent.item_uid = Preferences.Get("item_uid", "");
                        newPaymentIntent.num_items = Int32.Parse(Preferences.Get("item_name", "").Substring(0, Preferences.Get("item_name", "").IndexOf(" ")));
                        newPaymentIntent.num_deliveries = Int32.Parse(Preferences.Get("freqSelected", ""));
                        newPaymentIntent.delivery_discount = Math.Round(Double.Parse(discountPrice.Text.Substring(discountPrice.Text.IndexOf("$") + 1)) / Double.Parse(subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf("$") + 1)), 0);
                        PaymentSummary paymentSum = new PaymentSummary();
                        paymentSum.mealSubPrice = subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf("$") + 1);
                        paymentSum.discountAmount = discountPrice.Text.Substring(discountPrice.Text.IndexOf("$") + 1);
                        paymentSum.addOns = "0.00";
                        paymentSum.tip = tipPrice.Text.Substring(tipPrice.Text.IndexOf("$") + 1);
                        paymentSum.serviceFee = serviceFeePrice.Text.Substring(serviceFeePrice.Text.IndexOf("$") + 1);
                        paymentSum.deliveryFee = deliveryFeePrice.Text.Substring(deliveryFeePrice.Text.IndexOf("$") + 1);
                        paymentSum.taxRate = tax;
                        paymentSum.taxAmount = taxPrice.Text.Substring(taxPrice.Text.IndexOf("$") + 1);
                        paymentSum.ambassadorDiscount = ambassDisc.Text.Substring(ambassDisc.Text.IndexOf("$") + 1);
                        paymentSum.total = grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf("$") + 1);
                        paymentSum.subtotal = grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf("$") + 1);
                        newPaymentIntent.payment_summary = paymentSum;


                        var paymentIntentJSONString = JsonConvert.SerializeObject(newPaymentIntent);
                        Console.WriteLine("paymentIntentJSONString" + paymentIntentJSONString);
                        var content3 = new StringContent(paymentIntentJSONString, Encoding.UTF8, "application/json");
                        Console.WriteLine("Content: " + content3);
                        var client3 = new System.Net.Http.HttpClient();
                        var response3 = await client3.PostAsync("https://huo8rhh76i.execute-api.us-west-1.amazonaws.com/dev/api/v2/createPaymentIntent", content3);
                        var message3 = await response3.Content.ReadAsStringAsync();
                        Debug.WriteLine("create payment intent response: " + message3);
                        string payIntent = message3.Substring(1, message3.IndexOf("secret") - 2);
                        Debug.WriteLine("only the payment intent: " + message3.Substring(1, message3.IndexOf("secret") - 2));
                        string secret = message3.Substring(message3.IndexOf("secret") + 7);
                        secret = secret.Substring(0, secret.IndexOf("\""));
                        Debug.WriteLine("only the secret: " + secret);
                        string clientSec = message3.Substring(1);
                        clientSec = clientSec.Substring(0, clientSec.IndexOf("\""));
                        Debug.WriteLine("client secret: " + clientSec);

                        PaymentMethodCard payWithCard = new PaymentMethodCard();

                        Debug.WriteLine("MODE          : " + mode);
                        Debug.WriteLine("STRIPE SECRET : " + SK);

                        //Debug.WriteLine("SK" + SK);
                        StripeConfiguration.ApiKey = SK;

                        Dictionary<String, Object> card = new Dictionary<string, object>();
                        card.Add("number", "4242424242424242");
                        card.Add("exp_month", 4);
                        card.Add("exp_year", 2022);
                        card.Add("cvc", "314");
                        Dictionary<String, Object> param = new Dictionary<string, object>();
                        param.Add("type", "card");
                        param.Add("card", card);

                        //Stripe.PaymentMethodCard
                        Stripe.PaymentMethod paymentMethod = new Stripe.PaymentMethod();
                        Stripe.PaymentMethodCard paywith = new Stripe.PaymentMethodCard();
                        //var req = await stripe.createPaymentMethod();
                        StripeClient stripeClient = new StripeClient();


                        string CardNo = cardHolderNumber.Text.Trim();
                        string expMonth = cardExpMonth.Text.Trim();
                        string expYear = cardExpYear.Text.Trim();
                        string cardCvv = cardCVV.Text.Trim();

                        Debug.WriteLine("step 1 reached");
                        // Step 1: Create Card
                        TokenCardOptions stripeOption = new TokenCardOptions();
                        stripeOption.Number = CardNo;
                        stripeOption.ExpMonth = Convert.ToInt64(expMonth);
                        stripeOption.ExpYear = Convert.ToInt64(expYear);
                        stripeOption.Cvc = cardCvv;


                        Debug.WriteLine("step 2 reached");
                        // Step 2: Assign card to token object
                        TokenCreateOptions stripeCard = new TokenCreateOptions();
                        stripeCard.Card = stripeOption;

                        TokenService service = new TokenService();
                        Stripe.Token newToken = service.Create(stripeCard);

                        Debug.WriteLine("step 3 reached");
                        // Step 3: Assign the token to the soruce 
                        var option = new SourceCreateOptions();
                        option.Type = SourceType.Card;
                        option.Currency = "usd";
                        option.Token = newToken.Id;

                        var sourceService = new SourceService();
                        Source source = sourceService.Create(option);

                        source.ClientSecret = clientSec;
                        //source.Card

                        Debug.WriteLine("step 4 reached");
                        // Step 4: Create customer
                        CustomerCreateOptions customer = new CustomerCreateOptions();
                        customer.Name = cardHolderName.Text.Trim();
                        customer.Email = cardHolderEmail.Text.ToLower().Trim();
                        //if (cardDescription.Text == "" || cardDescription.Text == null)
                        customer.Description = "";
                        //else customer.Description = cardDescription.Text.Trim();
                        if (cardHolderUnit.Text == null)
                        {
                            cardHolderUnit.Text = "";
                        }
                        customer.Address = new AddressOptions { City = cardCity.Text.Trim(), Country = Constant.Contry, Line1 = cardHolderAddress.Text.Trim(), Line2 = cardHolderUnit.Text.Trim(), PostalCode = cardZip.Text.Trim(), State = cardState.Text.Trim() };


                        var customerService = new CustomerService();
                        var cust = customerService.Create(customer);

                        Debug.WriteLine("step 5 reached");
                        // Step 5: Charge option
                        var chargeOption = new ChargeCreateOptions();
                        chargeOption.Amount = (long)RemoveDecimalFromTotalAmount(total);

                        Debug.WriteLine("hopefully correct total: " + total);
                        chargeOption.Currency = "usd";
                        chargeOption.ReceiptEmail = cardHolderEmail.Text.ToLower().Trim();
                        chargeOption.Customer = cust.Id;
                        chargeOption.Source = source.Id;
                        //if (cardDescription.Text == "" || cardDescription.Text == null)
                        chargeOption.Description = "";
                        //else chargeOption.Description = cardDescription.Text.Trim();

                        //chargeOption.Description = cardDescription.Text.Trim();

                        Debug.WriteLine("step 6 reached");
                        // Step 6: charge the customer COMMENTED OUT FOR TESTING, backend already charges stripe so we don't have to do it here
                        var chargeService = new ChargeService();

                        Charge charge = chargeService.Create(chargeOption);
                        //charge.PaymentIntent = (PaymentIntent)payIntent;
                        Debug.WriteLine("charge: " + charge.ToString());
                        Debug.WriteLine("charge id: " + charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3));
                        //chargeId = charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3);
                        if (charge.Status == "succeeded")
                        {
                            chargeId = charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3);

                            //await Navigation.PushAsync(new Loading());

                            PaymentScreen.HeightRequest = 0;
                            PaymentScreen.Margin = new Thickness(0, 0, 0, 0);
                            StripeScreen.Height = 0;
                            PayPalScreen.Height = 0;
                            orangeBox.HeightRequest = deviceHeight / 2;

                            Debug.WriteLine("STRIPE PAYMENT WAS SUCCESSFUL");
                            //end of stripe payment frontend processing


                            //Preferences.Set("price", "00.00");
                            //DisplayAlert("Payment Completed", "Your payment was successful. Press 'CONTINUE' to select your meals!", "OK");
                            //when purchasing a first meal, might not send to endpoint on time when clicking the continue button as fast as possible, resulting in error on select pg
                            //wait half a second
                            Task.Delay(500).Wait();
                            if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                            {
                                //spacer6.IsVisible = true;
                                //passLabel.IsVisible = true;
                                ////spacer7.IsVisible = true;
                                //password.IsVisible = true;
                                //passwordEntry.IsVisible = true;
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
                            //Preferences.Set(purchaseDescription, cardDescription.Text);

                            await setPaymentInfo();
                            Preferences.Set("canChooseSelect", true);
                            //await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                            if (paymentSucceed)
                            {
                                await Navigation.PushAsync(new CongratsPage(passingZones, cust_firstName, cust_lastName, cust_email, new_purchase_id));
                            }
                            //done from checkout button clicked
                        }
                        else
                        {
                            await Navigation.PopAsync(false);
                            // Fail
                            await DisplayAlert("Ooops", "Payment was not succesfull. Please try again", "OK");
                        }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    await Navigation.PopAsync(false);
                    await DisplayAlert("Alert!", ex.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        //private void pay()
        //{
        //    //if (!this.paymentIntentClientSecret)
        //    //{
        //    //    NSLog("PaymentIntent hasn\'t been created");
        //    //    return;
        //    //}
        //    STPPaymentMethodCardParams* cardParams = this.cardTextField.cardParams;
        //    STPPaymentMethodParams* paymentMethodParams = STPPaymentMethodParams.paramsWithCard(cardParams) billingDetails(null) metadata(null);
        //    STPPaymentIntentParams* paymentIntentParams = STPPaymentIntentParams.alloc().initWithClientSecret(this.paymentIntentClientSecret);
        //    paymentIntentParams.paymentMethodParams = paymentMethodParams;
        //    STPPaymentHandler* paymentHandler = STPPaymentHandler.sharedHandler();
        //    paymentHandler.confirmPayment(paymentIntentParams) withAuthenticationContext(this) completion((status, paymentIntent, error) => {
        //        dispatch_async(dispatch_get_main_queue(), () => {
        //            switch (status)
        //            {
        //                case STPPaymentHandlerActionStatusFailed:
        //                    this.displayAlertWithTitle("Payment failed") message((error.localizedDescription ? null : "")) restartDemo(false);
        //                case STPPaymentHandlerActionStatusCanceled:
        //                    this.displayAlertWithTitle("Payment canceled") message((error.localizedDescription ? null : "")) restartDemo(false);
        //                case STPPaymentHandlerActionStatusSucceeded:
        //                    this.displayAlertWithTitle("Payment succeeded") message((paymentIntent.description ? null : "")) restartDemo(true);
        //                default:
        //                    break;
        //            }
        //        });
        //    });
        //}

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
        public async void CancelViaStripe(System.Object sender, System.EventArgs e)
        {
            onStripeScreen = false;

            headingGrid.IsVisible = true;
            checkoutButton.IsVisible = true;
            backButton.IsVisible = true;
            mainStack.IsVisible = true;
            paymentStack.IsVisible = true;
            await scroller.ScrollToAsync(0, mainStack.Height + 80, true);
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
            //SetPayPalCredentials();
            paymentMethod = "PAYPAL";

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
                mainStack.IsVisible = false;
                paymentStack.IsVisible = false;
                await scroller.ScrollToAsync(0, mainStack.Height + 150, true);
                checkoutButton.IsVisible = false;
                backButton.IsVisible = false;
                PaymentScreen.HeightRequest = this.Height;
                //PaymentScreen.Margin = new Thickness(0, -PaymentScreen.HeightRequest / 2, 0, 0);
                //PaymentScreen.Margin = new Thickness(0, -mainStack.Height, 0, 0);
                PayPalScreen.Height = 0;
                StripeScreen.Height = this.Height;
                Browser.HeightRequest = this.Height - (this.Height / 8);
                orangeBox.HeightRequest = 0;
                await scroller.ScrollToAsync(0, -40, false);

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
            if (source.Url == "https://mealsfor.me/login")
            {
                //Navigation.PushAsync(new Loading());
                if (paypalPaymentDone == true)
                {
                    Debug.WriteLine("true entered");
                    headingGrid.IsVisible = true;
                    checkoutButton.IsVisible = true;
                    backButton.IsVisible = true;
                    mainStack.IsVisible = false;
                    paymentStack.IsVisible = false;
                    checkoutGrid.IsVisible = true;
                }
                headingGrid.IsVisible = true;
                mainStack.IsVisible = true;
                paymentStack.IsVisible = true;
                checkoutButton.IsVisible = true;
                backButton.IsVisible = true;
                checkoutGrid.IsVisible = true;
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
                    //these checks are probably coming from the paypal server itself
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

                    Debug.WriteLine("deliveryentry: " + DeliveryEntry.Text);
                    if (DeliveryEntry.Text == "M4METEST" || DeliveryEntry.Text == "M4ME TEST")
                    {
                        mode = "TEST";
                        clientId = Constant.TestClientId;
                        secret = Constant.TestSecret;
                    }
                    else
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
                    ReturnUrl = "https://mealsfor.me/login",
                    CancelUrl = "https://mealsfor.me/login"
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
            //await Navigation.PushAsync(new Loading());
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
            chargeId = result.Id;
            Debug.WriteLine("ID                 : " + id);

            if (result.Status == "COMPLETED")
            {
                Debug.WriteLine("PAYPAL PAYMENT WAS SUCCESSFUL");


                //testing with password
                if ((string)Xamarin.Forms.Application.Current.Properties["platform"] == "DIRECT")
                {
                    paypalPaymentDone = true;
                    orangeBox.HeightRequest = deviceHeight / 2;
                    passStack.IsVisible = true;
                    mainStack.IsVisible = false;
                    paymentStack.IsVisible = false;
                    //spacer6.IsVisible = true;
                    //passLabel.IsVisible = true;
                    //spacer7.IsVisible = true;
                    //password.IsVisible = true;
                    //passwordEntry.IsVisible = true;
                    //spacer8.IsVisible = true;
                    checkoutButton.IsVisible = true;
                    backButton.IsVisible = true;

                    checkoutButton.Text = "CONTINUE";
                    //await Navigation.PopAsync();
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
                    //Preferences.Set(purchaseDescription, cardDescription.Text);

                    await setPaymentInfo();
                    Preferences.Set("canChooseSelect", true);
                    //await Navigation.PushAsync(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                    if (paymentSucceed)
                    {
                        await Navigation.PushAsync(new CongratsPage(passingZones, cust_firstName, cust_lastName, cust_email, new_purchase_id));
                    }

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
                //await Navigation.PopAsync();
                Debug.WriteLine("didn't work");
                await DisplayAlert("Ooops", "You payment was cancel or not sucessful. Please try again", "OK");
            }

            return response;
        }

        void menuBackButton_Clicked(System.Object sender, System.EventArgs e)
        {
        }

        // Auto-complete
        private ObservableCollection<AddressAutocomplete> _addresses;
        public ObservableCollection<AddressAutocomplete> Addresses
        {
            get => _addresses ?? (_addresses = new ObservableCollection<AddressAutocomplete>());
            set
            {
                if (_addresses != value)
                {
                    _addresses = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _addressText;
        public string AddressText
        {
            get => _addressText;
            set
            {
                if (_addressText != value)
                {
                    _addressText = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _addressText2;
        public string AddressText2
        {
            get => _addressText2;
            set
            {
                if (_addressText2 != value)
                {
                    _addressText2 = value;
                    OnPropertyChanged();
                }
            }
        }

        string origAdd = "";
        string origUnit = "";
        string origCity = "";
        string origState = "";
        string origZip = "";

        //added async
        private async void OnAddressChanged(object sender, TextChangedEventArgs eventArgs)
        {
            Debug.WriteLine("onaddresschanged entered with: " + ((Entry)sender).Placeholder);
            //if (((Entry)sender).Equals(AddressEntry) && onCardAdd == true)
            //{
            //    Debug.WriteLine("first if of onAddressChanged");
            //    var ent = (Entry)sender;
            //    ent.Text = eventArgs.OldTextValue;
            //    return;
            //}

            if (((Entry)sender).Equals(AddressEntry) && onCardAdd == false)
            {
                Debug.WriteLine("second if of onAddressChanged");
                paymentStack.IsVisible = false;
                await scroller.ScrollToAsync(0, -50, true);
                addressList.IsVisible = true;
                UnitCity.IsVisible = false;
                StateZip.IsVisible = false;
                addressList.ItemsSource = await addr.GetPlacesPredictionsAsync(AddressEntry.Text);
                //addr.OnAddressChanged(addressList, Addresses, _addressText);
            }
            else
            {
                //jonathan's code
                // Debug.WriteLine("third if of onAddressChanged");
                // addr.OnAddressChanged(addressList2, Addresses, _addressText2);

                addressListFrame.IsVisible = true;
                addressList2.IsVisible = true;
                CityStateZip.IsVisible = false;
                addressList2.ItemsSource = await addr.GetPlacesPredictionsAsync(cardHolderAddress.Text);
                //addr.OnAddressChanged(addressList2, Addresses, _addressText);
            }
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            if (((Entry)sender).Equals(AddressEntry))
            {
                //addr.addressEntryFocused(addressList, new Grid[] { UnitCity, StateZip });
            }
            else
            {
                if (((Entry)sender).Equals(cardHolderAddress))
                    onCardAdd = true;

                //commented out in ashley's code
                // addressListFrame.IsVisible = true;
                // addr.addressEntryFocused(addressList2, new Grid[] { CityStateZip });
            }
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            if (((Entry)sender).Equals(AddressEntry))
            {
                addr.addressEntryUnfocused(addressList, new Grid[] { UnitCity, StateZip });
            }
            else
            {
                if (((Entry)sender).Equals(cardHolderAddress))
                    onCardAdd = false;
                addressListFrame.IsVisible = false;
                addr.addressEntryUnfocused(addressList2, new Grid[] { UnitGrid, CityStateZip });
            }
        }

        private async void addressSelected(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (((ListView)sender).Equals(addressList))
                {
                    addr.addressSelected(addressList, new Grid[] { UnitCity, StateZip }, AddressEntry, CityEntry, StateEntry, ZipEntry);
                    addressList.IsVisible = false;
                    UnitCity.IsVisible = true;
                    StateZip.IsVisible = true;

                    string unit = "";
                    if (AptEntry.Text != "")
                        unit = AptEntry.Text;

                    // Setting request for USPS API
                    XDocument requestDoc = new XDocument(
                        new XElement("AddressValidateRequest",
                        new XAttribute("USERID", "400INFIN1745"),
                        new XElement("Revision", "1"),
                        new XElement("Address",
                        new XAttribute("ID", "0"),
                        new XElement("Address1", AddressEntry.Text),
                        new XElement("Address2", unit),
                        new XElement("City", CityEntry.Text),
                        new XElement("State", StateEntry.Text),
                        new XElement("Zip5", ZipEntry.Text),
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
                            //  && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())
                            if (GetXMLElement(element, "DPVConfirmation").Equals("Y")) // Best case
                            {
                                // Get longitude and latitide because we can make a deliver here. Move on to next page.
                                // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                                //GetAddressLatitudeLongitude();
                                Geocoder geoCoder = new Geocoder();

                                Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                                Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                                Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                                IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                                Position position = approximateLocations.FirstOrDefault();

                                latitude = $"{position.Latitude}";
                                longitude = $"{position.Longitude}";

                                map.MapType = MapType.Street;
                                var mapSpan = new MapSpan(position, 0.001, 0.001);

                                Pin address = new Pin();
                                address.Label = "Delivery Address";
                                address.Type = PinType.SearchResult;
                                address.Position = position;

                                map.MoveToRegion(mapSpan);
                                map.Pins.Add(address);

                                string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;
                                //request3.RequestUri = new Uri(url3);
                                //request3.Method = HttpMethod.Get;
                                //var client3 = new HttpClient();
                                //HttpResponseMessage response3 = await client3.SendAsync(request3);
                                Debug.WriteLine("categorical options url: " + url3);

                                var content = client4.DownloadString(url3);
                                var obj = JsonConvert.DeserializeObject<ZonesDto>(content);

                                //HttpContent content3 = response3.Content;
                                //Console.WriteLine("content: " + content3);
                                //var userString3 = await content3.ReadAsStringAsync();
                                //Debug.WriteLine("userString3: " + userString3);
                                //JObject info_obj3 = JObject.Parse(userString3);
                                if (obj.Result.Length == 0)
                                {
                                    CheckAddressHeading.Text = "Still Growing…";
                                    CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your neighborhood yet. Enter your email address and we will let you know as soon as we come to your neighborhood.";
                                    EmailFrame.IsVisible = true;
                                    OkayButton.Text = "Okay";
                                    loginButton2.IsVisible = false;
                                    fade.IsVisible = true;
                                    CheckAddressGrid.IsVisible = true;
                                }
                            }
                        }
                    }
                    //end address updating
                }
                else
                {
                    addr.addressSelected(addressList2, new Grid[] { UnitGrid, CityStateZip }, cardHolderAddress, cardCity, cardState, cardZip);
                    addressListFrame.IsVisible = false;
                    addressList2.IsVisible = false;
                    UnitGrid.IsVisible = true;
                    CityStateZip.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        //start of menu functions
        void clickedOpenMenu(object sender, EventArgs e)
        {
            openedMenu.IsVisible = true;
        }

        void clickedCloseMenu(object sender, EventArgs e)
        {
            openedMenu.IsVisible = false;
        }

        async void clickedLanding(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealPlan(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MealPlans(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSelect(System.Object sender, System.EventArgs e)
        {
            if (Preferences.Get("canChooseSelect", false) == false)
                DisplayAlert("Error", "please purchase a meal plan first", "OK");
            else
            {
                Zones[] zones = new Zones[] { };
                await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email), false);
                //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
            }
        }

        async void clickedSubscription(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSubHistory(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionHistory(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        void xButtonClicked(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            baaPopUpGrid.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
        }

        void clickedBecomeAmb(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = true;
            baaPopUpGrid.IsVisible = true;
        }

        void clickedCreateAmb(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (AmbEmailEntry.Text != null && AmbEmailEntry.Text != "")
                {
                    createAmb newAmb = new createAmb();
                    newAmb.code = AmbEmailEntry.Text.Trim();
                    var createAmbSerializedObj = JsonConvert.SerializeObject(newAmb);
                    var content = new StringContent(createAmbSerializedObj, Encoding.UTF8, "application/json");
                    var client = new System.Net.Http.HttpClient();
                    var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/brandAmbassador/create_ambassador", content);
                    Console.WriteLine("RESPONSE TO CREATE_AMBASSADOR   " + response.Result);
                    Console.WriteLine("CREATE JSON OBJECT BEING SENT: " + createAmbSerializedObj);
                    fade.IsVisible = false;
                    baaPopUpGrid.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        void clickedLogout(System.Object sender, System.EventArgs e)
        {
            Xamarin.Forms.Application.Current.Properties.Remove("user_id");
            Xamarin.Forms.Application.Current.Properties["platform"] = "GUEST";
            Xamarin.Forms.Application.Current.Properties.Remove("time_stamp");
            //Application.Current.Properties.Remove("platform");
            Xamarin.Forms.Application.Current.MainPage = new MainPage();
        }
        //end of menu functions

        async void OkayClicked(System.Object sender, System.EventArgs e)
        {
            if (EmailFrame.IsVisible && EmailEntry.Text != null && EmailEntry.Text.Length != 0)
            {
                // add email to new neighborhood notification list
            }
            fade.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
            //Application.Current.MainPage = new NavigationPage(new ExploreMeals());
        }

        //void xButtonClicked(System.Object sender, System.EventArgs e)
        //{
        //    fade.IsVisible = false;
        //    CheckAddressGrid.IsVisible = false;
        //}

        void loginButtonClicked(System.Object sender, System.EventArgs e)
        {
            Xamarin.Forms.Application.Current.MainPage = new MainLogin();
        }

        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        void checkBoxChecked(object sender, CheckedChangedEventArgs e)
        {
            Debug.WriteLine("checkbox value: " + e.Value.ToString());
            if (e.Value.ToString() == "True")
                termsChecked = true;
            else termsChecked = false;
        }
    }
}