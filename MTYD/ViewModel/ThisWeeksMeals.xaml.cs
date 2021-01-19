using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using MTYD.Model;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class ThisWeeksMeals : ContentPage
    {
        public static ObservableCollection<MealInfo> Meals1 = new ObservableCollection<MealInfo>();
        public static ObservableCollection<MealInfo> Meals2 = new ObservableCollection<MealInfo>();
        WebClient client = new WebClient();
        private const string menuUrl = "https://kur4j57ved.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu";
        public string text1;
        private static Dictionary<string, string> qtyDict = new Dictionary<string, string>();
        int mealCount;
        int addOnCount;

        public ThisWeeksMeals()
        {
            InitializeComponent();
            BackgroundColor = Color.FromHex("#f3f2dc");

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            setMenu();
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                heading.FontSize = width / 33;

                back.FontSize = width / 36;
                signUp.FontSize = width / 36;

                addOns.FontSize = width / 32;
                //weekOneMenu.HeightRequest = height - heading.Height - spacer1.Height - bottomGrid.Height - spacer2.Height - spacer3.Height;
            }
        }

        private void setMenu()
        {
            try
            {
                mealCount = 0;
                addOnCount = 0;
                Meals1 = new ObservableCollection<MealInfo>();
                Meals2 = new ObservableCollection<MealInfo>();
                int mealQty;
                var content = client.DownloadString(menuUrl);
                var obj = JsonConvert.DeserializeObject<UpcomingMenu>(content);
                Debug.WriteLine("first date: " + obj.Result[0].MenuDate.ToString());
                text1 = obj.Result[0].MenuDate.ToString();

                // Convert dates to json date format 2020-09-13
                var convertDay1 = String.Format("{0:yyyy-MM-dd}", text1);

                System.Diagnostics.Debug.WriteLine("Here " + convertDay1.ToString());
                

                for (int i = 0; i < obj.Result.Length; i++)
                {
                    if (!(obj.Result[i].MealCat == "Add-On") && obj.Result[i].MenuDate.Equals(convertDay1))
                    {
                        //if (qtyDict.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        //{
                        //    System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict[obj.Result[i].MenuUid.ToString()]);
                        //}
                        //System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        //if (qtyDict.ContainsKey(obj.Result[i].MealUid.ToString()))
                        //{
                        //    mealQty = Int32.Parse(qtyDict[obj.Result[i].MealUid.ToString()]);
                        //    System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);

                        //}
                        //else
                        //{
                        //    System.Diagnostics.Debug.WriteLine("Made it here2");
                        //    mealQty = 0;
                        //}
                        mealQty = 0;

                        Meals1.Add(new MealInfo
                        {
                            MealName = obj.Result[i].MealName,
                            MealCalories = "Cal: " + obj.Result[i].MealCalories.ToString(),
                            MealImage = obj.Result[i].MealPhotoUrl,
                            MealQuantity = mealQty,
                            MealPrice = obj.Result[i].MealPrice,
                            ItemUid = obj.Result[i].MealUid,
                        });

                        weekOneMenu.ItemsSource = Meals1;
                        mealCount++;
                    }
                    else if (obj.Result[i].MealCat == "Add-On" && obj.Result[i].MenuDate.Equals(convertDay1))
                    {
                        Debug.WriteLine("add-on if entered");

                        if (qtyDict.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict[obj.Result[i].MenuUid.ToString()]);
                        }
                        System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        if (qtyDict.ContainsKey(obj.Result[i].MealUid.ToString()))
                        {
                            mealQty = Int32.Parse(qtyDict[obj.Result[i].MealUid.ToString()]);
                            System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here2");
                            mealQty = 0;
                        }

                        Meals2.Add(new MealInfo
                        {
                            MealName = obj.Result[i].MealName,
                            MealCalories = "Cal: " + obj.Result[i].MealCalories.ToString(),
                            MealImage = obj.Result[i].MealPhotoUrl,
                            MealQuantity = mealQty,
                            MealPrice = obj.Result[i].MealPrice,
                            ItemUid = obj.Result[i].MealUid,
                        });

                        weekOneAddOns.ItemsSource = Meals2;
                        addOnCount++;
                    }
                }
                if (mealCount % 2 != 0)
                    mealCount++;
                weekOneMenu.HeightRequest = 280 * ((mealCount / 2));

                if (addOnCount % 2 != 0)
                    addOnCount++;
                weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));
                //mealCount++;
                BindingContext = this;
            }
            catch
            {
                Console.WriteLine("SET MENU IS CRASHING!");
            }
        }

        private void setDates()
        {
            Debug.WriteLine("setDates entered");
            try
            {
                var content = client.DownloadString(menuUrl);
                Debug.WriteLine("after content reached");
                Debug.WriteLine("content: " + content);
                var obj = JsonConvert.DeserializeObject<UpcomingMenu>(content);
                Debug.WriteLine("after obj reached");
                string[] dateArray = new string[4];
                string dayOfWeekString = String.Format("{0:dddd}", DateTime.Now);
                DateTime today = DateTime.Now;
                Dictionary<string, int> hm = new Dictionary<string, int>();
                Debug.WriteLine("after Dictionary reached");

                for (int i = 0; i < obj.Result.Length; i++)
                {
                    if (hm.ContainsKey(obj.Result[i].MenuDate))
                        hm.Remove(obj.Result[i].MenuDate);
                    hm.Add(obj.Result[i].MenuDate, i);
                }
                Debug.WriteLine("after adding to Dictionary reached");

                foreach (var i in hm)
                {
                    //datePicker.Items.Add(i.Key);
                    //String.Format("MMMM dd, yyyy", i.Key);
                }
                Debug.WriteLine("after adding to picker reached");

                //datePicker.SelectedIndex = 0;
                //text1 = hm.Keys
                //text1 = datePicker.SelectedItem.ToString();
                Debug.WriteLine("date picked: " + text1);
                Preferences.Set("dateSelected", text1.Substring(0, 11));
                Console.WriteLine("dateSet: " + Preferences.Get("dateSelected", ""));
            }
            catch
            {
                Console.WriteLine("SET DATA IS CRASHING");
            }

        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new HowItWorks();
        }

        async void clickedSignUp(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }
    }
}
