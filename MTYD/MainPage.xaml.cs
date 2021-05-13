using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

using MTYD.Model.Login;
using System.Threading.Tasks;
using MTYD.Model.Database;
using System.Windows.Input;
using Xamarin.Auth;
using System.Diagnostics;
using System.Collections;
using MTYD.ViewModel;
using MTYD.Model.User;
using System.IO;

using System.Collections.Generic;
using System.ComponentModel;
using MTYD.Model.Login.LoginClasses.Apple;
using MTYD.Model.Login.LoginClasses;
using MTYD.Constants;
using MTYD.LogInClasses;
using Newtonsoft.Json.Linq;
using MTYD.Model;
using System.Collections.ObjectModel;
using MTYD.Interfaces;
using System.Xml.Linq;
using Xamarin.Forms.Maps;
using Plugin.LatestVersion;
using Xamarin.Forms.PlatformConfiguration;

//testing
namespace MTYD
{
    //keys in Preferences
    //canChooseSelect - boolean - used to know if the user can navigate to the select page from the menu
    //mainPageAdd - string - used to autofill the address section on paymentpage if going through as guest
    //mainPageCity - string - used to autofill the address section on paymentpage if going through as guest
    //mainPageState - string - used to autofill the address section on paymentpage if going through as guest
    //mainPageZip - string - used to autofill the address section on paymentpage if going through as guest


    public partial class MainPage : ContentPage
    {
        public HttpClient client = new HttpClient();
        public event EventHandler SignIn;
        public bool createAccount = false;
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        LoginViewModel vm = new LoginViewModel();
        //-1 if the email_verified section is not found in the returned message, 0 if false, 1 if true
        int directEmailVerified = 0;
        string deviceId;
        List<string> menuNames;
        List<string> menuImages;
        string cust_firstName; string cust_lastName; string cust_email;
        WebClient client4 = new WebClient();
        Account account;
        [Obsolete]
        AccountStore store;
        Address addr;
        bool withinZones = false;
        Zones[] passingZones;

