using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using MTYD.Model;
using MTYD.Model.Login.LoginClasses;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using MTYD.Constants;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Xamarin.Forms.Maps;
using MTYD.ViewModel;

namespace MTYD.ViewModel
{
    public partial class CongratsPage : ContentPage
    {
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        string cust_firstName; string cust_lastName; string cust_email;
        Zones[] passingZones;
        string firstDeliveryTime;
        string fullAddress;
        public SignUpPost directSignUp = new SignUpPost();
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
        public static string userId = (string)Application.Current.Properties["user_id"];


        public CongratsPage(Zones[] zones, string Fname, string Lname, string email)
        {
            cust_firstName = Fname;
            cust_lastName = Lname;
            cust_email = email;
            passingZones = zones;
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            InitializeComponent();

            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                menu.IsVisible = false;
                innerGrid.IsVisible = false;
            }

            InitializeSignUpPost();

            fullAddress = Preferences.Get(savedAdd, "");
            fullAddress += ", ";
            fullAddress += Preferences.Get(savedApt, "");
            if (fullAddress.Substring(fullAddress.Length - 2) != ", ")
            {
                fullAddress += ", ";
            }
            fullAddress += Preferences.Get(savedCity, "");
            fullAddress += ", ";
            fullAddress += Preferences.Get(savedState, "");
            fullAddress += ", ";
            fullAddress += Preferences.Get(savedZip, "");

            toAddress.Text = fullAddress;
            getDeliveryTime();
            checkPlatform(height, width);
        }

        protected async Task getDeliveryTime()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/lplp_specific/" + userId);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject plan_obj = JObject.Parse(userString);
                this.NewMainPage.Clear();

