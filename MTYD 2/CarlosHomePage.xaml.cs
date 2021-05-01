using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MTYD
{
    public partial class CarlosHomePage : ContentPage
    {
        public CarlosHomePage()
        {
            InitializeComponent();
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties.Remove("time_stamp");
            Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }
    }
}