        public MainPage()
        {
            try
            {

                //_ = CheckVersion();

                Preferences.Set("mainPageAdd", "");
                Preferences.Set("mainPageCity", "");
                Preferences.Set("mainPageState", "");
                Preferences.Set("mainPageZip", "");

                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                Console.WriteLine("Width = " + width.ToString());
                Console.WriteLine("Height = " + height.ToString());
                //DisplayAlert("Hello", "main page constructor called", "OK");
                addr = new Address();
                InitializeComponent();
                BindingContext = this;

                string version = "";
                string build = "";
                version = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
                build = DependencyService.Get<IAppVersionAndBuild>().GetBuildNumber();

                appVersion.Text = "App version: " + version + ", App build: " + build;

                var cvItems = new List<string>
            {
                "Who has time?\n\nSave time and money! Ready to heat meals come to your door and you can order up to 10 deliveries in advance so you know what's coming!",
                "Food when you're hungry\n\nIf you order food when you're hungry, you're starving by the time it arrives! With MealsFor.Me there is always something in the fridge and your next meals are in route!",
                "Better Value\n\nYou get restaurant quality food at a fraction of the cost plus it is made from the highest quality ingredients by exceptional Chefs."
            };
                TheCarousel.ItemsSource = cvItems;

                store = AccountStore.Create();

                HomePage.IsVisible = true;
                MenuBar.IsVisible = false;
                LoginPage.IsVisible = false;
                LandingPage.IsVisible = false;
                checkPlatform(height, width);
                //_ = CheckVersion();

                //in check version
                setGrid();
                // BackgroundImageSource = "new_background.png";

                // APPLE
                //var vm = new LoginViewModel();
                vm.AppleError += AppleError;
                //vm.PlatformError += PlatformError;
                //BindingContext = vm;

                if (Device.RuntimePlatform == Device.Android)
                {
                    appleLoginButton.IsEnabled = false;
                }
                //in check version

                //    setGrid();
                //    // BackgroundImageSource = "new_background.png";

                //    // APPLE
                //    //var vm = new LoginViewModel();
                //    vm.AppleError += AppleError;
                //    //vm.PlatformError += PlatformError;
                //    //BindingContext = vm;

                //    if (Device.RuntimePlatform == Device.Android)
                //    {
                //        appleLoginButton.IsEnabled = false;
                //    }

                //Application.Current.Properties["user_id"] = "000-00000";
                //Application.Current.Properties["platform"] = "GUEST";
                //Preferences.Set("user_latitude", "0.0");
                //Preferences.Set("user_longitude", "0.0");
                //Application.Current.SavePropertiesAsync();
                //double marg = HomePage.Height - 100;
                //Debug.WriteLine("marg: " + marg.ToString());
                //CheckAddressGrid.Margin = new Thickness(50, marg, 50, 120);
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public MainPage(string firstName, string lastName, string email)
        {
            try
            {

                //On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);


                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                Console.WriteLine("Width = " + width.ToString());
                Console.WriteLine("Height = " + height.ToString());
                cust_firstName = firstName;
                cust_lastName = lastName;
                cust_email = email;
                InitializeComponent();

                getDiscounts();
                var cvItems = new List<string>
            {
                "Who has time?\n\nSave time and money! Ready to heat meals come to your door and you can order up to 10 deliveries in advance so you know what's coming!",
                "Food when you're hungry\n\nIf you order food when you're hungry, you're starving by the time it arrives! With MealsFor.Me there is always something in the fridge and your next meals are in route!",
                "Better Value\n\nYou get restaurant quality food at a fraction of the cost plus it is made from the highest quality ingredients by exceptional Chefs."
            };
                TheCarousel.ItemsSource = cvItems;

                HomePage.IsVisible = false;
                MenuBar.IsVisible = true;
                LoginPage.IsVisible = false;
                LandingPage.IsVisible = true;
                checkLandingPlatform(height, width);
                setGrid();

            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void PlatformError(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("platform"))
            {
                string platform = (string)Application.Current.Properties["platform"];
                await DisplayAlert("Alert!", "Our records show that you have an account associated with " + platform + ". Please log in with " + platform, "OK");
            }

        }

        private async void AppleError(object sender, EventArgs e)
        {
            await DisplayAlert("Error", "We weren't able to set an account for you", "OK");
        }

        private void checkPlatform(double height, double width)
        {
            orangeBox.HeightRequest = height / 2;
            orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
            orangeBox.CornerRadius = height / 40;
            heading.WidthRequest = 140;
            pfp.HeightRequest = 40;
            pfp.WidthRequest = 40;
            pfp.CornerRadius = 20;
            //pfp.Margin = new Thickness(0, 0, 23, 27);
            innerGrid.Margin = new Thickness(0, 0, 23, 27);
            fade.Margin = new Thickness(0, -height / 3, 0, 0);

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

            //menu.HeightRequest = width / 23;
            menu.WidthRequest = 40;
            //menu.WidthRequest = 40; = width / 23;
            menu2.WidthRequest = 40;
            menu.Margin = new Thickness(25, 0, 0, 30);

            if (width == 1125 && height == 2436) //iPhone X only
            {
                Console.WriteLine("entered for iPhone X");

                //social media buttons
                //googleLoginButton.HeightRequest = width / 17;
                //googleLoginButton.WidthRequest = width / 17;
                //googleLoginButton.CornerRadius = (int)(width / 34);
                //facebookLoginButton.HeightRequest = width / 17;
                //facebookLoginButton.WidthRequest = width / 17;
                //facebookLoginButton.CornerRadius = (int)(width / 34);
                //appleLoginButton.HeightRequest = width / 17;
                //appleLoginButton.WidthRequest = width / 17;
                //appleLoginButton.CornerRadius = (int)(width / 34);
                googleLoginButton.HeightRequest = 42;
                googleLoginButton.WidthRequest = 42;
                googleLoginButton.CornerRadius = 21;
                facebookLoginButton.HeightRequest = 42;
                facebookLoginButton.WidthRequest = 42;
                facebookLoginButton.CornerRadius = 21;
                appleLoginButton.HeightRequest = 42;
                appleLoginButton.WidthRequest = 42;
                appleLoginButton.CornerRadius = 21;
                directLoginButton.HeightRequest = 42;
                directLoginButton.WidthRequest = 42;
                directLoginButton.CornerRadius = 21;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                //googleLoginButton.HeightRequest = width / 17;
                //googleLoginButton.WidthRequest = width / 17;
                //googleLoginButton.CornerRadius = (int)(width / 34);
                //facebookLoginButton.HeightRequest = width / 17;
                //facebookLoginButton.WidthRequest = width / 17;
                //facebookLoginButton.CornerRadius = (int)(width / 34);
                //directLoginButton.HeightRequest = width / 17;
                //directLoginButton.WidthRequest = width / 17;
                //directLoginButton.CornerRadius = (int)(width / 34);
                googleLoginButton.HeightRequest = 42;
                googleLoginButton.WidthRequest = 42;
                googleLoginButton.CornerRadius = 21;
                facebookLoginButton.HeightRequest = 42;
                facebookLoginButton.WidthRequest = 42;
                facebookLoginButton.CornerRadius = 21;
                appleLoginButton.HeightRequest = 42;
                appleLoginButton.WidthRequest = 42;
                appleLoginButton.CornerRadius = 21;
                directLoginButton.HeightRequest = 42;
                directLoginButton.WidthRequest = 42;
                directLoginButton.CornerRadius = 21;
            }
            else //android
            {
                googleLoginButton.HeightRequest = 42;
                googleLoginButton.WidthRequest = 42;
                googleLoginButton.CornerRadius = 21;
                facebookLoginButton.HeightRequest = 42;
                facebookLoginButton.WidthRequest = 42;
                facebookLoginButton.CornerRadius = 21;
                appleLoginButton.HeightRequest = 42;
                appleLoginButton.WidthRequest = 42;
                appleLoginButton.CornerRadius = 21;
                directLoginButton.HeightRequest = 42;
                directLoginButton.WidthRequest = 42;
                directLoginButton.CornerRadius = 21;
            }
        }

        protected async Task setGrid()
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu");
                request.Method = HttpMethod.Get;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var userString = await content.ReadAsStringAsync();
                    JObject plan_obj = JObject.Parse(userString);

                    menuNames = new List<string>();
                    menuImages = new List<string>();
                    HashSet<string> dates = new HashSet<string>();
                    foreach (var m in plan_obj["result"])
                    {
                        dates.Add(m["menu_date"].ToString());
                        Debug.WriteLine("dates menu_date: " + m["menu_date"].ToString());
                        if (dates.Count > 2) break;
                        if (m["meal_cat"].ToString() == "Add-On")
                            continue;
                        menuNames.Add(m["meal_name"].ToString());
                        menuImages.Add(m["meal_photo_URL"].ToString());
                    }
                }

                upcomingMenuGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                for (int i = 0; i < menuNames.Count; i++)
                {
                    upcomingMenuGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140, GridUnitType.Absolute) });
                }
                for (int i = 0; i < menuNames.Count; i++)
                {
                    upcomingMenuGrid.Children.Add(new ImageButton
                    {
                        Source = menuImages[i],
                        IsEnabled = false,
                        Aspect = Aspect.AspectFill,
                        CornerRadius = 0
                    }, i, 0);
                    upcomingMenuGrid.Children.Add(new Frame
                    {
                        Content = new Label
                        {
                            Text = menuNames[i],
                            TextColor = Color.Black,
                            FontSize = 12,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center
                        },
                        BorderColor = Color.FromHex("#F26522"),
                        HasShadow = false,
                        Padding = 5,
                        CornerRadius = 0
                    }, i, 1);
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private void checkLandingPlatform(double height, double width)
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            orangeBox.HeightRequest = height / 2;
            orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
            orangeBox.CornerRadius = height / 40;
            innerGrid.Margin = new Thickness(0, 0, 23, 27);
            heading.WidthRequest = 140;

            menu.Margin = new Thickness(25, 0, 0, 30);
            menu.WidthRequest = 40;
            //menu.Margin = new Thickness(25, 0, 0, 30);

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

            //landingPic.Margin = new Thickness(0, -height / 30, 0, 0);

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
                menu.WidthRequest = 40;
                //heading adjustments

                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;

                //mainLogo.HeightRequest = height / 18;
                //mainLogo.WidthRequest = 250;
                //getStarted.HeightRequest = height / 35;
                //getStarted.WidthRequest = 200;
                //getStarted.CornerRadius = (int)(height / 70);
                getStarted.HeightRequest = 50;
                getStarted.CornerRadius = 25;
                getStarted.Margin = new Thickness(80, 0);

                fade.Margin = new Thickness(0, -height / 3, 0, 0);

                //xButton.FontSize = width / 37;
                //DiscountGrid.Margin = new Thickness(width / 30, height / 9, width / 30, 0);
                //DiscountGrid.HeightRequest = height / 4.5;
                //DiscountGrid.WidthRequest = width / 2.3;
                //couponGrid.HeightRequest = height / 4.5;
                //couponGrid.WidthRequest = width / 2.3;

                //outerFrame.HeightRequest = height / 4.5;

                //discHeader.FontSize = width / 35;

                //couponGrid.HeightRequest = height / 30;
                //couponImg.HeightRequest = height / 30;
                //couponAmt.FontSize = width / 50;
                //couponDesc.FontSize = width / 50;

                //couponGrid2.HeightRequest = height / 30;
                //couponImg2.HeightRequest = height / 30;
                //couponAmt2.FontSize = width / 50;
                //couponDesc2.FontSize = width / 50;

                //couponGrid3.HeightRequest = height / 30;
                //couponImg3.HeightRequest = height / 30;
                //couponAmt3.FontSize = width / 50;
                //couponDesc3.FontSize = width / 50;
            }
            else //Android
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

                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;

                //mainLogo.HeightRequest = height / 20;
                getStarted.HeightRequest = height / 45;
                getStarted.CornerRadius = (int)(height / 90);
                getStarted.Margin = new Thickness(width / 10, 0);

                fade.Margin = new Thickness(0, -height / 3, 0, 0);

                //xButton.WidthRequest = width / 40;
                //xButton.FontSize = width / 50;
                //DiscountGrid.Margin = new Thickness(width / 30, height / 9, width / 30, 0);
                //DiscountGrid.HeightRequest = height / 6.5;
                //DiscountGrid.WidthRequest = width / 2.3;

                //outerFrame.HeightRequest = height / 6.5;

                //discHeader.FontSize = width / 50;

                //couponGrid.HeightRequest = height / 30;
                //couponImg.HeightRequest = height / 40;
                //couponAmt.FontSize = width / 60;
                //couponDesc.FontSize = width / 60;

                //couponGrid2.HeightRequest = height / 30;
                //couponImg2.HeightRequest = height / 40;
                //couponAmt2.FontSize = width / 60;
                //couponDesc2.FontSize = width / 60;

