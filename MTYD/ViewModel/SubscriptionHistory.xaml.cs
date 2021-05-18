using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using MTYD.Model;
using MTYD.Model.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class SubscriptionHistory : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        ArrayList namesArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        ArrayList dateArray = new ArrayList();
        WebClient client2 = new WebClient();
        public ObservableCollection<SubHist> subHistColl = new ObservableCollection<SubHist>();

        public SubscriptionHistory(string firstName, string lastName, string email)
        {
            try
            {
                //_ = CheckVersion();

                Console.WriteLine("subscription page");
                cust_firstName = firstName;
                cust_lastName = lastName;
                if (lastName == "")
                    Debug.WriteLine("caught parameter");
                if (cust_lastName == "")
                    Debug.WriteLine("caught variable");
                cust_email = email;
                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                InitializeComponent();


                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);

                checkPlatform(height, width);

                getPlans();
                //GetDeliveryDates();
                //GetPlans();
                ////Preferences.Set("freqSelected", "");
                //pfp.Source = Preferences.Get("profilePicLink", "");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        void checkPlatform(double height, double width)
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                //open menu adjustments
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = 140;
                menu.WidthRequest = 40;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                heading.WidthRequest = 140;
                //heading adjustments

                orangeBox.HeightRequest = height / 2.3;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                //heading.WidthRequest = width / 3;
                heading.WidthRequest = 140;
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

                //#F8BB17
                //#F8BB17
                //menu.Margin = new Thickness(25, 0, 0, 30);
                menu.Margin = new Thickness(25, 0, 0, 30);
                //menu.HeightRequest = width / 18;
                menu.WidthRequest = 40;
                //back.Margin = new Thickness(25, 0, 0, 30);
                //back.HeightRequest = 25;
                //back.WidthRequest = width / 18;
                menu2.WidthRequest = 40;
                menu2.Margin = new Thickness(25, 0, 0, 30);
            }
            else //android
            {
                //open menu adjustments
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = width / 5.3;
                menu.WidthRequest = 40;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                //open menu adjustments

                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;

                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;
                pfp.Margin = new Thickness(0, 0, 23, 35);

                //menu.HeightRequest = width / 30;
                menu.WidthRequest = 40;
                menu.Margin = new Thickness(25, 0, 0, 40);
                //back.HeightRequest = 25;
                ////back.WidthRequest = width / 30;
                //back.Margin = new Thickness(25, 0, 0, 40);
            }

            //common adjustments regardless of platform
        }

        async void getPlans()
        {

            var request = new HttpRequestMessage();
            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("Inside GET MEAL PLANS: User ID:  " + userID);

            //sample: https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=100-000119
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            Console.WriteLine("GET MEALS PLAN ENDPOINT TRYING TO BE REACHED: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            Debug.WriteLine("get meal plans response: " + response.ToString());
            //Debug.WriteLine("get meal plans content: " + response.Content.ToString());

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject mealPlan_obj = JObject.Parse(userString);

                
                foreach (var m in mealPlan_obj["result"])
                {
                    Console.WriteLine("In first foreach loop of getmeal plans func:");
                    if (m["purchase_status"].ToString() == "ACTIVE")
                    {
                        //itemsArray.Add((m["items"].ToString()));
                        purchIdArray.Add((m["purchase_id"].ToString()));
                        JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(m["items"].ToString());

                        foreach (JObject config in newobj)
                        {
                            Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];

                            name = name.Substring(0, name.IndexOf(" "));
                            name = name + " Meals, ";
                            qty = qty + " Deliveries";
                            //while (purchIdCurrent.Substring(0, 1) == "0")
                            //    purchIdCurrent = purchIdCurrent.Substring(1);

                            //only includes meal plan name
                            //namesArray.Add(name);

                            //adds purchase uid to front of meal plan name
                            //namesArray.Add(purchIdArray[i].ToString().Substring(4) + " : " + name);
                            namesArray.Add(name + qty + " : " + m["purchase_id"].ToString().Substring(m["purchase_id"].ToString().IndexOf("-") + 1));
                        }

                        Debug.WriteLine(m["items"].ToString());
                        //purchUidArray.Add((m["purchase_uid"].ToString()));
                    }
                }

                dropDownList.ItemsSource = namesArray;
                dropDownList.SelectedItem = namesArray[0].ToString();
            }

            //planChange();

            //foreach (string purchId in purchIdArray)
            //{
            //    //sample https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=100-000127&purchase_id=400-000189
            //    string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=" + userID + "&purchase_id=" + purchId;
            //    Debug.WriteLine("url to be reached: " + url);
            //    var content2 = client2.DownloadString(url);
            //    var obj2 = JsonConvert.DeserializeObject<MealsSelected2>(content2);

            //    SubHist subHist = new SubHist();

            //    Debug.WriteLine("next billing date: " + obj2.NextBilling.MenuDate);
            //    var date1 = new DateTime(int.Parse(obj2.NextBilling.MenuDate.Substring(0, 4)), int.Parse(obj2.NextBilling.MenuDate.Substring(5, 2)), int.Parse(obj2.NextBilling.MenuDate.Substring(8, 2)));
            //    Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
            //    subHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);

            //    ObservableCollection<Meals> mealsColl = new ObservableCollection<Meals>();
            //    for (int i = 0; i < obj2.Result.Length; i++)
            //    {
            //        Debug.WriteLine("entered");
            //        JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].CombinedSelection);

            //        foreach (JObject config in newobj)
            //        {
            //            Meals m1 = new Meals();
            //            m1.mealName = (string)config["name"];
            //            mealsColl.Add(m1);
            //        }
            //    }

            //    subHist.mealColl = mealsColl;
            //    subHistColl.Add(subHist);
            //}

            //weekOneMenu.ItemsSource = subHistColl;
        }

        async void planChange()
        {
            dateArray.Clear();
            subHistColl.Clear();

            int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
            string selectedPurchId = (string)purchIdArray[selectedIndex];
            string userID = (string)Application.Current.Properties["user_id"];


            //sample https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=100-000127&purchase_id=400-000189
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=" + userID + "&purchase_id=" + selectedPurchId;
            Debug.WriteLine("url to be reached: " + url);
            var content2 = client2.DownloadString(url);
            var obj2 = JsonConvert.DeserializeObject<MealsSelected2>(content2);

            for (int i = 0; i < obj2.Result.Length; i++)
            {
                if (obj2.Result[i].PurchaseStatus == "ACTIVE" && dateArray.IndexOf(obj2.Result[i].SelMenuDate) == -1)
                {
                    dateArray.Add(obj2.Result[i].SelMenuDate);
                }
            }
            dateArray.Reverse();

            int dateIndex = 0;

            for (int j = 0; j < dateArray.Count; j++)
            {
                if (String.Compare(obj2.NextBilling.MenuDate, (string)dateArray[j]) < 0)
                    continue;

                SubHist subHist = new SubHist();

                var billDate = new DateTime(int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(0, 4)), int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(5, 2)), int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(8, 2)));
                //subHist.Date = billDate.ToString("D").Substring(billDate.ToString("D").IndexOf(" ") + 1);
                Debug.WriteLine("next billing date: " + obj2.NextBilling.MenuDate);
                nextBillingLabel.Text = "Next Billing Date: " + billDate.ToString("D").Substring(billDate.ToString("D").IndexOf(" ") + 1);
                //Debug.WriteLine("next billing date: " + dateArray[j]);
                var date1 = new DateTime(int.Parse(dateArray[j].ToString().Substring(0, 4)), int.Parse(dateArray[j].ToString().Substring(5, 2)), int.Parse(dateArray[j].ToString().Substring(8, 2)));
                Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
                subHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);
                subHist.CollVisible = false;
                ObservableCollection<Meals> mealsColl = new ObservableCollection<Meals>();
                for (int i = 0; i < obj2.Result.Length; i++)
                {
                    if (obj2.Result[i].SelMenuDate == (string)dateArray[j])
                    {
                        Debug.WriteLine("entered");
                        JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].CombinedSelection);

                        foreach (JObject config in newobj)
                        {
                            Meals m1 = new Meals();
                            m1.qty = (string)config["qty"];
                            m1.mealName = (string)config["name"];
                            mealsColl.Add(m1);
                        }

                        //get the name of the meal plan
                        JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].Items);

                        foreach (JObject config in newobj2)
                        {
                            Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];

                            name = name.Substring(0, name.IndexOf(" "));
                            name = name + " Meals, ";
                            qty = qty + " Deliveries";
                            subHist.mealPlanName = name + qty;
                        }

                        subHist.mealCollHeight = mealsColl.Count * 50;
                        break;
                    }
                }

                subHist.mealColl = mealsColl;
                subHistColl.Add(subHist);
            }

            weekOneMenu.HeightRequest = subHistColl.Count * 120;
            //SubHist subHist = new SubHist();

            //Debug.WriteLine("next billing date: " + obj2.NextBilling.MenuDate);
            //var date1 = new DateTime(int.Parse(obj2.NextBilling.MenuDate.Substring(0, 4)), int.Parse(obj2.NextBilling.MenuDate.Substring(5, 2)), int.Parse(obj2.NextBilling.MenuDate.Substring(8, 2)));
            //Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
            //subHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);

            //ObservableCollection<Meals> mealsColl = new ObservableCollection<Meals>();
            //for (int i = 0; i < obj2.Result.Length; i++)
            //{
            //    Debug.WriteLine("entered");
            //    JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].CombinedSelection);

            //    foreach (JObject config in newobj)
            //    {
            //        Meals m1 = new Meals();
            //        m1.mealName = (string)config["name"];
            //        mealsColl.Add(m1);
            //    }
            //}

            //subHist.mealColl = mealsColl;
            //subHistColl.Add(subHist);

            weekOneMenu.ItemsSource = subHistColl;
        }

        void clickedSeeMeals(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            SubHist sh = b.BindingContext as SubHist;

               

            if (sh.CollVisible == false)
            {
                weekOneMenu.HeightRequest += sh.mealCollHeight + 20;
                sh.CollVisible = true;
            }
            else
            {
                weekOneMenu.HeightRequest -= sh.mealCollHeight + 20;
                sh.CollVisible = false;
            }
        }

        async void clickedExpand(System.Object sender, System.EventArgs e)
        {
            if (dropDownList.IsVisible == false)
                dropDownList.IsVisible = true;
            else dropDownList.IsVisible = false;

        }

        void ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            dropDownText.Text = (string)dropDownList.SelectedItem;
            //Debug.WriteLine("addy index selected: " + ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text));

            dropDownList.IsVisible = false;
            planChange();
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
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
    }
}