                firstDeliveryTime = plan_obj["result"][0]["start_delivery_date"].ToString();
                firstDelivery.Text = firstDeliveryTime.Substring(0, firstDeliveryTime.IndexOf(' '));
            }
        }

        private void checkPlatform(double height, double width)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                createAccount.IsVisible = true;
            }

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            orangeBox.HeightRequest = height / 2;
            orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
            orangeBox.CornerRadius = height / 40;
            pfp.HeightRequest = width / 20;
            pfp.WidthRequest = width / 20;
            pfp.CornerRadius = (int)(width / 40);
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

            if (Device.RuntimePlatform == Device.iOS)
            {
                heading.FontSize = width / 32;
                heading.Margin = new Thickness(0, 0, 0, 30);

                congratsTitle.FontSize = width / 25;
                expectTitle.FontSize = width / 30;
                createAccountTitle.FontSize = width / 30;

                expect1.HeightRequest = width / 10;

                googleButton.HeightRequest = width / 13;
                googleButton.WidthRequest = width / 13;
                googleButton.CornerRadius = (int)(width / 26);
                fbButton.HeightRequest = width / 13;
                fbButton.WidthRequest = width / 13;
                fbButton.CornerRadius = (int)(width / 26);
                appleButton.HeightRequest = width / 13;
                appleButton.WidthRequest = width / 13;
                appleButton.CornerRadius = (int)(width / 26);

                passwordFrame.WidthRequest = width / 3;
                passwordFrame.HeightRequest = width / 20;
                confirmPasswordFrame.WidthRequest = width / 3;
                passwordFrame.HeightRequest = width / 20;

                finishButton.WidthRequest = width / 5;
                finishButton.HeightRequest = width / 20;
                finishButton.CornerRadius = (int)(width / 40);

                divider.Margin = new Thickness(width / 15, height / 80, width / 15, height / 100);

                //skipButton.WidthRequest = width / 2;
                skipButton.HeightRequest = width / 20;
                skipButton.CornerRadius = (int)(width / 40);
            }
            else //Android
            {
                heading.FontSize = width / 40;
                heading.Margin = new Thickness(0, 0, 0, 25);

                congratsTitle.FontSize = width / 35;
                expectTitle.FontSize = width / 35;
                createAccountTitle.FontSize = width / 35;

                expect1.HeightRequest = width / 15;

                googleButton.HeightRequest = width / 15;
                googleButton.WidthRequest = width / 15;
                googleButton.CornerRadius = (int)(width / 30);
                fbButton.HeightRequest = width / 15;
                fbButton.WidthRequest = width / 15;
                fbButton.CornerRadius = (int)(width / 30);
                appleButton.HeightRequest = width / 15;
                appleButton.WidthRequest = width / 15;
                appleButton.CornerRadius = (int)(width / 30);

                passwordFrame.WidthRequest = width / 3;
                passwordFrame.HeightRequest = width / 30;
                confirmPasswordFrame.WidthRequest = width / 3;
                passwordFrame.HeightRequest = width / 30;

                finishButton.WidthRequest = width / 5;
                finishButton.HeightRequest = width / 20;
                finishButton.CornerRadius = (int)(width / 40);

                divider.Margin = new Thickness(width / 15, height / 80, width / 15, height / 120);

                skipButton.WidthRequest = width / 2;
                skipButton.HeightRequest = width / 20;
                skipButton.CornerRadius = (int)(width / 40);
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

        public void InitializeSignUpPost()
        {
            directSignUp.email = Preferences.Get(savedEmail, "");
            directSignUp.first_name = Preferences.Get(savedFirstName, "");
            directSignUp.last_name = Preferences.Get(savedLastName, "");
            directSignUp.phone_number = Preferences.Get(savedPhone, "");
            directSignUp.address = Preferences.Get(savedAdd, "");
            directSignUp.unit = Preferences.Get(savedApt, "");
            directSignUp.city = Preferences.Get(savedCity, "");
            directSignUp.state = Preferences.Get(savedState, "");
            directSignUp.zip_code = Preferences.Get(savedZip, "");
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

        public async void skipClicked(object sender, EventArgs e)
        {
            Zones[] passZone = new Zones[0];
            await DisplayAlert("Success", "Your assigned password is \n" + Preferences.Get(savedFirstName, "") + Preferences.Get(savedAdd, "").Substring(0, Preferences.Get(savedAdd, "").IndexOf(" ")), "continue");
            await Navigation.PushAsync(new Select(passZone, cust_firstName, cust_lastName, cust_email));
        }

        public async void finishClicked(object sender, EventArgs e)
        {
            if (passwordEntry.Text != null && passwordEntry.Text == confirmPasswordEntry.Text)
            {
                PasswordInfo passwordUpdate = new PasswordInfo();
                passwordUpdate.customer_uid = (string)Application.Current.Properties["user_id"];
                //passwordUpdate.old_password = Preferences.Get("hashed_password", "");
                passwordUpdate.old_password = Preferences.Get(savedFirstName, "") + Preferences.Get(savedAdd, "").Substring(0, Preferences.Get(savedAdd, "").IndexOf(" "));
                //passwordUpdate.new_password = hashedPassword;
                passwordUpdate.new_password = passwordEntry.Text;
                var newPaymentJSONString = JsonConvert.SerializeObject(passwordUpdate);
                // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
                var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("Content: " + content2);
                var client = new HttpClient();
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_password", content2);
                await DisplayAlert("Success", "password updated!", "continue");
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedSave Func ENDED!");
                Zones[] passZone = new Zones[0];
                await Navigation.PushAsync(new Select(passZone, cust_firstName, cust_lastName, cust_email));
            }
            else if (passwordEntry.Text != null && passwordEntry.Text != confirmPasswordEntry.Text)
            {
                await DisplayAlert("Error", "Your password doesn't match", "OK");
                return;
            }
            else
            {
                await DisplayAlert("Error", "Please enter a password or click SKIP.", "OK");
                return;
                //Zones[] passZone = new Zones[0];
                //await Navigation.PushAsync(new Select(passZone, cust_firstName, cust_lastName, cust_email));
            }
            //if (passwordEntry.Text != null)
            //{
            //    directSignUp.password = passwordEntry.Text.Trim();
            //}
            //else
            //{
            //    await DisplayAlert("Error", "Please enter a password", "OK");
            //    return;
            //}
            //if (confirmPasswordEntry.Text != null)
            //{
            //    string password = confirmPasswordEntry.Text.Trim();
            //    if (!directSignUp.password.Equals(password))
            //    {
            //        await DisplayAlert("Error", "Your password doesn't match", "OK");
            //        return;
            //    }
            //}
            //else
            //{
            //    await DisplayAlert("Error", "Your password doesn't match", "OK");
            //    return;
            //}

            //// Setting request for USPS API
            //XDocument requestDoc = new XDocument(
            //    new XElement("AddressValidateRequest",
            //    new XAttribute("USERID", "400INFIN1745"),
            //    new XElement("Revision", "1"),
            //    new XElement("Address",
            //    new XAttribute("ID", "0"),
            //    new XElement("Address1", directSignUp.address),
            //    new XElement("Address2", directSignUp.unit),
            //    new XElement("City", directSignUp.city),
            //    new XElement("State", directSignUp.state),
            //    new XElement("Zip5", directSignUp.zip_code),
            //    new XElement("Zip4", "")
            //         )
            //     )
            // );
            //var url = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
            //Console.WriteLine(url);
            //var client = new WebClient();
            //var response = client.DownloadString(url);

            //var xdoc = XDocument.Parse(response.ToString());
            //Console.WriteLine(xdoc);
            //string latitude = "0";
            //string longitude = "0";
            //foreach (XElement element in xdoc.Descendants("Address"))
            //{
            //    if (GetXMLElement(element, "Error").Equals(""))
            //    {
            //        if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(directSignUp.zip_code) && GetXMLElement(element, "City").Equals(directSignUp.city.ToUpper())) // Best case
            //        {
            //            // Get longitude and latitide because we can make a deliver here. Move on to next page.
            //            // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
            //            //GetAddressLatitudeLongitude();
            //            Geocoder geoCoder = new Geocoder();

            //            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(directSignUp.address + "," + directSignUp.city + "," + directSignUp.state);
            //            Position position = approximateLocations.FirstOrDefault();

            //            latitude = $"{position.Latitude}";
            //            longitude = $"{position.Longitude}";

            //            directSignUp.latitude = latitude;
            //            directSignUp.longitude = longitude;
            //            //map.MapType = MapType.Street;
            //            //var mapSpan = new MapSpan(position, 0.001, 0.001);

            //            //Pin address = new Pin();
            //            //address.Label = "Delivery Address";
            //            //address.Type = PinType.SearchResult;
            //            //address.Position = position;

            //            //map.MoveToRegion(mapSpan);
            //            //map.Pins.Add(address);

            //            break;
            //        }
            //        else if (GetXMLElement(element, "DPVConfirmation").Equals("D"))
            //        {
            //            //await DisplayAlert("Alert!", "Address is missing information like 'Apartment number'.", "Ok");
            //            //return;
            //        }
            //        else
            //        {
            //            //await DisplayAlert("Alert!", "Seems like your address is invalid.", "Ok");
            //            //return;
            //        }
            //    }
            //    else
            //    {   // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
            //        //Console.WriteLine("Seems like your address is invalid.");
            //        //await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
            //        //return;
            //    }
            //}
            //if (latitude == "0" || longitude == "0")
            //{
            //    await DisplayAlert("We couldn't find your address", "Please check for errors.", "Ok");
            //}
            //else
            //{
            //    //await Application.Current.SavePropertiesAsync();
            //    //await tagUser(Preferences.Get(savedEmail, ""), Preferences.Get(savedZip, ""));
            //    //SignUpNewUser(sender, e);
            //}
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
                    DisplayAlert("Error", "email address is already in use, please log in", "OK");
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
                    emailVer.email = Preferences.Get(savedEmail, "");
                    var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                    var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                    var client3 = new HttpClient();
                    var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                    Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                    Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                    Console.WriteLine("\nUser Signed Up:" +
                        "\n  email: " + directSignUp.email +
                        "\n  first name: " + directSignUp.first_name +
                        "\n  last name: " + directSignUp.last_name +
                        "\n  phone number: " + directSignUp.phone_number +
                        "\n  address: " + directSignUp.address +
                        "\n  unit: " + directSignUp.unit +
                        "\n  city: " + directSignUp.city +
                        "\n  state: " + directSignUp.state +
                        "\n  zip: " + directSignUp.zip_code +
                        "\n  latitude: " + directSignUp.latitude +
                        "\n  longitude: " + directSignUp.longitude +
                        "\n  referral source: " + directSignUp.referral_source +
                        "\n  role: " + directSignUp.role +
                        "\n  mobile access token: " + directSignUp.mobile_access_token +
                        "\n  mobile refresh token: " + directSignUp.mobile_refresh_token +
                        "\n  user access token: " + directSignUp.user_access_token +
                        "\n  user refresh token: " + directSignUp.user_refresh_token +
                        "\n  social: " + directSignUp.social +
                        "\n  password: " + directSignUp.password +
                        "\n  social id: " + directSignUp.social_id
                        );

                    Application.Current.MainPage = new NavigationPage(new Select(passingZones, cust_firstName, cust_lastName, cust_email));
                }
            }
        }
    }
}
