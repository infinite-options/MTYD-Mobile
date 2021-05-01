using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OldMealSched : ContentPage
    {
        public OldMealSched()
        {
            InitializeComponent();
        }

        private void clickedDelDay1(object sender, EventArgs e)
        {
            MonButton.BackgroundColor = Color.Green;
            MonButton.TextColor = Color.White;
            WedButton.BackgroundColor = Color.Transparent;
            WedButton.TextColor = Color.Black;
            FriButton.BackgroundColor = Color.Transparent;
            FriButton.TextColor = Color.Black;
        }

        private void clickedDelDay2(object sender, EventArgs e)
        {
            WedButton.BackgroundColor = Color.Green;
            WedButton.TextColor = Color.White;
            FriButton.BackgroundColor = Color.Transparent;
            FriButton.TextColor = Color.Black;
            MonButton.BackgroundColor = Color.Transparent;
            MonButton.TextColor = Color.Black;
        }

        private void clickedDelDay3(object sender, EventArgs e)
        {
            FriButton.BackgroundColor = Color.Green;
            FriButton.TextColor = Color.White;
            WedButton.BackgroundColor = Color.Transparent;
            WedButton.TextColor = Color.Black;
            MonButton.BackgroundColor = Color.Transparent;
            MonButton.TextColor = Color.Black;
        }

        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.BackgroundColor = Color.Green;
            meals1.TextColor = Color.White;
            meals2.BackgroundColor = Color.Transparent;
            meals2.TextColor = Color.Black;
            meals3.BackgroundColor = Color.Transparent;
            meals3.TextColor = Color.Black;
            meals4.BackgroundColor = Color.Transparent;
            meals4.TextColor = Color.Black;
            TotalPrice.Text = "$05.00";
        }
        private void clickedMeals2(object sender, EventArgs e)
        {
            meals2.BackgroundColor = Color.Green;
            meals2.TextColor = Color.White;
            meals1.BackgroundColor = Color.Transparent;
            meals1.TextColor = Color.Black;
            meals3.BackgroundColor = Color.Transparent;
            meals3.TextColor = Color.Black;
            meals4.BackgroundColor = Color.Transparent;
            meals4.TextColor = Color.Black;
            TotalPrice.Text = "$10.00";
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals3.BackgroundColor = Color.Green;
            meals3.TextColor = Color.White;
            meals1.BackgroundColor = Color.Transparent;
            meals1.TextColor = Color.Black;
            meals2.BackgroundColor = Color.Transparent;
            meals2.TextColor = Color.Black;
            meals4.BackgroundColor = Color.Transparent;
            meals4.TextColor = Color.Black;
            TotalPrice.Text = "$15.00";
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals4.BackgroundColor = Color.Green;
            meals4.TextColor = Color.White;
            meals1.BackgroundColor = Color.Transparent;
            meals1.TextColor = Color.Black;
            meals3.BackgroundColor = Color.Transparent;
            meals3.TextColor = Color.Black;
            meals2.BackgroundColor = Color.Transparent;
            meals2.TextColor = Color.Black;
            TotalPrice.Text = "$20.00";
        }

        private void clickedPayOp1(object sender, EventArgs e)
        {
            payOp1.BackgroundColor = Color.Green;
            payOp1.TextColor = Color.White;
            payOp2.BackgroundColor = Color.Transparent;
            payOp2.TextColor = Color.Black;
            payOp3.BackgroundColor = Color.Transparent;
            payOp3.TextColor = Color.Black;
            //TryParse(TotalPrice.Text.Substring(1, 5), double val);
        }

        private void clickedPayOp2(object sender, EventArgs e)
        {
            payOp2.BackgroundColor = Color.Green;
            payOp2.TextColor = Color.White;
            payOp1.BackgroundColor = Color.Transparent;
            payOp1.TextColor = Color.Black;
            payOp3.BackgroundColor = Color.Transparent;
            payOp3.TextColor = Color.Black;
        }

        private void clickedPayOp3(object sender, EventArgs e)
        {
            payOp3.BackgroundColor = Color.Green;
            payOp3.TextColor = Color.White;
            payOp2.BackgroundColor = Color.Transparent;
            payOp2.TextColor = Color.Black;
            payOp1.BackgroundColor = Color.Transparent;
            payOp1.TextColor = Color.Black;
        }
    }
}