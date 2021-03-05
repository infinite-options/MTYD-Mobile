using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public class RestaurantPic
    {
        public string picSource { get; set; }
    }


    public partial class AboutUs : ContentPage
    {
        double deviceHeight; double deviceWidth;
        string first; string last; string email;

        public AboutUs(string Fname, string Lname, string emailAdd)
        {
            first = Fname; last = Lname; email = emailAdd;

            List<RestaurantPic> pics = new List<RestaurantPic>(3);
            RestaurantPic pic1 = new RestaurantPic();
            pic1.picSource = "oliveGarden.png";
            RestaurantPic pic2 = new RestaurantPic();
            pic2.picSource = "paradiseBay.png";
            RestaurantPic pic3 = new RestaurantPic();
            pic3.picSource = "Inchin.png";
            pics.Add(pic1);
            pics.Add(pic2);
            pics.Add(pic3);
            pics.Add(pic1);
            pics.Add(pic2);
            pics.Add(pic3);


            InitializeComponent();
            picCarousel.ItemsSource = pics;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            deviceWidth = DeviceDisplay.MainDisplayInfo.Width;
            deviceHeight = DeviceDisplay.MainDisplayInfo.Height;

            

            checkPlatform();
        }

        void checkPlatform()
        {
            orangeBox.HeightRequest = deviceHeight / 2;
            orangeBox.Margin = new Thickness(0, -deviceHeight / 2.2, 0, 0);
            orangeBox.CornerRadius = deviceHeight / 40;
            heading.FontSize = deviceWidth / 32;
            heading.Margin = new Thickness(0, 0, 0, 30);
            pfp.HeightRequest = deviceWidth / 20;
            pfp.WidthRequest = deviceWidth / 20;
            pfp.CornerRadius = (int)(deviceWidth / 40);
            //pfp.Margin = new Thickness(0, 0, 23, 27);
            innerGrid.Margin = new Thickness(0, 0, 23, 27);


            string userInitials = "";
            if (Preferences.Get("profilePicLink", "") == "")
            {
                if (first != "" && first != null)
                {
                    userInitials += first.Substring(0, 1);
                }
                if (last != "" && last != null)
                {
                    userInitials += last.Substring(0, 1);
                }
                initials.Text = userInitials.ToUpper();
                initials.FontSize = deviceWidth / 38;
            }
            else pfp.Source = Preferences.Get("profilePicLink", "");

            menu.HeightRequest = deviceWidth / 25;
            menu.WidthRequest = deviceWidth / 25;
            menu.Margin = new Thickness(25, 0, 0, 30);

            OurStoryGrid.HeightRequest = ourStoryFrame.Height + 500;
        }


        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(first, last, email));
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(first, last, email));
        }
    }
}
