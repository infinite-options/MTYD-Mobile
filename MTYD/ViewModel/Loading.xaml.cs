//using System;
//using System.Collections.Generic;
//using Xamarin.Essentials;
//using Xamarin.Forms;
//using Xamarin.CommunityToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using MTYD.Model;
using MTYD.Model.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Diagnostics;
using Plugin.LatestVersion;
using Xamarin.CommunityToolkit;
using System.Threading;

namespace MTYD.ViewModel
{
    public partial class Loading : ContentPage
    {
        public Loading()
        {
            InitializeComponent();
            //BackgroundColor = Color.FromHex("#f3f2dc");

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);

            //loading.Source = "ms-appx:///pulseLoadingAnim2.mp4";
            //loading.Play();

            //options
            //ms-appdata:///local/XamarinVideo.mp4
            //ms-appx:///
            //ms-appx:///XamarinForms101UsingEmbeddedImages.mp4
            //loadingAnim.Position = TimeSpan.Zero;
            //loadingAnim.Play();
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                logo.WidthRequest = width / 3;
            }
        }
    }
}
