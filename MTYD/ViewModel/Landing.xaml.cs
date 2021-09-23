using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MTYD.Interfaces;

namespace MTYD.ViewModel
{
    public partial class Landing : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        List<string> menuNames;
        List<string> menuImages;

        public Landing(string firstName, string lastName, string email)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            InitializeComponent();

            string version = "";
            string build = "";
            version = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
            build = DependencyService.Get<IAppVersionAndBuild>().GetBuildNumber();

            appVersion.Text = "App version: " + version + ", App build: " + build;

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            var cvItems = new List<string>
            {
                "Who has time?\n\nSave time and money! Ready to heat meals come to your door and you can order up to 10 deliveries in advance so you know what's coming!",
                "Food when you're hungry\n\nIf you order food when you're hungry, you're starving by the time it arrives! With MealsFor.Me there is always something in the fridge and your next meals are in route!",
                "Better Value\n\nYou get restaurant quality food at a fraction of the cost plus it is made from the highest quality ingredients by exceptional Chefs."
            };
            TheCarousel.ItemsSource = cvItems;

            checkPlatform(height, width);
            setGrid();
        }

        public void checkPlatform(double height, double width)
        {
            orangeBox.HeightRequest = height / 2;
            orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
            orangeBox.CornerRadius = height / 40;
            logo.HeightRequest = width / 15;
            logo.Margin = new Thickness(0, 0, 0, 30);
            //innerGrid.Margin = new Thickness(0, 0, 23, 27);

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

            landingPic.Margin = new Thickness(0, -height / 30, 0, 0);

            //partners.FontSize = width / 60;

            ambassadorBtn.WidthRequest = width / 4;
            ambassadorBtn.HeightRequest = width / 20;
            ambassadorBtn.CornerRadius = (int)(width / 40);

            if (Device.RuntimePlatform == Device.iOS)
            {
                
                //pfp.HeightRequest = 40;
                //pfp.WidthRequest = 40;
                //pfp.CornerRadius = 20;

                mainLogo.HeightRequest = height / 18;
                getStarted.HeightRequest = height / 35;
                getStarted.CornerRadius = (int)(height / 70);
                getStarted.Margin = new Thickness(width / 7, 0);

                fade.Margin = new Thickness(0, -height / 3, 0, 0);

                xButton.FontSize = width / 37;
                DiscountGrid.Margin = new Thickness(width / 30, height / 9, width / 30, 0);
                DiscountGrid.HeightRequest = height / 5.2;
                DiscountGrid.WidthRequest = width / 2.3;
                couponGrid.HeightRequest = height / 5.2;
                couponGrid.WidthRequest = width / 2.3;

                outerFrame.HeightRequest = height / 5.2;

                discHeader.FontSize = width / 35;

                couponGrid.HeightRequest = height / 30;
                couponImg.HeightRequest = height / 30;
                couponAmt.FontSize = width / 50;
                couponDesc.FontSize = width / 43;

                couponGrid2.HeightRequest = height / 30;
                couponImg2.HeightRequest = height / 30;
                couponAmt2.FontSize = width / 50;
                couponDesc2.FontSize = width / 43;

                searchPic.Margin = new Thickness(width / 5.5, 0);
                first.FontSize = width / 30;
                first.HeightRequest = width / 15;
                first.WidthRequest = width / 15;
                first.CornerRadius = (int)(width / 30);
                first.Margin = new Thickness(0, 0, 3, 0);
                step1.Margin = new Thickness(3, 0, 0, 0);
                step1.FontSize = width / 30;
                sub1.FontSize = width / 45;

                cardPic.Margin = new Thickness(width / 5.5, 0);
                second.FontSize = width / 30;
                second.HeightRequest = width / 15;
                second.WidthRequest = width / 15;
                second.CornerRadius = (int)(width / 30);
                second.Margin = new Thickness(0, 0, 3, 0);
                step2.Margin = new Thickness(3, 0, 0, 0);
                step2.FontSize = width / 30;
                sub2.FontSize = width / 45;

                pickPic.Margin = new Thickness(width / 5.5, 0);
                third.FontSize = width / 30;
                third.HeightRequest = width / 15;
                third.WidthRequest = width / 15;
                third.CornerRadius = (int)(width / 30);
                third.Margin = new Thickness(0, 0, 3, 0);
                step3.Margin = new Thickness(3, 0, 0, 0);
                step3.FontSize = width / 30;
                sub3.FontSize = width / 45;

                delivPic.Margin = new Thickness(width / 5.5, 0);
                fourth.FontSize = width / 30;
                fourth.HeightRequest = width / 15;
                fourth.WidthRequest = width / 15;
                fourth.CornerRadius = (int)(width / 30);
                fourth.Margin = new Thickness(0, 0, 3, 0);
                step4.Margin = new Thickness(3, 0, 0, 0);
                step4.FontSize = width / 30;
                sub4.FontSize = width / 45;

                partners.FontSize = width / 45;
            }
            else //Android
            {
                //pfp.HeightRequest = 40;
                //pfp.WidthRequest = 40;
                //pfp.CornerRadius = 20;

                mainLogo.HeightRequest = height / 20;
                getStarted.HeightRequest = height / 45;
                getStarted.CornerRadius = (int)(height / 90);
                getStarted.Margin = new Thickness(width / 10, 0);

                fade.Margin = new Thickness(0, -height / 3, 0, 0);

                xButton.WidthRequest = width / 40;
                xButton.FontSize = width / 50;
                DiscountGrid.Margin = new Thickness(width / 30, height / 9, width / 30, 0);
                DiscountGrid.HeightRequest = height / 6.5;
                DiscountGrid.WidthRequest = width / 2.3;

                outerFrame.HeightRequest = height / 6.5;

                discHeader.FontSize = width / 50;

                couponGrid.HeightRequest = height / 30;
                couponImg.HeightRequest = height / 40;
                couponAmt.FontSize = width / 60;
                couponDesc.FontSize = width / 60;

                couponGrid2.HeightRequest = height / 30;
                couponImg2.HeightRequest = height / 40;
                couponAmt2.FontSize = width / 60;
                couponDesc2.FontSize = width / 60;

                searchPic.WidthRequest = width / 9;
                first.FontSize = width / 40;
                first.HeightRequest = width / 20;
                first.WidthRequest = width / 20;
                first.CornerRadius = (int)(width / 40);
                step1.FontSize = width / 35;
                sub1.FontSize = width / 60;

                cardPic.WidthRequest = width / 9;
                second.FontSize = width / 40;
                second.HeightRequest = width / 20;
                second.WidthRequest = width / 20;
                second.CornerRadius = (int)(width / 40);
                step2.FontSize = width / 35;
                sub2.FontSize = width / 60;

                pickPic.WidthRequest = width / 9;
                third.FontSize = width / 40;
                third.HeightRequest = width / 20;
                third.WidthRequest = width / 20;
                third.CornerRadius = (int)(width / 40);
                step3.FontSize = width / 35;
                sub3.FontSize = width / 60;

                delivPic.WidthRequest = width / 9;
                fourth.FontSize = width / 40;
                fourth.HeightRequest = width / 20;
                fourth.WidthRequest = width / 20;
                fourth.CornerRadius = (int)(width / 40);
                step4.FontSize = width / 35;
                sub4.FontSize = width / 60;

                partners.FontSize = width / 60;
            }
        }

        protected async Task setGrid()
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
                    if (dates.Count > 2) break;
                    menuNames.Add(m["meal_name"].ToString());
                    menuImages.Add(m["meal_photo_URL"].ToString());
                }
            }

            upcomingMenuGrid.ColumnDefinitions = new ColumnDefinitionCollection();
            for (int i = 0; i < menuNames.Count; i++)
            {
                upcomingMenuGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120, GridUnitType.Absolute) });
            }
            for (int i = 0; i < menuNames.Count; i++)
            {
                upcomingMenuGrid.Children.Add(new Label
                {
                    Text = menuNames[i],
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                }, i, 0);
                upcomingMenuGrid.Children.Add(new ImageButton
                {
                    Source = menuImages[i],
                    CornerRadius = 10,
                    IsEnabled = false,
                    Aspect = Aspect.AspectFill
                }, i, 1);
            }
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

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            //await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedStarted(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email));
        }
    }
}
