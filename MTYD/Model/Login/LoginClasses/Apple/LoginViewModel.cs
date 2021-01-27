using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MTYD.Constants;
using MTYD.ViewModel;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
//using ServingFresh.Config;
using MTYD.Model.Login;
using MTYD.Model.SignUp;
using MTYD.Model.User;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;
//using ServingFresh.Views;

namespace MTYD.Model.Login.LoginClasses.Apple
{
    //=======================================
    // CARLOS APPLE ADDITIONAL CLASSES
    public class Info
    {
        public string customer_email { get; set; }
    }

    public class AppleUser
    {
        public string message { get; set; }
        public int code { get; set; }
        public IList<Info> result { get; set; }
    }

    public class AppleEmail
    {
        public string social_id { get; set; }
    }
    //=======================================
    public class LoginViewModel
    {
        public HttpClient client = new HttpClient();
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        public ObservableCollection<Plans> NewLogin = new ObservableCollection<Plans>();
        public static string apple_token = null;
        public static string apple_email = null;

        public bool IsAppleSignInAvailable { get { return appleSignInService?.IsAvailable ?? false; } }
        public ICommand SignInWithAppleCommand { get; set; }

        public event EventHandler AppleError = delegate { };
        public event EventHandler PlatformError = delegate { };

        IAppleSignInService appleSignInService = null;
        public LoginViewModel()
        {
            Console.WriteLine("in LoginViewModel, entered LoginViewModel()");

            appleSignInService = DependencyService.Get<IAppleSignInService>();
            Console.WriteLine("after appleSignInService reached");
            SignInWithAppleCommand = new Command(OnAppleSignInRequest);
            Console.WriteLine("after SignInWithAppleCommand reached");
        }

        public async void OnAppleSignInRequest()
        {
            Console.WriteLine("in LoginViewModel, entered OnAppleSignInRequest");
            //================================
            //CARLOS UPDATE APPLE SOLUTION
            try
            {
                var account = await appleSignInService.SignInAsync();
                if (account != null)
                {
                    Debug.WriteLine("reached inside account");
                    Preferences.Set(App.LoggedInKey, true);
                    await SecureStorage.SetAsync(App.AppleUserIdKey, account.UserId);

                    if (account.Token == null) { account.Token = ""; }
                    if (account.Email != null)
                    {
                        Debug.WriteLine("email not null: " + account.Email);
                        if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                        {
                            //Application.Current.Properties[account.UserId.ToString()] = account.Email;
                            Debug.WriteLine((string)Application.Current.Properties[account.UserId.ToString()]);
                        }
                        else
                        {
                            Application.Current.Properties[account.UserId.ToString()] = account.Email;
                        }
                    }
                    if (account.Email == null) { account.Email = ""; }
                    if (account.Name == null) { account.Name = ""; }

                    if (Application.Current.Properties.ContainsKey(account.UserId.ToString()))
                    {
                        AppleUserProfileAsync(account.UserId, account.Token, (string)Application.Current.Properties[account.UserId.ToString()], account.Name);
                    }
                    else
                    {
                        var client = new HttpClient();
                        var getAppleEmail = new AppleEmail();
                        getAppleEmail.social_id = account.UserId;

                        var socialLogInPostSerialized = JsonConvert.SerializeObject(getAppleEmail);

                        Debug.WriteLine(socialLogInPostSerialized);
                        Debug.WriteLine("email:" + account.Email);
                        Debug.WriteLine("userid:" + account.UserId);
                        var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");
                        var RDSResponse = await client.PostAsync(Constant.AppleEmailUrl, postContent);
                        var responseContent = await RDSResponse.Content.ReadAsStringAsync();

                        Debug.WriteLine(responseContent);
                        if (RDSResponse.IsSuccessStatusCode)
                        {
                            var data = JsonConvert.DeserializeObject<AppleUser>(responseContent);
                            Debug.WriteLine("data:" + data);
                            Debug.WriteLine("data:" + data.ToString());
                            Application.Current.Properties[account.UserId.ToString()] = data.result[0].customer_email;
                            //Application.Current.Properties[account.UserId.ToString()] = "jonathantly.01@gmail.com";
                            AppleUserProfileAsync(account.UserId, account.Token, (string)Application.Current.Properties[account.UserId.ToString()], account.Name);
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Ooops", "Our system is not working. We can't process your request at this moment", "OK");
                        }
                    }
                }
                else
                {
                    AppleError?.Invoke(this, default(EventArgs));
                }
            }
            catch (Exception apple)
            {
                await Application.Current.MainPage.DisplayAlert("Error", apple.Message, "OK");
            }
            //================================
        }

