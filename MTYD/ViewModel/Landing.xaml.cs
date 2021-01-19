using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class Landing : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;

        public Landing(string firstName, string lastName, string email)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            InitializeComponent();

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                logo.HeightRequest = width / 15;
                logo.Margin = new Thickness(0, 0, 0, 30);
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
            }
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }
    }
}
