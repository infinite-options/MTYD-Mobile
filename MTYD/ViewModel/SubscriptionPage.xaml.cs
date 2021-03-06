﻿using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    public partial class SubscriptionPage : ContentPage
    {
        public ObservableCollection<Plans> NewPlan = new ObservableCollection<Plans>();

        string cust_firstName; string cust_lastName; string cust_email;
        int mealSelected;
        int deliverySelected;
        double[,] discounts;
        double[,] itemPrices;
        String[,] itemNames, itemUids;
        double total;

        public object NavigationStack { get; private set; }

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

                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;
                menu.Margin = new Thickness(25, 0, 0, 30);

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
                delivery1.HeightRequest = width / 11;
                delivery1Text1.FontSize = width / 50;

                delivery2.WidthRequest = width / 10;
                delivery2.HeightRequest = width / 11;
                delivery2Text1.FontSize = width / 50;
                delivery2Text2.FontSize = width / 65;

                delivery3.WidthRequest = width / 10;
                delivery3.HeightRequest = width / 11;
                delivery3Text1.FontSize = width / 50;
                delivery3Text2.FontSize = width / 65;

                delivery4.WidthRequest = width / 10;
                delivery4.HeightRequest = width / 11;
                delivery4Text1.FontSize = width / 50;
                delivery4Text2.FontSize = width / 65;

                delivery5.WidthRequest = width / 10;
                delivery5.HeightRequest = width / 11;
                delivery5Text1.FontSize = width / 50;
                delivery5Text2.FontSize = width / 65;

                delivery6.WidthRequest = width / 10;
                delivery6.HeightRequest = width / 11;
                delivery6Text1.FontSize = width / 50;
                delivery6Text2.FontSize = width / 65;

                delivery7.WidthRequest = width / 10;
                delivery7.HeightRequest = width / 11;
                delivery7Text1.FontSize = width / 50;
                delivery7Text2.FontSize = width / 65;

                delivery8.WidthRequest = width / 10;
                delivery8.HeightRequest = width / 11;
                delivery8Text1.FontSize = width / 50;
                delivery8Text2.FontSize = width / 65;

                delivery9.WidthRequest = width / 10;
                delivery9.HeightRequest = width / 11;
                delivery9Text1.FontSize = width / 50;
                delivery9Text2.FontSize = width / 65;

                delivery10.WidthRequest = width / 10;
                delivery10.HeightRequest = width / 11;
                delivery10Text1.FontSize = width / 50;
                delivery10Text2.FontSize = width / 65;

                SignUpButton.WidthRequest = width / 4;
                SignUpButton.HeightRequest = width / 18;
                SignUpButton.CornerRadius = (int)(width / 36);
                SignUpButton.FontSize = width / 50;

                backButton.WidthRequest = width / 4;
                backButton.HeightRequest = width / 18;
                backButton.CornerRadius = (int)(width / 36);
                backButton.FontSize = width / 55;
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

        public SubscriptionPage(string firstName, string lastName, string email)
        {
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

            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                menu.IsVisible = false;
                backButton.IsVisible = true;
                innerGrid.IsVisible = false;
            }

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            checkPlatform(height, width);
            GetDeliveryDates();
            GetPlans();
            //Preferences.Set("freqSelected", "");
            pfp.Source = Preferences.Get("profilePicLink", "");
        }


        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#FFBA00");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Preferences.Set("mealSelected", "1");
            mealSelected = int.Parse(meals1.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                //double basePrice_dub = itemPrices[deliverySelected - 1, 6 - mealSelected];
                double discountAmt_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                string basePrice = "";
                string discountAmt = "";
                basePrice = basePrice_dub.ToString();
                discountAmt = discountAmt_dub.ToString();
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt);
                Preferences.Set("basePrice", basePrice_dub);
                Preferences.Set("discountAmt", discountAmt_dub);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private void clickedMeals2(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#FFBA00");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Preferences.Set("mealSelected", "2");
            mealSelected = int.Parse(meals2.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0; 
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#FFBA00");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Preferences.Set("mealSelected", "3");
            mealSelected = int.Parse(meals3.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#FFBA00");
            meals5.BackgroundColor = Color.FromHex("#f5f5f5");
            Preferences.Set("mealSelected", "4");
            mealSelected = int.Parse(meals4.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private void clickedMeals5(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#f5f5f5");
            meals2.BackgroundColor = Color.FromHex("#f5f5f5");
            meals3.BackgroundColor = Color.FromHex("#f5f5f5");
            meals4.BackgroundColor = Color.FromHex("#f5f5f5");
            meals5.BackgroundColor = Color.FromHex("#FFBA00");
            Preferences.Set("mealSelected", "5");
            mealSelected = int.Parse(meals5.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private void clickedDeliveryNum(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

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
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                mealNum.Text = mealSelected.ToString();
                deliveryNum.Text = deliverySelected.ToString();
                discountPercentage.Text = ((int)discounts[deliverySelected - 1, 6 - mealSelected]).ToString() + "%";
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                Debug.WriteLine("base price: " + basePrice + " , discount amount: " + discountAmt + " , item price: " + itemPrice);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                TotalPrice.Text = "$" + total.ToString();
                pricePerMeal.Text = "That's Less Than $" + Math.Ceiling(total / mealSelected / deliverySelected) + " per Meal!";
            }
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            if (TotalPrice.Text == "$0" || TotalPrice.Text == "$00.00")
            {
                await DisplayAlert("Warning!", "pick a valid plan to continue", "OK");
                return;
            }

            int length = (TotalPrice.Text).Length;
            string price = TotalPrice.Text.Substring(1, length - 1);
            Preferences.Set("price", price);

            Console.WriteLine("Price selected: " + price);

            Console.WriteLine("freqSelected: " + Preferences.Get("freqSelected", ""));
            Console.WriteLine("mealSelected: " + Preferences.Get("mealSelected", ""));
            Console.WriteLine("item_name: " + Preferences.Get("item_name", ""));
            Console.WriteLine("item_uid: " + Preferences.Get("item_uid", ""));

            await Navigation.PushAsync(new PaymentPage(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new DeliveryBilling();
            //await NavigationPage.PushAsync(DeliveryBilling());
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new UserProfile();
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            if ((string)Application.Current.Properties["platform"] == "GUEST")
            {
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                await Navigation.PushAsync(new MainPage(cust_firstName, cust_lastName, cust_email));
            }
            //if (Navigation.NavigationStack.Count == 1)
            //{
            //    Application.Current.MainPage = new MainPage();
            //}
            //else
            //{
            //    await Navigation.PopAsync(false);
            //}
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new Menu();
        }
    }
}