        public async void AppleUserProfileAsync(string appleId, string appleToken, string appleUserEmail, string userName)
        {
            System.Diagnostics.Debug.WriteLine("LINE 95");
            var client = new HttpClient();
            var socialLogInPost = new SocialLogInPost();

            socialLogInPost.email = appleUserEmail;
            socialLogInPost.password = "";
            socialLogInPost.social_id = appleId;
            socialLogInPost.signup_platform = "APPLE";

            var socialLogInPostSerialized = JsonConvert.SerializeObject(socialLogInPost);

            System.Diagnostics.Debug.WriteLine(socialLogInPostSerialized);

            var postContent = new StringContent(socialLogInPostSerialized, Encoding.UTF8, "application/json");
            var RDSResponse = await client.PostAsync(Constant.LogInUrl, postContent);
            var responseContent = await RDSResponse.Content.ReadAsStringAsync();

            System.Diagnostics.Debug.WriteLine(responseContent);

            if (RDSResponse.IsSuccessStatusCode)
            {
                if (responseContent != null)
                {
                    var data5 = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                    if (data5.code.ToString() == Constant.EmailNotFound)
                    {
                        var signUp = await Application.Current.MainPage.DisplayAlert("Message", "It looks like you don't have a MTYD account. Please sign up!", "OK", "Cancel");
                        if (signUp)
                        {
                            // HERE YOU NEED TO SUBSTITUTE MY SOCIAL SIGN UP PAGE WITH MTYD SOCIAL SIGN UP
                            // NOTE THAT THIS SOCIAL SIGN UP PAGE NEEDS A CONSTRUCTOR LIKE THE FOLLOWING ONE
                            // SocialSignUp(string socialId, string firstName, string lastName, string emailAddress, string accessToken, string refreshToken, string platform)
                            Application.Current.MainPage = new CarlosSocialSignUp(appleId, userName, "", appleUserEmail, appleToken, appleToken, "APPLE");
                        }
                    }
                    else if (data5.code.ToString() == Constant.AutheticatedSuccesful)
                    {
                        var data = JsonConvert.DeserializeObject<SuccessfulSocialLogIn>(responseContent);
                        Application.Current.Properties["user_id"] = data.result[0].customer_uid;

                        UpdateTokensPost updateTokesPost = new UpdateTokensPost();
                        updateTokesPost.uid = data.result[0].customer_uid;
                        updateTokesPost.mobile_access_token = appleToken;
                        updateTokesPost.mobile_refresh_token = appleToken;

                        var updateTokesPostSerializedObject = JsonConvert.SerializeObject(updateTokesPost);
                        Console.WriteLine("updateTokesPostSerializedObject: " + updateTokesPostSerializedObject.ToString());
                        var updateTokesContent = new StringContent(updateTokesPostSerializedObject, Encoding.UTF8, "application/json");
                        Console.WriteLine("updateTokesContent: " + updateTokesContent.ToString());
                        var updateTokesResponse = await client.PostAsync(Constant.UpdateTokensUrl, updateTokesContent);
                        Console.WriteLine("updateTokesResponse: " + updateTokesResponse.ToString());
                        var updateTokenResponseContent = await updateTokesResponse.Content.ReadAsStringAsync();
                        Console.WriteLine("updateTokenResponseContent: " + updateTokenResponseContent.ToString());
                        System.Diagnostics.Debug.WriteLine(updateTokenResponseContent);

                        if (updateTokesResponse.IsSuccessStatusCode)
                        {
                            DateTime today = DateTime.Now;
                            DateTime expDate = today.AddDays(Constant.days);

                            Application.Current.Properties["time_stamp"] = expDate;
                            Application.Current.Properties["platform"] = "APPLE";

                            var request = new HttpRequestMessage();
                            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                            Console.WriteLine("url: " + url);
                            request.RequestUri = new Uri(url);
                            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                            request.Method = HttpMethod.Get;
                            var client2 = new HttpClient();
                            HttpResponseMessage response = await client2.SendAsync(request);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                                HttpContent content = response.Content;
                                Console.WriteLine("content: " + content);
                                var userString = await content.ReadAsStringAsync();
                                Console.WriteLine(userString);

                                if (userString.ToString()[0] != '{')
                                {
                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    HttpResponseMessage response2 = await client.SendAsync(request3);
                                    content = response2.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");

                                    Console.WriteLine("go to SubscriptionPage");
                                    Preferences.Set("canChooseSelect", false);

                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj3["result"])[0]["customer_first_name"].ToString(), (info_obj3["result"])[0]["customer_last_name"].ToString(), (info_obj3["result"])[0]["customer_email"].ToString()));
                                    return;
                                }

                                JObject info_obj2 = JObject.Parse(userString);
                                this.NewLogin.Clear();

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
                                if ((info_obj2["result"]).ToString() == "[]" || (info_obj2["result"]).ToString() == "204")
                                {
                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    response = await client.SendAsync(request3);
                                    content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");

                                    Console.WriteLine("go to SubscriptionPage");
                                    Preferences.Set("canChooseSelect", false);
                                    Application.Current.MainPage = new NavigationPage(new SubscriptionPage((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                }
                                else
                                {
                                    url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                                    var request3 = new HttpRequestMessage();
                                    request3.RequestUri = new Uri(url);
                                    request3.Method = HttpMethod.Get;
                                    response = await client.SendAsync(request3);
                                    content = response.Content;
                                    Console.WriteLine("content: " + content);
                                    userString = await content.ReadAsStringAsync();
                                    JObject info_obj3 = JObject.Parse(userString);
                                    this.NewMainPage.Clear();
                                    Preferences.Set("user_latitude", (info_obj3["result"])[0]["customer_lat"].ToString());
                                    Debug.WriteLine("user latitude" + Preferences.Get("user_latitude", ""));
                                    Preferences.Set("user_longitude", (info_obj3["result"])[0]["customer_long"].ToString());
                                    Debug.WriteLine("user longitude" + Preferences.Get("user_longitude", ""));

                                    Preferences.Set("profilePicLink", "");
                                    Preferences.Set("canChooseSelect", true);
                                    Application.Current.MainPage = new NavigationPage(new Select((info_obj2["result"])[0]["customer_first_name"].ToString(), (info_obj2["result"])[0]["customer_last_name"].ToString(), (info_obj2["result"])[0]["customer_email"].ToString()));
                                }
                            }

                            // Application.Current.MainPage = new SubscriptionPage();
                            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage());

                            // THIS IS HOW YOU CAN ACCESS YOUR USER ID FROM THE APP
                            // string userID = (string)Application.Current.Properties["user_id"];
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Oops", "We are facing some problems with our internal system. We weren't able to update your credentials", "OK");
                        }
                    }
                    else if (data5.code.ToString() == Constant.ErrorPlatform)
                    {
                        var RDSCode = JsonConvert.DeserializeObject<RDSLogInMessage>(responseContent);
                        await Application.Current.MainPage.DisplayAlert("Message", RDSCode.message, "OK");
                    }

                    else if (data5.code.ToString() == Constant.ErrorUserDirectLogIn)
                    {
                        await Application.Current.MainPage.DisplayAlert("Oops!", "You have an existing MTYD account. Please use direct login", "OK");
                    }
                }
            }
        }
    }
}
