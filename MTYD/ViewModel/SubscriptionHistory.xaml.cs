using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
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
        Dictionary<string, Dictionary<string, List<HistorySel>>> historyDict = new Dictionary<string, Dictionary<string, List<HistorySel>>>();
        History subHistJson;

        ObservableCollection<PlanName> namesColl = new ObservableCollection<PlanName>();

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

            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/subscription_history/" + userID;
            Debug.WriteLine("url to be reached: " + url);
            var content2 = client2.DownloadString(url);
            var obj2 = JsonConvert.DeserializeObject<History>(content2);
            subHistJson = obj2;
            Debug.WriteLine("content from sub history: " + content2.ToString());

            for (int i = 0; i < obj2.Result.Length; i++)
            {

                if (obj2.Result[i].PurchStatus == "ACTIVE" && obj2.Result[i].PayTimeStamp != null && historyDict.ContainsKey(obj2.Result[i].PurchId) == false)
                {
                    //if (nextBillingLabel.Text == "TBD")
                    //{
                    //    var nextBill = new DateTime(int.Parse(obj2.Result[i].NextBillingDate.Substring(0, 4)), int.Parse(obj2.Result[i].NextBillingDate.Substring(5, 2)), int.Parse(obj2.Result[i].NextBillingDate.Substring(8, 2)));
                    //    nextBillingLabel.Text =  nextBill.ToString("D").Substring(nextBill.ToString("D").IndexOf(" ") + 1);
                    //}
                    //SubHist newSubHist = new SubHist();
                    //var date1 = new DateTime(int.Parse(obj2.Result[i].SelMenuDate.Substring(0, 4)), int.Parse(obj2.Result[i].SelMenuDate.Substring(5, 2)), int.Parse(obj2.Result[i].SelMenuDate.Substring(8, 2)));
                    //Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
                    //newSubHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);

                        //List<string> newList = new List<string>();
                        //newList.Add(obj2.Result[i].SelMenuDate);
                        //historyDict.Add(obj2.Result[i].PurchId, newList);
                        //holds the objects with all of the meal info
                    List<HistorySel> newHistList = new List<HistorySel>();
                    newHistList.Add(obj2.Result[i]);
                    //key = menu date, value = json objects that hold the meal info for a certain menu date
                    Dictionary<string, List<HistorySel>> newHistDict = new Dictionary<string, List<HistorySel>>();
                    newHistDict.Add(obj2.Result[i].PayTimeStamp, newHistList);
                    //key = purch id, value = (by purch id) dictionary with meal info sorted by menu dates (the key)
                    historyDict.Add(obj2.Result[i].PurchId, newHistDict);
                }
                else if (obj2.Result[i].PurchStatus == "ACTIVE" && obj2.Result[i].PayTimeStamp != null && historyDict.ContainsKey(obj2.Result[i].PurchId) == true)
                {
                    //if (nextBillingLabel.Text == "TBD")
                    //{
                    //    var nextBill = new DateTime(int.Parse(obj2.Result[i].NextBillingDate.Substring(0, 4)), int.Parse(obj2.Result[i].NextBillingDate.Substring(5, 2)), int.Parse(obj2.Result[i].NextBillingDate.Substring(8, 2)));
                    //    nextBillingLabel.Text = nextBill.ToString("D").Substring(nextBill.ToString("D").IndexOf(" ") + 1);
                    //}

                    if (historyDict[obj2.Result[i].PurchId].ContainsKey(obj2.Result[i].PayTimeStamp))
                    {
                        historyDict[obj2.Result[i].PurchId][obj2.Result[i].PayTimeStamp].Add(obj2.Result[i]);
                    }
                    else
                    {
                        List<HistorySel> newHistList = new List<HistorySel>();
                        newHistList.Add(obj2.Result[i]);
                        historyDict[obj2.Result[i].PurchId].Add(obj2.Result[i].PayTimeStamp, newHistList);

                    }
                    //historyDict[obj2.Result[i].PurchId].Add(obj2.Result[i].SelMenuDate);
                }
            }

            //List<string> selectedList = historyDict["400-000001"];
            //for (int i = 0; i < selectedList.Count; i++)
            //{
            //    Debug.WriteLine(i + " index in selected list: " + selectedList[i]);
            //}


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
                            PlanName newPlan = new PlanName();
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
                            newPlan.name = name + qty + " : " + m["purchase_id"].ToString().Substring(m["purchase_id"].ToString().IndexOf("-") + 1);
                            namesColl.Add(newPlan);
                            namesArray.Add(name + qty + " : " + m["purchase_id"].ToString().Substring(m["purchase_id"].ToString().IndexOf("-") + 1));
                        }

                        Debug.WriteLine(m["items"].ToString());
                        //purchUidArray.Add((m["purchase_uid"].ToString()));
                    }
                }

                //dropDownList.ItemsSource = namesArray;
                dropDownList.ItemsSource = namesColl;
                dropDownList.SelectedItem = namesColl[0];
                //dropDownList.SelectedItem = namesArray[0].ToString();
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

            int selectedIndex = -1;
            //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
            foreach (var plan in namesColl)
            {
                selectedIndex++;
                if (plan.name == dropDownText.Text)
                {
                    break;
                }
            }
            //int selectedIndex = ((ObservableCollection<PlanName>)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
            string selectedPurchId = (string)purchIdArray[selectedIndex];
            string userID = (string)Application.Current.Properties["user_id"];


            ////sample https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=100-000127&purchase_id=400-000189
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_with_billing?customer_uid=" + userID + "&purchase_id=" + selectedPurchId;
            //Debug.WriteLine("url to be reached: " + url);
            //var content2 = client2.DownloadString(url);
            //var obj2 = JsonConvert.DeserializeObject<MealsSelected2>(content2);
            //Debug.WriteLine("content from meals_selected_with_billing: " + content2.ToString());

            //for (int i = 0; i < obj2.Result.Length; i++)
            //{
            //    if (obj2.Result[i].PurchaseStatus == "ACTIVE" && dateArray.IndexOf(obj2.Result[i].MenuDate) == -1)
            //    {
            //        dateArray.Add(obj2.Result[i].MenuDate);
            //    }
            //}
            //dateArray.Reverse();

            int dateIndex = 0;
            string mostRecentMenuDate = "";
            int numMealsDeliv = 0;

            //extra subHist for testing
            SubHist subHist_test = new SubHist();
            ObservableCollection<Meals> mealsColl2 = new ObservableCollection<Meals>();
            Meals m2 = new Meals();
            m2.qty = "3";
            m2.mealName = "YES";
            mealsColl2.Add(m2);
            mealsColl2.Add(m2);
            subHist_test.Date = "Filler Date";
            subHist_test.mealPlanName = "Filler Meals, Deliveries";
            subHist_test.mealColl = mealsColl2;
            subHist_test.mealCollHeight = mealsColl2.Count * 60;
            subHist_test.CollVisible = false;
            subHist_test.mainGridVisible = false;
            subHistColl.Add(subHist_test);

            //each value is the dictionary with the menu date key, meal list value
            foreach (var value in historyDict[selectedPurchId])
            //foreach (var value in historyDict["400-000001"])
            {
                var nextBill = new DateTime(int.Parse((value.Value)[0].NextBillingDate.Substring(0, 4)), int.Parse((value.Value)[0].NextBillingDate.Substring(5, 2)), int.Parse((value.Value)[0].NextBillingDate.Substring(8, 2)));
                nextBillingLabel.Text = nextBill.ToString("D").Substring(nextBill.ToString("D").IndexOf(" ") + 1);

                //nextBillingLabel.Text = (value.Value)[0].NextBillingDate;
                SubHist subHist = new SubHist();
                SubHist subHist2 = new SubHist();
                var date1 = new DateTime(int.Parse(value.Key.Substring(0, 4)), int.Parse(value.Key.Substring(5, 2)), int.Parse(value.Key.ToString().Substring(8, 2)));
                Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
                subHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);
                subHist2.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);

                ObservableCollection<Meals> mealsColl = new ObservableCollection<Meals>();
                ObservableCollection<Meals> mealsColl3 = new ObservableCollection<Meals>();

                JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>((value.Value)[0].Items);
                Debug.WriteLine("obj2.Result[i].Items: " + subHistJson.Result[0].Items.ToString());
                Debug.WriteLine("newobj2: " + newobj2.ToString());
                Debug.WriteLine("newobj2 object: " + newobj2[0].ToString());

                foreach (JObject config in newobj2)
                {
                    Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                    string qty = (string)config["qty"];
                    string name = (string)config["name"];
                    Debug.WriteLine("quantity: " + qty);
                    Debug.WriteLine("name: " + name);
                    name = name.Substring(0, name.IndexOf(" "));
                    name = name + " Meals, ";
                    qty = qty + " Deliveries  ▼";
                    //qty = qty + " Deliveries  ▲";

                    subHist.mealPlanName = name + qty;
                }

                //list of meals
                for (int i = 0; i < value.Value.Count; i++)
                {
                    try
                    {
                        if ((value.Value)[i].MealName == null && (value.Value)[i].MealDesc != "SURPRISE")
                            continue;
                        DateTime currentTime = DateTime.Now;

                        var deliv = new DateTime(int.Parse((value.Value)[i].StartDelivDate.Substring(0, 4)), int.Parse((value.Value)[i].StartDelivDate.Substring(5, 2)), int.Parse((value.Value)[i].StartDelivDate.Substring(8, 2)));
                        var selmenu = new DateTime(int.Parse((value.Value)[i].SelMenuDate.Substring(0, 4)), int.Parse((value.Value)[i].SelMenuDate.Substring(5, 2)), int.Parse((value.Value)[i].SelMenuDate.Substring(8, 2)));
                        if (String.Compare(deliv.ToString("u").Substring(0, deliv.ToString("u").IndexOf(" ")), currentTime.ToString("u").Substring(0, currentTime.ToString("u").IndexOf(" "))) >= 0 ||
                            String.Compare(selmenu.ToString("u").Substring(0, selmenu.ToString("u").IndexOf(" ")), currentTime.ToString("u").Substring(0, currentTime.ToString("u").IndexOf(" "))) >= 0)
                            continue;

                        Meals m1 = new Meals();
                        if ((value.Value)[i].SelMenuDate != mostRecentMenuDate)
                        {
                            mostRecentMenuDate = (value.Value)[i].SelMenuDate;
                            m1.DelivDateVisible = true;
                            numMealsDeliv++;
                        }
                        else m1.DelivDateVisible = false;

                        if ((value.Value)[i].MealDesc == "SURPRISE")
                        {
                            m1.DelivDate = selmenu.ToString("D").Substring(selmenu.ToString("D").IndexOf(" ") + 1);
                            m1.mealName = "SURPRISE";
                            m1.qty = subHist.mealPlanName.Substring(0, 1);
                        }
                        else
                        {
                            m1.DelivDate = selmenu.ToString("D").Substring(selmenu.ToString("D").IndexOf(" ") + 1);
                            m1.qty = (value.Value)[i].MealQty;
                            m1.mealName = (value.Value)[i].MealName;
                            m1.urlLink = (value.Value)[i].MealPhotoUrl;
                        }
                        mealsColl.Add(m1);
                        mealsColl3.Add(m1);
                    }
                    catch
                    {
                        Debug.WriteLine("bad meal caught");
                        //try catch block for the objects that don't have any meal info
                    }
                }

                
                //old location
                //JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>((value.Value)[0].Items);
                //Debug.WriteLine("obj2.Result[i].Items: " + subHistJson.Result[0].Items.ToString());
                //Debug.WriteLine("newobj2: " + newobj2.ToString());
                //Debug.WriteLine("newobj2 object: " + newobj2[0].ToString());

                //foreach (JObject config in newobj2)
                //{
                //    Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                //    string qty = (string)config["qty"];
                //    string name = (string)config["name"];
                //    Debug.WriteLine("quantity: " + qty);
                //    Debug.WriteLine("name: " + name);
                //    name = name.Substring(0, name.IndexOf(" "));
                //    name = name + " Meals, ";
                //    qty = qty + " Deliveries  ▼";
                //    //qty = qty + " Deliveries  ▲";

                //    subHist.mealPlanName = name + qty;
                //}

                subHist.mealCollHeight = (mealsColl.Count * 70) + (numMealsDeliv * 55);
                Debug.WriteLine("mealcoll height: " + subHist.mealCollHeight.ToString());
                subHist.mealColl = mealsColl;
                subHist.CollVisible = false;
                subHist.mainGridVisible = true;
                subHist2.CollVisible = false;
                subHist2.mainGridVisible = true;
                subHistColl.Add(subHist);
                subHist2.mealPlanName = "Second";
                subHist2.mealCollHeight = mealsColl3.Count * 70;
                subHist2.mealColl = mealsColl3;
                //subHistColl.Add(subHist2);
                //subHistColl.Add(subHist);
                //subHistColl.Add(subHist);
            }

            SubHist subHist_test2 = new SubHist();
            subHist_test2.mealCollHeight = mealsColl2.Count * 60;
            subHist_test2.mealColl = mealsColl2;
            subHist_test2.CollVisible = false;
            subHist_test2.mainGridVisible = false;
            subHistColl.Add(subHist_test2);
            subHistColl.Add(subHist_test2);

            //for (int j = 0; j < dateArray.Count; j++)
            //{
            //    if (String.Compare(obj2.NextBilling.MenuDate, (string)dateArray[j]) < 0)
            //        continue;

            //    SubHist subHist = new SubHist();

            //    var billDate = new DateTime(int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(0, 4)), int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(5, 2)), int.Parse(obj2.NextBilling.MenuDate.ToString().Substring(8, 2)));
            //    //subHist.Date = billDate.ToString("D").Substring(billDate.ToString("D").IndexOf(" ") + 1);
            //    Debug.WriteLine("next billing date: " + obj2.NextBilling.MenuDate);
            //    nextBillingLabel.Text = billDate.ToString("D").Substring(billDate.ToString("D").IndexOf(" ") + 1);
            //    //Debug.WriteLine("next billing date: " + dateArray[j]);
            //    var date1 = new DateTime(int.Parse(dateArray[j].ToString().Substring(0, 4)), int.Parse(dateArray[j].ToString().Substring(5, 2)), int.Parse(dateArray[j].ToString().Substring(8, 2)));
            //    Debug.WriteLine("formatted next billing date: " + date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1));
            //    subHist.Date = date1.ToString("D").Substring(date1.ToString("D").IndexOf(" ") + 1);
            //    subHist.CollVisible = false;
            //    ObservableCollection<Meals> mealsColl = new ObservableCollection<Meals>();
            //    for (int i = 0; i < obj2.Result.Length; i++)
            //    {
            //        if (obj2.Result[i].MenuDate == (string)dateArray[j])
            //        {
            //            Debug.WriteLine("entered");
            //            try
            //            {
            //                JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].CombinedSelection);


            //                foreach (JObject config in newobj)
            //                {
            //                    Meals m1 = new Meals();
            //                    m1.qty = (string)config["qty"];
            //                    m1.mealName = (string)config["name"];
            //                    Debug.WriteLine("this was tracked: " + (string)config["name"] + " : " + (string)config["qty"]);
            //                    mealsColl.Add(m1);
            //                }
            //            }
            //            catch
            //            {
            //                Debug.WriteLine("caught from processing combined selection");
            //            }

            //            subHist.mainGridVisible = true;
            //            //get the name of the meal plan
            //            JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(obj2.Result[i].Items);
            //            Debug.WriteLine("obj2.Result[i].Items: " + obj2.Result[i].Items.ToString());
            //            Debug.WriteLine("newobj2: " + newobj2.ToString());
            //            Debug.WriteLine("newobj2 object: " + newobj2[0].ToString());
            //            foreach (JObject config in newobj2)
            //            {
            //                Console.WriteLine("Inside foreach loop in GetmealsPlan func");
            //                string qty = (string)config["qty"];
            //                string name = (string)config["name"];
            //                Debug.WriteLine("quantity: " + qty);
            //                Debug.WriteLine("name: " + name);
            //                name = name.Substring(0, name.IndexOf(" "));
            //                name = name + " Meals, ";
            //                qty = qty + " Deliveries";
            //                subHist.mealPlanName = name + qty;
            //            }

            //            subHist.mealCollHeight = mealsColl.Count * 60;
            //            break;
            //        }
            //    }

            //    subHist.mealColl = mealsColl;
            //    subHistColl.Add(subHist);
            //}

            weekOneMenu.HeightRequest = subHistColl.Count * 110;
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
                sh.mealPlanName = sh.mealPlanName.Substring(0, sh.mealPlanName.IndexOf("▼")) + "▲";
                sh.CollVisible = true;
            }
            else
            {
                weekOneMenu.HeightRequest -= sh.mealCollHeight + 20;
                sh.mealPlanName = sh.mealPlanName.Substring(0, sh.mealPlanName.IndexOf("▲")) + "▼";
                sh.CollVisible = false;
            }
        }

        async void clickedExpand(System.Object sender, System.EventArgs e)
        {
            if (dropDownList.IsVisible == false)
            {
                connect.IsVisible = true;
                dropDownList.IsVisible = true;
            }
            else
            {
                connect.IsVisible = false;
                dropDownList.IsVisible = false;
            }

        }

        void ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            Debug.WriteLine("itemselected entered");
            //dropDownText.Text = (string)dropDownList.SelectedItem;
            dropDownText.Text = ((PlanName)dropDownList.SelectedItem).name;
            //dropDownText.Text = "hello";
            //Debug.WriteLine("addy index selected: " + ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text));
            connect.IsVisible = false;
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

        async void clickedSubHistory(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionHistory(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        void xButtonClicked(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            baaPopUpGrid.IsVisible = false;
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
