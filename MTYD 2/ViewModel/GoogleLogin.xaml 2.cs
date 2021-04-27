using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class GoogleLogin : ContentPage
    {
        public GoogleLogin()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform();
        }

        public void checkPlatform()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                googleIcon.CornerRadius = 29;
                profilePic1.CornerRadius = 25;
                profilePic2.CornerRadius = 25;
                addAccount.CornerRadius = 25;
            }
            else if (Device.RuntimePlatform == Device.Android)
            {

            }
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }
    }
}