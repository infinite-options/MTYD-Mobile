using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    public partial class SubscriptionModal : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewPlan = new ObservableCollection<Plans>();

        double m1price_f1 = 0.0; double m1price_f2 = 0.0; double m1price_f3 = 0.0; double m2price_f1 = 0.0; double m2price_f2 = 0.0; double m2price_f3 = 0.0;
        double m3price_f1 = 0.0; double m3price_f2 = 0.0; double m3price_f3 = 0.0; double m4price_f1 = 0.0; double m4price_f2 = 0.0; double m4price_f3 = 0.0;
        string m1f1name = "", m1f2name = "", m1f3name = "", m2f1name = "", m2f2name = "", m2f3name = "", m3f1name = "", m3f2name = "", m3f3name = "", m4f1name = "", m4f2name = "", m4f3name = "";
        string m1f1uid = "", m1f2uid = "", m1f3uid = "", m2f1uid = "", m2f2uid = "", m2f3uid = "", m3f1uid = "", m3f2uid = "", m3f3uid = "", m4f1uid = "", m4f2uid = "", m4f3uid = "";

        string socialLogin; string refresh_token; string cc_num; string cc_exp_year; string cc_exp_month; string cc_cvv; string purchase_id;
        string new_item_id; string customer_id; string itm_business_uid; string cc_zip; string cc_exp_date; string qty; string numMeal;
        string passedAdd, passedUnit, passedCity, passedState, passedZip;

        int mealSelected;
        int deliverySelected;
        double[,] discounts;
        double[,] itemPrices;
        String[,] itemNames, itemUids;
        double total;
        WebClient client4 = new WebClient();
        double taxRate = 0;

        public object NavigationStack { get; private set; }


        public SubscriptionModal(string firstName, string lastName, string email, string social, string token, string num, string expDate, string cvv, string zip, string purchaseID, string businessID, string itemID, string customerID, string quantity, string numOfMeals, string add, string unit, string city, string state, string zipDeliv)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            Console.WriteLine("SubscriptionModal entered");
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            Console.WriteLine("next entered");
            socialLogin = social; refresh_token = token; cc_num = num; cc_exp_date = expDate; cc_cvv = cvv; purchase_id = purchaseID;
            new_item_id = itemID; customer_id = customerID; cc_zip = zip; itm_business_uid = businessID; qty = quantity; numMeal = numOfMeals;
            passedAdd = add.Trim(); passedUnit = unit.Trim(); passedCity = city.Trim(); passedState = state.Trim(); passedZip = zipDeliv.Trim();
            Debug.WriteLine("new_item_id: " + new_item_id);

            Debug.WriteLine("Address passed in: " + passedAdd + ", unit: " + passedUnit + ", " + passedCity + ", " + passedState + " " + passedZip);

            Console.WriteLine("next2 entered");

            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            GetDeliveryDates();
            GetPlans();
            Preferences.Set("freqSelected", "");
        }


        protected async Task GetPlans()
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
                this.NewPlan.Clear();

                ArrayList numMealsList = new ArrayList();
                int i = 0, j = 0;
                discounts = new double[10, 5];
                itemPrices = new double[10, 5];
                itemNames = new String[10, 5];
                itemUids = new String[10, 5];
                foreach (var m in plan_obj["result"])
                {
                    int num_meals = int.Parse(m["num_items"].ToString());
                    if (!numMealsList.Contains(num_meals))
                    {
                        numMealsList.Add(num_meals);
                    }
                    discounts[i, j] = double.Parse(m["delivery_discount"].ToString());
                    itemPrices[i, j] = double.Parse(m["item_price"].ToString());
                    itemNames[i, j] = m["item_name"].ToString();
                    itemUids[i, j] = m["item_uid"].ToString();
                    Debug.WriteLine("received item_uid: " + m["item_uid"].ToString());
                    Debug.WriteLine("item name: " + m["item_name"].ToString());
                    //if (itemUids[i, j] == new_item_id)
                    //{
                    //    Debug.WriteLine("row (# of deliveries):" + i.ToString());
                    //    Debug.WriteLine("column (# of meals):" + j.ToString());
                    //    EventArgs ev = new EventArgs();
                    //    if (qty == "1")
                    //        clickedDeliveryNum(delivery1, ev);
                    //    else if (qty == "2")
                    //        clickedDeliveryNum(delivery2, ev);
                    //    else if (qty == "3")
                    //        clickedDeliveryNum(delivery3, ev);
                    //    else if (qty == "4")
                    //        clickedDeliveryNum(delivery4, ev);
                    //    else if (qty == "5")
                    //        clickedDeliveryNum(delivery5, ev);
                    //    else if (qty == "6")
                    //        clickedDeliveryNum(delivery6, ev);
                    //    else if (qty == "7")
                    //        clickedDeliveryNum(delivery7, ev);
                    //    else if (qty == "8")
                    //        clickedDeliveryNum(delivery8, ev);
                    //    else if (qty == "9")
                    //        clickedDeliveryNum(delivery9, ev);
                    //    else clickedDeliveryNum(delivery10, ev);

                    //    if (j == 0)
                    //        clickedMeals1(meals1, ev);
                    //    else if (j == 1)
                    //        clickedMeals2(meals2, ev);
                    //    else if (j == 2)
                    //        clickedMeals3(meals3, ev);
                    //    else if (j == 3)
                    //        clickedMeals4(meals4, ev);
                    //    else clickedMeals5(meals5, ev);

                    //}

                    if (j == 4)
                    {
                        i++;
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                }

                meals1.Text = numMealsList[4].ToString() + " MEALS";
                meals2.Text = numMealsList[3].ToString() + " MEALS";
                meals3.Text = numMealsList[2].ToString() + " MEALS";
                meals4.Text = numMealsList[1].ToString() + " MEALS";
                meals5.Text = numMealsList[0].ToString() + " MEALS";

                //for (int k = 0; k < 5; k++)
                //{
                //    if (itemUids[0, k] == new_item_id)
                //    {
                //        EventArgs ev = new EventArgs();
                //        if (qty == "1")
                //            clickedDeliveryNum(delivery1, ev);
                //        else if (qty == "2")
                //            clickedDeliveryNum(delivery2, ev);
                //        else if (qty == "3")
                //            clickedDeliveryNum(delivery3, ev);
                //        else if (qty == "4")
                //            clickedDeliveryNum(delivery4, ev);
                //        else if (qty == "5")
                //            clickedDeliveryNum(delivery5, ev);
                //        else if (qty == "6")
                //            clickedDeliveryNum(delivery6, ev);
                //        else if (qty == "7")
                //            clickedDeliveryNum(delivery7, ev);
                //        else if (qty == "8")
                //            clickedDeliveryNum(delivery8, ev);
                //        else if (qty == "9")
                //            clickedDeliveryNum(delivery9, ev);
                //        else clickedDeliveryNum(delivery10, ev);

                //        if (k == 0)
                //            clickedMeals1(meals1, ev);
                //        else if (k == 1)
                //            clickedMeals2(meals2, ev);
                //        else if (k == 2)
                //            clickedMeals3(meals3, ev);
                //        else if (k == 3)
                //            clickedMeals4(meals4, ev);
                //        else clickedMeals5(meals5, ev);

                //        return;
                //    }
                //}

                EventArgs ev = new EventArgs();
                if (qty == "1")
                    clickedDeliveryNum(delivery1, ev);
                else if (qty == "2")
                    clickedDeliveryNum(delivery2, ev);
                else if (qty == "3")
                    clickedDeliveryNum(delivery3, ev);
                else if (qty == "4")
                    clickedDeliveryNum(delivery4, ev);
                else if (qty == "5")
                    clickedDeliveryNum(delivery5, ev);
                else if (qty == "6")
                    clickedDeliveryNum(delivery6, ev);
                else if (qty == "7")
                    clickedDeliveryNum(delivery7, ev);
                else if (qty == "8")
                    clickedDeliveryNum(delivery8, ev);
                else if (qty == "9")
                    clickedDeliveryNum(delivery9, ev);
                else clickedDeliveryNum(delivery10, ev);

                if (numMeal == "2")
                    clickedMeals1(meals1, ev);
                else if (numMeal == "3")
                    clickedMeals2(meals2, ev);
                else if (numMeal == "4")
                    clickedMeals3(meals3, ev);
                else if (numMeal == "5")
                    clickedMeals4(meals4, ev);
                else clickedMeals5(meals5, ev);
            }
        }


        void checkPlatform(double height, double width)
        {
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 32;
                heading.Margin = new Thickness(0, 0, 0, 30);
                pfp.HeightRequest = width / 20;
                pfp.WidthRequest = width / 20;
                pfp.CornerRadius = (int)(width / 40);
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

                menu.HeightRequest = width / 20;
                menu.HeightRequest = width / 20;
                menu.Margin = new Thickness(25, 0, 0, 30);

                backButton.HeightRequest = width / 25;
                backButton.WidthRequest = width / 25;
                backButton.Margin = new Thickness(25, 0, 0, 30);

                takeoutGrid.Margin = new Thickness(20, 10, 20, 10);
                takeout.HeightRequest = width / 18;
                takeout.WidthRequest = width / 18;
                numMeals.Margin = new Thickness(25, 10, 0, 10);

                prepay.Margin = new Thickness(30, 0, 0, 0);

                meals1.WidthRequest = width / 5;
                meals1.HeightRequest = width / 20;
                meals1.CornerRadius = (int)(width / 40);
                meals2.WidthRequest = width / 5;
                meals2.HeightRequest = width / 20;
                meals2.CornerRadius = (int)(width / 40);
                meals3.WidthRequest = width / 5;
                meals3.HeightRequest = width / 20;
                meals3.CornerRadius = (int)(width / 40);
                meals4.WidthRequest = width / 5;
                meals4.HeightRequest = width / 20;
                meals4.CornerRadius = (int)(width / 40);
                meals5.WidthRequest = width / 5;
                meals5.HeightRequest = width / 20;
                meals5.CornerRadius = (int)(width / 40);

                delivery1.WidthRequest = width / 10;
                delivery1.HeightRequest = width / 9;
                delivery1Text1.FontSize = width / 50;

                delivery2.WidthRequest = width / 10;
                delivery2.HeightRequest = width / 9;
                delivery2Text1.FontSize = width / 50;
                delivery2Text2.FontSize = width / 65;

                delivery3.WidthRequest = width / 10;
                delivery3.HeightRequest = width / 9;
                delivery3Text1.FontSize = width / 50;
                delivery3Text2.FontSize = width / 65;

                delivery4.WidthRequest = width / 10;
                delivery4.HeightRequest = width / 9;
                delivery4Text1.FontSize = width / 50;
                delivery4Text2.FontSize = width / 65;

                delivery5.WidthRequest = width / 10;
                delivery5.HeightRequest = width / 9;
                delivery5Text1.FontSize = width / 50;
                delivery5Text2.FontSize = width / 65;

                delivery6.WidthRequest = width / 10;
                delivery6.HeightRequest = width / 9;
                delivery6Text1.FontSize = width / 50;
                delivery6Text2.FontSize = width / 65;

                delivery7.WidthRequest = width / 10;
                delivery7.HeightRequest = width / 9;
                delivery7Text1.FontSize = width / 50;
                delivery7Text2.FontSize = width / 65;

                delivery8.WidthRequest = width / 10;
                delivery8.HeightRequest = width / 9;
                delivery8Text1.FontSize = width / 50;
                delivery8Text2.FontSize = width / 65;

                delivery9.WidthRequest = width / 10;
                delivery9.HeightRequest = width / 9;
                delivery9Text1.FontSize = width / 50;
                delivery9Text2.FontSize = width / 65;

                delivery10.WidthRequest = width / 10;
                delivery10.HeightRequest = width / 9;
                delivery10Text1.FontSize = width / 50;
                delivery10Text2.FontSize = width / 65;

                //SignUpButton.HeightRequest = height / 50;
                SignUpButton.WidthRequest = width / 4;
                SignUpButton.HeightRequest = width / 15;
                SignUpButton.CornerRadius = (int)(width / 30);
                SignUpButton.FontSize = width / 50;
            }
            else //android
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 45;
                heading.Margin = new Thickness(0, 0, 0, 40);
                pfp.HeightRequest = width / 25;
                pfp.WidthRequest = width / 25;
                pfp.CornerRadius = (int)(width / 50);
                pfp.Margin = new Thickness(0, 0, 23, 35);
                menu.HeightRequest = width / 30;
                menu.WidthRequest = width / 30;
                menu.Margin = new Thickness(25, 0, 0, 40);

                backButton.HeightRequest = width / 30;
                backButton.WidthRequest = width / 30;
                backButton.Margin = new Thickness(25, 0, 0, 40);

                takeoutGrid.Margin = new Thickness(20, 10, 20, 10);
                takeout.HeightRequest = width / 22;
                takeout.WidthRequest = width / 22;
                deliveryDays.FontSize = width / 47;
                deliveryDays2.FontSize = width / 47;
                numMeals.FontSize = width / 48;
                numMeals.Margin = new Thickness(25, 10, 0, 10);

                prepay.Margin = new Thickness(30, 0, 0, 0);
                prepay.FontSize = width / 48;

                meals1.WidthRequest = width / 7;
                meals1.HeightRequest = height / 60;
                meals1.CornerRadius = (int)(height / 120);
                meals2.WidthRequest = width / 7;
                meals2.HeightRequest = height / 60;
                meals2.CornerRadius = (int)(height / 120);
                meals3.WidthRequest = width / 7;
                meals3.HeightRequest = height / 60;
                meals3.CornerRadius = (int)(height / 120);
                meals4.WidthRequest = width / 7;
                meals4.HeightRequest = height / 60;
                meals4.CornerRadius = (int)(height / 120);
                meals5.WidthRequest = width / 7;
                meals5.HeightRequest = height / 60;
                meals5.CornerRadius = (int)(height / 120);

                delivery1.WidthRequest = width / 13;
                delivery1.HeightRequest = width / 11;
                delivery1Text1.FontSize = width / 50;

                delivery2.WidthRequest = width / 13;
                delivery2.HeightRequest = width / 11;
                delivery2Text1.FontSize = width / 50;
                delivery2Text2.FontSize = width / 70;

                delivery3.WidthRequest = width / 13;
                delivery3.HeightRequest = width / 11;
                delivery3Text1.FontSize = width / 50;
                delivery3Text2.FontSize = width / 70;

                delivery4.WidthRequest = width / 13;
                delivery4.HeightRequest = width / 11;
                delivery4Text1.FontSize = width / 50;
                delivery4Text2.FontSize = width / 70;

                delivery5.WidthRequest = width / 13;
                delivery5.HeightRequest = width / 11;
                delivery5Text1.FontSize = width / 50;
                delivery5Text2.FontSize = width / 70;

                delivery6.WidthRequest = width / 13;
                delivery6.HeightRequest = width / 11;
                delivery6Text1.FontSize = width / 50;
                delivery6Text2.FontSize = width / 70;

                delivery7.WidthRequest = width / 13;
                delivery7.HeightRequest = width / 11;
                delivery7Text1.FontSize = width / 50;
                delivery7Text2.FontSize = width / 70;

                delivery8.WidthRequest = width / 13;
                delivery8.HeightRequest = width / 11;
                delivery8Text1.FontSize = width / 50;
                delivery8Text2.FontSize = width / 70;

                delivery9.WidthRequest = width / 13;
                delivery9.HeightRequest = width / 11;
                delivery9Text1.FontSize = width / 50;
                delivery9Text2.FontSize = width / 70;

                delivery10.WidthRequest = width / 13;
                delivery10.HeightRequest = width / 11;
                delivery10Text1.FontSize = width / 50;
                delivery10Text2.FontSize = width / 70;

                spacer4.HeightRequest = 10;
                SignUpButton.HeightRequest = height / 50;
                SignUpButton.WidthRequest = width / 4;
                SignUpButton.CornerRadius = (int)(height / 100);
                SignUpButton.FontSize = width / 60;
            }

            //common adjustments regardless of platform
        }

        protected async Task GetDeliveryDates()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/delivery_weekdays");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject dates_obj = JObject.Parse(userString);
                HashSet<String> dates = new HashSet<String>();
                foreach (var m in dates_obj["result"])
                {
                    Console.WriteLine(m["weekday(menu_date)"].ToString());
                    dates.Add(m["weekday(menu_date)"].ToString());
                }
                String deliveryDatesText = "";
                if (dates.Contains("0")) deliveryDatesText += "Monday, ";
                if (dates.Contains("1")) deliveryDatesText += "Tuesday, ";
                if (dates.Contains("2")) deliveryDatesText += "Wednesday, ";
                if (dates.Contains("3")) deliveryDatesText += "Thursday, ";
                if (dates.Contains("4")) deliveryDatesText += "Friday, ";
                if (dates.Contains("5")) deliveryDatesText += "Saturday, ";
                if (dates.Contains("6")) deliveryDatesText += "Sunday, ";
                if (deliveryDatesText.Length != 0)
                {
                    deliveryDays2.Text = deliveryDatesText.Substring(0, deliveryDatesText.Length - 2);
                }
            }
        }

        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#FFBA00");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Button b = (Button)sender;
            Preferences.Set("mealSelected", b.Text.Substring(0, b.Text.IndexOf(" ")));
            mealSelected = int.Parse(meals1.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 4]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals2(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#FFBA00");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Button b = (Button)sender;
            Preferences.Set("mealSelected", b.Text.Substring(0, b.Text.IndexOf(" ")));
            mealSelected = int.Parse(meals2.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 3]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#FFBA00");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Button b = (Button)sender;
            Preferences.Set("mealSelected", b.Text.Substring(0, b.Text.IndexOf(" ")));
            mealSelected = int.Parse(meals3.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 2]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#FFBA00");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Button b = (Button)sender;
            Preferences.Set("mealSelected", b.Text.Substring(0, b.Text.IndexOf(" ")));
            mealSelected = int.Parse(meals4.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 1]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals5(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#FFBA00");
            Button b = (Button)sender;
            Preferences.Set("mealSelected", b.Text.Substring(0, b.Text.IndexOf(" ")));
            mealSelected = int.Parse(meals5.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 0]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedDeliveryNum(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            SignUpButton.Text = "CHECK PRICE";

            delivery1Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery2Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery3Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery4Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery5Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery6Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery7Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery8Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery9Frame.BackgroundColor = Color.FromHex("#f5f5f5");
            delivery10Frame.BackgroundColor = Color.FromHex("#f5f5f5");

            if (btn.Equals(delivery1))
            {
                delivery1Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "1");
                deliverySelected = 1;
            }
            else if (btn.Equals(delivery2))
            {
                delivery2Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "2");
                deliverySelected = 2;
            }
            else if (btn.Equals(delivery3))
            {
                delivery3Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "3");
                deliverySelected = 3;
            }
            else if (btn.Equals(delivery4))
            {
                delivery4Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "4");
                deliverySelected = 4;
            }
            else if (btn.Equals(delivery5))
            {
                delivery5Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "5");
                deliverySelected = 5;
            }
            else if (btn.Equals(delivery6))
            {
                delivery6Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "6");
                deliverySelected = 6;
            }
            else if (btn.Equals(delivery7))
            {
                delivery7Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "7");
                deliverySelected = 7;
            }
            else if (btn.Equals(delivery8))
            {
                delivery8Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "8");
                deliverySelected = 8;
            }
            else if (btn.Equals(delivery9))
            {
                delivery9Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "9");
                deliverySelected = 9;
            }
            else if (btn.Equals(delivery10))
            {
                delivery10Frame.BackgroundColor = Color.FromHex("#FFBA00");
                Preferences.Set("freqSelected", "10");
                deliverySelected = 10;
            }
            else
            {
                Preferences.Set("freqSelected", "0");
                deliverySelected = 0;
            }
            if (mealSelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                //Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                double itemPrice = itemPrices[0, 6 - mealSelected];
                Preferences.Set("itemPrice", itemPrice);
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            if (TotalPrice.Text == "$ TOTAL" || TotalPrice.Text == "$00.00" || TotalPrice.Text == "$0")
            {
                await DisplayAlert("Warning!", "pick a valid plan to continue", "OK");
                return;
            }

            if (SignUpButton.Text == "CHECK PRICE")
            {
                // Setting request for USPS API
                XDocument requestDoc = new XDocument(
                    new XElement("AddressValidateRequest",
                    new XAttribute("USERID", "400INFIN1745"),
                    new XElement("Revision", "1"),
                    new XElement("Address",
                    new XAttribute("ID", "0"),
                    new XElement("Address1", passedAdd),
                    new XElement("Address2", passedUnit),
                    new XElement("City", passedCity),
                    new XElement("State", passedState),
                    new XElement("Zip5", passedZip),
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
                        if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(passedZip) && GetXMLElement(element, "City").Equals(passedCity.ToUpper())) // Best case
                        {
                            // Get longitude and latitide because we can make a deliver here. Move on to next page.
                            // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                            //GetAddressLatitudeLongitude();
                            Geocoder geoCoder = new Geocoder();

                            //Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                            //Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                            //Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(passedAdd + "," + passedCity + "," + passedState);
                            Position position = approximateLocations.FirstOrDefault();

                            latitude = $"{position.Latitude}";
                            longitude = $"{position.Longitude}";



                            //used for createaccount endpoint
                            Preferences.Set("user_latitude", latitude);
                            Preferences.Set("user_longitude", longitude);
                            Debug.WriteLine("user latitude: " + latitude);
                            Debug.WriteLine("user longitude: " + longitude);

                            //directSignUp.latitude = latitude;
                            //directSignUp.longitude = longitude;
                            //map.MapType = MapType.Street;
                            //var mapSpan = new MapSpan(position, 0.001, 0.001);

                            //Pin address = new Pin();
                            //address.Label = "Delivery Address";
                            //address.Type = PinType.SearchResult;
                            //address.Position = position;

                            //map.MoveToRegion(mapSpan);
                            //map.Pins.Add(address);

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

                                Debug.WriteLine("first business: " + obj.Result[0].business_name);
                                taxRate = obj.Result[0].tax_rate / 100;
                                //deliveryFee = obj.Result[0].delivery_fee;
                                //serviceFee = obj.Result[0].service_fee;
                                //passingZones = obj.Result;
                                //withinZones = true;
                            }

                            break;
                        }
                }




                var request3 = new HttpRequestMessage();
                Debug.WriteLine("subscriptionModal trying to delete this purchase uid: " + purchase_id.ToString());
                //request2.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/refund_calculator?purchase_uid=" + purchase_id);
                request3.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_id);
                request3.Method = HttpMethod.Get;
                var client3 = new HttpClient();
                HttpResponseMessage response3 = await client3.SendAsync(request3);

                if (response3.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content3 = response3.Content;
                    var userString3 = await content3.ReadAsStringAsync();
                    JObject refund_obj = JObject.Parse(userString3);


                    Debug.WriteLine("first start" + refund_obj.ToString());
                    Debug.WriteLine("this is what I'm getting: " + refund_obj["refund_amount"].ToString());
                    double amt = Double.Parse(refund_obj["refund_amount"].ToString());
                    double newTotal = Math.Round(Double.Parse(TotalPrice.Text.Substring(1)) * (1 + taxRate),2);
                    Debug.WriteLine("this is the amount for tax: " + Math.Round(Double.Parse(TotalPrice.Text.Substring(1)) * (taxRate), 2).ToString());
                    Debug.WriteLine("tax rate from categorical options: " + taxRate.ToString());
                    Debug.WriteLine("newTotal: " + newTotal.ToString());
                    newTotal += Double.Parse(refund_obj["delivery_fee"].ToString());
                    newTotal += Double.Parse(refund_obj["service_fee"].ToString());
                    newTotal += Double.Parse(refund_obj["driver_tip"].ToString());
                    newTotal = Math.Round(newTotal, 2);
                    Debug.WriteLine("amt before subtracting: " + amt.ToString());
                    Debug.WriteLine("newTotal before subtracting: " + newTotal.ToString());
                    double currentPrice = Double.Parse(TotalPrice.Text.ToString().Substring(1));

                    double correct;
                    //check if the ambassador code discount completely covers the meal plan price
                    if (Double.Parse(refund_obj["ambassador_code"].ToString()) - newTotal > 0)
                        correct = amt;
                    else correct = amt + Double.Parse(refund_obj["ambassador_code"].ToString()) - newTotal;

                    //double correct = amt - newTotal;
                    correct = Math.Round(correct, 2);
                    Debug.WriteLine("correct after subtracting: " + correct.ToString());


                    if (correct < 0)
                    {
                        correct *= -1;
                        await DisplayAlert("Extra Charge", "You will be charged $" + correct.ToString() + " for this plan change.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Reimbursement", "You will be reimbursed $" + correct.ToString() + " for this plan change.", "OK");
                    }

                    SignUpButton.Text = "SAVE CHANGES";
                }
            }
            else
            {
                int length = (TotalPrice.Text).Length;
                string price = TotalPrice.Text.Substring(1, length - 1);
                Preferences.Set("price", price);

                Console.WriteLine("Price selected: " + price);

                PurchaseInfo2 updated = new PurchaseInfo2();

                //if direct login
                if (socialLogin == "NULL")
                {
                    var request2 = new HttpRequestMessage();
                    Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                    string url2 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                    request2.RequestUri = new Uri(url2);
                    //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                    request2.Method = HttpMethod.Get;
                    var client2 = new HttpClient();
                    HttpResponseMessage response2 = await client2.SendAsync(request2);

                    if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContent content2 = response2.Content;
                        Console.WriteLine("content: " + content2);
                        var userString2 = await content2.ReadAsStringAsync();
                        var info_obj2 = JObject.Parse(userString2);

                        //updated.password = (info_obj2["result"])[0]["password_hashed"].ToString();
                    }
                }
                //else updated.password = "NULL";

                //updated.password = password;
                //updated.refresh_token = refresh_token;
                //updated.cc_num = cc_num;
                //testing
                updated.cc_num = cc_num;
                updated.cc_exp_date = cc_exp_date;
                //updated.cc_exp_year = cc_exp_year;
                //updated.cc_exp_month = cc_exp_month;
                updated.cc_cvv = cc_cvv;
                updated.purchase_id = purchase_id;
                //updated.purchase_id = "400-000019";
                updated.new_item_id = Preferences.Get("item_uid", "");
                updated.customer_email = cust_email;
                updated.cc_zip = cc_zip;
                updated.start_delivery_date = "";

                List<Item> list1 = new List<Item>();
                Item item1 = new Item();
                item1.qty = Preferences.Get("freqSelected", "");
                item1.name = Preferences.Get("mealSelected", "") + " Meal Plan";
                //item1.price = Preferences.Get("price", "");
                item1.price = Preferences.Get("itemPrice", "00.00");
                item1.item_uid = updated.new_item_id;
                item1.itm_business_uid = itm_business_uid;
                list1.Add(item1);
                updated.items = list1;

                var newPaymentJSONString = JsonConvert.SerializeObject(updated);
                Console.WriteLine("updatedJSONString" + newPaymentJSONString);
                var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("updatedContent: " + content);
                /*var request = new HttpRequestMessage();
                request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                request.Method = HttpMethod.Post;
                request.Content = content;*/
                var client = new HttpClient();
                Debug.WriteLine("url we are posting to: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_id);
                //var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_id, content);
                // HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedDone Func ENDED!");

                Zones[] zones = new Zones[] { };
                await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email));
                //old nav
                //await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
                //Application.Current.MainPage = new DeliveryBilling();
                //await NavigationPage.PushAsync(DeliveryBilling());
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

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new UserProfile();
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            Console.WriteLine("navigation stack count: " + Navigation.NavigationStack.Count);
            if (Navigation.NavigationStack.Count == 1)
            {
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                await Navigation.PopAsync(false);
            }
            //await Navigation.PushAsync(new MainPage());
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new Menu();
        }
    }
}
