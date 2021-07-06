using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using MTYD.Model.SignUp;
using MTYD.Model.Login.LoginClasses;
using MTYD.Model;
using MTYD.LogInClasses;
using System.Threading.Tasks;
using MTYD.Constants;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Net;

namespace MTYD.ViewModel
{
    public partial class changePassword : ContentPage
    {
        string userEmail;
        public ObservableCollection<Plans> NewMainPage = new ObservableCollection<Plans>();
        public bool createAccount = false;
        public HttpClient client = new HttpClient();

        public changePassword(string email)
        {
            InitializeComponent();
            userEmail = email;
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            Console.WriteLine("Width = " + width.ToString());
            Console.WriteLine("Height = " + height.ToString());

            BackgroundColor = Color.White;

            if (Device.RuntimePlatform == Device.Android)
            {
                headSpacer.HeightRequest = 80;
            }
            //passFrame.HeightRequest = 50;
            //passFrame2.HeightRequest = 50;
            //headSpacer.HeightRequest = height / 8;

            //grid2.Margin = new Thickness(width / 13, height / 90, width / 13, 0);
            //currentPassword.Margin = new Thickness(0, height / (-120), width / 55, height / (-120));
            //password.Margin = new Thickness(0, height / (-120), width / 55, height / (-120));
            //password2.Margin = new Thickness(0, height / (-120), width / 55, height / (-120));
            //currentPassFrame.HeightRequest = height / 180;
            //passFrame.HeightRequest = height / 180;
            //passFrame2.HeightRequest = height / 180;

            //loginButton.HeightRequest = height / 35;
            //signUpButton.HeightRequest = height / 35;
            //loginButton.WidthRequest = width / 4;
            //signUpButton.WidthRequest = width / 4;
            //loginButton.CornerRadius = (int)(height / 70);
            //signUpButton.CornerRadius = (int)(height / 70);
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        void clickedReset(System.Object sender, System.EventArgs e)
        {
            //if any entries empty, return
            //if passwords not matching
            if (password.Text == "" || password.Text == null)
            { //1
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    DisplayAlert("Error", "Please enter your new password", "OK");
                }
                return;
            }
            else if (password2.Text == "" || password2.Text == null)
            { //1
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    DisplayAlert("Error", "Please re-enter your new password", "OK");
                }
                return;
            }
            else if (password.Text != password2.Text)
            { //41
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    DisplayAlert(obj.result[40].title, obj.result[40].message, obj.result[40].responses);
                }
                catch
                {
                    DisplayAlert("Error", "passwords don't match", "close");
                }
                return;
            }

            changePasswordPost changePass = new changePasswordPost();

            Application.Current.MainPage = new MainPage();
        }

        void clickedSeeCurrentPassword(System.Object sender, System.EventArgs e)
        {
            if (currentPassword.IsPassword == true)
                currentPassword.IsPassword = false;
            else currentPassword.IsPassword = true;
        }

        void clickedSeePassword(System.Object sender, System.EventArgs e)
        {
            if (password.IsPassword == true)
                password.IsPassword = false;
            else password.IsPassword = true;
        }

        void clickedSeePassword2(System.Object sender, System.EventArgs e)
        {
            if (password2.IsPassword == true)
                password2.IsPassword = false;
            else password2.IsPassword = true;
        }

        private async void clickedLogin(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(currentPassword.Text) || String.IsNullOrEmpty(password.Text) || String.IsNullOrEmpty(password2.Text))
            { // check if all fields are filled out //1
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    await DisplayAlert(obj.result[0].title, obj.result[0].message, obj.result[0].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "Please fill in all fields", "OK");
                }
            }
            else if (password.Text != password2.Text)
            {//41
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    await DisplayAlert(obj.result[40].title, obj.result[40].message, obj.result[40].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "New passwords don't match", "OK");
                }

                
            }    
            else
            {
                var accountSalt = await retrieveAccountSalt(userEmail.ToLower().Trim());

                if (accountSalt != null)
                {
                    var loginAttempt = await LogInUser(userEmail.ToLower(), currentPassword.Text, accountSalt);

                    if (loginAttempt != null && loginAttempt.message != "Request failed, wrong password.")
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

                                savePassword();

                                MainPage clientMP = new MainPage();
                                //MainPage = clientMP;

                                clientMP.navToSub(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email);
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
                            if ((info_obj2["result"]).ToString() == "[]" || (info_obj2["result"]).ToString() == "204")
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

                                savePassword();

                                MainPage clientMP = new MainPage();
                                clientMP.navToSub(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email);
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
                                savePassword();

                                MainPage clientMP = new MainPage();
                                clientMP.navToSelect(loginAttempt.result[0].customer_first_name, loginAttempt.result[0].customer_last_name, loginAttempt.result[0].customer_email);
                            }
                        }
                    }
                    else
                    { //21
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content3 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                            await DisplayAlert(obj.result[20].title, obj.result[20].message, obj.result[20].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Error", "Wrong password was entered", "OK");
                        }
                    }
                }
            }
        }

        private async Task<AccountSalt> retrieveAccountSalt(string userEmail)
        {
            try
            {
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
                        createAccount = true;
                        System.Diagnostics.Debug.WriteLine(DRSMessage);
                        await DisplayAlert("Oops!", data.message, "OK");
                    }
                    else if (DRSMessage.Contains(Constant.EmailNotFound))
                    { //15
                        try
                        {
                            WebClient client4 = new WebClient();
                            var content3 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                            await DisplayAlert(obj.result[14].title, obj.result[14].message, obj.result[14].responses);
                        }
                        catch
                        {
                            await DisplayAlert("Oops!", "Our records show that you don't have an account. Please sign up!", "OK");
                        }
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
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
                System.Diagnostics.Debug.WriteLine(message);

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
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception message: " + e.Message);
                return null;
            }
        }

        async void savePassword()
        {
            PasswordInfo passwordUpdate = new PasswordInfo();

            //customer_uid, old_password, new_password
            passwordUpdate.customer_uid = (string)Application.Current.Properties["user_id"];

            SHA512 sHA512 = new SHA512Managed();
            byte[] data = sHA512.ComputeHash(Encoding.UTF8.GetBytes(password.Text + Preferences.Get("password_salt", ""))); // take the password and account salt to generate hash
            string hashedPassword = BitConverter.ToString(data).Replace("-", string.Empty).ToLower(); // convert hash to hex
            //passwordUpdate.old_password = hashedPassword;

            if (password.Text == password2.Text)
            {
                //passwordUpdate.old_password = Preferences.Get("hashed_password", "");
                passwordUpdate.old_password = Preferences.Get("user_password", "");
                //passwordUpdate.new_password = hashedPassword;
                passwordUpdate.new_password = password.Text;
                var newPaymentJSONString = JsonConvert.SerializeObject(passwordUpdate);
                // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
                var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("Content: " + content2);
                var client = new HttpClient();
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_password", content2);
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    await DisplayAlert(obj.result[10].title, obj.result[10].message, obj.result[10].responses);
                }
                catch
                {
                    await DisplayAlert("Success", "password updated!", "continue");
                } //40
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedSave Func ENDED!");
            }
            else
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    await DisplayAlert(obj.result[40].title, obj.result[40].message, obj.result[40].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "passwords don't match", "close");
                }
            }



            //await Navigation.PushAsync(new UserProfile(), false);
        }
    }
}
