using MTYD.Model;
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

                meals1.Text = numMealsList[4].ToString();
                meals2.Text = numMealsList[3].ToString();
                meals3.Text = numMealsList[2].ToString();
                meals4.Text = numMealsList[1].ToString();
                meals5.Text = numMealsList[0].ToString();
            }
        }


        void checkPlatform(double height, double width)
        {
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
                deliveryDays.FontSize = width / 38;
                deliveryDays2.FontSize = width / 38;
                numMeals.FontSize = width / 37;
                numMeals.Margin = new Thickness(25, 10, 0, 10);

                prepay.Margin = new Thickness(30, 0, 0, 0);
                prepay.FontSize = width / 37;

                PriceFrame.HeightRequest = height / 30;
                PriceFrame.WidthRequest = width / 6;
                PriceFrame.CornerRadius = 30;
                TotalPrice.FontSize = width / 40;
                SignUpButton.HeightRequest = height / 30;
                SignUpButton.WidthRequest = width / 6;
                SignUpButton.CornerRadius = (int)(height / 60);
                SignUpButton.FontSize = width / 40;
            }
            else //android
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 45;
                heading.Margin = new Thickness(0, 0, 0, 40);
                //heading.VerticalOptions = LayoutOptions.Center;
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

                spacer4.HeightRequest = 10;
                PriceFrame.HeightRequest = height / 33;
                PriceFrame.WidthRequest = width / 8;
                PriceFrame.CornerRadius = 33;
                TotalPrice.FontSize = width / 50;
                SignUpButton.HeightRequest = height / 33;
                SignUpButton.WidthRequest = width / 8;
                SignUpButton.CornerRadius = (int)(height / 66);
                SignUpButton.FontSize = width / 50;
            }

            //common adjustments regardless of platform
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
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            checkPlatform(height, width);
            GetPlans();
            Preferences.Set("freqSelected", "");
            pfp.Source = Preferences.Get("profilePicLink", "");
        }


        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#FFBA00");
            meals2.BackgroundColor = Color.FromHex("#ffffff");
            meals3.BackgroundColor = Color.FromHex("#ffffff");
            meals4.BackgroundColor = Color.FromHex("#ffffff");
            meals5.BackgroundColor = Color.FromHex("#ffffff");
            Preferences.Set("mealSelected", "1");
            mealSelected = int.Parse(meals1.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private void clickedMeals2(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#ffffff");
            meals2.BackgroundColor = Color.FromHex("#FFBA00");
            meals3.BackgroundColor = Color.FromHex("#ffffff");
            meals4.BackgroundColor = Color.FromHex("#ffffff");
            meals5.BackgroundColor = Color.FromHex("#ffffff");
            Preferences.Set("mealSelected", "2");
            mealSelected = int.Parse(meals2.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#ffffff");
            meals2.BackgroundColor = Color.FromHex("#ffffff");
            meals3.BackgroundColor = Color.FromHex("#FFBA00");
            meals4.BackgroundColor = Color.FromHex("#ffffff");
            meals5.BackgroundColor = Color.FromHex("#ffffff");
            Preferences.Set("mealSelected", "3");
            mealSelected = int.Parse(meals3.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#ffffff");
            meals2.BackgroundColor = Color.FromHex("#ffffff");
            meals3.BackgroundColor = Color.FromHex("#ffffff");
            meals4.BackgroundColor = Color.FromHex("#FFBA00");
            meals5.BackgroundColor = Color.FromHex("#ffffff");
            Preferences.Set("mealSelected", "4");
            mealSelected = int.Parse(meals4.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private void clickedMeals5(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.FromHex("#ffffff");
            meals2.BackgroundColor = Color.FromHex("#ffffff");
            meals3.BackgroundColor = Color.FromHex("#ffffff");
            meals4.BackgroundColor = Color.FromHex("#ffffff");
            meals5.BackgroundColor = Color.FromHex("#FFBA00");
            Preferences.Set("mealSelected", "5");
            mealSelected = int.Parse(meals5.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private void clickedDeliveryNum(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            delivery1.BackgroundColor = Color.FromHex("#ffffff");
            delivery2.BackgroundColor = Color.FromHex("#ffffff");
            delivery3.BackgroundColor = Color.FromHex("#ffffff");
            delivery4.BackgroundColor = Color.FromHex("#ffffff");
            delivery5.BackgroundColor = Color.FromHex("#ffffff");
            delivery6.BackgroundColor = Color.FromHex("#ffffff");
            delivery7.BackgroundColor = Color.FromHex("#ffffff");
            delivery8.BackgroundColor = Color.FromHex("#ffffff");
            delivery9.BackgroundColor = Color.FromHex("#ffffff");
            delivery10.BackgroundColor = Color.FromHex("#ffffff");
            btn.BackgroundColor = Color.FromHex("#FFBA00");

            if (btn.Equals(delivery1))
            {
                Preferences.Set("freqSelected", "1");
                deliverySelected = 1;
            }
            else if (btn.Equals(delivery2))
            {
                Preferences.Set("freqSelected", "2");
                deliverySelected = 2;
            }
            else if (btn.Equals(delivery3))
            {
                Preferences.Set("freqSelected", "3");
                deliverySelected = 3;
            }
            else if (btn.Equals(delivery4))
            {
                Preferences.Set("freqSelected", "4");
                deliverySelected = 4;
            }
            else if (btn.Equals(delivery5))
            {
                Preferences.Set("freqSelected", "5");
                deliverySelected = 5;
            }
            else if (btn.Equals(delivery6))
            {
                Preferences.Set("freqSelected", "6");
                deliverySelected = 6;
            }
            else if (btn.Equals(delivery7))
            {
                Preferences.Set("freqSelected", "7");
                deliverySelected = 7;
            }
            else if (btn.Equals(delivery8))
            {
                Preferences.Set("freqSelected", "8");
                deliverySelected = 8;
            }
            else if (btn.Equals(delivery9))
            {
                Preferences.Set("freqSelected", "9");
                deliverySelected = 9;
            }
            else if (btn.Equals(delivery10))
            {
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
                TotalPrice.Text = "$" + deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
            }
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            if (TotalPrice.Text == "$ TOTAL" || TotalPrice.Text == "$00.00" || TotalPrice.Text == "$0")
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

            await Navigation.PushAsync(new DeliveryBilling(cust_firstName, cust_lastName, cust_email));
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
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new Menu();
        }
    }
}
