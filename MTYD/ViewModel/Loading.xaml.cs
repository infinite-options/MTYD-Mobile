using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class Loading : ContentPage
    {
        public Loading()
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
                loading.FontSize = width / 15;
            }
        }
    }
}
