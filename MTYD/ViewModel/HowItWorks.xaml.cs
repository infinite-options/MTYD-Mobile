using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class HowItWorks : ContentPage
    {
        public HowItWorks()
        {
            InitializeComponent();

            BackgroundColor = Color.FromHex("#f3f2dc");

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
                heading.Margin = new Thickness(0, height / 27, 0, 0);
                heading.FontSize = width / 33;
                introParagraph.FontSize = width / 50;

                heading2.FontSize = width / 30;

                first.FontSize = width / 32;
                first.HeightRequest = width / 17;
                first.WidthRequest = width / 17;
                first.CornerRadius = (int)(width / 34);
                first.Margin = new Thickness(width / 30, 0);
                step1.FontSize = width / 34;
                sub1.FontSize = width / 47;

                second.FontSize = width / 32;
                second.HeightRequest = width / 17;
                second.WidthRequest = width / 17;
                second.CornerRadius = (int)(width / 34);
                second.Margin = new Thickness(width / 30, 0);
                step2.FontSize = width / 34;
                sub2.FontSize = width / 47;

                third.FontSize = width / 32;
                third.HeightRequest = width / 17;
                third.WidthRequest = width / 17;
                third.CornerRadius = (int)(width / 34);
                third.Margin = new Thickness(width / 30, 0);
                step3.FontSize = width / 34;
                sub3.FontSize = width / 47;

                back.FontSize = width / 36;
                signUp.FontSize = width / 36;
                browseMenu.FontSize = width / 33;
                //back.Margin = new Thickness(0, 0, 5, 0);
                //browseMenu.Margin = new Thickness(5, 0, 0, 0);
            }
            else //android
            {

            }
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        async void clickedWeeksMeals(object sender, EventArgs e)
        {
            Application.Current.MainPage = new ThisWeeksMeals();
        }
    }
}
