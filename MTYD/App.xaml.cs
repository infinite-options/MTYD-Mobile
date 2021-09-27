using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using MTYD.Model.Login.LoginClasses.Apple;
using MTYD.Constants;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Xaml;

using MTYD.Model.Database;

using MTYD.ViewModel;
using System.Net.Http;
using MTYD.Model.Login.LoginClasses;
using Newtonsoft.Json;
using System.Text;
using MTYD.Model;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

[assembly: ExportFont("PlatNomor-WyVnn.ttf", Alias = "ButtonFont")]


namespace MTYD
{
    public partial class App : Application
    {
        //public HttpClient client = new HttpClient();
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        static UserLoginDatabase database;
        static Boolean loggedIn = false;
        JObject info_obj2;

        // Initialize variables for Apple Login
        public const string LoggedInKey = "LoggedIn";
        public const string AppleUserIdKey = "AppleUserIdKey";
        string userId;

        async public void getProfileInfo()
        {
            MainPage client = new MainPage();
            MainPage = client;

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
            info_obj2 = JObject.Parse(userString);
            Debug.WriteLine("info_obj2: " + info_obj2.ToString());
            this.NewMainPage.Clear();
            return;
        }

        public App()
        {
            InitializeComponent();

            //for MediaElement
            Device.SetFlags(new string[] { "MediaElement_Experimental" });

            // User id and time_stamp are retrieved from local phone memory (written by Login View Model, Signup, Social Signup and MainPage.xaml.cs)
            if (Application.Current.Properties.ContainsKey("user_id"))
            {
                System.Diagnostics.Debug.WriteLine("UserID is:" + (string)Application.Current.Properties["user_id"]);
                if (Application.Current.Properties.ContainsKey("time_stamp"))
                    
                {
                    System.Diagnostics.Debug.WriteLine("Time Stamp is:" + (DateTime)Application.Current.Properties["time_stamp"]);
                    DateTime today = DateTime.Now;
                    DateTime expTime = (DateTime)Application.Current.Properties["time_stamp"];
                    Console.WriteLine("today" + today.ToString());
                    Console.WriteLine("expTime" + expTime.ToString());
                    

                    if (today <= expTime)
                    {
                        MainPage client = new MainPage();
                        MainPage = client;
                        client.navToLoading();

                        client.getProfileInfo();

                        //getProfileInfo();

                        // MainPage = new CarlosHomePage();
                        Console.WriteLine("entered time check");
                        //MainPage = new NavigationPage(new SubscriptionPage("first", "last", "email"));
                        //MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));


                        // Push Pop Navigation examples
                        // MainPage = new NavigationPage(new MainPage());          // Initializes stack and pushes the first stack element
                        // MainPage.Navigation.PushAsync(new SubscriptionPage());  // Pushes new element on top of stack
                        // MainPage.Navigation.PushAsync(new Select());            // Pushes another element on top of stack
                        // MainPage.Navigation.PopAsync();                         // Pops the last element from the stack

                    }
                    else
                    {
                        Application.Current.Properties["platform"] = "GUEST";
                        Application.Current.Properties["user_id"] = "000-00000";
                        Preferences.Set("user_latitude", "0.0");
                        Preferences.Set("user_longitude", "0.0");

                        MainPage client = new MainPage();
                        MainPage = client;

                        if (Application.Current.Properties.ContainsKey("platform"))
                        {
                            System.Diagnostics.Debug.WriteLine("platform is:" + (string)Application.Current.Properties["platform"]);
                            string socialPlatform = (string)Application.Current.Properties["platform"];

                            if (socialPlatform.Equals("FACEBOOK"))
                            {
                                client.facebookLoginButtonClicked(new object(), new EventArgs());
                                // Goes to MainPage.xaml.cs
                            }
                            else if (socialPlatform.Equals("GOOGLE"))
                            {
                                client.googleLoginButtonClicked(new object(), new EventArgs());
                            }
                            else if (socialPlatform.Equals("APPLE"))
                            {
                                client.appleLoginButtonClicked(new object(), new EventArgs());
                            }
                            else
                            {
                                MainPage = new MainPage();
                            }
                        }
                    }
                }
                else
                {
                    Application.Current.Properties["platform"] = "GUEST";
                    Application.Current.Properties["user_id"] = "000-00000";
                    Preferences.Set("user_latitude", "0.0");
                    Preferences.Set("user_longitude", "0.0");

                    MainPage = new MainPage();
                }
            }
            else
            {
                Application.Current.Properties["platform"] = "GUEST";
                Application.Current.Properties["user_id"] = "000-00000";
                Preferences.Set("user_latitude", "0.0");
                Preferences.Set("user_longitude", "0.0");

                MainPage = new MainPage();
            }
        
        }

        protected override async void OnStart()
        {
            var appleSignInService = DependencyService.Get<IAppleSignInService>();

            if (appleSignInService != null)
            {
                userId = await SecureStorage.GetAsync(AppleUserIdKey);

                if (appleSignInService.IsAvailable && !string.IsNullOrEmpty(userId))
                {
                    var credentialState = await appleSignInService.GetCredentialStateAsync(userId);
                    switch (credentialState)
                    {
                        case AppleSignInCredentialState.Authorized:
                            break;
                        case AppleSignInCredentialState.NotFound:
                        case AppleSignInCredentialState.Revoked:
                            //Logout;
                            SecureStorage.Remove(AppleUserIdKey);
                            Preferences.Set(LoggedInKey, false);
                            MainPage = new MainPage();
                            break;
                    }
                }
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static UserLoginDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new UserLoginDatabase();
                }
                return database;
            }
        }


        public static Boolean LoggedIn
        {
            get
            {
                return loggedIn;
            }
        }

        public static void setLoggedIn(Boolean loggedIn)
        {
            App.loggedIn = loggedIn;
        }
    }
}
