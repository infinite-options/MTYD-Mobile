using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using MTYD.Constants;
using MTYD.Model;
using MTYD.Model.Login.LoginClasses;
using MTYD.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace MTYD
{
    public partial class CarlosSocialSignUp : ContentPage
    {
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        public SignUpPost socialSignUp = new SignUpPost();
        public bool isAddressValidated = false;
        WebClient client4 = new WebClient();
        bool withinZones = false;
        Zones[] passingZones;
        public bool isAddessValidated = false;
        Address addr;

        public CarlosSocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
        {
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            addr = new Address();
            InitializeComponent();
            InitializeSignUpPost();
            FNameEntry.Text = firstName;
            LNameEntry.Text = lastName;
            emailEntry.Text = emailAddress;
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            checkPlatform(height, width);

            socialSignUp.email = emailAddress;
            socialSignUp.first_name = firstName;
            socialSignUp.last_name = lastName;
            socialSignUp.mobile_access_token = accessToken;
            socialSignUp.mobile_refresh_token = refreshToken;
            socialSignUp.user_access_token = "FALSE";
            socialSignUp.user_refresh_token = "FALSE";
            socialSignUp.social = platform;
            socialSignUp.social_id = socialId;
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                //heading.FontSize = width / 25;
                //heading.Margin = new Thickness(20, 100, 0, 0);
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.WidthRequest = 140;
                fade.Margin = new Thickness(0, -height / 3, 0, 0);
                addressList.HeightRequest = width / 5;
                backButton.Margin = new Thickness(25, 0, 0, 30);
                backButton.HeightRequest = 25;
                CheckAddressGrid.Margin = new Thickness(50, 170, 50, 120);

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

                SignUpButton.CornerRadius = 25;
            }
            else
            {
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.WidthRequest = 140;
                backButton.Margin = new Thickness(25, 0, 0, 30);
                backButton.HeightRequest = 25;
            }
        }



        void InitializeSignUpPost()
        {
            socialSignUp.email = "";
            socialSignUp.first_name = "";
            socialSignUp.last_name = "";
            socialSignUp.phone_number = "";
            socialSignUp.address = "";
            socialSignUp.unit = "";
            socialSignUp.city = "";
            socialSignUp.state = "";
            socialSignUp.zip_code = "";
            socialSignUp.latitude = "0.0";
            socialSignUp.longitude = "0.0";
            socialSignUp.referral_source = "MOBILE";
            socialSignUp.role = "CUSTOMER";
            socialSignUp.mobile_access_token = "";
            socialSignUp.mobile_refresh_token = "";
            socialSignUp.user_access_token = "FALSE";
            socialSignUp.user_refresh_token = "FALSE";
            socialSignUp.social = "";
            socialSignUp.password = "";
        }

        async void BackClick(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
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

        private async void OnAddressChanged(object sender, EventArgs eventArgs)
        {
            addressList.IsVisible = true;
            UnitCity.IsVisible = false;
            StateZip.IsVisible = false;
            addressList.ItemsSource = await addr.GetPlacesPredictionsAsync(AddressEntry.Text);
            //addr.OnAddressChanged(addressList, Addresses, _addressText);
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            //addr.addressEntryFocused(addressList, new Grid[] { UnitCity, StateZip });
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            addr.addressEntryUnfocused(addressList, new Grid[] { UnitCity, StateZip });
        }

        async void addressSelected(System.Object sender, System.EventArgs e)
        {
            addr.addressSelected(addressList, new Grid[] { UnitCity, StateZip }, AddressEntry, CityEntry, StateEntry, ZipEntry);
            addressList.IsVisible = false;
            UnitCity.IsVisible = true;
            StateZip.IsVisible = true;
        }

        void xButtonClicked(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
            if (CheckAddressHeading.Text == "Hooray!")
            {
                addressEntries.IsVisible = false;
                mainEntries.IsVisible = true;
            }
        }

        async void OkayClicked(System.Object sender, System.EventArgs e)
        {
            if (EmailFrame.IsVisible && EmailEntry.Text != null && EmailEntry.Text.Length != 0)
            {
                // add email to new neighborhood notification list
            }
            fade.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
            if (CheckAddressHeading.Text == "Hooray!")
            {
                addressEntries.IsVisible = false;
                mainEntries.IsVisible = true;
            }
        }


        async void ValidatingAddressClick(System.Object sender, System.EventArgs e)
        {

            if (PhoneEntry.Text != null && PhoneEntry.Text.Length == 10)
            {
                socialSignUp.phone_number = PhoneEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Alert!", "Please enter a 10 valid phone number", "OK");
                }

            }

            if (AddressEntry.Text != null)
            {
                socialSignUp.address = AddressEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your address", "OK");
                }
            }

            if (AptEntry.Text != null)
            {
                socialSignUp.unit = AptEntry.Text.Trim();
            }

            if (FNameEntry.Text != null)
            {
                socialSignUp.first_name = FNameEntry.Text.Trim();
            }

            if (LNameEntry.Text != null)
            {
                socialSignUp.last_name = LNameEntry.Text.Trim();
            }

            if (CityEntry.Text != null)
            {
                socialSignUp.city = CityEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your city", "OK");
                }
            }

            if (StateEntry.Text != null)
            {
                socialSignUp.state = StateEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your state", "OK");
                }
            }

            if (ZipEntry.Text != null && ZipEntry.Text.Length == 5)
            {
                socialSignUp.zip_code = ZipEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your zipcode", "OK");
                }
            }

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
                new XElement("Address1", AddressEntry.Text),
                new XElement("Address2", AptEntry.Text),
                new XElement("City", CityEntry.Text),
                new XElement("State", StateEntry.Text),
                new XElement("Zip5", ZipEntry.Text),
                new XElement("Zip4", "")
                     )
                 )
             );
            var url = "https://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
            Console.WriteLine(url);
            var client = new WebClient();
            var response = client.DownloadString(url);  // USPS endpoint call

            var xdoc = XDocument.Parse(response.ToString());
            Console.WriteLine(xdoc);
            string latitude = "0";
            string longitude = "0";


            foreach (XElement element in xdoc.Descendants("Address"))
            {

                System.Diagnostics.Debug.WriteLine(GetXMLElement(element, "Error"));
                if (GetXMLElement(element, "Error").Equals(""))
                {
                    if ((GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("S"))
                        && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text) &&
                        GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())) // Best case
                    {
                        Geocoder geoCoder = new Geocoder();

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text + "," + CityEntry.Text + "," + StateEntry.Text);
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

                        socialSignUp.latitude = latitude;
                        socialSignUp.longitude = longitude;

                        isAddressValidated = true;
                        SignUpSocialUserClick(sender, e);

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
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content2 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                            await DisplayAlert(obj.result[5].title, obj.result[5].message, obj.result[5].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Missing Info", "Please enter your unit/apartment number into the appropriate field.", "OK");
                        }
                        return;
                    }
                    else
                    {
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content2 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                            await DisplayAlert(obj.result[6].title, obj.result[6].message, obj.result[6].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Invalid Address", "The address you entered couldn't be confirmed. Please enter another one.", "OK");
                        }
                        return;
                    }
                }
                else
                {
                    try
                    {
                        WebClient client4 = new WebClient();
                        var content2 = client4.DownloadString(Constant.AlertUrl);
                        var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                        await DisplayAlert(obj.result[6].title, obj.result[6].message, obj.result[6].responses);
                    }
                    catch
                    {
                        await DisplayAlert("Invalid Address", "The address you entered couldn't be confirmed. Please enter another one.", "OK");
                    }
                    return;
                    // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
                    //Console.WriteLine("Seems like your address is invalid.");
                    //await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
                    //return;
                }
            }

            if (!isAddressValidated)
            {
                //8
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[7].title, obj.result[7].message, obj.result[7].responses);
                }
                catch
                {
                    await DisplayAlert("Message", "We were not able to validate your address. Please try again.", "OK");
                }

                
            }
            else
            {
                //9
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[8].title, obj.result[8].message, obj.result[8].responses);
                }
                catch
                {
                    await DisplayAlert("Message", "We validated your address please tap on the Sign up button to create your account!", "OK");
                }

                
            }
        }

        async void continueClicked(System.Object sender, System.EventArgs e)
        {
            if (AddressEntry.Text != null)
            {
                socialSignUp.address = AddressEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your address", "OK");
                }
                return;
            }

            if (AptEntry.Text != null)
            {
                socialSignUp.unit = AptEntry.Text.Trim();
            }

            if (CityEntry.Text != null)
            {
                socialSignUp.city = CityEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your city", "OK");
                }
                return;
            }

            if (StateEntry.Text != null)
            {
                socialSignUp.state = StateEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your state", "OK");
                }
                return;
            }

            if (ZipEntry.Text != null)
            {
                socialSignUp.zip_code = ZipEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your zipcode", "OK");
                }
                return;
            }

            if (PhoneEntry.Text != null && PhoneEntry.Text.Length == 10)
            {
                socialSignUp.phone_number = PhoneEntry.Text.Trim();
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please enter your phone number", "OK");
                }
                return;
            }

            // Setting request for USPS API
            XDocument requestDoc = new XDocument(
                new XElement("AddressValidateRequest",
                new XAttribute("USERID", "400INFIN1745"),
                new XElement("Revision", "1"),
                new XElement("Address",
                new XAttribute("ID", "0"),
                new XElement("Address1", socialSignUp.address),
                new XElement("Address2", socialSignUp.unit),
                new XElement("City", socialSignUp.city),
                new XElement("State", socialSignUp.state),
                new XElement("Zip5", socialSignUp.zip_code),
                new XElement("Zip4", "")
                     )
                 )
             );
            var url = "https://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
            Console.WriteLine(url);
            var client = new WebClient();
            var response = client.DownloadString(url);

            var xdoc = XDocument.Parse(response.ToString());
            Console.WriteLine(xdoc);
            string latitude = "0";
            string longitude = "0";
            foreach (XElement element in xdoc.Descendants("Address"))
            {
                if (GetXMLElement(element, "Error").Equals(""))
                {
                    if (GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("S"))
                    {
                        Geocoder geoCoder = new Geocoder();

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(socialSignUp.address + "," + socialSignUp.city + "," + socialSignUp.state);
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

                        socialSignUp.latitude = latitude;
                        socialSignUp.longitude = longitude;

                        string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;

                        var content = client4.DownloadString(url3);
                        var obj = JsonConvert.DeserializeObject<ZonesDto>(content);

                        if (obj.Result.Length == 0)
                        {
                            withinZones = false;
                        }
                        else
                        {
                            Debug.WriteLine("first business: " + obj.Result[0].business_name);
                            passingZones = obj.Result;
                            withinZones = true;
                        }

                        break;
                    }
                    else if (GetXMLElement(element, "DPVConfirmation").Equals("D"))
                    {
                        //6
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content2 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                            await DisplayAlert(obj.result[5].title, obj.result[5].message, obj.result[5].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Missing Info", "Please enter your unit/apartment number into the appropriate field.", "OK");
                        }
                        return;
                    }
                    else
                    {
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content2 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                            await DisplayAlert(obj.result[6].title, obj.result[6].message, obj.result[6].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Invalid Address", "The address you entered couldn't be confirmed. Please enter another one.", "OK");
                        }
                        return;
                    }
                }
                else
                {
                    try
                    {
                        WebClient client4 = new WebClient();
                        var content2 = client4.DownloadString(Constant.AlertUrl);
                        var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                        await DisplayAlert(obj.result[6].title, obj.result[6].message, obj.result[6].responses);
                    }
                    catch
                    {
                        await DisplayAlert("Invalid Address", "The address you entered couldn't be confirmed. Please enter another one.", "OK");
                    }
                    return;
                    // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
                    //Console.WriteLine("Seems like your address is invalid.");
                    //await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
                    //return;
                }
            }
            if (latitude == "0" || longitude == "0")
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[7].title, obj.result[7].message, obj.result[7].responses);
                }
                catch
                {
                    await DisplayAlert("We couldn't find your address", "Please check for errors.", "OK");
                }
            }
            else if (withinZones == false)
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    //await DisplayAlert(obj.result[7].title, obj.result[7].message, obj.result[7].responses);
                    CheckAddressHeading.Text = obj.result[41].title;
                    CheckAddressBody.Text = obj.result[41].message;
                    OkayButton.Text = obj.result[41].responses;
                }
                catch
                {
                    CheckAddressHeading.Text = "Oops!"; //42
                    CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your Zip Code yet. Please feel free to leave us your email address and we will let you know as soon as we come to your neighborhood.";
                    OkayButton.Text = "Okay";
                }

                fade.IsVisible = true;
                CheckAddressGrid.IsVisible = true;
                //CheckAddressHeading.Text = "Oops!"; //42
                //CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your Zip Code yet. Please feel free to leave us your email address and we will let you know as soon as we come to your neighborhood.";
                EmailFrame.IsVisible = true;
                //OkayButton.Text = "Okay";
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    //await DisplayAlert(obj.result[7].title, obj.result[7].message, obj.result[7].responses);
                    CheckAddressHeading.Text = obj.result[42].title;
                    CheckAddressBody.Text = obj.result[42].message;
                    OkayButton.Text = obj.result[42].responses;
                }
                catch
                {
                    CheckAddressHeading.Text = "Hooray!"; //42
                    CheckAddressBody.Text = "We are so glad that we deliver to your neighborhood. Please click Continue to complete the Sign Up process.";
                    OkayButton.Text = "Continue";
                }

                fade.IsVisible = true;
                CheckAddressGrid.IsVisible = true;
                //CheckAddressHeading.Text = "Hooray!"; //43
                //CheckAddressBody.Text = "We are so glad that we deliver to your neighborhood. Please click Continue to complete the Sign Up process.";
                EmailFrame.IsVisible = false;
                //OkayButton.Text = "Continue";

                isAddessValidated = true;

            }
        }

        async void SignUpSocialUserClick(System.Object sender, System.EventArgs e)
        {
            if (isAddessValidated)
            {
                var signUpSerializedObject = JsonConvert.SerializeObject(socialSignUp);
                var singUpContent = new StringContent(signUpSerializedObject, Encoding.UTF8, "application/json");
                System.Diagnostics.Debug.WriteLine(signUpSerializedObject);

                var client = new HttpClient();
                var RDSResponse = await client.PostAsync(Constant.SignUpUrl, singUpContent);  // Post to RDS database
                Debug.WriteLine("signupurl:" + Constant.SignUpUrl);
                var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(RDSMessage);

                if (RDSResponse.IsSuccessStatusCode)
                {
                    var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);

                    System.Diagnostics.Debug.WriteLine("This are the variable you can use from RDSMessage");
                    System.Diagnostics.Debug.WriteLine("First Name: " + RDSData.result.first_name);
                    System.Diagnostics.Debug.WriteLine("Last Name: " + RDSData.result.last_name);
                    System.Diagnostics.Debug.WriteLine("Customer ID: " + RDSData.result.customer_uid);
                    System.Diagnostics.Debug.WriteLine("Access Token: " + RDSData.result.access_token);
                    System.Diagnostics.Debug.WriteLine("Refresh Token: " + RDSData.result.refresh_token);
                    Application.Current.Properties["user_id"] = RDSData.result.customer_uid;

                    string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                    var request3 = new HttpRequestMessage();
                    request3.RequestUri = new Uri(url);
                    request3.Method = HttpMethod.Get;
                    var client2 = new HttpClient();
                    HttpResponseMessage response = await client2.SendAsync(request3);
                    HttpContent content = response.Content;
                    Console.WriteLine("content: " + content);
                    var userString = await content.ReadAsStringAsync();
                    JObject info_obj2 = JObject.Parse(userString);
                    this.NewMainPage.Clear();

                    DateTime today = DateTime.Now;
                    DateTime expDate = today.AddDays(Constant.days);

                    //Application.Current.Properties["user_id"] = RDSData.result.customer_uid;
                    Application.Current.Properties["time_stamp"] = expDate;
                    Application.Current.Properties["platform"] = socialSignUp.social;

                    if (socialSignUp.social == "GOOGLE")
                    {
                        Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + (info_obj2["result"])[0]["mobile_access_token"].ToString());
                        //request profile image
                        using (var webClient = new System.Net.WebClient())
                        {
                            var json = webClient.DownloadString(apiRequestUri);
                            var data2 = JsonConvert.DeserializeObject<profilePicLogIn>(json);
                            Debug.WriteLine(data2.ToString());
                            var userPicture = data2.picture;
                            //var holder = userPicture[0];
                            Debug.WriteLine(userPicture);
                            Preferences.Set("profilePicLink", userPicture);

                            //var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            //Application.Current.Properties["user_id"] = data.result[0].customer_uid;
                        }
                    }
                    else Preferences.Set("profilePicLink", null);


                    // Application.Current.MainPage = new SubscriptionPage();
                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                }
            }
            else
            {
                //6
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[4].title, obj.result[4].message, obj.result[4].responses);
                }
                catch
                {
                    await DisplayAlert("Message", "We weren't able to sign you up", "OK");
                }
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

        public static string GetXMLAttribute(XElement element, string name)
        {
            var el = element.Attribute(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
        }
    }
}