                //couponGrid3.HeightRequest = height / 30;
                //couponImg3.HeightRequest = height / 40;
                //couponAmt3.FontSize = width / 60;
                //couponDesc3.FontSize = width / 60;
            }
        }

        async void clickedWeeksMeals(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ThisWeeksMeals();
        }

        // DIRECT LOGIN CLICK
        private async void clickedLogin(object sender, EventArgs e)
        {
            try
            {
                loginButton.IsEnabled = false;
                if (String.IsNullOrEmpty(loginUsername.Text) || String.IsNullOrEmpty(loginPassword.Text))
                { // check if all fields are filled out
                    await DisplayAlert("Error", "Please fill in all fields", "OK");
                    loginButton.IsEnabled = true;
                }
                else
                {
                    var accountSalt = await retrieveAccountSalt(loginUsername.Text.ToLower().Trim());

                    if (accountSalt != null)
                    {
                        var loginAttempt = await LogInUser(loginUsername.Text.ToLower(), loginPassword.Text, accountSalt);

                        if (directEmailVerified == 0)
                        {
                            DisplayAlert("Please Verify Email", "Please click the link in the email sent to " + loginUsername.Text + ". Check inbox and spam folders.", "OK");

                            //send email to verify email
                            emailVerifyPost emailVer = new emailVerifyPost();
                            emailVer.email = loginUsername.Text.Trim();
                            var emailVerSerializedObj = JsonConvert.SerializeObject(emailVer);
                            var content4 = new StringContent(emailVerSerializedObj, Encoding.UTF8, "application/json");
                            var client3 = new HttpClient();
                            var response3 = client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/email_verification", content4);
                            Console.WriteLine("RESPONSE TO CHECKOUT   " + response3.Result);
                            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + emailVerSerializedObj);

                            loginButton.IsEnabled = true;
                        }
                        else if (loginAttempt != null && loginAttempt.message != "Request failed, wrong password.")
                        {
                            System.Diagnostics.Debug.WriteLine("USER'S DATA");
                            System.Diagnostics.Debug.WriteLine("USER CUSTOMER_UID: " + loginAttempt.result[0].customer_uid);
                            System.Diagnostics.Debug.WriteLine("USER FIRST NAME: " + loginAttempt.result[0].customer_first_name);
                            System.Diagnostics.Debug.WriteLine("USER LAST NAME: " + loginAttempt.result[0].customer_last_name);
                            System.Diagnostics.Debug.WriteLine("USER EMAIL: " + loginAttempt.result[0].customer_email);

                            DateTime today = DateTime.Now;
                            DateTime expDate = today.AddDays(Constant.days);

                            Application.Current.Properties["user_id"] = loginAttempt.result[0].customer_uid;
                            Application.Current.Properties["time_stamp"] = expDate;
                            Application.Current.Properties["platform"] = "DIRECT";

                            // Application.Current.MainPage = new CarlosHomePage();
                            // This statement initializes the stack to Subscription Page
                            //check to see if user has already selected a meal plan before
                            var request = new HttpRequestMessage();
                            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //old db
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                            Console.WriteLine("url: " + url);
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
                                Console.WriteLine(userString);

                                //writing guid to db
                                if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                                {
                                    if (Device.RuntimePlatform == Device.iOS)
                                    {
                                        deviceId = Preferences.Get("guid", null);
                                        if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                                    }
                                    else
                                    {
                                        deviceId = Preferences.Get("guid", null);
                                        if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                                    }

                                    if (deviceId != null)
                                    {
                                        GuidPost notificationPost = new GuidPost();

                                        notificationPost.uid = (string)Application.Current.Properties["user_id"];
                                        notificationPost.guid = deviceId.Substring(5);
                                        Application.Current.Properties["guid"] = deviceId.Substring(5);
                                        notificationPost.notification = "TRUE";

                                        var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                        Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                        var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                        var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                                        Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                        if (clientResponse.IsSuccessStatusCode)
                                        {
                                            System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                                            Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                                        }
                                        else
                                        {
                                            await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                        }
                                    }
                                }
                                //written

                                if (userString.ToString()[0] != '{')
                                {
                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    //old db
                                    //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    response = await client.SendAsync(request3);
                                    content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("password_salt", (info_obj3["result"])[0]["password_salt"].ToString());
                                    Preferences.Set("password_hashed", (info_obj3["result"])[0]["password_hashed"].ToString());

                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");

                                    Console.WriteLine("go to SubscriptionPage");
                                    Preferences.Set("canChooseSelect", false);

                                    Debug.WriteLine("email verified:" + (info_obj3["result"])[0]["email_verified"].ToString());
                                    await Application.Current.SavePropertiesAsync();
                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email));
                                    directEmailVerified = 0;
                                    return;
                                }

                                JObject info_obj2 = JObject.Parse(userString);
                                this.NewMainPage.Clear();

                                //ArrayList item_price = new ArrayList();
                                //ArrayList num_items = new ArrayList();
                                //ArrayList payment_frequency = new ArrayList();
                                //ArrayList groupArray = new ArrayList();

                                //int counter = 0;
                                //while (((info_obj2["result"])[0]).ToString() != "{}")
                                //{
                                //    Console.WriteLine("worked" + counter);
                                //    counter++;
                                //}

                                Console.WriteLine("string: " + (info_obj2["result"]).ToString());
                                //check if the user hasn't entered any info before, if so put in the placeholders
                                if ((info_obj2["result"]).ToString() == "[]" || (info_obj2["result"]).ToString() == "204" || (info_obj2["result"]).ToString().Contains("ACTIVE") == false)
                                {

                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                    //old db
                                    //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    response = await client.SendAsync(request3);
                                    content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("password_salt", (info_obj3["result"])[0]["password_salt"].ToString());
                                    Preferences.Set("password_hashed", (info_obj3["result"])[0]["password_hashed"].ToString());

                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");

                                    Console.WriteLine("go to SubscriptionPage");
                                    Preferences.Set("canChooseSelect", false);
                                    await Application.Current.SavePropertiesAsync();
                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email));
                                    directEmailVerified = 0;
                                }
                                else
                                {
                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                    //old db
                                    //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    response = await client.SendAsync(request3);
                                    content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("password_salt", (info_obj3["result"])[0]["password_salt"].ToString());
                                    Preferences.Set("password_hashed", (info_obj3["result"])[0]["password_hashed"].ToString());

                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");
                                    //Application.Current.MainPage = new NavigationPage(new SubscriptionPage(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email));
                                    //TEMPORARY
                                    Preferences.Set("canChooseSelect", true);
                                    Zones[] zones = new Zones[] { };
                                    await Application.Current.SavePropertiesAsync();
                                    Application.Current.MainPage = new NavigationPage(new Select(zones, loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email));
                                    directEmailVerified = 0;
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Error", "Wrong password was entered", "OK");
                            loginButton.IsEnabled = true;
                        }
                    }
                    loginButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async Task<AccountSalt> retrieveAccountSalt(string userEmail)
        {
            try {
                System.Diagnostics.Debug.WriteLine(userEmail);

                SaltPost saltPost = new SaltPost();
                saltPost.email = userEmail;

                var saltPostSerilizedObject = JsonConvert.SerializeObject(saltPost);
                var saltPostContent = new StringContent(saltPostSerilizedObject, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(saltPostSerilizedObject);

                var client = new HttpClient();
                var DRSResponse = await client.PostAsync(Constant.AccountSaltUrl, saltPostContent);
                var DRSMessage = await DRSResponse.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(DRSMessage);

                AccountSalt userInformation = null;

                if (DRSResponse.IsSuccessStatusCode)
                {
                    var result = await DRSResponse.Content.ReadAsStringAsync();

                    AcountSaltCredentials data = new AcountSaltCredentials();
                    data = JsonConvert.DeserializeObject<AcountSaltCredentials>(result);

                    if (DRSMessage.Contains(Constant.UseSocialMediaLogin))
                    {
                        Debug.WriteLine("check if social login already exists for this email");
                        createAccount = true;
                        System.Diagnostics.Debug.WriteLine(DRSMessage);
                        await DisplayAlert("Oops!", data.message, "OK");
                    }
                    else if (DRSMessage.Contains(Constant.EmailNotFound))
                    {
                        await DisplayAlert("Oops!", "Our records show that you don't have an accout. Please sign up!", "OK");
                    }
                    else
                    {
                        userInformation = new AccountSalt
                        {
                            password_algorithm = data.result[0].password_algorithm,
                            password_salt = data.result[0].password_salt
                        };
                    }
                }

                return userInformation;
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
                return null;
            }
        }



        public async Task<LogInResponse> LogInUser(string userEmail, string userPassword, AccountSalt accountSalt)
        {
            try
            {
                SHA512 sHA512 = new SHA512Managed();
                byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(userPassword + accountSalt.password_salt)); // take the password and account salt to generate hash
                string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex

                LogInPost loginPostContent = new LogInPost();
                loginPostContent.email = userEmail;
                loginPostContent.password = hashedPassword;
                loginPostContent.social_id = "";
                loginPostContent.signup_platform = "";
                Preferences.Set("hashed_password", hashedPassword);
                Preferences.Set("user_password", userPassword);
                Console.WriteLine("accountSalt: " + accountSalt.password_salt);
                Console.WriteLine("userPassword: " + userPassword);

                string loginPostContentJson = JsonConvert.SerializeObject(loginPostContent); // make orderContent into json

                var httpContent = new StringContent(loginPostContentJson, Encoding.UTF8, "application/json"); // encode orderContentJson into format to send to database
                var response = await client.PostAsync(Constant.LogInUrl, httpContent); // try to post to database
                var message = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("LogInUser message: " + message);
                string emailVerify = message.Substring(message.IndexOf("email_verified") + 18, 1);
                Debug.WriteLine("emailVerify: " + emailVerify);
                if (emailVerify == "1")
                    directEmailVerified = 1;
                if (message.IndexOf("email_verified") == -1)
                    directEmailVerified = -1;


                if (message.Contains(Constant.AutheticatedSuccesful))
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LogInResponse>(responseContent);
                    return loginResponse;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
                return null;
            }
        }


        // FACEBOOK LOGIN CLICK
        public async void facebookLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Initialize variables
                string clientID = string.Empty;
                string redirectURL = string.Empty;

                switch (Device.RuntimePlatform)
                {
                    // depending on the device, get constants from Login>Constants>Constants.cs file
                    case Device.iOS:
                        clientID = Constant.FacebookiOSClientID;
                        redirectURL = Constant.FacebookiOSRedirectUrl;
                        break;
                    case Device.Android:
                        clientID = Constant.FacebookAndroidClientID;
                        redirectURL = Constant.FacebookAndroidRedirectUrl;
                        break;
                }

                // Store all the information in a variable called authenticator (for client) and presenter for http client (who is going to present the credentials)
                var authenticator = new OAuth2Authenticator(clientID, Constant.FacebookScope, new Uri(Constant.FacebookAuthorizeUrl), new Uri(redirectURL), null, false);
                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

                // Creates Completed and Error Event Handler functions;  "+=" means create
                authenticator.Completed += FacebookAuthenticatorCompleted;
                authenticator.Error += FacebookAutheticatorError;


                // This is the actual call to Facebook
                presenter.Login(authenticator);
                // Facebooks sends back an authenticator that goes directly into the Event Handlers created above as "sender".  Data is stored in arguement "e" (account, user name, access token, etc).
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }




        // sender contains nothing then there is an error.  sender contains an authenticator from Facebook
        public async void FacebookAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                var authenticator = sender as OAuth2Authenticator;
                Console.WriteLine("authenticator" + authenticator.ToString());
                if (authenticator != null)
                {
                    // Removes Event Handler functions;  "-=" means delete
                    authenticator.Completed -= FacebookAuthenticatorCompleted;
                    authenticator.Error -= FacebookAutheticatorError;
                }

                if (e.IsAuthenticated)
                {
                    // Uses access token from Facebook as an input to FacebookUserProfileAsync
                    FacebookUserProfileAsync(e.Account.Properties["access_token"]);
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async void FacebookUserProfileAsync(string accessToken)
        {
            try
            {
                //email might not be found so loading page can't be here forever
                //testing with loading page FB
                Application.Current.MainPage = new Loading();
                var client = new HttpClient();

                var socialLogInPost = new SocialLogInPost();

                // Actual call to Facebooks end point now that we have the token (appending accessToken to URL in constants file)
                var facebookResponse = client.GetStringAsync(Constant.FacebookUserInfoUrl + accessToken);  // makes the call to Facebook and returns True/False
                var userData = facebookResponse.Result;  // returns Facebook email and social ID

                System.Diagnostics.Debug.WriteLine(facebookResponse);
                System.Diagnostics.Debug.WriteLine(userData);


                // Deserializes JSON object from info provided by Facebook
                FacebookResponse facebookData = JsonConvert.DeserializeObject<FacebookResponse>(userData);
                socialLogInPost.email = facebookData.email;
                socialLogInPost.password = "";
                socialLogInPost.social_id = facebookData.id;
                socialLogInPost.signup_platform = "FACEBOOK";

                // Create JSON object for Login Endpoint
                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

                // Call to RDS database with endpoint and JSON data
                var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);  //  True or False if Parva's endpoint ran preperly.
                var responseContent = await RDSResponse.Content.ReadAsStringAsync();  // Contains Parva's code containing all the user data including userid

                System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);  // Response code is Yes/True if successful from httpclient system.net package
                System.Diagnostics.Debug.WriteLine(responseContent);  // Response JSON that RDS returns

                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        var data5 = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        // Do I don't have the email in RDS
                        if (data5.code.ToString() == Constant.EmailNotFound)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();


                            var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a MTYD account. Please sign up!", "OK", "Cancel");
                            if (signUp)
                            {
                                // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH MTYD SOCIAL SIGN UP
                                // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                                // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                                Preferences.Set("canChooseSelect", false);
                                Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                Application.Current.MainPage = new CarlosSocialSignUp(facebookData.id, facebookData.name, "", facebookData.email, accessToken, accessToken, "FACEBOOK");
                                // need to write new statment here ...
                            }
                        }


                        // if Response content contains 200
                        else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                        {
                            var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            Application.Current.Properties["user_id"] = data.result[0].customer_uid;  // converts RDS data into appication data.

                            UpdateTokensPost updateTokensPost = new UpdateTokensPost();
                            updateTokensPost.uid = data.result[0].customer_uid;
                            updateTokensPost.mobile_access_token = accessToken;
                            updateTokensPost.mobile_refresh_token = accessToken;  // only get access token from Facebook so we store the data again

                            var updateTokensPostSerializedObject = JsonConvert.SerializeObject(updateTokensPost);
                            var updateTokensContent = new StringContent(updateTokensPostSerializedObject, Encoding.UTF8, "application/json");
                            var updateTokensResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokensContent);  // This calls the database and returns True or False
                            var updateTokenResponseContent = await updateTokensResponse.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                            if (updateTokensResponse.IsSuccessStatusCode)
                            {
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(Constant.days);  // Internal assignment - not from the database

                                Application.Current.Properties["time_stamp"] = expDate;
                                Application.Current.Properties["platform"] = "FACEBOOK";
                                // Application.Current.MainPage = new SubscriptionPage();


                                //check to see if user has already selected a meal plan before
                                var request2 = new HttpRequestMessage();
                                Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];

                                //old db
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"]; 
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                                request2.RequestUri = new Uri(url);
                                //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                                request2.Method = HttpMethod.Get;
                                var client2 = new HttpClient();
                                HttpResponseMessage response = await client.SendAsync(request2);


                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {

                                    HttpContent content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    var userString = await content.ReadAsStringAsync();
                                    //Console.WriteLine(userString);


                                    //writing guid to db
                                    if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                                    {
                                        if (Device.RuntimePlatform == Device.iOS)
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                                        }
                                        else
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                                        }

                                        if (deviceId != null)
                                        {
                                            GuidPost notificationPost = new GuidPost();

                                            notificationPost.uid = (string)Application.Current.Properties["user_id"];
                                            notificationPost.guid = deviceId.Substring(5);
                                            Application.Current.Properties["guid"] = deviceId.Substring(5);
                                            notificationPost.notification = "TRUE";

                                            var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                            Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                            var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                            var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                                            Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                            if (clientResponse.IsSuccessStatusCode)
                                            {
                                                System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                                                Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                                            }
                                            else
                                            {
                                                await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                            }
                                        }
                                    }
                                    //written



                                    if (userString.ToString()[0] != '{')
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Console.WriteLine("go to SubscriptionPage");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                        return;
                                    }

                                    JObject info_obj = JObject.Parse(userString);
                                    this.NewMainPage.Clear();

                                    //ArrayList item_price = new ArrayList();
                                    //ArrayList num_items = new ArrayList();
                                    //ArrayList payment_frequency = new ArrayList();
                                    //ArrayList groupArray = new ArrayList();

                                    Console.WriteLine("string: " + (info_obj["result"]).ToString());
                                    Debug.WriteLine("what is canChooseSelect set to?: " + Preferences.Get("canChooseSelect", false).ToString());
                                    //check if the user hasn't entered any info before, if so put in the placeholders
                                    if ((info_obj["result"]).ToString() == "[]" || (info_obj["result"]).ToString().Contains("ACTIVE") == false)
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Console.WriteLine("go to SubscriptionPage");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }
                                    else
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        Preferences.Set("profilePicLink", facebookData.picture.data.url);
                                        Debug.WriteLine("fb profile pic:" + facebookData.picture.data.url);

                                        Preferences.Set("canChooseSelect", true);
                                        Zones[] zones = new Zones[] { };
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new Select(zones, (info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }
                                }

                                // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                                //string userID = (string)Application.Current.Properties["user_id"];
                                //printing id for testing
                                //System.Diagnostics.Debug.WriteLine("user ID after success: " + userID);
                            }
                            else
                            {
                                //testing with loading page
                                Application.Current.MainPage = new MainPage();

                                await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                            }
                        }

                        // Wrong Platform message
                        else if (data5.code.ToString() == Constant.ErrorPlatform)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                        }


                        // Wrong LOGIN method message
                        else if (data5.code.ToString() == Constant.ErrorUserDirectLogIn)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing MTYD account. Please use direct login", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }



        private async void FacebookAutheticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            try
            {
                var authenticator = sender as OAuth2Authenticator;
                if (authenticator != null)
                {
                    authenticator.Completed -= FacebookAuthenticatorCompleted;
                    authenticator.Error -= FacebookAutheticatorError;
                }

                await DisplayAlert("Authentication error: ", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }



        // GOOGLE LOGIN CLICK
        public async void googleLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("googleLoginButtonClicked entered");

                string clientId = string.Empty;
                string redirectUri = string.Empty;

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        clientId = Constant.GoogleiOSClientID;
                        redirectUri = Constant.GoogleRedirectUrliOS;
                        break;

                    case Device.Android:
                        clientId = Constant.GoogleAndroidClientID;
                        redirectUri = Constant.GoogleRedirectUrlAndroid;
                        break;
                }

                Console.WriteLine("after switch entered");

                var authenticator = new OAuth2Authenticator(clientId, string.Empty, Constant.GoogleScope, new Uri(Constant.GoogleAuthorizeUrl), new Uri(redirectUri), new Uri(Constant.GoogleAccessTokenUrl), null, true);
                var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();

                Console.WriteLine("after vars entered");

                authenticator.Completed += GoogleAuthenticatorCompleted;
                authenticator.Error += GoogleAuthenticatorError;

                Console.WriteLine("after completed/error entered");

                AuthenticationState.Authenticator = authenticator;

                Console.WriteLine("before Login entered");
                presenter.Login(authenticator);
                Console.WriteLine("after Login entered");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void GoogleAuthenticatorCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("googleAuthenticatorCompleted entered");
                //Application.Current.MainPage = new Landing("", "", "");

                var authenticator = sender as OAuth2Authenticator;

                if (authenticator != null)
                {
                    authenticator.Completed -= GoogleAuthenticatorCompleted;
                    authenticator.Error -= GoogleAuthenticatorError;
                }

                Console.WriteLine("Authenticator authenticated:" + e.IsAuthenticated);

                if (e.IsAuthenticated)
                {
                    GoogleUserProfileAsync(e.Account.Properties["access_token"], e.Account.Properties["refresh_token"], e);
                }
                else
                {
                    await DisplayAlert("Error", "Google was not able to autheticate your account", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async void GoogleUserProfileAsync(string accessToken, string refreshToken, AuthenticatorCompletedEventArgs e)
        {
            try
            {
                Console.WriteLine("googleUserProfileAsync entered");

                //testing with loading page
                Application.Current.MainPage = new Loading();

                var client = new HttpClient();
                var socialLogInPost = new SocialLogInPost();

                var request = new OAuth2Request("GET", new Uri(Constant.GoogleUserInfoUrl), null, e.Account);
                var GoogleResponse = await request.GetResponseAsync();
                Debug.WriteLine("google response: " + GoogleResponse);
                var userData = GoogleResponse.GetResponseText();
                Debug.WriteLine("user Data: " + userData);
                //Application.Current.MainPage = new NavigationPage(new Loading());

                System.Diagnostics.Debug.WriteLine(userData);
                GoogleResponse googleData = JsonConvert.DeserializeObject<GoogleResponse>(userData);
                Debug.WriteLine("googleData: " + googleData);
                socialLogInPost.email = googleData.email;
                socialLogInPost.password = "";
                socialLogInPost.social_id = googleData.id;
                socialLogInPost.signup_platform = "GOOGLE";

                var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);
                var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

                var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
                var responseContent = await RDSResponse.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(responseContent);
                System.Diagnostics.Debug.WriteLine(RDSResponse.IsSuccessStatusCode);

                if (RDSResponse.IsSuccessStatusCode)
                {
                    if (responseContent != null)
                    {
                        var data5 = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        //if (responseContent.Contains(Constant.EmailNotFound))
                        if (data5.code.ToString() == Constant.EmailNotFound)
                        {
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a MTYD account. Please sign up!", "OK", "Cancel");
                            if (signUp)
                            {
                                // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH MTYD SOCIAL SIGN UP
                                // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                                // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                                Preferences.Set("canChooseSelect", false);
                                Application.Current.MainPage = new CarlosSocialSignUp(googleData.id, googleData.given_name, googleData.family_name, googleData.email, accessToken, refreshToken, "GOOGLE");
                            }
                        }
                        //else if (responseContent.Contains(Constant.AutheticatedSuccesful))
                        else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                        {
                            //testing with loading page
                            //Application.Current.MainPage = new Loading();

                            var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            Debug.WriteLine("responseContent: " + responseContent.ToString());
                            Debug.WriteLine("data: " + data.ToString());
                            Application.Current.Properties["user_id"] = data.result[0].customer_uid;

                            UpdateTokensPost updateTokesPost = new UpdateTokensPost();
                            updateTokesPost.uid = data.result[0].customer_uid;
                            updateTokesPost.mobile_access_token = accessToken;
                            updateTokesPost.mobile_refresh_token = refreshToken;

                            var updateTokesPostSerializedObject = JsonConvert.SerializeObject(updateTokesPost);
                            var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                            var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                            var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();
                            System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                            if (updateTokesResponse.IsSuccessStatusCode)
                            {
                                DateTime today = DateTime.Now;
                                DateTime expDate = today.AddDays(Constant.days);

                                Application.Current.Properties["time_stamp"] = expDate;
                                Application.Current.Properties["platform"] = "GOOGLE";
                                // Application.Current.MainPage = new SubscriptionPage();


                                //check to see if user has already selected a meal plan before
                                var request2 = new HttpRequestMessage();
                                Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                                //old db
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                                request2.RequestUri = new Uri(url);
                                //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                                request2.Method = HttpMethod.Get;
                                var client2 = new HttpClient();
                                HttpResponseMessage response = await client.SendAsync(request2);

                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {

                                    HttpContent content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    var userString = await content.ReadAsStringAsync();
                                    Console.WriteLine(userString.ToString());

                                    //writing guid to db
                                    if (Preferences.Get("setGuid" + (string)Application.Current.Properties["user_id"], false) == false)
                                    {

                                        if (Device.RuntimePlatform == Device.iOS)
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the iOS GUID from Log in: " + deviceId); }
                                        }
                                        else
                                        {
                                            deviceId = Preferences.Get("guid", null);
                                            if (deviceId != null) { Debug.WriteLine("This is the Android GUID from Log in " + deviceId); }
                                        }

                                        if (deviceId != null)
                                        {
                                            Debug.WriteLine("entered inside setting guid");
                                            GuidPost notificationPost = new GuidPost();

                                            notificationPost.uid = (string)Application.Current.Properties["user_id"];
                                            notificationPost.guid = deviceId.Substring(5);
                                            Application.Current.Properties["guid"] = deviceId.Substring(5);
                                            notificationPost.notification = "TRUE";

                                            var notificationSerializedObject = JsonConvert.SerializeObject(notificationPost);
                                            Debug.WriteLine("Notification JSON Object to send: " + notificationSerializedObject);

                                            var notificationContent = new StringContent(notificationSerializedObject, Encoding.UTF8, "application/json");

                                            var clientResponse = await client.PostAsync(Constant.GuidUrl, notificationContent);

                                            Debug.WriteLine("Status code: " + clientResponse.IsSuccessStatusCode);

                                            if (clientResponse.IsSuccessStatusCode)
                                            {
                                                System.Diagnostics.Debug.WriteLine("We have post the guid to the database");
                                                Preferences.Set("setGuid" + (string)Application.Current.Properties["user_id"], true);
                                            }
                                            else
                                            {
                                                await DisplayAlert("Ooops!", "Something went wrong. We are not able to send you notification at this moment", "OK");
                                            }
                                        }
                                    }
                                    //written


                                    //testing for if the user only has serving fresh stuff
                                    if (userString.ToString()[0] != '{')
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Console.WriteLine("google first: " + (info_obj2["result"])[0]["customer_first_name"].ToString());
                                        Console.WriteLine("google last: " + (info_obj2["result"])[0]["customer_last_name"].ToString());
                                        Console.WriteLine("google email: " + (info_obj2["result"])[0]["customer_email"].ToString());
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                        //var accessToken = loginInfo.ExternalIdentity.Claims.Where(c => c.Type.Equals("urn:google:accesstoken")).Select(c => c.Value).FirstOrDefault();
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

                                        //Debug.WriteLine("picture link: " + userPicture);

                                        Console.WriteLine("go to SubscriptionPage");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                        return;
                                    }
                                    //testing

                                    JObject info_obj = JObject.Parse(userString);
                                    this.NewMainPage.Clear();

                                    //ArrayList item_price = new ArrayList();
                                    //ArrayList num_items = new ArrayList();
                                    //ArrayList payment_frequency = new ArrayList();
                                    //ArrayList groupArray = new ArrayList();

                                    //int counter = 0;
                                    //Console.WriteLine("testing: " + ((info_obj["result"]).Count().ToString()));
                                    //Console.WriteLine("testing: " + ((info_obj["result"]).Last().ToString()));
                                    //while (((info_obj["result"])[counter]) != null)
                                    //{
                                    //    Console.WriteLine("worked" + counter);
                                    //    counter++;
                                    //}

                                    //check if the user hasn't entered any info before, if so put in the placeholders

                                    Console.WriteLine("string: " + (info_obj["result"]).ToString());
                                    //check if the user hasn't entered any info before, if so put in the placeholders
                                    if ((info_obj["result"]).ToString() == "[]" || (info_obj["result"]).ToString() == "204" || (info_obj["result"]).ToString().Contains("ACTIVE") == false)
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Console.WriteLine("google first: " + (info_obj2["result"])[0]["customer_first_name"].ToString());
                                        Console.WriteLine("google last: " + (info_obj2["result"])[0]["customer_last_name"].ToString());
                                        Console.WriteLine("google email: " + (info_obj2["result"])[0]["customer_email"].ToString());
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

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

                                        Console.WriteLine("go to SubscriptionPage");
                                        DisplayAlert("navigation", "sending to subscription", "close");
                                        Preferences.Set("canChooseSelect", false);
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }
                                    else
                                    {
                                        url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                                        //old db
                                        //url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                        var request3 = new HttpRequestMessage();
                                        request3.RequestUri = new Uri(url);
                                        request3.Method = HttpMethod.Get;
                                        response = await client.SendAsync(request3);
                                        content = response.Content;
                                        Console.WriteLine("content: " + content);
                                        userString = await content.ReadAsStringAsync();
                                        JObject info_obj2 = JObject.Parse(userString);
                                        this.NewMainPage.Clear();
                                        Preferences.Set("user_latitude", (info_obj2["result"])[0]["customer_lat"].ToString());
                                        Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                        Preferences.Set("user_longitude", (info_obj2["result"])[0]["customer_long"].ToString());
                                        Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));
                                        Console.WriteLine("google first: " + (info_obj2["result"])[0]["customer_first_name"].ToString());
                                        Console.WriteLine("google last: " + (info_obj2["result"])[0]["customer_last_name"].ToString());
                                        Console.WriteLine("google email: " + (info_obj2["result"])[0]["customer_email"].ToString());
                                        Debug.WriteLine("user access token: " + (info_obj2["result"])[0]["user_access_token"].ToString());
                                        Debug.WriteLine("mobile access token: " + (info_obj2["result"])[0]["mobile_access_token"].ToString());
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

                                        DisplayAlert("navigation", "sending to select", "close");
                                        Console.WriteLine("delivery first name: " + (info_obj["result"])[0]["delivery_first_name"].ToString());
                                        Console.WriteLine("delivery last name: " + (info_obj["result"])[0]["delivery_last_name"].ToString());
                                        Console.WriteLine("delivery email: " + (info_obj["result"])[0]["delivery_email"].ToString());
                                        Preferences.Set("canChooseSelect", true);
                                        //await Debug.WriteLine("a");
                                        //navToSelect((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString());
                                        Zones[] zones = new Zones[] { };
                                        await Application.Current.SavePropertiesAsync();
                                        Application.Current.MainPage = new NavigationPage(new Select(zones, (info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                        //Application.Current.MainPage = new NavigationPage(new CongratsPage());
                                        //Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                        //Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                    }

                                }

                                // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                                // string userID = (string)Application.Current.Properties["user_id"];
                            }
                            else
                            {
                                //testing with loading page
                                Application.Current.MainPage = new MainPage();

                                await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                            }
                        }
                        //else if (responseContent.Contains(Constant.ErrorPlatform))
                        else if (data5.code.ToString() == Constant.ErrorPlatform)
                        {
                            Debug.WriteLine("google login: check if the user's email is already used elsewhere");
                            //testing with loading page
                            Application.Current.MainPage = new MainPage();

                            var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                            await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                        }
                        //else if (responseContent.Contains(Constant.ErrorUserDirectLogIn))
                        else if (data5.code.ToString() == Constant.ErrorUserDirectLogIn)
                        {

                            var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                            Debug.WriteLine("responseContent direct login: " + responseContent.ToString());
                            //testing with loading page
                            //await Navigation.PopAsync();
                            Application.Current.MainPage = new MainPage();
                            //Navigation.RemovePage(this.Navigation.NavigationStack[0]);

                            await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing MTYD account. Please use direct login", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void GoogleAuthenticatorError(object sender, AuthenticatorErrorEventArgs e)
        {
            try
            {
                Console.WriteLine("googleAuthenticatorError entered");

                var authenticator = sender as OAuth2Authenticator;

                if (authenticator != null)
                {
                    authenticator.Completed -= GoogleAuthenticatorCompleted;
                    authenticator.Error -= GoogleAuthenticatorError;
                }

                await DisplayAlert("Authentication error: ", e.Message, "OK");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // APPLE LOGIN CLICK
        public async void appleLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("appleLogin clicked");

                SignIn?.Invoke(sender, e);
                var c = (ImageButton)sender;
                Console.WriteLine("appleLogin c: " + c.ToString());
                c.Command?.Execute(c.CommandParameter);

                //testing
                var testingVar = new LoginViewModel();
                //testingVar.OnAppleSignInRequest();
                vm.OnAppleSignInRequest();
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public void InvokeSignInEvent(object sender, EventArgs e)
            => SignIn?.Invoke(sender, e);

        public async void directLoginButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new MainLogin());

            HomePage.IsVisible = false;
            BackgroundImageSource = null;
            innerGrid.IsVisible = false;
            MenuBar.IsVisible = true;
            LoginPage.IsVisible = true;
            intro.IsVisible = false;
        }

        async void clickedSignUp(object sender, EventArgs e)
        {
            Preferences.Set("canChooseSelect", false);
            Application.Current.MainPage = new CarlosSignUp();
        }

        async void clickedForgotPass(System.Object sender, System.EventArgs e)
        {
            //reset password (get): https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/reset_password?email=welks@gmail.com
            //change password (post): https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_password
            try
            {
                if (loginUsername.Text == "" || loginUsername.Text == null)
                {
                    DisplayAlert("Error", "please enter your email into the username box first", "OK");
                    return;
                }
                //if endpoint returns email not found, display an alert
                else
                {
                    string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/reset_password?email=" + loginUsername.Text;
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(url);
                    request.Method = HttpMethod.Get;
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(request);
                    HttpContent content = response.Content;
                    Console.WriteLine("content: " + content.ToString());
                    var userString = await content.ReadAsStringAsync();
                    Debug.WriteLine("userString: " + userString);
                    JObject info_obj = JObject.Parse(userString);
                    Debug.WriteLine("info_obj");
                }

                //for testing 
                Application.Current.MainPage = new changePassword(loginUsername.Text);
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            if (loginPassword.IsPassword == true)
                loginPassword.IsPassword = false;
            else loginPassword.IsPassword = true;
        }

        void HIWclicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new HowItWorks();
        }

        async public void getProfileInfo()
        {
            try
            {
                //await DisplayAlert("success", "reached inside of getprofileinfo with this user id: " + (string)Application.Current.Properties["user_id"], "OK");
                string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];

                //old db
                //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                Debug.WriteLine("getProfileInfo url: " + url);
                var request3 = new HttpRequestMessage();
                request3.RequestUri = new Uri(url);
                request3.Method = HttpMethod.Get;
                var client2 = new HttpClient();
                HttpResponseMessage response = await client2.SendAsync(request3);
                HttpContent content = response.Content;
                Console.WriteLine("content: " + content.ToString());
                var userString = await content.ReadAsStringAsync();
                Debug.WriteLine("userString: " + userString);
                //await DisplayAlert("success", "reached after userString: " + userString, "OK");
                JObject info_obj3 = JObject.Parse(userString);
                Debug.WriteLine("info_obj3: " + info_obj3.ToString());
                //await DisplayAlert("success", "reached after info_obj: " + info_obj3.ToString(), "OK");
                this.NewMainPage.Clear();
                Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                //Preferences.Set("profilePicLink", "");
                //Preferences.Set("canChooseSelect", false);
                if (Preferences.Get("canChooseSelect", false) == true)
                {
                    //await DisplayAlert("success", "going to select page", "OK");
                    Zones[] zones = new Zones[] { };
                    Application.Current.MainPage = new NavigationPage(new Select(zones, (info_obj3["result"])[0]["customer_first_name"].ToString(), (info_obj3["result"])[0]["customer_last_name"].ToString(), (info_obj3["result"])[0]["customer_email"].ToString()));
                }
                else
                {
                    //await DisplayAlert("success", "going to subscription page", "OK");
                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj3["result"])[0]["customer_first_name"].ToString(), (info_obj3["result"])[0]["customer_last_name"].ToString(), (info_obj3["result"])[0]["customer_email"].ToString()));

                }
                return;
            }
            catch (Exception ex)
            {
                Application.Current.MainPage = new MainPage();
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public void navToSub(string first, string last, string email)
        {
            Debug.WriteLine("reached navToSub");
            Application.Current.MainPage = new NavigationPage(new SubscriptionPage(first, last, email));
        }

        public void navToSelect(string first, string last, string email)
        {
            Debug.WriteLine("reached navToSelect");
            Zones[] zones = new Zones[] { };
            Application.Current.MainPage = new NavigationPage(new Select(zones, first, last, email));
        }

        public void navToLoading()
        {
            Debug.WriteLine("reached loading");
            Application.Current.MainPage = new Loading();
        }
        //void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    Application.Current.MainPage = new MainPageExperiment();
        //    //Navigation.PushAsync(new MainPageExperiment());
        //}

        //added function
        async void clickedExplore(System.Object sender, System.EventArgs e)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                Application.Current.MainPage = new NavigationPage(new ExploreMeals());
            }
            else
            {
                if (Preferences.Get("canChooseSelect", false) == false)
                    DisplayAlert("Error", "please purchase a meal plan first", "OK");
                else
                {
                    Zones[] zones = new Zones[] { };
                    await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email), false);
                }
            }
        }

        async void clickedPurchase(System.Object sender, System.EventArgs e)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                Application.Current.Properties["user_id"] = "000-00000";
                Preferences.Set("user_latitude", "0.0");
                Preferences.Set("user_longitude", "0.0");
                await Application.Current.SavePropertiesAsync();
                Application.Current.MainPage = new NavigationPage(new SubscriptionPage("Welcome", "Guest", "welcome@guest.com"));
            }
            else
            {
                await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email), false);
            }
            //Application.Current.Properties["user_id"] = "000-00000";
            //Application.Current.Properties["platform"] = "GUEST";
            //Preferences.Set("user_latitude", "0.0");
            //Preferences.Set("user_longitude", "0.0");
            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage("Welcome", "Guest", "welcome@guest.com"));
        }

        async void clickedX(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            DiscountGrid.IsVisible = false;
        }

        async void clickedDiscounts(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = true;
            DiscountGrid.IsVisible = true;
        }

        async void getDiscounts()
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/plans?business_uid=200-000002");
                request.Method = HttpMethod.Get;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var userString = await content.ReadAsStringAsync();
                    JObject plan_obj = JObject.Parse(userString);

                    foreach (var m in plan_obj["result"])
                    {
                        if (m["num_deliveries"].ToString() == "4")
                        {
                            string disc = "";
                            if (m["delivery_discount"].ToString().IndexOf(".") != -1 && m["delivery_discount"].ToString().Substring(m["delivery_discount"].ToString().IndexOf(".") + 1, 1) == "0")
                                disc = m["delivery_discount"].ToString().Substring(0, m["delivery_discount"].ToString().IndexOf("."));
                            else disc = m["delivery_discount"].ToString().Trim();

                            couponAmt.Text = disc + "% off";

                            couponDesc.Text = "Select 4 deliveries pre-pay option and Get " + disc + "% off";
                        }

                        if (m["num_deliveries"].ToString() == "8")
                        {
                            string disc = "";
                            if (m["delivery_discount"].ToString().IndexOf(".") != -1 && m["delivery_discount"].ToString().Substring(m["delivery_discount"].ToString().IndexOf(".") + 1, 1) == "0")
                                disc = m["delivery_discount"].ToString().Substring(0, m["delivery_discount"].ToString().IndexOf("."));
                            else disc = m["delivery_discount"].ToString().Trim();

                            couponAmt2.Text = disc + "% off";

                            couponDesc2.Text = "Select 8 deliveries pre-pay option and Get " + disc + "% off";
                        }

                        if (m["num_deliveries"].ToString() == "10")
                        {
                            string disc = "";
                            if (m["delivery_discount"].ToString().IndexOf(".") != -1 && m["delivery_discount"].ToString().Substring(m["delivery_discount"].ToString().IndexOf(".") + 1, 1) == "0")
                                disc = m["delivery_discount"].ToString().Substring(0, m["delivery_discount"].ToString().IndexOf("."));
                            else disc = m["delivery_discount"].ToString().Trim();

                            couponAmt3.Text = disc + "% off";

                            couponDesc3.Text = "Select 10 deliveries pre-pay option and Get " + disc + "% off";
                        }
                        //int num_meals = int.Parse(m["num_items"].ToString());
                        //if (!numMealsList.Contains(num_meals))
                        //{
                        //    numMealsList.Add(num_meals);
                        //}
                        //discounts[i, j] = double.Parse(m["delivery_discount"].ToString());
                        //itemPrices[i, j] = double.Parse(m["item_price"].ToString());
                        //itemNames[i, j] = m["item_name"].ToString();
                        //itemUids[i, j] = m["item_uid"].ToString();
                        //if (j == 4)
                        //{
                        //    i++;
                        //    j = 0;
                        //}
                        //else
                        //{
                        //    j++;
                        //}
                    }

                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new ViewModel.Menu(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedStarted(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email));
        }

        void clickedFb(System.Object sender, System.EventArgs e)
        {
            Launcher.OpenAsync(new Uri("https://www.facebook.com/Meals-For-Me-101737768566584"));
        }

        void clickedIns(System.Object sender, System.EventArgs e)
        {
            Launcher.OpenAsync(new Uri("https://www.instagram.com/mealsfor.me/"));
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
            addressList.ItemsSource = await addr.GetPlacesPredictionsAsync(AddressEntry.Text);
            //addr.OnAddressChanged(addressList, Addresses, _addressText);
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            CheckAddressGrid.Margin = new Thickness(50, HomePage.Height - 75, 50, 120);
            scroll.ScrollToAsync(0, 400, true);
            addr.addressEntryFocused(addressList);
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            addr.addressEntryUnfocused(addressList);
        }

        async void addressSelected(System.Object sender, System.EventArgs e)
        {
            try
            {
                addr.addressSelected(addressList, AddressEntry);
                addressList.IsVisible = false;
                string inputAddress = AddressEntry.Text;

                string[] addressSplit = inputAddress.Split(',');
                string addr1 = addressSplit[0].Trim();
                string city = addressSplit[1].Trim();
                string state = addressSplit[2].Trim();
                string zip = addressSplit[3].Trim();

                //string zip = addressSplit[2].Trim().Substring(3);

                // Setting request for USPS API
                XDocument requestDoc = new XDocument(
                    new XElement("AddressValidateRequest",
                    new XAttribute("USERID", "400INFIN1745"),
                    new XElement("Revision", "1"),
                    new XElement("Address",
                    new XAttribute("ID", "0"),
                    new XElement("Address1", addr1),
                    new XElement("Address2", ""),
                    new XElement("City", city),
                    new XElement("State", state),
                    new XElement("Zip5", ""),
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
                        if (GetXMLElement(element, "DPVConfirmation").Equals("Y") || GetXMLElement(element, "DPVConfirmation").Equals("D") || GetXMLElement(element, "DPVConfirmation").Equals("S"))
                        {
                            Geocoder geoCoder = new Geocoder();

                            Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim());
                            Position position = approximateLocations.FirstOrDefault();

                            latitude = $"{position.Latitude}";
                            longitude = $"{position.Longitude}";

                            //used for createaccount endpoint
                            Preferences.Set("user_latitude", latitude);
                            Preferences.Set("user_longitude", longitude);
                            Debug.WriteLine("user latitude: " + latitude);
                            Debug.WriteLine("user longitude: " + longitude);

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
                                passingZones = obj.Result;
                                withinZones = true;
                            }

                            break;
                        }
                        //else if (GetXMLElement(element, "DPVConfirmation").Equals("D"))
                        //{
                        //    await DisplayAlert("Alert!", "Please enter a unit number.", "Ok");
                        //    return;
                        //}
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
                    await DisplayAlert("We couldn't find your address", "Please check for errors.", "OK");
                }
                else if (withinZones == false)
                {
                    fade.IsVisible = true;
                    signUpButton2.IsVisible = false;
                    CheckAddressGrid.IsVisible = true;
                    CheckAddressHeading.Text = "Still Growing…";
                    CheckAddressBody.Text = "Sorry, it looks like we don’t deliver to your neighborhood yet. Enter your email address and we will let you know as soon as we come to your neighborhood.";
                    EmailFrame.IsVisible = true;
                    OkayButton.Text = "Okay";
                    //await scroll.ScrollToAsync(0, 0, true);
                    AddressEntry.Unfocus();
                }
                else
                {
                    fade.IsVisible = true;
                    signUpButton2.IsVisible = true;
                    CheckAddressGrid.IsVisible = true;
                    CheckAddressHeading.Text = "Hooray!";
                    CheckAddressBody.Text = "We are so glad that we deliver to your neighborhood. Please click Okay to continue enjoying MealsFor.Me";
                    EmailFrame.IsVisible = false;
                    OkayButton.Text = "Explore Meals";

                    Preferences.Set("mainPageAdd", addr1);
                    Preferences.Set("mainPageCity", city);
                    if (state.IndexOf(" ") != -1)
                        state = state.Substring(0, state.IndexOf(" "));
                    Preferences.Set("mainPageState", state);
                    Preferences.Set("mainPageZip", zip);
                    //await scroll.ScrollToAsync(0, 0, true);
                    AddressEntry.Unfocus();
                }
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

        void xButtonClicked(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
            baaPopUpGrid.IsVisible = false;
            DiscountGrid.IsVisible = false;
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
                    var client = new HttpClient();
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

        async void OkayClicked(System.Object sender, System.EventArgs e)
        {
            if (EmailFrame.IsVisible && EmailEntry.Text != null && EmailEntry.Text.Length != 0)
            {
                // add email to new neighborhood notification list
            }
            fade.IsVisible = false;
            CheckAddressGrid.IsVisible = false;
            Application.Current.MainPage = new NavigationPage(new ExploreMeals());
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

        void clickedLogout(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties["platform"] = "GUEST";
            Application.Current.Properties.Remove("time_stamp");
            //Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }
        //end of menu functions

        async Task CheckVersion()
        {
            var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();

            if (!isLatest)
            {
                await DisplayAlert("Mealsfor.Me\nhas gotten even better!", "Please visit the App Store to get the latest version.", "OK");
                await CrossLatestVersion.Current.OpenAppInStore();
            }
            else
            {
                restOfConstructor();
                //GetBusinesses();

                //CartTotal.Text = CheckoutPage.total_qty.ToString();
            }
        }

        void restOfConstructor()
        {
            try
            {
                setGrid();
                // BackgroundImageSource = "new_background.png";

                // APPLE
                //var vm = new LoginViewModel();
                vm.AppleError += AppleError;
                //vm.PlatformError += PlatformError;
                //BindingContext = vm;

                if (Device.RuntimePlatform == Device.Android)
                {
                    appleLoginButton.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }
    }
}


