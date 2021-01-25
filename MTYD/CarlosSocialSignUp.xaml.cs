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

        public CarlosSocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
        {
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
                heading.FontSize = width / 25;
                heading.Margin = new Thickness(20, 100, 0, 0);

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

        async void ValidatingAddressClick(System.Object sender, System.EventArgs e)
        {

            if (PhoneEntry.Text != null && PhoneEntry.Text.Length == 10)
            {
                socialSignUp.phone_number = PhoneEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Alert!", "Please enter a 10 valid phone number", "OK");
            }

            if (AddressEntry.Text != null)
            {
                socialSignUp.address = AddressEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Alert!", "Please enter your address", "OK");
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
                await DisplayAlert("Alert!", "Please enter your city", "OK");
            }

            if (StateEntry.Text != null)
            {
                socialSignUp.state = StateEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Alert!", "Please enter your state", "OK");
            }

            if (ZipEntry.Text != null && ZipEntry.Text.Length == 5)
            {
                socialSignUp.zip_code = ZipEntry.Text.Trim();
            }
            else
            {
                await DisplayAlert("Alert!", "Please enter a 5 digit zipcode", "OK");
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
            var url = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
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
                    if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())) // Best case
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
                        // await DisplayAlert("Alert!", "Address is missing information like 'Apartment number'.", "Ok");
                        // return;
                    }
                    else
                    {
                        // await DisplayAlert("Alert!", "Seems like your address is invalid.", "Ok");
                        // return;
                    }
                }
                else
                {   // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
                    // Console.WriteLine("Seems like your address is invalid.");
                    // await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
                    // return;
                }
            }

            if (!isAddressValidated)
            {
                await DisplayAlert("Message", "We were not able to validate your address. Please try again.", "OK");
            }
            else
            {
                await DisplayAlert("Message", "We validated your address please tap on the Sign up button to create your account!", "OK");
            }
        }

        async void SignUpSocialUserClick(System.Object sender, System.EventArgs e)
        {
            if (isAddressValidated)
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
                await DisplayAlert("Message", "We weren't able to sign you up", "OK");
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
