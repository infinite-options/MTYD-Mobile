using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MTYD.Constants;
using MTYD.Model.Login.LoginClasses;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using MTYD.ViewModel;
using Newtonsoft.Json.Linq;
using MTYD.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MTYD
{
    public partial class CarlosSignUp : ContentPage
    {
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        public SignUpPost directSignUp = new SignUpPost();
        public bool isAddessValidated = false;
        Address addr;
        WebClient client4 = new WebClient();
        bool withinZones = false;
        Zones[] passingZones;

        public CarlosSignUp()
        {
            addr = new Address();
            InitializeComponent();
            BindingContext = this;
            InitializeSignUpPost();
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            checkPlatform(height, width);
        }

        public void checkPlatform(double height, double width)
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            orangeBox.HeightRequest = height / 2;
            orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
            orangeBox.CornerRadius = height / 40;
            heading.WidthRequest = width / 3;
            fade.Margin = new Thickness(0, -height / 3, 0, 0);

            if (Device.RuntimePlatform == Device.iOS)
            {
                appleSignupFrame.WidthRequest = width / 3.5;
                appleSignupFrame.HeightRequest = width / 25;
                appleSignupButton.WidthRequest = width / 3.5;
                appleSignupButton.HeightRequest = width / 25;
                appleSignupText.FontSize = width / 50;
                fbSignupFrame.WidthRequest = width / 3.5;
                fbSignupFrame.HeightRequest = width / 25;
                fbSignupButton.WidthRequest = width / 3.5;
                fbSignupButton.HeightRequest = width / 25;
                fbSignupText.FontSize = width / 50;
                googleSignupFrame.WidthRequest = width / 3.5;
                googleSignupFrame.HeightRequest = width / 25;
                googleSignupButton.WidthRequest = width / 3.5;
                googleSignupButton.HeightRequest = width / 25;
                googleSignupText.FontSize = width / 50;

                addressList.HeightRequest = width / 5;

                //backButton.WidthRequest = width / 8;
                //backButton.HeightRequest = width / 17;
                //backButton.CornerRadius = (int)(width / 34);

                //SignUpButton.WidthRequest = width / 8;
                //SignUpButton.HeightRequest = width / 17;
                //SignUpButton.CornerRadius = (int)(width / 34);
            }
            else //Android
            {
                appleSignupFrame.WidthRequest = width / 5;
                appleSignupFrame.HeightRequest = width / 35;
                appleSignupButton.WidthRequest = width / 5;
                appleSignupButton.HeightRequest = width / 35;
                appleSignupText.FontSize = width / 65;
                fbSignupFrame.WidthRequest = width / 5;
                fbSignupFrame.HeightRequest = width / 35;
                fbSignupButton.WidthRequest = width / 5;
                fbSignupButton.HeightRequest = width / 35;
                fbSignupText.FontSize = width / 65;
                googleSignupFrame.WidthRequest = width / 5;
                googleSignupFrame.HeightRequest = width / 35;
                googleSignupButton.WidthRequest = width / 5;
                googleSignupButton.HeightRequest = width / 35;
                googleSignupText.FontSize = width / 65;

                //backButton.WidthRequest = width / 10;
                //backButton.HeightRequest = width / 25;
                //backButton.CornerRadius = (int)(width / 50);

                //SignUpButton.WidthRequest = width / 10;
                //SignUpButton.HeightRequest = width / 25;
                //SignUpButton.CornerRadius = (int)(width / 50);
            }
        }

        // This is what is written into the db
        public void InitializeSignUpPost()
        {

            directSignUp.email = "";
            directSignUp.first_name = "";
            directSignUp.last_name = "";
            directSignUp.phone_number = "";
            directSignUp.address = "";
            directSignUp.unit = "";
            directSignUp.city = "";
            directSignUp.state = "";
            directSignUp.zip_code = "";
            directSignUp.latitude = "0.0";
            directSignUp.longitude = "0.0";
            directSignUp.referral_source = "MOBILE";
            directSignUp.role = "CUSTOMER";
            directSignUp.mobile_access_token = "FALSE";
            directSignUp.mobile_refresh_token = "FALSE";
            directSignUp.user_access_token = "FALSE";
            directSignUp.user_refresh_token = "FALSE";
            directSignUp.social = "FALSE";
            directSignUp.password = "";
            directSignUp.social_id = "NULL";
        }

        async void BackClick(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        async void SignupClicked(object sender, System.EventArgs e)
        {
            if (FNameEntry.Text != null)
            {
                directSignUp.first_name = FNameEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your first name.", "OK");
                return;
            }

            if (LNameEntry.Text != null)
            {
                directSignUp.last_name = LNameEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your last name.", "OK");
                return;
            }

            if (emailEntry.Text != null)
            {
                directSignUp.email = emailEntry.Text.ToLower().Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            if (reenterEmailEntry.Text != null)
            {
                string conformEmail = reenterEmailEntry.Text.ToLower().Trim();
                if (!directSignUp.email.Equals(conformEmail))
                {
                    await DisplayAlert("Error", "Your email doesn't match", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            if (passwordEntry.Text != null)
            {
                directSignUp.password = passwordEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter a password", "OK");
                return;
            }

            if (reenterPasswordEntry.Text != null)
            {
                string password = reenterPasswordEntry.Text.Trim();
                if (!directSignUp.password.Equals(password))
                {
                    await DisplayAlert("Error", "Your password doesn't match", "OK");
                    return;
                }
            }

            if (directSignUp.unit == null)
            {
                directSignUp.unit = "";
            }
            
            await Application.Current.SavePropertiesAsync();
            await tagUser(emailEntry.Text, ZipEntry.Text);
            SignUpNewUser(sender, e);
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

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        async Task tagUser(string email, string zipCode)
        {
            var guid = Preferences.Get("guid", null);
            if (guid == null)
            {
                return;
            }
            var tags = "email_" + email + "," + "zip_" + zipCode;

            MultipartFormDataContent updateRegistrationInfoContent = new MultipartFormDataContent();
            StringContent guidContent = new StringContent(guid, Encoding.UTF8);
            StringContent tagsContent = new StringContent(tags, Encoding.UTF8);
            updateRegistrationInfoContent.Add(guidContent, "guid");
            updateRegistrationInfoContent.Add(tagsContent, "tags");

            var updateRegistrationRequest = new HttpRequestMessage();
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    updateRegistrationRequest.RequestUri = new Uri("https://phaqvwjbw6.execute-api.us-west-1.amazonaws.com/dev/api/v1/update_registration_guid_iOS");
                    //updateRegistrationRequest.RequestUri = new Uri("http://10.0.2.2:5000/api/v1/update_registration_guid_iOS");
                    break;
                case Device.Android:
                    updateRegistrationRequest.RequestUri = new Uri("https://phaqvwjbw6.execute-api.us-west-1.amazonaws.com/dev/api/v1/update_registration_guid_android");
                    //updateRegistrationRequest.RequestUri = new Uri("http://10.0.2.2:5000/api/v1/update_registration_guid_android");
                    break;
            }
            updateRegistrationRequest.Method = HttpMethod.Post;
            updateRegistrationRequest.Content = updateRegistrationInfoContent;
            var updateRegistrationClient = new HttpClient();
            HttpResponseMessage updateRegistrationResponse = await updateRegistrationClient.SendAsync(updateRegistrationRequest);
        }

        async void SignUpNewUser(System.Object sender, System.EventArgs e)
        {
            if (isAddessValidated)
            {
                var directSignUpSerializedObject = JsonConvert.SerializeObject(directSignUp);
                var content = new StringContent(directSignUpSerializedObject, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(directSignUpSerializedObject);

                var signUpclient = new HttpClient();
                var RDSResponse = await signUpclient.PostAsync(Constant.SignUpUrl, content);
                Debug.WriteLine("RDSResponse: " + RDSResponse.ToString());
                var RDSMessage = await RDSResponse.Content.ReadAsStringAsync();
                Debug.WriteLine("RDSMessage: " + RDSMessage.ToString());

                // if Sign up is has successfully ie 200 response code
                if (RDSResponse.IsSuccessStatusCode)
                {
                    var RDSData = JsonConvert.DeserializeObject<SignUpResponse>(RDSMessage);
                    Debug.WriteLine("RDSData: " + RDSData.ToString());
                    DateTime today = DateTime.Now;
                    DateTime expDate = today.AddDays(Constant.days);

                    if (RDSData.message.Contains("taken"))
                    {
                        DisplayAlert("Error", "email address is already in use", "OK");
                    }
                    else
                    {
                        // Local Variables in Xamarin that can be used throughout the App
                        Application.Current.Properties["user_id"] = RDSData.result.customer_uid;
                        Application.Current.Properties["time_stamp"] = expDate;
                        Application.Current.Properties["platform"] = "DIRECT";
                        System.Diagnostics.Debug.WriteLine("UserID is:" + (string)Application.Current.Properties["user_id"]);
                        System.Diagnostics.Debug.WriteLine("Time Stamp is:" + Application.Current.Properties["time_stamp"].ToString());
                        System.Diagnostics.Debug.WriteLine("platform is:" + (string)Application.Current.Properties["platform"]);

                        string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                        var request3 = new HttpRequestMessage();
                        request3.RequestUri = new Uri(url);
                        request3.Method = HttpMethod.Get;
                        var client2 = new HttpClient();
                        HttpResponseMessage response = await client2.SendAsync(request3);
                        HttpContent content2 = response.Content;
                        Console.WriteLine("content: " + content2);
                        var userString = await content2.ReadAsStringAsync();
                        JObject info_obj2 = JObject.Parse(userString);
                        this.NewMainPage.Clear();
                        Preferences.Set("profilePicLink", null);
                        // Go to Subscripton page
                        // Application.Current.MainPage = new SubscriptionPage();

                        //send email to verify email
                        emailVerifyPost emailVer = new emailVerifyPost();
                        emailVer.email = emailEntry.Text.Trim();
                        var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                        var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                        var client3 = new HttpClient();
                        var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                        Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                        Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                    }
                }
            }
            else
            {
                await DisplayAlert("Message", "We weren't able to sign you up", "OK");
            }
        }

        public async void googleSignupButtonClicked(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            mainPage.googleLoginButtonClicked(sender, e);
        }

        public async void appleSignupButtonClicked(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            mainPage.appleLoginButtonClicked(sender, e);
        }
        public async void fbSignupButtonClicked(object sender, EventArgs e)
        {
            MainPage mainPage = new MainPage();
            mainPage.facebookLoginButtonClicked(sender, e);
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

        async void continueClicked(System.Object sender, System.EventArgs e)
        {
            if (AddressEntry.Text != null)
            {
                directSignUp.address = AddressEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your address", "OK");
            }

            if (AptEntry.Text != null)
            {
                directSignUp.unit = AptEntry.Text.Trim();
            }

            if (CityEntry.Text != null)
            {
                directSignUp.city = CityEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your city", "OK");
            }

            if (StateEntry.Text != null)
            {
                directSignUp.state = StateEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your state", "OK");
            }

            if (ZipEntry.Text != null)
            {
                directSignUp.zip_code = ZipEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your zipcode", "OK");
            }

            if (PhoneEntry.Text != null && PhoneEntry.Text.Length == 10)
            {
                directSignUp.phone_number = PhoneEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Error", "Please enter your phone number", "OK");
            }

            // Setting request for USPS API
            XDocument requestDoc = new XDocument(
                new XElement("AddressValidateRequest",
                new XAttribute("USERID", "400INFIN1745"),
                new XElement("Revision", "1"),
                new XElement("Address",
                new XAttribute("ID", "0"),
                new XElement("Address1", directSignUp.address),
                new XElement("Address2", directSignUp.unit),
                new XElement("City", directSignUp.city),
                new XElement("State", directSignUp.state),
                new XElement("Zip5", directSignUp.zip_code),
                new XElement("Zip4", "")
                     )
                 )
             );
            var url = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
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
                    if (GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("D") || GetXMLElement(element, "DPVConfirmation").Equals("S"))
                    {
                        Geocoder geoCoder = new Geocoder();

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(directSignUp.address + "," + directSignUp.city + "," + directSignUp.state);
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

                        directSignUp.latitude = latitude;
                        directSignUp.longitude = longitude;

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
                fade.IsVisible = true;
                CheckAddressGrid.IsVisible = true;
                CheckAddressHeading.Text = "Oops!";
                CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your Zip Code yet. Please feel free to leave us your email address and we will let you know as soon as we come to your neighborhood.";
                EmailFrame.IsVisible = true;
                OkayButton.Text = "Okay";
            }
            else
            {
                fade.IsVisible = true;
                CheckAddressGrid.IsVisible = true;
                CheckAddressHeading.Text = "Hooray!";
                CheckAddressBody.Text = "We are so glad that we deliver to your neighborhood. Please click Continue to complete the Sign Up process.";
                EmailFrame.IsVisible = false;
                OkayButton.Text = "Continue";

                isAddessValidated = true;
                
            }
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
    }
}
