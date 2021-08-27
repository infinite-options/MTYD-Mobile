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
using MTYD.Interfaces;
using MTYD.Constants;

namespace MTYD.ViewModel
{
    //==========================================
    // CARLOS CLASS FOR PROGRESS BAR
    public class Origin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public Thickness margin { get; set; }
        public Thickness update
        {
            get { return margin; }
            set
            {
                margin = value;
                PropertyChanged(this, new PropertyChangedEventArgs("margin"));
            }
        }
        public string mealsLeft { get; set; }
        public string barLabel
        {
            get { return mealsLeft; }
            set
            {
                mealsLeft = value;
                PropertyChanged(this, new PropertyChangedEventArgs("mealsLeft"));
            }
        }

    }
    //==========================================

    


    public partial class Select : ContentPage
    {
        //==========================================
        // CARLOS GLOBAL VARIABLES
        public double factor = 0;
        public ObservableCollection<Origin> BarParameters = new ObservableCollection<Origin>();
        //==========================================
        public ObservableCollection<PaymentInfo> NewPlan = new ObservableCollection<PaymentInfo>();

        //public string mealsLeft = "yessir";
        public string text1;
        public int weekNumber;
        public Color orange = Color.FromHex("#f59a28");
        public Color green = Color.FromHex("#006633");
        public Color beige = Color.FromHex("#f3f2dc");
        private const string purchaseId = "200-000010";
        private static string jsonMeals;
        public static ObservableCollection<MealInfo> Meals1 = new ObservableCollection<MealInfo>();
        public static ObservableCollection<MealInfo> Meals2 = new ObservableCollection<MealInfo>();
        public string userId = (string)Application.Current.Properties["user_id"];
        private string postUrl;// = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selection?customer_uid=" + userId;
        private const string menuUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu";
        private string userMeals;// = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + userId;
        //private const string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=100-000001";
        private static Dictionary<string, string> qtyDict = new Dictionary<string, string>();
        private static Dictionary<string, string> qtyDict_addon = new Dictionary<string, string>();
        private static List<MealInformation> mealsSaved = new List<MealInformation>();
        private static int mealsAllowed;
        public int count;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        ArrayList purchUidArray = new ArrayList();
        string firstIndex = "";
        public int totalMealsCount = 0;
        public bool isAlreadySelected;
        public bool isSurprise = false;
        public bool isSkip = false;
        public int firstTotalCount;
        string first; string last; string email;
        int mealCount;
        int addOnCount;
        bool addOnSelected = false;
        string selectedDotw;
        Zones[] passedZones;
        //for the carousel of dates
        List<Date> availableDates;
        int availDateIndex;
        Date selectedDate;
        Dictionary<string, Date> dateDict;
        Dictionary<string, bool> favDict;
        List<ImageButton> plates;
        bool confirmChangeDate = true;
        object dateDestination;
        int loadingCount;
        ObservableCollection<PlanName> namesColl = new ObservableCollection<PlanName>();
        DateTime addOnChargeDate;

        WebClient client = new WebClient();

        public Select(Zones[] zones, string firstName, string lastName, string userEmail)
        {
            try
            {
                addOnChargeDate = DateTime.Now;
                postUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selection?customer_uid=" + userId;
                userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + userId;
                loadingCount = 0;
                //public ObservableCollection<AddressAutocomplete> Add;
                first = firstName;
                last = lastName;
                email = userEmail;
                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                NavigationPage.SetHasBackButton(this, false);
                NavigationPage.SetHasNavigationBar(this, false);
                //checkPlatform(height, width);
                Preferences.Set("canChooseSelect", true);

                availableDates = new List<Date>();
                dateDict = new Dictionary<string, Date>();
                favDict = new Dictionary<string, bool>();
                passedZones = zones;
                InitializeComponent();
                Debug.WriteLine("user id: " + (string)Application.Current.Properties["user_id"]);

                //checkPlatform(height, width);


                //move bar initialization testing
                //==========================================
                // CARLOS PROGRESS BAR INITIALIZATION
                var m = new Origin();
                m.margin = new Thickness(0, 0, 0, 0);
                m.mealsLeft = "";

                plates = new List<ImageButton>();
                plates.Add(plate1);
                plates.Add(plate2);
                plates.Add(plate3);
                plates.Add(plate4);
                plates.Add(plate5);
                plates.Add(plate6);

                BarParameters.Add(m);
                MyCollectionView.ItemsSource = BarParameters;
                //===========================================
                Debug.WriteLine("bar initialization done");

                Preferences.Set("origMax", 0);
                //CheckVersion();

                //in check version
                getFavorites();
                GetMealPlans();
                //Task.Delay(1000).Wait();
                setDates();
                getUserMeals();
                setMenu();

                checkPlatform(height, width);
                //in check version

                //getFavorites();
                //GetMealPlans();
                ////Task.Delay(1000).Wait();
                //setDates();
                //getUserMeals();
                //setMenu();

                //var width = DeviceDisplay.MainDisplayInfo.Width;
                //var height = DeviceDisplay.MainDisplayInfo.Height;
                //NavigationPage.SetHasBackButton(this, false);
                //NavigationPage.SetHasNavigationBar(this, false);

                //dateCarousel.PeekAreaInsets = new Thickness((width / 2) - 250, 0);



                //first = firstName;
                //last = lastName;
                //email = userEmail;
                //checkPlatform(height, width);
                //Preferences.Set("canChooseSelect", true);

                ////==========================================
                //// CARLOS PROGRESS BAR INITIALIZATION
                //var m = new Origin();
                //    m.margin = new Thickness(0, 0, 0, 0);
                //    m.mealsLeft = "";

                //BarParameters.Add(m);
                //MyCollectionView.ItemsSource = BarParameters;
                ////===========================================
                //mealsSaved.Clear();
                //resetAll();
                //GetRecentSelection();

                //firstTotalCount = Int32.Parse(totalCount.Text.ToString().Substring(0,2));
                //SubscriptionPicker.SelectedIndex = 0;
                // SubscriptionPicker.SelectedIndex = 0;
                //SubscriptionPicker.Title = firstIndex;

                //weekOneMenu.HeightRequest = 2500;
                //weekOneMenu.HeightRequest = 175 * ((mealCount / 2) - 1);
                //Debug.WriteLine("mealCount:" + mealCount.ToString());
                //Debug.WriteLine("height:" + weekOneMenu.Height.ToString());
                //fillGrid();

                CheckVersion();
                Debug.WriteLine("finished with constructor");

                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    baaHeader.Text = obj.result[58].title;
                    baaBody.Text = obj.result[58].message;
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async Task CheckVersion()
        {
            var client = new AppVersion();
            string versionStr = DependencyService.Get<IAppVersionAndBuild>().GetVersionNumber();
            var result = client.isRunningLatestVersion(versionStr);
            Debug.WriteLine("isRunningLatestVersion: " + result);
            string resultStr = await result;
            if (resultStr == "FALSE")
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    await DisplayAlert(obj.result[24].title, obj.result[24].message, obj.result[24].responses);
                }
                catch
                {
                    await DisplayAlert("Mealsfor.Me\nhas gotten even better!", "Please visit the App Store to get the latest version.", "OK");
                }

                

                await CrossLatestVersion.Current.OpenAppInStore();
            }
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                //open menu adjustments
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = 140;
                menu2.Margin = new Thickness(25, 0, 0, 0);
                //menu.WidthRequest = 40; = width / 20;
                //menu2.WidthRequest = width / 20;
                //menu2.Margin = new Thickness(25, 0, 0, 30);
                heading.WidthRequest = 140;
                //open menu adjustments

                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                //orangeBox2.HeightRequest = height / 2;
                //orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                //orangeBox2.CornerRadius = height / 40;
                //heading.FontSize = width / 32;
                //heading.Margin = new Thickness(0, 0, 0, 30);
                //heading2.WidthRequest = width / 5.3;
                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;
                //pfp.Margin = new Thickness(0, 0, 23, 27);
                innerGrid.Margin = new Thickness(0, 0, 23, 27);


                if (Preferences.Get("profilePicLink", "") == "")
                {
                    string userInitials = "";
                    if (first != "" && first != null)
                    {
                        userInitials += first.Substring(0, 1);
                    }
                    if (last != "" && last != null)
                    {
                        userInitials += last.Substring(0, 1);
                    }
                    initials.Text = userInitials.ToUpper();
                    initials.FontSize = width / 38;
                }
                else pfp.Source = Preferences.Get("profilePicLink", "");

                //#F8BB17
                //#F8BB17
                menu.Margin = new Thickness(25, 0, 0, 30);
                //#F8BB17
                menu.WidthRequest = 40;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                //menu.WidthRequest = 40; = width / 20;
                menu2.WidthRequest = 40;
                //menu2.Margin = new Thickness(25, 0, 0, 30);
                //menu.WidthRequest = 40; = width / 25;
                //menu.WidthRequest = 40; = width / 20;
                //menu2.Margin = new Thickness(0, 0, 25, 15);
                
                popUpFrame.Margin = new Thickness(0, height / 10, 0, 0);
                popUpFrame.WidthRequest = width / 2.5;
                //popUpFrame.HeightRequest = popUpFrame.WidthRequest * (5/6);
                //popButton1.HeightRequest = popUpFrame.HeightRequest / 7;
                Debug.WriteLine("width: " + width.ToString());
                var frameVal = (width / 2) - 20;
                Debug.WriteLine("frameVal: " + frameVal.ToString());
                //selectPlanFrame.Margin = new Thickness(25, 7);
                //selectPlanFrame.Padding = new Thickness(15, 5);
                //selectPlanFrame.HeightRequest = height / 50;
                //lunchPic.HeightRequest = height / 30;
                //lunchPic.WidthRequest = height / 30;
                //lunchPic.Margin = new Thickness(5, -1, 0, -1);
                //SubscriptionPicker.FontSize = height / 95;
                SubscriptionPicker.VerticalOptions = LayoutOptions.Fill;
                SubscriptionPicker.HorizontalOptions = LayoutOptions.Fill;

                //selectDateFrame.Margin = new Thickness(25, 3, 25, 7);
                //selectDateFrame.Padding = new Thickness(15, 5);
                // selectDateFrame.HeightRequest = height / 50;
                //calendarPic.HeightRequest = height / 30;
                //calendarPic.WidthRequest = height / 30;
                //calendarPic.Margin = new Thickness(5, 1, 0, 1);
                //datePicker.FontSize = height / 95;
                datePicker.VerticalOptions = LayoutOptions.Fill;
                datePicker.HorizontalOptions = LayoutOptions.Fill;

                //weekOneMenu.HeightRequest = height / 6.8;

                addOns.FontSize = width / 32;

                dropDownButton.WidthRequest = (width / 2) + 20;
                dropDownList.WidthRequest = (width / 2) + 20;

                //weekOneAddOns.HeightRequest = height / 6.8;

            }
            else //android
            {
                //open menu adjustments
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = 140;
                menu2.Margin = new Thickness(25, 0, 0, 0);
                //menu.WidthRequest = 40; = width / 20;
                //menu2.WidthRequest = width / 20;
                //menu2.Margin = new Thickness(25, 0, 0, 30);
                heading.WidthRequest = 140;
                //open menu adjustments

                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                //orangeBox2.HeightRequest = height / 2;
                //orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                //orangeBox2.CornerRadius = height / 40;
                //heading.FontSize = width / 32;
                //heading.Margin = new Thickness(0, 0, 0, 30);
                //heading2.WidthRequest = width / 5.3;
                pfp.HeightRequest = 40;
                pfp.WidthRequest = 40;
                pfp.CornerRadius = 20;
                //pfp.Margin = new Thickness(0, 0, 23, 27);
                innerGrid.Margin = new Thickness(0, 0, 23, 27);

                initials.FontSize = 20;
                if (Preferences.Get("profilePicLink", "") == "")
                {
                    string userInitials = "";
                    if (first != "" && first != null)
                    {
                        userInitials += first.Substring(0, 1);
                    }
                    if (last != "" && last != null)
                    {
                        userInitials += last.Substring(0, 1);
                    }
                    initials.Text = userInitials.ToUpper();
                    initials.FontSize = 20;
                }
                else pfp.Source = Preferences.Get("profilePicLink", "");

                //#F8BB17
                //#F8BB17
                menu.Margin = new Thickness(25, 0, 0, 30);
                //#F8BB17
                menu.WidthRequest = 40;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                //menu.WidthRequest = 40; = width / 20;
                menu2.WidthRequest = 40;

                popUpFrame.Margin = new Thickness(0, height / 10, 0, 0);
                popUpFrame.WidthRequest = width / 2.5;
                Debug.WriteLine("width: " + width.ToString());
                var frameVal = (width / 2) - 20;
                Debug.WriteLine("frameVal: " + frameVal.ToString());
                SubscriptionPicker.VerticalOptions = LayoutOptions.Fill;
                SubscriptionPicker.HorizontalOptions = LayoutOptions.Fill;

                datePicker.VerticalOptions = LayoutOptions.Fill;
                datePicker.HorizontalOptions = LayoutOptions.Fill;

                //weekOneMenu.HeightRequest = height / 6.8;

                addOns.FontSize = width / 32;

                dropDownButton.WidthRequest = (width / 2) + 20;
                dropDownList.WidthRequest = (width / 2) + 20;

                saveBttn.FontSize = 14;
                surpriseBttn.FontSize = 14;
                skipBttn.FontSize = 14;
                saveBttn.Text = "Save Meals";
                surpriseBttn.Text = "Surprise Me";
                skipBttn.Text = "Skip This Day"; 
                mealsLeftLabel.FontSize = 18;
                
            }
            Debug.WriteLine("checkPlatform done");
        }

        //async void getZones()
        //{
        //    //string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;

        //    var content = client.DownloadString(url3);
        //    var obj = JsonConvert.DeserializeObject<ZonesDto>(content);
        //}
        void addy_ItemSelected(System.Object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            //dropDownText.Text = (string)dropDownList.SelectedItem;
            dropDownText.Text = ((PlanName)dropDownList.SelectedItem).name;
            //Debug.WriteLine("addy index selected: " + ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text));

            //dropDownList.IsVisible = false;
            connect.IsVisible = false;
            dropDownList.IsVisible = false;
            planChange(sender, e);
        }

        async void getFavorites()
        {
            try
            {
                GetFavPost getFav = new GetFavPost();
                getFav.customer_uid = (string)Application.Current.Properties["user_id"];
                var getFavSerializedObj = JsonConvert.SerializeObject(getFav);
                var content4 = new StringContent(getFavSerializedObj, Encoding.UTF8, "application/json");
                var client3 = new System.Net.Http.HttpClient();
                var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/get", content4);
                var message = await response3.Content.ReadAsStringAsync();
                Debug.WriteLine("RESPONSE TO getfavs   " + response3.ToString());
                Debug.WriteLine("json object sent:  " + getFavSerializedObj.ToString());
                Debug.WriteLine("message received:  " + message.ToString());

                //if there are any favorites stored
                if (message.Contains("840") == true)
                {
                    var data = JsonConvert.DeserializeObject<FavsDto>(message);
                    Debug.WriteLine("RESPONSE TO getfavs   " + response3.ToString());
                    Debug.WriteLine("favorites: " + data.result[0].favorites);

                    string favoritesList = data.result[0].favorites;

                    while (favoritesList.Contains(",") != false)
                    {
                        Debug.WriteLine("favorite: " + favoritesList.Substring(0, favoritesList.IndexOf(",")));
                        favDict.Add(favoritesList.Substring(0, favoritesList.IndexOf(",")), true);
                        favoritesList = favoritesList.Substring(favoritesList.IndexOf(",") + 1);
                    }

                    Debug.WriteLine("favorite: " + favoritesList);
                    favDict.Add(favoritesList, true);

                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        async void clickedExpand(System.Object sender, System.EventArgs e)
        {
            if (dropDownList.IsVisible == false)
            {
                connect.IsVisible = true;
                dropDownList.IsVisible = true;
            }
            else
            {
                connect.IsVisible = false;
                dropDownList.IsVisible = false;
            }

        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(first, last, email), false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(first, last, email));
        }
        /*
        protected async Task SetMealSelect()
        {
            Console.WriteLine("SetPaymentInfo Func Started!");
            PaymentInfo newPayment = new PaymentInfo();

            Item item1 = new Item();
            item1.name = Preferences.Get("item_name", "");
            item1.price = Preferences.Get("price", "00.00");
            item1.qty = Preferences.Get("freqSelected", "");
            item1.item_uid = Preferences.Get("item_uid", ""); ;
            List<Item> itemsList = new List<Item> { item1 };
            Preferences.Set("unitNum", AptEntry.Text);

            //itemsList.Add("1"); //{ "1", "5 Meal Plan", "59.99" };
            var newPaymentJSONString = JsonConvert.SerializeObject(newPayment);
            Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("SetPaymentInfo Func ENDED!");
        }*/

        private void setMenu()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                Debug.WriteLine("setMenu entered");
                mealCount = 0;
                addOnCount = 0;
                Meals1 = new ObservableCollection<MealInfo>();
                Meals2 = new ObservableCollection<MealInfo>();
                int mealQty;
                var content = client.DownloadString(menuUrl);
                var obj = JsonConvert.DeserializeObject<UpcomingMenu>(content);

                // Convert dates to json date format 2020-09-13
                var convertDay1 = String.Format("{0:yyyy-MM-dd}", selectedDate.fullDateTime);

                System.Diagnostics.Debug.WriteLine("Here " + convertDay1.ToString());

                Debug.WriteLine("obj.Result.Length:" + obj.Result.Length.ToString());
                for (int i = 0; i < obj.Result.Length; i++)
                {
                    Debug.WriteLine("meal_cat: " + obj.Result[i].MealCat);
                    Debug.WriteLine("menu_category: " + obj.Result[i].MenuCategory);
                    if (!(obj.Result[i].MealCat == "Add-On") && obj.Result[i].MenuDate.Equals(convertDay1))
                    {
                        if (qtyDict.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict[obj.Result[i].MenuUid.ToString()]);
                        }
                        System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        if (qtyDict.ContainsKey(obj.Result[i].MealName.ToString()))
                        {
                            Debug.WriteLine("inside if statement");
                            mealQty = Int32.Parse(qtyDict[obj.Result[i].MealName.ToString()]);
                            System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here2");
                            mealQty = 0;
                        }

                        Debug.WriteLine("itemuid: " + obj.Result[i].MealUid + " and meal name: " + obj.Result[i].MealName + " and meal qty: " + mealQty);

                        //b.Source = "filledHeart.png";
                        //b.Source = "emptyHeart.png";

                        string source;
                        if (favDict.ContainsKey(obj.Result[i].MealUid) == true)
                            source = "leftFilledHeart.png";
                        else source = "leftHeart.png";

                        Color backgroundColor;

                        if (mealQty > 0)
                            backgroundColor = Color.FromHex("#F8BB17");
                        else backgroundColor = Color.White;

                        Meals1.Add(new MealInfo
                        {
                            MealName = obj.Result[i].MealName,
                            MealCalories = "Cal: " + obj.Result[i].MealCalories.ToString(),
                            MealImage = obj.Result[i].MealPhotoUrl,
                            MealQuantity = mealQty,
                            Background = backgroundColor,
                            MealPrice = obj.Result[i].MenuMealPrice,
                            ItemUid = obj.Result[i].MealUid,
                            MealDesc = obj.Result[i].MealDesc,
                            SeeDesc = false,
                            SeeImage = true,
                            HeartSource = source,
                            extraBlockVisible = false
                        });

                        mealCount++;

                        //testing in new location
                        //if (mealCount % 2 != 0)
                        //    mealCount++;
                        //weekOneMenu.HeightRequest = 320 * ((mealCount / 2));

                        //weekOneMenu.ItemsSource = Meals1;
                        
                        Debug.WriteLine("mealCount incremented:" + mealCount.ToString());
                    }
                    else if (obj.Result[i].MealCat == "Add-On" && obj.Result[i].MenuDate.Equals(convertDay1))
                    {
                        Debug.WriteLine("add-on if entered");

                        //original code working with one quantity dictionary to save previous selection
                        //if (qtyDict.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        //{
                        //    System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict[obj.Result[i].MenuUid.ToString()]);
                        //}
                        //System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        //if (qtyDict.ContainsKey(obj.Result[i].MealName.ToString()))
                        //{
                        //    mealQty = Int32.Parse(qtyDict[obj.Result[i].MealName.ToString()]);
                        //    System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);

                        //}
                        //else
                        //{
                        //    System.Diagnostics.Debug.WriteLine("Made it here2");
                        //    mealQty = 0;
                        //}

                        if (qtyDict_addon.ContainsKey(obj.Result[i].MenuUid.ToString()))
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here item dict " + qtyDict_addon[obj.Result[i].MenuUid.ToString()]);
                        }
                        System.Diagnostics.Debug.WriteLine("Made it here item " + obj.Result[i].MenuUid.ToString());

                        if (qtyDict_addon.ContainsKey(obj.Result[i].MealName.ToString()))
                        {
                            mealQty = Int32.Parse(qtyDict_addon[obj.Result[i].MealName.ToString()]);
                            System.Diagnostics.Debug.WriteLine("Made it here 11 " + mealQty);

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Made it here2");
                            mealQty = 0;
                        }

                        string source;
                        if (favDict.ContainsKey(obj.Result[i].MealUid) == true)
                            source = "leftFilledHeart.png";
                        else source = "leftHeart.png";

                        Color backgroundColor;

                        if (mealQty > 0)
                            backgroundColor = Color.FromHex("#F8BB17");
                        else backgroundColor = Color.White;

                        Meals2.Add(new MealInfo
                        {
                            MealName = obj.Result[i].MealName,
                            MealCalories = "Cal: " + obj.Result[i].MealCalories.ToString(),
                            MealImage = obj.Result[i].MealPhotoUrl,
                            MealQuantity = mealQty,
                            Background = backgroundColor,
                            MealPrice = obj.Result[i].MenuMealPrice,
                            ItemUid = obj.Result[i].MealUid,
                            MealDesc = obj.Result[i].MealDesc,
                            SeeDesc = false,
                            SeeImage = true,
                            HeartSource = source,
                            extraBlockVisible = false
                        });


                        addOnCount++;
                        //testing in new location
                        //weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));

                        //weekOneAddOns.ItemsSource = Meals2;
                        
                    }
                }

                //if (mealCount % 2 != 0)
                //{
                //    Debug.WriteLine("extra meal block added");

                //    Meals1.Add(new MealInfo
                //    {
                //        MealName = "",
                //        MealCalories = "",
                //        MealImage = "",
                //        MealQuantity = 0,
                //        Background = Color.White,
                //        MealPrice = 0,
                //        ItemUid = "",
                //        MealDesc = "",
                //        SeeDesc = false,
                //        SeeImage = true,
                //        HeartSource = "",
                //        extraBlockVisible = true
                //    });
                //}

                //if (addOnCount % 2 != 0)
                //{
                //    Debug.WriteLine("extra add-on block added");

                //    Meals2.Add(new MealInfo
                //    {
                //        MealName = "fill",
                //        MealCalories = "fill",
                //        MealImage = "",
                //        MealQuantity = 0,
                //        Background = Color.White,
                //        MealPrice = 0,
                //        ItemUid = "",
                //        MealDesc = "",
                //        SeeDesc = false,
                //        SeeImage = true,
                //        HeartSource = "",
                //        extraBlockVisible = false
                //    });
                //}

                if (mealCount % 2 != 0)
                    mealCount++;
                if (addOnCount % 2 != 0)
                    addOnCount++;
                weekOneMenu.HeightRequest = 290 * ((mealCount / 2));
                weekOneAddOns.HeightRequest = 290 * ((addOnCount / 2));
                //weekOneMenu.ItemsSource = Meals1;
                //commented out to test
                //if (mealCount % 2 != 0)
                //    mealCount++;
                //weekOneMenu.HeightRequest = 320 * ((mealCount / 2));
                weekOneMenu.ItemsSource = Meals1;
                weekOneAddOns.ItemsSource = Meals2;

                //if (addOnCount % 2 != 0)
                //    addOnCount++;
                //weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));
                Debug.WriteLine("mealCount:" + mealCount.ToString());
                Debug.WriteLine("mealCount half:" + ((int)(mealCount / 2)).ToString());
                Debug.WriteLine("height:" + weekOneMenu.HeightRequest.ToString());
                BindingContext = this;

                Debug.WriteLine("weekonemenu width: " + weekOneMenu.WidthRequest.ToString());

                //testing to make sure the bar is filled
                if (isAlreadySelected == true)
                {
                    Debug.WriteLine("inside isalreadyselected if");
                    BarParameters[0].margin = new Thickness(this.Width, 0, 0, 0);
                    BarParameters[0].update = new Thickness(this.Width, 0, 0, 0);
                }

                //Task.Delay(1000).Wait();

                //Thread.Sleep(3000);

                Debug.WriteLine("SetMenu() ended");
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
                    
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        // Set Dates of Each Label
        private void setDates()
        {
            Debug.WriteLine("setDates entered");
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }


                availableDates.Clear();
                dateDict.Clear();
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

                int index1 = 0;
                foreach (var i in hm)
                {
                    datePicker.Items.Add(i.Key);
                    Debug.WriteLine("date key: " + i.Key.ToString());
                    //String.Format("MMMM dd, yyyy", i.Key);

                    //testing with carouselView
                    //format of i.Key = yyyy-mm-dd hh-mm-ss
                    //format for DateTime constructor: (year, month, day, hour, minute, second)
                    var dateObj = new DateTime(int.Parse(i.Key.Substring(0, 4)), int.Parse(i.Key.Substring(5, 2)), int.Parse(i.Key.Substring(8, 2)), 0, 0, 0);

                    Date d = new Date();
                    d.dotw = dateObj.ToString("ddd").ToUpper();
                    d.date = dateObj.ToString("MMM") + " " + dateObj.Day.ToString();
                    //d.day = ;
                    //if (d.day.Substring(0, 1) == "0")
                    //    d.day = d.day.Substring(1, 2);
                    //for just date use Substring(0, 10)
                    d.fullDateTime = i.Key;
                    d.fillColor = Color.White;
                    d.outlineColor = Color.White;
                    d.status = "Surprise / No Selection";
                    d.index = index1;
                    index1++;
                    Debug.WriteLine("fullDate: $" + d.fullDateTime + "$");
                    availableDates.Add(d);
                    Debug.WriteLine("availableDates size: " + availableDates.Count.ToString());
                    //dateDict.Add(d.fullDateTime, d);
                }

                Debug.WriteLine("after adding to picker reached");
                //dateCarousel.ItemsSource = availableDates; 
                datePicker.SelectedIndex = 0;
                availDateIndex = 0;
                selectedDate = availableDates[0];
                selectedDate.outlineColor = Color.FromHex("#F26522");
                text1 = selectedDate.fullDateTime;
                DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
                addOnChargeDate = selected.Subtract(oneDay);
                //Debug.WriteLine("date picked: " + text1);
                Preferences.Set("dateSelected", availableDates[0].fullDateTime.Substring(0,10));
                Console.WriteLine("dateSet: " + Preferences.Get("dateSelected", ""));




                int orig = Preferences.Get("origMax", 0);
                if (orig != 0)
                {
                    totalCount.Text = orig.ToString();

                }
                else
                {
                    totalCount.Text = "Count";
                }
                Preferences.Set("total", orig);
                Console.WriteLine("here before");
                //BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                //BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                Preferences.Set("dateSelected", selectedDate.fullDateTime.Substring(0, 10));
                Console.WriteLine("dateSelected: " + Preferences.Get("dateSelected", ""));

                //testing here
                getUserMeals();

                mealsSaved.Clear();   //New Addition SV
                //testing commenting out resetall 5/17
                resetAll(); //New Addition SV

                isSkip = false;
                isSurprise = false;

                if ((string)Application.Current.Properties["platform"] != "GUEST")
                {
                    checkDateStatuses();
                    GetRecentSelection();
                    //testing commenting the 2nd function out 5/17/21
                    //GetRecentSelection2();
                }
                else
                {
                    isSurprise = true;
                    isSkip = false;
                    isAlreadySelected = false;
                }
                
                dateCarousel.ItemsSource = availableDates;
                //dateCarousel1.ItemSource = availableDates;
                //testing setMenu here
                setMenu();


                Console.WriteLine("isAlreadySeleced in planchange" + isAlreadySelected);
                //bool isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                if (isAlreadySelected == true)
                {
                    Debug.WriteLine("isalreadyselected=true");
                    //selectedDate.fillColor = Color.FromHex("#F8BB17");
                    saveBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    saveFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    saveBttn.TextColor = Color.White;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;


                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);

                    totalCount.Text = "0";
                    Preferences.Set("total", 0);
                    Console.WriteLine("before true");
                    BarParameters[0].mealsLeft = "All Meals Selected";
                    mealsLeftLabel.Text = "All Meals Selected";
                    BarParameters[0].barLabel = "All Meals Selected";
                    BarParameters[0].margin = new Thickness(this.Width, 0, 0, 0);
                    BarParameters[0].update = new Thickness(this.Width, 0, 0, 0);
                    Console.WriteLine("after true");
                    Preferences.Set("origMax", int.Parse(s));

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "plate.png";
                    }
                        

                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    //DisplayAlert("Alert", "Select reset button to change your meal selections", "OK");
                    //weekOneProgress.Progress = 1;
                }
                else if (isAlreadySelected == false)
                {
                    Debug.WriteLine("isalreadyselected=false");
                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before false");
                    var holder = BarParameters[0].mealsLeft;
                    holder = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    BarParameters[0].mealsLeft = holder;
                    mealsLeftLabel.Text = "Please Select " + Preferences.Get("total", "").ToString() + " Meals"; ;

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }

                    //for (int i = (6 - int.Parse(s)); i < int.Parse(Preferences.Get("total", "").ToString()) + ((6 - int.Parse(s))); i++)
                    //    plates[i].Source = "plate.png";
                    //for (int i = int.Parse(Preferences.Get("total", "").ToString()) + ((6 - int.Parse(s))); i < 6; i++)
                    //    plates[i].Source = "";

                    BarParameters[0].barLabel = holder;
                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    Console.WriteLine("after false");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }

                if (isSkip)
                {
                    Debug.WriteLine("isSkip");
                    //selectedDate.fillColor = Color.FromHex("#FF9E19");
                    //testing to try and send correct json object
                    //qtyDict.Clear();

                    skipBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    skipFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    skipBttn.TextColor = Color.White;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;



                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before skip");
                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    mealsLeftLabel.Text = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                        //plates[i].IsVisible = true;

                    Console.WriteLine("after skip");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }
                else if (isSurprise)
                {
                    Debug.WriteLine("isSurprise");
                    //testing to try and send correct json object
                    //qtyDict.Clear();

                    surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseBttn.TextColor = Color.White;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;



                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before surprise");
                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    mealsLeftLabel.Text = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                        //plates[i].IsVisible = true;

                    Console.WriteLine("after surprise");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }
                else
                {
                    Debug.WriteLine("new plan");
                    //If neither skip or surprise (new plan), then initialize to surprise
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;



                    if (isAlreadySelected == false)
                        surprise();

                }

                //Thread.Sleep(3000);

                Debug.WriteLine("setDates ended");
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }

        }

        void clickedContinue(System.Object sender, System.EventArgs e)
        {
            //fade.IsVisible = false;
            clickedClosePopUp(sender, e);
            confirmChangeDate = true;
            dateChangeCar(dateDestination, e);
            //confirmChangeDate = true;
        }
       
        /////////date change for carousel view
        async private void dateChangeCar(object sender, EventArgs e)
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                string s2 = dropDownText.Text;
                s2 = s2.Substring(0, 2);
                //Preferences.Set("total", int.Parse(s));
                //totalCount.Text = Preferences.Get("total", 0).ToString();
                if (confirmChangeDate == false && (int.Parse(s2).ToString() != totalCount.Text || mealsLeftLabel.Text.IndexOf("Click Save") != -1))
                {
                    popButton1.Text = "Go Back";
                    popButton2.IsVisible = true;
                    dateDestination = sender; //52

                    try
                    {
                        WebClient client4 = new WebClient();
                        var content3 = client4.DownloadString(Constant.AlertUrl);
                        var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                        showPopUp(obj.result[51].title, obj.result[51].message);
                    }
                    catch
                    {
                        showPopUp("Selections Not Saved", "Please click Save Meals to save your meal selections before moving to another date. You can also click Surprise or Skip.");
                    }

                    

                    return;
                }

                dateCarousel.ItemsSource = availableDates;
                qtyDict.Clear();
                qtyDict_addon.Clear();

                //testing setMenu() earlier didnt work
                //setMenu();

                //getUserMeals();
                Console.WriteLine("Setting now");
                text1 = selectedDate.fullDateTime;
                string tempHolder = text1;
                Debug.WriteLine("year:" + tempHolder.Substring(0, 4));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("month:" + text1.Substring(5, 2));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("day:" + text1.Substring(8, 2));
                //getDayOfTheWeek();
                //DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                //TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
                //addOnChargeDate = selected.Subtract(oneDay);
                //Debug.WriteLine("selected date: " + selected.ToString("D") + " and addOnChargeDate: " + addOnChargeDate.ToString("D"));

                Debug.WriteLine(sender.GetType().ToString());
                Button button1 = (Button)sender;
                Date dateChosen = button1.BindingContext as Date;
                selectedDate.outlineColor = Color.White;
                selectedDate = dateChosen;
                text1 = dateChosen.fullDateTime;
                DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
                addOnChargeDate = selected.Subtract(oneDay);
                Debug.WriteLine("selected date: " + selected.ToString("D") + " and addOnChargeDate: " + addOnChargeDate.ToString("D"));
                selectedDate.outlineColor = Color.FromHex("#F26522");
                //dateChosen.fillColor = Color.LightGray;
                selectedDotw = dateChosen.dotw;
                Debug.WriteLine("dayOfWeek: " + selectedDotw);




                //testing no setMenu();
                //setMenu();

                //weekOneProgress.Progress = 0;


                int orig = Preferences.Get("origMax", 0);
                if (orig != 0)
                {
                    totalCount.Text = orig.ToString();

                }
                else
                {
                    totalCount.Text = "Count";
                }
                Preferences.Set("total", orig);
                Console.WriteLine("here before");
                //BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                //BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                Preferences.Set("dateSelected", dateChosen.fullDateTime.Substring(0, 10));
                Console.WriteLine("dateSelected: " + Preferences.Get("dateSelected", ""));

                //testing here
                getUserMeals();

                mealsSaved.Clear();   //New Addition SV
                //testing commenting out resetall 5/17
                resetAll(); //New Addition SV

                isSkip = false;
                isSurprise = false;

                if ((string)Application.Current.Properties["platform"] != "GUEST")
                {
                    await GetRecentSelection();
                    //testing commenting the 2nd function out 5/17/21
                    //GetRecentSelection2();
                }
                else
                {
                    isSurprise = true;
                    isSkip = false;
                    isAlreadySelected = false;
                }

                //testing setMenu here
                setMenu();


                Console.WriteLine("isAlreadySeleced in planchange" + isAlreadySelected);
                //bool isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                if (isAlreadySelected == true)
                {
                    saveBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    saveFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    saveBttn.TextColor = Color.White;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;


                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);

                    totalCount.Text = "0";
                    Preferences.Set("total", 0);
                    Console.WriteLine("before true");
                    BarParameters[0].mealsLeft = "All Meals Selected";
                    mealsLeftLabel.Text = "All Meals Selected";
                    BarParameters[0].barLabel = "All Meals Selected";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "plate.png";
                    }
                    //plates[i].IsVisible = true;

                    BarParameters[0].margin = new Thickness(this.Width, 0, 0, 0);
                    BarParameters[0].update = new Thickness(this.Width, 0, 0, 0);
                    Console.WriteLine("after true");
                    Preferences.Set("origMax", int.Parse(s));
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    //DisplayAlert("Alert", "Select reset button to change your meal selections", "OK");
                    //weekOneProgress.Progress = 1;
                }
                else if (isAlreadySelected == false)
                {
                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before false");
                    var holder = BarParameters[0].mealsLeft;
                    holder = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    BarParameters[0].mealsLeft = holder;
                    mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                    BarParameters[0].barLabel = holder;

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                    //plates[i].IsVisible = true;

                    //for (int i = (6 - int.Parse(s)); i < int.Parse(Preferences.Get("total", "").ToString()) + ((6 - int.Parse(s))); i++)
                    //    plates[i].Source = "plate.png";
                    //for (int i = int.Parse(Preferences.Get("total", "").ToString()) + ((6 - int.Parse(s))); i < 6; i++)
                    //    plates[i].Source = "";

                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    Console.WriteLine("after false");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }

                if (isSkip)
                {
                    //testing to try and send correct json object
                    //qtyDict.Clear();

                    skipBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    skipFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    skipBttn.TextColor = Color.White;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;



                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before skip");
                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                    BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                    //plates[i].IsVisible = true;

                    Console.WriteLine("after skip");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }
                else if (isSurprise)
                {
                    //testing to try and send correct json object
                    //qtyDict.Clear();

                    surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseBttn.TextColor = Color.White;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;



                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before surprise");
                    BarParameters[0].margin = 0;
                    BarParameters[0].update = 0;
                    BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                    BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                    //plates[i].IsVisible = true;



                    Console.WriteLine("after surprise");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                    //weekOneProgress.Progress = 0;
                }
                else
                {
                    Debug.WriteLine("new plan");
                    //If neither skip or surprise (new plan), then initialize to surprise
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;



                    if (isAlreadySelected == false)
                        surprise();

                }
                //reset the buttons
                //default to surprise if null
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                Debug.WriteLine("dateChange ended");
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }
        //////////

        private async void planChange(object sender, EventArgs e)
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                qtyDict.Clear();
                qtyDict_addon.Clear();
                //getUserMeals();
                //if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("5 "))
                //{
                //    mealsAllowed = 5;
                //}
                //else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("10"))
                //{
                //    mealsAllowed = 10;
                //}
                //else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("15"))
                //{
                //    mealsAllowed = 15;
                //}
                //else if (SubscriptionPicker.SelectedItem.ToString().Substring(0, 2).Equals("20"))
                //{
                //    mealsAllowed = 20;
                //}
                Console.WriteLine("meals allowed " + mealsAllowed);

                isSkip = false;
                isSurprise = false;
                //weekOneProgress.Progress = 0;
                //firstTotalCount = Int32.Parse(totalCount.Text.ToString().Substring(0, 2));

                /* 
                 * SV COMMENT 11/17 Testing TotalCount.Text
                int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                string s = SubscriptionPicker.SelectedItem.ToString();
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                //totalCount.Text = Preferences.Get("total", 0).ToString();
              //  Preferences.Set("origMax", int.Parse(s));
                */
                if ((string)Application.Current.Properties["platform"] != "GUEST")
                {
                    //int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
                    //int indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                    int indexOfMealPlanSelected = -1;
                    //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                    foreach (var plan in namesColl)
                    {
                        indexOfMealPlanSelected++;
                        if (plan.name == dropDownText.Text)
                        {
                            break;
                        }
                    }

                    if (indexOfMealPlanSelected < 0)
                        indexOfMealPlanSelected = 0;
                    Debug.WriteLine("index of meal plan selected in plan change: " + indexOfMealPlanSelected.ToString());
                    Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                    Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                    Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));
                    //testing
                    getUserMeals();
                    await checkDateStatuses();

                    // Button b = (Button)sender;
                    // MealInfo ms = b.BindingContext as MealInfo;
                    // ms.MealQuantity = 0;
                    mealsSaved.Clear(); //New Addition SV
                    //testing commenting out resetall 5/17
                    resetAll(); //New Addition SV
                                //Task.Delay(500).Wait();
                                //getUserMeals();
                    await GetRecentSelection();
                    //testing commenting the 2nd function out 5/17/21
                    //GetRecentSelection2();
                    setMenu();

                    Console.WriteLine("isAlreadySeleced in planchange" + isAlreadySelected);

                    //bool isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                    if (isAlreadySelected == true)
                    {
                        saveBttn.BackgroundColor = Color.FromHex("#F8BB17");
                        saveFrame.BackgroundColor = Color.FromHex("#F8BB17");
                        saveBttn.TextColor = Color.White;
                        skipBttn.BackgroundColor = Color.White;
                        skipFrame.BackgroundColor = Color.White;
                        skipBttn.TextColor = Color.Black;
                        surpriseBttn.BackgroundColor = Color.White;
                        surpriseFrame.BackgroundColor = Color.White;
                        surpriseBttn.TextColor = Color.Black;

                        //string s = SubscriptionPicker.SelectedItem.ToString();
                        string s = dropDownText.Text;
                        s = s.Substring(0, 2);

                        totalCount.Text = "0";
                        Preferences.Set("total", 0);
                        Console.WriteLine("before true alreadyselected");
                        BarParameters[0].mealsLeft = "All Meals Selected";
                        mealsLeftLabel.Text = "All Meals Selected";
                        BarParameters[0].barLabel = "All Meals Selected";
                        BarParameters[0].margin = new Thickness(this.Width, 0, 0, 0);
                        BarParameters[0].update = new Thickness(this.Width, 0, 0, 0);

                        for (int i = 0; i < (6 - int.Parse(s)); i++)
                            plates[i].IsVisible = false;
                        for (int i = (6 - int.Parse(s)); i < 6; i++)
                        {
                            plates[i].IsVisible = true;
                            plates[i].Source = "plate.png";
                        }
                        //plates[i].IsVisible = true;

                        Console.WriteLine("after true");
                        Preferences.Set("origMax", int.Parse(s));
                        totalCount.Text = Preferences.Get("total", 0).ToString();
                        //DisplayAlert("Alert", "Select reset button to change your meal selections", "OK");
                        //weekOneProgress.Progress = 1;
                    }
                    else if (isAlreadySelected == false)
                    {
                        //string s = SubscriptionPicker.SelectedItem.ToString();
                        string s = dropDownText.Text;
                        s = s.Substring(0, 2);
                        Preferences.Set("total", int.Parse(s));
                        Console.WriteLine("before false");
                        var holder = BarParameters[0].mealsLeft;
                        holder = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                        BarParameters[0].mealsLeft = holder;
                        mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                        BarParameters[0].barLabel = holder;

                        for (int i = 0; i < (6 - int.Parse(s)); i++)
                            plates[i].IsVisible = false;
                        for (int i = (6 - int.Parse(s)); i < 6; i++)
                        {
                            plates[i].IsVisible = true;
                            plates[i].Source = "";
                        }
                        //plates[i].IsVisible = true;



                        BarParameters[0].margin = 0;
                        BarParameters[0].update = 0;
                        Console.WriteLine("after false");
                        totalCount.Text = Preferences.Get("total", 0).ToString();
                        Preferences.Set("origMax", int.Parse(s));
                        //weekOneProgress.Progress = 0;
                    }

                    if (isSkip)
                    {
                        //testing to try and send correct json object
                        //qtyDict.Clear();

                        skipBttn.BackgroundColor = Color.FromHex("#F8BB17");
                        skipFrame.BackgroundColor = Color.FromHex("#F8BB17");
                        skipBttn.TextColor = Color.White;
                        surpriseBttn.BackgroundColor = Color.White;
                        surpriseFrame.BackgroundColor = Color.White;
                        surpriseBttn.TextColor = Color.Black;
                        saveBttn.BackgroundColor = Color.White;
                        saveFrame.BackgroundColor = Color.White;
                        saveBttn.TextColor = Color.Black;

                        //indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                        int selectedIndex = -1;
                        //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                        foreach (var plan in namesColl)
                        {
                            selectedIndex++;
                            if (plan.name == dropDownText.Text)
                            {
                                break;
                            }
                        }
                        Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                        Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                        Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                        //string s = SubscriptionPicker.SelectedItem.ToString();
                        string s = dropDownText.Text;
                        s = s.Substring(0, 2);
                        Preferences.Set("total", int.Parse(s));
                        Console.WriteLine("before skip");
                        BarParameters[0].margin = 1;
                        BarParameters[0].update = 1;
                        BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                        mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                        BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                        for (int i = 0; i < (6 - int.Parse(s)); i++)
                            plates[i].IsVisible = false;
                        for (int i = (6 - int.Parse(s)); i < 6; i++)
                        {
                            plates[i].IsVisible = true;
                            plates[i].Source = "";
                        }
                        //plates[i].IsVisible = true;

                        Console.WriteLine("after skip");
                        totalCount.Text = Preferences.Get("total", 0).ToString();
                        Preferences.Set("origMax", int.Parse(s));
                        //weekOneProgress.Progress = 0;
                    }
                    else if (isSurprise)
                    {
                        //testing to try and send correct json object
                        //qtyDict.Clear();

                        surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                        surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                        surpriseBttn.TextColor = Color.White;
                        skipBttn.BackgroundColor = Color.White;
                        skipFrame.BackgroundColor = Color.White;
                        skipBttn.TextColor = Color.Black;
                        saveBttn.BackgroundColor = Color.White;
                        saveFrame.BackgroundColor = Color.White;
                        saveBttn.TextColor = Color.Black;

                        //indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                        int selectedIndex = -1;
                        //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                        foreach (var plan in namesColl)
                        {
                            selectedIndex++;
                            if (plan.name == dropDownText.Text)
                            {
                                break;
                            }
                        }
                        Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                        Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                        Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                        //string s = SubscriptionPicker.SelectedItem.ToString();
                        string s = dropDownText.Text;
                        s = s.Substring(0, 2);
                        Preferences.Set("total", int.Parse(s));
                        Console.WriteLine("before surprise");
                        BarParameters[0].margin = 1;
                        BarParameters[0].update = 1;
                        BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                        mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                        BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                        for (int i = 0; i < (6 - int.Parse(s)); i++)
                            plates[i].IsVisible = false;
                        for (int i = (6 - int.Parse(s)); i < 6; i++)
                        {
                            plates[i].IsVisible = true;
                            plates[i].Source = "";
                        }
                        //plates[i].IsVisible = true;

                        Console.WriteLine("after surprise");
                        totalCount.Text = Preferences.Get("total", 0).ToString();
                        Preferences.Set("origMax", int.Parse(s));
                        //weekOneProgress.Progress = 0;
                    }
                    else
                    {

                        //If neither skip or surprise (new plan), then initialize to surprise
                        skipBttn.BackgroundColor = Color.White;
                        skipFrame.BackgroundColor = Color.White;
                        skipBttn.TextColor = Color.Black;
                        surpriseBttn.BackgroundColor = Color.White;
                        surpriseFrame.BackgroundColor = Color.White;
                        surpriseBttn.TextColor = Color.Black;
                        if (isAlreadySelected == false)
                            surprise();

                    }
                    //GetRecentSelection(); //11/17 10pm comment SV

                    //calcTotal();
                    /* //Testing 11/12 Total meals count
                    int totalMealsCount = 110;
                    for (int i = 0; i < Meals1.Count; i++)
                    {
                        if (Meals1[i].MealQuantity > 0)
                        {
                            totalMealsCount += Int32.Parse(Meals1[i].MealQuantity.ToString());
                        }
                    } */
                    Console.WriteLine("Meals1 Count: " + totalMealsCount);
                    //11/12
                    //Preferences.Set("total", Meals1.Count);
                    //totalCount.Text = Preferences.Get("total", 0).ToString();
                    //Preferences.Set("origMax", int.Parse(s));

                    //
                    //GetMealPlans();
                    //setDates();

                    //commented out 11/11 for second merge

                    //setMenu();
                }
                else
                {
                    surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    surpriseBttn.TextColor = Color.White;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;

                    //indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
                    //Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                    //Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                    //string s = SubscriptionPicker.SelectedItem.ToString();
                    string s = dropDownText.Text;
                    s = s.Substring(0, 2);
                    Preferences.Set("total", int.Parse(s));
                    Console.WriteLine("before surprise");
                    BarParameters[0].margin = 1;
                    BarParameters[0].update = 1;
                    BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                    mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                    BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";

                    for (int i = 0; i < (6 - int.Parse(s)); i++)
                        plates[i].IsVisible = false;
                    for (int i = (6 - int.Parse(s)); i < 6; i++)
                    {
                        plates[i].IsVisible = true;
                        plates[i].Source = "";
                    }
                    //plates[i].IsVisible = true;

                    Console.WriteLine("after surprise");
                    totalCount.Text = Preferences.Get("total", 0).ToString();
                    Preferences.Set("origMax", int.Parse(s));
                }

                //Thread.Sleep(3000);
                await Task.Delay(1000);
                Debug.WriteLine("planChange ended");
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        /*
        // Navigation Bar
        private async void onNavClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Equals(SubscribeNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectNav))
            {
                await Navigation.PushAsync(new Select());
            }
        }

        // Navigation Bar Icons
        private async void onNavIconClick(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;

            if (button.Equals(SubscribeIconNav))
            {
                await Navigation.PushAsync(new SubscriptionPage());
            }
            else if (button.Equals(ProfileIconNav))
            {
                await Navigation.PushAsync(new Profile());
            }
            else if (button.Equals(SelectIconNav))
            {
                await Navigation.PushAsync(new Select());
            }

        }*/

        // Favorite BUtton
        private async void clickedFavorite(object sender, EventArgs e)
        {
            try
            {
                ImageButton b = (ImageButton)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                //ms.MealQuantity++;

                //favoriting
                if (b.Source.ToString().Equals("File: leftHeart.png"))
                {
                    favDict.Add(ms.ItemUid, true);
                    UpdateFavPost updateFav = new UpdateFavPost();
                    updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                    updateFav.favorite = ms.ItemUid;
                    var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                    var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                    var client3 = new System.Net.Http.HttpClient();
                    //post endpoint passes in 1 favorite and appends it to the end of the list of favorites for the user
                    var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/post", content4);
                    var message = await response3.Content.ReadAsStringAsync();
                    Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                    Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                    Debug.WriteLine("message received:  " + message.ToString());
                    //b.Source = "heart.png";
                    ms.HeartSource = "leftFilledHeart.png";

                }
                //unfavoriting
                else
                {
                    favDict.Remove(ms.ItemUid);

                    //if there are no favorites to be saved, send in a blank list of favorites
                    if (favDict.Count == 0)
                    {
                        UpdateFavPost updateFav = new UpdateFavPost();
                        updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                        updateFav.favorite = "";
                        var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                        var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                        var client3 = new System.Net.Http.HttpClient();
                        //overwrites the entire favorites list and only keep the favorite that is passed in
                        var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                        var message = await response3.Content.ReadAsStringAsync();
                        Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                        Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                        Debug.WriteLine("message received:  " + message.ToString());
                    }
                    else if (favDict.Count == 1)
                    {
                        UpdateFavPost updateFav = new UpdateFavPost();
                        updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                        updateFav.favorite = favDict.Keys.First();
                        var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                        var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                        var client3 = new System.Net.Http.HttpClient();
                        //update overwrites the entire favorites list and only keep the favorite that is passed in
                        var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                        var message = await response3.Content.ReadAsStringAsync();
                        Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                        Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                        Debug.WriteLine("message received:  " + message.ToString());
                    }
                    else if (favDict.Count > 1)
                    {
                        UpdateFavPost updateFav = new UpdateFavPost();
                        updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                        updateFav.favorite = favDict.Keys.First();
                        //var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                        //var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                        //var client3 = new System.Net.Http.HttpClient();
                        ////update overwrites the entire favorites list and only keep the favorite that is passed in
                        //var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                        //var message = await response3.Content.ReadAsStringAsync();
                        //Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                        //Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                        //Debug.WriteLine("message received:  " + message.ToString());

                        foreach (string uid in favDict.Keys)
                        {
                            if (uid == favDict.Keys.First())
                                continue;
                            else updateFav.favorite += "," + uid;


                            //UpdateFavPost updateFav2 = new UpdateFavPost();
                            //updateFav2.customer_uid = (string)Application.Current.Properties["user_id"];
                            //updateFav2.favorite = uid;
                            //var updateFavSerializedObj2 = JsonConvert.SerializeObject(updateFav);
                            //var content2 = new StringContent(updateFavSerializedObj2, Encoding.UTF8, "application/json");
                            //var client2 = new System.Net.Http.HttpClient();
                            ////post endpoint passes in 1 favorite and appends it to the end of the list of favorites for the user
                            //var response2 = await client2.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/post", content2);
                            //var message2 = await response2.Content.ReadAsStringAsync();
                            //Debug.WriteLine("RESPONSE TO updatefavs   " + response2.ToString());
                            //Debug.WriteLine("json object sent:  " + updateFavSerializedObj2.ToString());
                            //Debug.WriteLine("message received:  " + message2.ToString());
                        }
                        var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                        var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                        var client3 = new System.Net.Http.HttpClient();
                        //update overwrites the entire favorites list and only keep the favorite that is passed in
                        var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                        var message = await response3.Content.ReadAsStringAsync();
                        Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                        Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                        Debug.WriteLine("message received:  " + message.ToString());
                    }

                    //b.Source = "heartoutline.png";
                    ms.HeartSource = "leftHeart.png";

                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        //info button
        private void clickedInfo(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            MealInfo ms = b.BindingContext as MealInfo;

            if (ms.SeeDesc == false)
            {
                ms.SeeImage = false;
                ms.SeeDesc = true;
            }
            else
            {
                ms.SeeImage = true;
                ms.SeeDesc = false;
            }
        }

        ////info button
        //private void clickedInfo(object sender, EventArgs e)
        //{
        //    ImageButton b = (ImageButton)sender;
        //    MealInfo ms = b.BindingContext as MealInfo;

        //    if (ms.SeeDesc == false)
        //    {
        //        ms.SeeImage = false;
        //        ms.SeeDesc = true;
        //    }
        //    else
        //    {
        //        ms.SeeImage = true;
        //        ms.SeeDesc = false;
        //    }
        //}

        private async void clickIncrease(object sender, EventArgs e)
        {
            try
            {
                int count = Preferences.Get("total", 0);
                int permCount = Preferences.Get("origMax", 0);
                if (count != 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Debug.WriteLine("plate source: " + plates[i].Source.ToString());
                        if (plates[i].IsVisible == true && !(plates[i].Source.ToString().Contains("plate.png")))
                        {
                            plates[i].Source = "plate.png";
                            break;
                        }
                    }

                    confirmChangeDate = false;

                    //testing reseting the save button when they increase or decrease amount of meals/add-ons
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    // ======================================
                    // CARLOS PROGRESS BAR INTEGRATION
                    var width = this.Width;
                    var result = this.Width / Preferences.Get("origMax", 0);
                    Debug.WriteLine("DIVISOR INCREMENT FUNCTION: " + Preferences.Get("origMax", 0));
                    factor = result;

                    Debug.WriteLine("FACTOR TO INCREASE OR DECREASE PROGRESS BAR: " + factor);
                    Debug.WriteLine("WIDTH                                      : " + width);

                    var currentMargin = BarParameters[0].margin;
                    var currentLeft = currentMargin.Left;
                    var newLeft = currentLeft + factor;
                    Debug.WriteLine("testing for meals left: " + (newLeft / factor).ToString());

                    Debug.WriteLine("CURRENT LEFT: " + currentLeft);
                    Debug.WriteLine("NEW LEFT" + newLeft);

                    currentMargin.Left = newLeft;

                    BarParameters[0].margin = currentMargin;
                    BarParameters[0].update = currentMargin;
                    // ======================================
                    totalCount.Text = (--count).ToString();
                    //BarParameters[0].mealsLeft = count.ToString();

                    if (count == 0)
                    {
                        Debug.WriteLine("final margin: " + BarParameters[0].update.ToString());
                        BarParameters[0].mealsLeft = "All Meals Selected (Click Save)";
                        mealsLeftLabel.Text = "All Meals Selected (Click Save)";
                        BarParameters[0].barLabel = "All Meals Selected (Click Save)";
                        //progress.Text = "All Meals Selected";
                    }
                    else
                    {
                        BarParameters[0].mealsLeft = "Please Select " + count.ToString() + " Meals";
                        mealsLeftLabel.Text = "Please Select " + count.ToString() + " Meals";
                        BarParameters[0].barLabel = "Please Select " + count.ToString() + " Meals";
                    }
                    //else progressLabel.Text = "Please Select " + count.ToString() + " Meals";

                    Preferences.Set("total", count);

                    ImageButton b = (ImageButton)sender;
                    MealInfo ms = b.BindingContext as MealInfo;
                    ms.MealQuantity++;
                    ms.Background = Color.FromHex("#F8BB17");

                    //float adder = 0.0f;
                    if (permCount == 5)
                    {
                        //adder = 0.2f;
                        //weekOneProgress.Progress += 0.2;
                    }
                    else if (permCount == 10)
                    {
                        //adder = 0.1f;
                        //weekOneProgress.Progress += 0.1;
                    }
                    else if (permCount == 15)
                    {
                        //adder = 0.067f;
                        //weekOneProgress.Progress += 0.067;
                    }
                    else if (permCount == 20)
                    {
                        //adder = 0.05f;
                        //weekOneProgress.Progress += 0.05;
                    }

                    //weekOneProgress.Progress -= 0.1;
                    //weekOneProgress.Progress += adder;

                    //if (weekOneProgress.Progress < 0.3)
                    //{
                    //    weekOneProgress.ProgressColor = Color.LightGoldenrodYellow;
                    //}
                    //else if (weekOneProgress.Progress >= 0.3 && weekOneProgress.Progress < 0.5)
                    //{
                    //    weekOneProgress.ProgressColor = Color.Orange;
                    //}
                    //else if (weekOneProgress.Progress >= 0.5 && weekOneProgress.Progress <= 0.7)
                    //{
                    //    weekOneProgress.ProgressColor = Color.LightGreen;
                    //}
                    //else if (weekOneProgress.Progress >= 0.8)
                    //{
                    //    weekOneProgress.ProgressColor = Color.DarkOliveGreen;
                    //}
                }
                else
                {
                    popButton1.Text = "Okay";
                    popButton2.IsVisible = false; //53

                    try
                    {
                        WebClient client4 = new WebClient();
                        var content3 = client4.DownloadString(Constant.AlertUrl);
                        var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                        showPopUp(obj.result[52].title, obj.result[52].message);
                    }
                    catch
                    {
                        showPopUp("Oops", "You have reached the maximum amount of meals that can be selected for this plan.");
                    }

                    
                    //DisplayAlert("Alert", "You have reached the maximum amount of meals that can be selected for this plan.", "OK");
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void clickIncreaseAddOn(object sender, EventArgs e)
        {
            try
            {
                //testing reseting the save button when they increase or decrease amount of meals/add-ons
                surpriseBttn.BackgroundColor = Color.White;
                surpriseFrame.BackgroundColor = Color.White;
                surpriseBttn.TextColor = Color.Black;
                saveBttn.BackgroundColor = Color.White;
                saveFrame.BackgroundColor = Color.White;
                saveBttn.TextColor = Color.Black;
                skipBttn.BackgroundColor = Color.White;
                skipFrame.BackgroundColor = Color.White;
                skipBttn.TextColor = Color.Black;
                confirmChangeDate = false;

                ImageButton b = (ImageButton)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                ms.MealQuantity++;
                ms.Background = Color.FromHex("#F8BB17");
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void clickDecrease(object sender, EventArgs e)
        {
            try
            {
                ImageButton b = (ImageButton)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                int count = Preferences.Get("total", 0);
                //if meal quantity is 0, don't do anything
                if (count != Preferences.Get("origMax", 0) && ms.MealQuantity != 0)
                {

                    for (int i = 5; i >= 0; i--)
                    {
                        if (plates[i].IsVisible == true && plates[i].Source.ToString().Contains("plate.png"))
                        {
                            plates[i].Source = "";
                            break;
                        }
                    }

                    //testing reseting the save button when they increase or decrease amount of meals/add-ons
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;
                    // ======================================
                    // CARLOS PROGRESS BAR INTEGRATION
                    var width = this.Width;
                    var result = this.Width / Preferences.Get("origMax", 0);
                    Debug.WriteLine("DIVISOR DECREMENT FUNCTION: " + Preferences.Get("origMax", 0));
                    factor = result;

                    Debug.WriteLine("FACTOR TO INCREASE OR DECREASE PROGRESS BAR: " + factor);
                    Debug.WriteLine("WIDTH                                      : " + width);

                    var currentMargin = BarParameters[0].margin;
                    var currentLeft = currentMargin.Left;
                    var newLeft = currentLeft - factor;

                    Debug.WriteLine("CURRENT LEFT: " + currentLeft);
                    Debug.WriteLine("NEW LEFT" + newLeft);

                    currentMargin.Left = newLeft;

                    BarParameters[0].margin = currentMargin;
                    BarParameters[0].update = currentMargin;

                    // ======================================
                    if (ms.MealQuantity != 0)
                    {
                        confirmChangeDate = false;

                        totalCount.Text = (++count).ToString();
                        BarParameters[0].mealsLeft = "Please Select " + count.ToString() + " Meals";
                        mealsLeftLabel.Text = "Please Select " + count.ToString() + " Meals";
                        BarParameters[0].barLabel = "Please Select " + count.ToString() + " Meals";
                        Preferences.Set("total", count);
                        //BarParameters[0].mealsLeft = count.ToString();
                        ms.MealQuantity--;

                        if (ms.MealQuantity == 0)
                            ms.Background = Color.White;

                        int permCount = Preferences.Get("origMax", 0);
                        //float adder = 0.0f;
                        //if (permCount == 5)
                        //{
                        //    //adder = 0.2f;
                        //    weekOneProgress.Progress -= 0.2;
                        //}
                        //else if (permCount == 10)
                        //{
                        //    //adder = 0.1f;
                        //    weekOneProgress.Progress -= 0.1;
                        //}
                        //else if (permCount == 15)
                        //{
                        //    //adder = 0.067f;
                        //    weekOneProgress.Progress -= 0.067;
                        //}
                        //else if (permCount == 20)
                        //{
                        //    //adder = 0.05f;
                        //    weekOneProgress.Progress -= 0.05;
                        //}

                        //weekOneProgress.Progress -= 0.1;
                        // weekOneProgress.Progress -= adder;
                        //if (weekOneProgress.Progress < 0.3)
                        //{
                        //    weekOneProgress.ProgressColor = Color.LightGoldenrodYellow;
                        //}
                        //else if (weekOneProgress.Progress >= 0.3 && weekOneProgress.Progress < 0.5)
                        //{
                        //    weekOneProgress.ProgressColor = Color.Orange;
                        //}
                        //else if (weekOneProgress.Progress >= 0.5 && weekOneProgress.Progress <= 0.7)
                        //{
                        //    weekOneProgress.ProgressColor = Color.LightGreen;
                        //}
                        //else if (weekOneProgress.Progress >= 0.8)
                        //{
                        //    weekOneProgress.ProgressColor = Color.DarkOliveGreen;
                        //}
                    }
                    else { }

                }
                else { }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void clickDecreaseAddOn(object sender, EventArgs e)
        {
            try
            {
                ImageButton b = (ImageButton)sender;
                MealInfo ms = b.BindingContext as MealInfo;
                if (ms.MealQuantity != 0)
                {
                    confirmChangeDate = false;
                    //testing reseting the save button when they increase or decrease amount of meals/add-ons
                    surpriseBttn.BackgroundColor = Color.White;
                    surpriseFrame.BackgroundColor = Color.White;
                    surpriseBttn.TextColor = Color.Black;
                    saveBttn.BackgroundColor = Color.White;
                    saveFrame.BackgroundColor = Color.White;
                    saveBttn.TextColor = Color.Black;
                    skipBttn.BackgroundColor = Color.White;
                    skipFrame.BackgroundColor = Color.White;
                    skipBttn.TextColor = Color.Black;

                    ms.MealQuantity--;
                    if (ms.MealQuantity == 0)
                    {
                        ms.Background = Color.White;
                    }
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        protected async Task GetMealPlans()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                Console.WriteLine("ENTER GET MEAL PLANS FUNCTION");

                if ((string)Application.Current.Properties["platform"] == "GUEST")
                {
                    ArrayList namesArray = new ArrayList();
                    PlanName newPlan = new PlanName();
                    newPlan.name = (Preferences.Get("item_name", "").Substring(0, 1) + " Meal Plan");
                    namesColl.Add(newPlan);
                    namesArray.Add(Preferences.Get("item_name", "").Substring(0, 1) + " Meal Plan");
                    SubscriptionPicker.ItemsSource = namesArray;
                    //mealPlansList.ItemsSource = namesArray;
                    dropDownList.HeightRequest = 100;
                    //dropDownList.ItemsSource = namesArray;
                    dropDownList.ItemsSource = namesColl;
                    SubscriptionPicker.SelectedItem = namesArray[0].ToString();
                    //dropDownList.SelectedItem = namesArray[0].ToString();
                    dropDownList.SelectedItem = namesColl[0];
                    //Preferences.Get("item_name", "").Substring(0, 1) + " Meals for " + Preferences.Get("freqSelected", "") + " Deliveries): 
                }
                else
                {
                    var request = new HttpRequestMessage();
                    string userID = (string)Application.Current.Properties["user_id"];
                    Console.WriteLine("Inside GET MEAL PLANS: User ID:  " + userID);

                    request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
                    Console.WriteLine("GET MEALS PLAN ENDPOINT TRYING TO BE REACHED: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
                    request.Method = HttpMethod.Get;
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(request);
                    Debug.WriteLine("get meal plans response: " + response.ToString());
                    //Debug.WriteLine("get meal plans content: " + response.Content.ToString());

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContent content = response.Content;
                        var userString = await content.ReadAsStringAsync();
                        JObject mealPlan_obj = JObject.Parse(userString);
                        this.NewPlan.Clear();

                        ArrayList itemsArray = new ArrayList();
                        // List<Item> itemsArray = new List<Item>;
                        ArrayList namesArray = new ArrayList();

                        Console.WriteLine("itemsArray contents:");

                        foreach (var m in mealPlan_obj["result"])
                        {
                            Console.WriteLine("In first foreach loop of getmeal plans func:");
                            if (m["purchase_status"].ToString() != "CANCELLED and REFUNDED")
                            {
                                itemsArray.Add((m["items"].ToString()));
                                purchIdArray.Add((m["purchase_id"].ToString()));
                                purchUidArray.Add((m["purchase_uid"].ToString()));
                            }
                        }

                        // Console.WriteLine("itemsArray contents:" + itemsArray[0]);

                        for (int i = 0; i < itemsArray.Count; i++)
                        {
                            JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(itemsArray[i].ToString());

                            Console.WriteLine("Inside forloop before foreach in GetmealsPlan func");

                            foreach (JObject config in newobj)
                            {
                                Console.WriteLine("Inside foreach loop in GetmealsPlan func");
                                string qty = (string)config["qty"];
                                string name = (string)config["name"];

                                name = name.Substring(0, name.IndexOf(" "));
                                name = name + " Meals, ";
                                qty = qty + " Deliveries";
                                //string price = (string)config["price"];
                                //string mealid = (string)config["item_uid"];
                                string purchIdCurrent = purchIdArray[i].ToString().Substring(4);
                                string purchUidCurrent = purchUidArray[i].ToString().Substring(4);
                                //while (purchIdCurrent.Substring(0, 1) == "0")
                                //    purchIdCurrent = purchIdCurrent.Substring(1);

                                //only includes meal plan name
                                //namesArray.Add(name);

                                //adds purchase uid to front of meal plan name
                                //namesArray.Add(purchIdArray[i].ToString().Substring(4) + " : " + name);
                                PlanName newPlan = new PlanName();
                                newPlan.name = (name + qty + " : " + purchIdCurrent);
                                namesColl.Add(newPlan);
                                namesArray.Add(name + qty + " : " + purchIdCurrent);
                                //newPlan.name = (name + qty + " : " + purchUidCurrent);
                                //namesColl.Add(newPlan);
                                //namesArray.Add(name + qty + " : " + purchUidCurrent);
                            }
                        }
                        Console.WriteLine("Outside foreach in GetmealsPlan func");
                        //Find unique number of meals
                        //firstIndex = namesArray[0].ToString();
                        //Console.WriteLine("namesArray contents:" + namesArray[0].ToString() + " " + namesArray[1].ToString() + " " + namesArray[2].ToString() + " ");
                        SubscriptionPicker.ItemsSource = namesArray;
                        //mealPlansList.ItemsSource = namesArray;
                        //dropDownList.ItemsSource = namesArray;
                        dropDownList.ItemsSource = namesColl;
                        
                        if (namesColl.Count < 4)
                            dropDownList.HeightRequest = 100;

                        SubscriptionPicker.SelectedItem = namesArray[0].ToString();
                        //dropDownList.SelectedItem = namesArray[0].ToString();
                        dropDownList.SelectedItem = namesColl[0];
                        Console.WriteLine("namesArray contents:" + namesArray[0].ToString());
                        //SubscriptionPicker.Title = namesArray[0];

                        Console.WriteLine("END OF GET MEAL PLANS FUNCTION");
                    }
                }

                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private void getUserMeals()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                if ((string)Application.Current.Properties["platform"] != "GUEST")
                {
                    try
                    {
                        MealInformation jsonobj;
                        // UID = 100-000001 PID = 400-000001
                        var content = client.DownloadString(userMeals);
                        var obj = JsonConvert.DeserializeObject<MealsSelected>(content);
                        Debug.WriteLine("for this url: " + userMeals + "\n we are getting this: \n" + obj.ToString());

                        for (int i = 0; i < obj.Result.Length; i++)
                        {
                            // If meals selected matches menu date, get meals selected
                            //Debug.WriteLine("purchId: " + Preferences.Get("purchId", ""));
                            //Debug.WriteLine("Selection purchase id: " + obj.Result[i].SelPurchaseId.ToString());
                            //Debug.WriteLine("purchase uid: " + obj.Result[i].PurchaseUid.ToString());
                            //Debug.WriteLine("purchase id: " + obj.Result[i].PurchaseId.ToString());
                            //Debug.WriteLine("purchase id: " + obj.Result[i].PurchaseId.ToString());
                            //commented for id vs uid
                            //used to be SelPurchaseId
                            Debug.WriteLine("obj.Result[i].MenuDate = " + obj.Result[i].MenuDate);
                            Debug.WriteLine("selectedDate.fullDateTime = " + selectedDate.fullDateTime);
                            Debug.WriteLine("Preferences.Get(purchId, ) = " + Preferences.Get("purchId", ""));
                            Debug.WriteLine("obj.Result[i].PurchaseId = " + obj.Result[i].PurchaseId);
                            if (obj.Result[i].MenuDate.Equals(selectedDate.fullDateTime) && Preferences.Get("purchId", "") == obj.Result[i].PurchaseId && obj.Result[i].PurchaseStatus.Equals("ACTIVE"))
                            //if (obj.Result[i].SelMenuDate.Equals(selectedDate.fullDateTime) && Preferences.Get("purchUid", "") == obj.Result[i].PurchaseUid)
                            {
                                string json = obj.Result[i].MealSelection;
                                JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json);

                                foreach (JObject config in newobj)
                                {
                                    string qty = (string)config["qty"];
                                    string name = (string)config["name"];
                                    string mealid = (string)config["item_uid"];


                                    if (qty != null)
                                    {
                                        if (qtyDict.ContainsKey(name)) //mealid
                                        {
                                            qtyDict.Remove(name);
                                        }
                                        Debug.WriteLine("meal tracked: " + name + " amount: " + qty);
                                        qtyDict.Add(name, qty);
                                    }

                                }

                                string json2 = obj.Result[i].AddonSelection;
                                JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(json2);

                                foreach (JObject config in newobj2)
                                {
                                    string qty = (string)config["qty"];
                                    string name = (string)config["name"];
                                    string mealid = (string)config["item_uid"];


                                    if (qty != null)
                                    {
                                        //original code working with one quantity dictionary to save previous selection
                                        //if (qtyDict.ContainsKey(name)) //mealid
                                        //{
                                        //    qtyDict.Remove(name);
                                        //}
                                        //Debug.WriteLine("add-on tracked: " + name + " amount: " + qty);
                                        //qtyDict.Add(name, qty);
                                        if (qtyDict_addon.ContainsKey(name))
                                        {
                                            qtyDict_addon.Remove(name);
                                        }
                                        Debug.WriteLine("add-on tracked: " + name + " amount: " + qty);
                                        qtyDict_addon.Add(name, qty);

                                    }

                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("exception from getting user meals: " + ex.ToString());
                        Console.WriteLine("GET USER MEALS ERROR CATCHED");
                    }
                }

                //Thread.Sleep(3000);
                
                Debug.WriteLine("getUserMeals ended");
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }


        private async void saveUserMeals(object sender, EventArgs e)
        {
            try
            {
                confirmChangeDate = true;
                //set delivery day of the week
                string tempHolder = selectedDate.fullDateTime;
                Debug.WriteLine("year:" + tempHolder.Substring(0, 4));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("month:" + text1.Substring(5, 2));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("day:" + text1.Substring(8, 2));
                //getDayOfTheWeek();
                DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                selectedDotw = selected.ToString("dddd");
                Debug.WriteLine("dayOfWeek: " + selectedDotw);

                surpriseBttn.BackgroundColor = Color.White;
                surpriseFrame.BackgroundColor = Color.White;
                surpriseBttn.TextColor = Color.Black;
                skipBttn.BackgroundColor = Color.White;
                skipFrame.BackgroundColor = Color.White;
                skipBttn.TextColor = Color.Black;
                //saveFrame.BackgroundColor = Color.Orange;
                //saveBttn.BackgroundColor = Color.Orange;
                //saveBttn.TextColor = Color.White;

                int count = Preferences.Get("total", 0);
                if (totalCount.Text == "0" || count == 0)
                {
                    saveFrame.BackgroundColor = Color.FromHex("#F8BB17");
                    saveBttn.BackgroundColor = Color.FromHex("#F8BB17");
                    saveBttn.TextColor = Color.White;
                    selectedDate.fillColor = Color.FromHex("#F8BB17");
                    selectedDate.status = "Selected";

                    for (int i = 0; i < Meals1.Count; i++)
                    {
                        if (Meals1[i].MealQuantity > 0)
                        {
                            mealsSaved.Add(new MealInformation
                            {
                                Qty = Meals1[i].MealQuantity.ToString(),
                                Name = Meals1[i].MealName,
                                Price = Meals1[i].MealPrice.ToString(),
                                ItemUid = Meals1[i].ItemUid,
                            }
                            );
                        }
                    }

                    jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                    Console.WriteLine("line 302 " + jsonMeals);
                    postData();
                    mealsSaved = new List<MealInformation>();

                    for (int i = 0; i < Meals2.Count; i++)
                    {
                        if (Meals2[i].MealQuantity > 0)
                        {
                            //if (Meals2[i].ItemUid == null)
                            //    Meals2[i].ItemUid = "";

                            mealsSaved.Add(new MealInformation
                            {
                                Qty = Meals2[i].MealQuantity.ToString(),
                                Name = Meals2[i].MealName,
                                Price = Meals2[i].MealPrice.ToString(),
                                ItemUid = Meals2[i].ItemUid,
                            }
                            );
                            addOnSelected = true;
                        }
                    }

                    //send second JSON object for add-ons only
                    if (addOnSelected == true)
                    {
                        jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                        Console.WriteLine("line 302 " + jsonMeals);
                        postData();
                        addOnSelected = false;
                        BarParameters[0].mealsLeft = "All Meals Selected";
                        mealsLeftLabel.Text = "All Meals Selected";
                        BarParameters[0].barLabel = "All Meals Selected";
                        popButton1.Text = "Okay";
                        popButton2.IsVisible = false;
                        string addOnMsg = "You will be charged for your add-ons on " + addOnChargeDate.ToString("M") + ".";

                        try
                        {
                            WebClient client4 = new WebClient();
                            var content3 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);
                            string msg = obj.result[53].message;
                            msg = msg.Replace("#", addOnChargeDate.ToString("M"));

                            showPopUp(obj.result[53].title, msg);
                        }
                        catch
                        {
                            showPopUp("Selection Saved", addOnMsg);
                        }

                         //54
                        //DisplayAlert("Selection Saved", "You will be charged for your add-ons on 1/1/2021.", "OK");
                    }
                    else
                    {
                        //clear add-ons on backend
                        addOnSelected = true;
                        mealsSaved = new List<MealInformation>();
                        //mealsSaved = null;
                        jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                        Console.WriteLine("line 302 " + jsonMeals);
                        postData();
                        BarParameters[0].mealsLeft = "All Meals Selected";
                        mealsLeftLabel.Text = "All Meals Selected";
                        BarParameters[0].barLabel = "All Meals Selected";
                        popButton1.Text = "Okay";
                        popButton2.IsVisible = false; //55

                        try
                        {
                            WebClient client4 = new WebClient();
                            var content3 = client4.DownloadString(Constant.AlertUrl);
                            var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                            showPopUp(obj.result[54].title, obj.result[54].message);
                        }
                        catch
                        {
                            showPopUp("Selection Saved", "You've successfully saved your meal selection.");
                        }

                        
                        //DisplayAlert("Selection Saved", "You've successfully saved your meal selection.", "OK");
                    }
                    addOnSelected = false;
                    //DisplayAlert("Selection Saved", "You've successfully saved your meal selection.", "OK");
                    //saveBttn.BackgroundColor = Color.White;
                    //saveBttn.TextColor = Color.Black;
                }
                else
                {
                    popButton1.Text = "Okay";
                    popButton2.IsVisible = false; //56

                    try
                    {
                        WebClient client4 = new WebClient();
                        var content3 = client4.DownloadString(Constant.AlertUrl);
                        var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                        showPopUp(obj.result[55].title, obj.result[55].message);
                    }
                    catch
                    {
                        showPopUp("Incomplete Meal Selection", "Please select additional meals.");
                    }

                    
                    //DisplayAlert("Incomplete Meal Selection", "Please select additional meals.", "OK");

                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private async void skipMealSelection(object sender, EventArgs e)
        {
            try
            {
                confirmChangeDate = true;

                foreach (var ms in Meals1)
                    ms.Background = Color.White;

                foreach (var ms in Meals2)
                    ms.Background = Color.White;

                selectedDotw = "SKIP";
                //addOnSelected = false;
                //qtyDict.Clear();
                skipBttn.BackgroundColor = Color.FromHex("#F8BB17");
                skipFrame.BackgroundColor = Color.FromHex("#F8BB17");
                skipBttn.TextColor = Color.White;
                surpriseBttn.BackgroundColor = Color.White;
                surpriseFrame.BackgroundColor = Color.White;
                surpriseBttn.TextColor = Color.Black;
                saveBttn.BackgroundColor = Color.White;
                saveFrame.BackgroundColor = Color.White;
                saveBttn.TextColor = Color.Black;

                for (int i = 0; i < 6; i++)
                    plates[i].Source = "";

                resetAll();
                mealsSaved.Clear();
                int count = Preferences.Get("total", 0);
                totalCount.Text = "SKIPPED";
                //for (int i = 0; i < Meals1.Count; i++)
                //{
                //if (Meals1[i].MealQuantity > 0)
                //{
                mealsSaved.Add(new MealInformation
                {
                    Qty = dropDownText.Text.Substring(0, dropDownText.Text.IndexOf(" ")),
                    Name = "SKIP",
                    Price = "",
                    ItemUid = "",
                }
                );
                // }
                //}

                jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                Console.WriteLine("line 302 " + jsonMeals);
                postData();

                //testing sending a null for add-ons when skipping
                addOnSelected = true;
                mealsSaved = new List<MealInformation>();
                //mealsSaved = null;
                jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                Console.WriteLine("line 302 " + jsonMeals);
                postData();
                addOnSelected = false;
                //mealsSaved = new List<MealInformation>();
                selectedDate.fillColor = Color.FromHex("#BBBBBB");
                selectedDate.status = "Skipped";
                //DisplayAlert("Delivery Skipped", "You won't receive any meals for this delivery cycle. We'll extend your subscription accordingly.", "OK");
                popButton1.Text = "Okay";
                popButton2.IsVisible = false; //57

                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    showPopUp(obj.result[56].title, obj.result[56].message);
                }
                catch
                {
                    showPopUp("Delivery Skipped", "You won't receive any meals this day. We will extend your subscription accordingly.");
                }

                
                mealsSaved.Clear();
                //int indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                int indexOfMealPlanSelected = -1;
                //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                foreach (var plan in namesColl)
                {
                    indexOfMealPlanSelected++;
                    if (plan.name == dropDownText.Text)
                    {
                        break;
                    }
                }
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                //string s = SubscriptionPicker.SelectedItem.ToString();
                string s = dropDownText.Text;
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));

                BarParameters[0].margin = 0;
                BarParameters[0].update = 0;
                BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private void surprise()
        {
            try
            {
                confirmChangeDate = true;

                Debug.WriteLine("surprise() entered");

                //set delivery day of the week
                string tempHolder = selectedDate.fullDateTime;
                Debug.WriteLine("year:" + tempHolder.Substring(0, 4));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("month:" + text1.Substring(5, 2));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("day:" + text1.Substring(8, 2));
                //getDayOfTheWeek();
                DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                selectedDotw = selected.ToString("dddd");
                Debug.WriteLine("dayOfWeek: " + selectedDotw);

                selectedDate.fillColor = Color.White;
                selectedDate.status = "Surprise / No Selection";
                //weekOneProgress.Progress = 0;
                surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                surpriseBttn.TextColor = Color.White;
                skipBttn.BackgroundColor = Color.White;
                skipFrame.BackgroundColor = Color.White;
                skipBttn.TextColor = Color.Black;
                saveBttn.BackgroundColor = Color.White;
                saveFrame.BackgroundColor = Color.White;
                saveBttn.TextColor = Color.Black;
                resetAll();
                mealsSaved.Clear();
                int count = Preferences.Get("total", 0);
                totalCount.Text = "SURPRISE";
                //for (int i = 0; i < Meals1.Count; i++)
                //{
                //if (Meals1[i].MealQuantity > 0)
                //{
                mealsSaved.Add(new MealInformation
                {
                    Qty = dropDownText.Text.Substring(0, dropDownText.Text.IndexOf(" ")),
                    Name = "SURPRISE",
                    Price = "",
                    ItemUid = "",
                }
                );
                // }
                //}

                jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                Console.WriteLine("line 302 " + jsonMeals);
                //postData();
                //DisplayAlert("SUPRISE", "You will be surprised with a randomized meal selection. If you want to select meals again for this meal plan then click the RESET button!", "OK");
                mealsSaved.Clear();
                //int indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                int indexOfMealPlanSelected = -1;
                //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                foreach (var plan in namesColl)
                {
                    indexOfMealPlanSelected++;
                    if (plan.name == dropDownText.Text)
                    {
                        break;
                    }
                }
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                //string s = SubscriptionPicker.SelectedItem.ToString();
                string s = dropDownText.Text;
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));

                BarParameters[0].margin = 0;
                BarParameters[0].update = 0;
                BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }
        private async void surpriseMealSelection(object sender, EventArgs e)
        {
            try
            {
                confirmChangeDate = true;

                Debug.WriteLine("surpriseMealSelection entered");

                foreach (var ms in Meals1)
                    ms.Background = Color.White;

                foreach (var ms in Meals2)
                    ms.Background = Color.White;


                //set delivery day of the week
                string tempHolder = selectedDate.fullDateTime;
                Debug.WriteLine("year:" + tempHolder.Substring(0, 4));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("month:" + text1.Substring(5, 2));
                //tempHolder = tempHolder.Substring(tempHolder.IndexOf("-") + 1);
                Debug.WriteLine("day:" + text1.Substring(8, 2));
                //getDayOfTheWeek();
                DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
                selectedDotw = selected.ToString("dddd");
                Debug.WriteLine("dayOfWeek: " + selectedDotw);

                //addOnSelected = false;
                //qtyDict.Clear();
                surpriseBttn.BackgroundColor = Color.FromHex("#F8BB17");
                surpriseFrame.BackgroundColor = Color.FromHex("#F8BB17");
                surpriseBttn.TextColor = Color.White;
                skipBttn.BackgroundColor = Color.White;
                skipFrame.BackgroundColor = Color.White;
                skipBttn.TextColor = Color.Black;
                saveBttn.BackgroundColor = Color.White;
                saveFrame.BackgroundColor = Color.White;
                saveBttn.TextColor = Color.Black;

                for (int i = 0; i < 6; i++)
                    plates[i].Source = "";

                resetAll();
                mealsSaved.Clear();
                int count = Preferences.Get("total", 0);
                totalCount.Text = "SURPRISE";
                selectedDate.fillColor = Color.White;
                selectedDate.status = "Surprise / No Selection";
                //for (int i = 0; i < Meals1.Count; i++)
                //{
                //if (Meals1[i].MealQuantity > 0)
                //{
                mealsSaved.Add(new MealInformation
                {
                    Qty = dropDownText.Text.Substring(0, dropDownText.Text.IndexOf(" ")),
                    Name = "SURPRISE",
                    Price = "",
                    ItemUid = "",
                }
                );
                // }
                //}

                jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                Console.WriteLine("line 302 " + jsonMeals);
                postData();

                //testing sending a null for add-ons when skipping
                addOnSelected = true;
                mealsSaved = new List<MealInformation>();
                //mealsSaved = null;
                jsonMeals = JsonConvert.SerializeObject(mealsSaved);
                Console.WriteLine("line 302 " + jsonMeals);
                postData();
                addOnSelected = false;

                //DisplayAlert("SURPRISE", "We'll select a random assortment of nutritious, healthy meals for you!", "OK");
                popButton1.Text = "Okay";
                popButton2.IsVisible = false; //58

                try
                {
                    WebClient client4 = new WebClient();
                    var content3 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content3);

                    showPopUp(obj.result[57].title, obj.result[57].message);
                }
                catch
                {
                    showPopUp("Surprise!", "We’ll surprise you with some of our specials on this day!");
                }

                
                mealsSaved.Clear();
                //int indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                int indexOfMealPlanSelected = -1;
                //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
                foreach (var plan in namesColl)
                {
                    indexOfMealPlanSelected++;
                    if (plan.name == dropDownText.Text)
                    {
                        break;
                    }
                }
                Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
                Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
                Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

                //string s = SubscriptionPicker.SelectedItem.ToString();
                string s = dropDownText.Text;
                s = s.Substring(0, 2);
                Preferences.Set("total", int.Parse(s));
                totalCount.Text = Preferences.Get("total", 0).ToString();
                Preferences.Set("origMax", int.Parse(s));

                BarParameters[0].margin = 0;
                BarParameters[0].update = 0;
                BarParameters[0].mealsLeft = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
                mealsLeftLabel.Text = "Select " + Preferences.Get("total", "").ToString() + " meals";
                BarParameters[0].barLabel = "Please Select " + Preferences.Get("total", "").ToString() + " Meals";
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        public async void postData()
        {
            try
            {
                HttpClient hclient = new HttpClient();

                var mealSelectInfoTosend = new PostMeals
                {
                    IsAddon = addOnSelected,
                    // Need to create json formatting for this
                    Items = mealsSaved,
                    //commented out for id vs uid
                    PurchaseId = Preferences.Get("purchId", ""),
                    //recommented out 5/16/21
                    //PurchaseId = Preferences.Get("purchUid", ""),
                    MenuDate = selectedDate.fullDateTime,
                    DeliveryDay = selectedDotw,
                };

                string mealSelectInfoJson = JsonConvert.SerializeObject(mealSelectInfoTosend);
                Console.WriteLine("line 322 " + mealSelectInfoJson);

                try
                {
                    var httpContent = new StringContent(mealSelectInfoJson, Encoding.UTF8, "application/json");
                    var response = await hclient.PostAsync(postUrl, httpContent);
                    if (response.Content != null)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine(responseContent);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }   // Clicked Save function
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        /*private bool isAlreadySelected()
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    return true;
                }

            }
            return false;
        }*/

        private void resetAll()
        {
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    //totalMealsCount += Meals1[i].MealQuantity;
                    Meals1[i].MealQuantity = 0;
                    Meals1[i].Background = Color.White;
                    /*
                    mealsSaved.Add(new MealInformation
                    {
                        Qty = Meals1[i].MealQuantity.ToString(),
                        Name = Meals1[i].MealName,
                        Price = Meals1[i].MealPrice.ToString(),
                        ItemUid = Meals1[i].ItemUid,
                    }
                    );*/
                }

            }

            for (int i = 0; i < Meals2.Count; i++)
            {
                if (Meals2[i].MealQuantity > 0)
                {
                    Meals2[i].MealQuantity = 0;
                    Meals2[i].Background = Color.White;
                    Debug.WriteLine("addon set to 0: " + Meals2[i].MealName);
                }
            }
        }

        private void calcTotal()
        {
            totalMealsCount = 0;
            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    totalMealsCount += Meals1[i].MealQuantity;
                }

            }
        }

        //highlight each date either light gray, orange, or white
        protected async Task checkDateStatuses()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                foreach (Date d in availableDates)
                {
                    Debug.WriteLine("INSIDE checkDateStatuses #1");
                    Debug.WriteLine("availableDate passed in: " + d.fullDateTime);
                    var request = new HttpRequestMessage();
                    //this needs to be purchase ID not uid
                    string purchaseID = Preferences.Get("purchId", "");
                    //recommented out 5/16/21
                    //string purchaseID = Preferences.Get("purchUid", "");
                    string date = d.fullDateTime.Substring(0, 10);
                    if (date == Preferences.Get("dateSelected", ""))
                        continue;
                    string userID = (string)Application.Current.Properties["user_id"];
                    string halfUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=" + userID;
                    string urlSent = halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date;
                    Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
                    request.RequestUri = new Uri(halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date);
                    request.Method = HttpMethod.Get;
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(request);

                    Console.WriteLine("Trying to enter if statement in Get Recent Selection");
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        bool isAlreadySelected2 = false;
                        bool isSkip2 = false;
                        bool isSurprise2 = false;

                        HttpContent content = response.Content;
                        Debug.WriteLine("for this url: " + urlSent + "\n we are getting this: \n" + content);
                        var userString = await content.ReadAsStringAsync();
                        JObject recentMeals = JObject.Parse(userString);
                        this.NewPlan.Clear();

                        ArrayList qtyList = new ArrayList();
                        //ArrayList nameList = new ArrayList();
                        //ArrayList itemUidList = new ArrayList();
                        ArrayList namesArray = new ArrayList();
                        ArrayList combinedArray = new ArrayList();
                        ArrayList addOnArray = new ArrayList();
                        ArrayList addOnQtyList = new ArrayList();
                        ArrayList addOnNamesArray = new ArrayList();

                        Console.WriteLine("Trying to enter foreach loop in Get Recent Meals");

                        foreach (var m in recentMeals["result"])
                        {
                            //Debug.WriteLine("meal_selection[i].ToString(): " + m["meals_selected"].ToString());
                            //if (m["meals_selected"].ToString().IndexOf("],") != -1)
                            //    Debug.WriteLine("combinedArray[i] take out the extra: " + m["meals_selected"].ToString().Substring(0, m["meals_selected"].ToString().IndexOf("],")));

                            //JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(m["meals_selected"].ToString().Substring(0, m["meals_selected"].ToString().IndexOf("],")));

                            //foreach (JObject config2 in newobj2)
                            //{
                            //    Debug.WriteLine("broken up successfully");
                            //    string qty = (string)config2["qty"];
                            //    string name = (string)config2["name"];
                            //    //string price = (string)config["price"];
                            //    //string mealid = (string)config["item_uid"];
                            //    namesArray.Add(name);
                            //    qtyList.Add(qty);
                            //    Debug.WriteLine("meal updating list name: " + name + " amount: " + qty);
                            //}
                            //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                            //qtyList.Add(double.Parse(m["qty"].ToString()));
                            //nameList.Add(int.Parse(m["name"].ToString()));
                            //changed 5/17
                            //combinedArray.Add((m["meal_selection"].ToString()));
                            combinedArray.Add((m["meals_selected"].ToString()));
                        }

                        foreach (var m in recentMeals["result"])
                        {
                            addOnArray.Add((m["addon_selection"].ToString()));
                        }

                        if (combinedArray.Count == 0)
                        {
                            //Preferences.Set("isAlreadySelected", false);
                            isAlreadySelected2 = false;
                        }
                        else
                        {
                            //Preferences.Set("isAlreadySelected", true);
                            isAlreadySelected2 = true;
                        }
                        //isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                        Console.WriteLine("isAlreadySelected" + isAlreadySelected2);

                        Console.WriteLine("Trying to enter for loop in Get Recent Selection");
                        for (int i = 0; i < combinedArray.Count; i++)
                        {
                            

                            JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(combinedArray[i].ToString());

                            foreach (JObject config in newobj)
                            {
                                string qty = (string)config["qty"];
                                string name = (string)config["name"];
                                //string price = (string)config["price"];
                                //string mealid = (string)config["item_uid"];
                                namesArray.Add(name);
                                qtyList.Add(qty);
                                Debug.WriteLine("meal updating list name: " + name + " amount: " + qty);
                            }
                        }


                        for (int i = 0; i < namesArray.Count; i++)
                        {
                            if (namesArray[i].ToString() == "SURPRISE")
                            {
                                isSurprise2 = true;
                                break;
                            }
                            else if (namesArray[i].ToString() == "SKIP")
                            {
                                isSkip2 = true;
                                break;
                            }
                            else
                            {
                                isSkip2 = false;
                                isSurprise2 = false;
                            }
                        }


                        if (isSkip2 == true)
                        {
                            d.fillColor = Color.FromHex("#BBBBBB");
                            d.status = "Skipped";
                        }
                        else if (isAlreadySelected2 == true && isSurprise2 == false)
                        {
                            d.fillColor = Color.FromHex("#F8BB17");
                            d.status = "Selected";
                        }
                        else
                        {
                            d.fillColor = Color.White;
                            d.status = "Surprise / No Selection";
                        }
                        Console.WriteLine("isSurprise value: " + isSurprise2 + " isSkip value: " + isSkip2);


                    }
                }
                Debug.WriteLine("checkDateStatuses ended");

                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        protected async Task GetRecentSelection()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }

                Console.WriteLine("INSIDE GetRecentSelection #1");
                var request = new HttpRequestMessage();
                string purchaseID = Preferences.Get("purchId", "");
                //recommented out 5/16/21
                //string purchaseID = Preferences.Get("purchUid", "");
                string date = Preferences.Get("dateSelected", "");
                string userID = (string)Application.Current.Properties["user_id"];
                string halfUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=" + userID;
                string urlSent = halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date;
                Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
                request.RequestUri = new Uri(halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date);
                request.Method = HttpMethod.Get;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine("Trying to enter if statement in Get Recent Selection");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var userString = await content.ReadAsStringAsync();
                    JObject recentMeals = JObject.Parse(userString);
                    this.NewPlan.Clear();

                    ArrayList qtyList = new ArrayList();
                    //ArrayList nameList = new ArrayList();
                    //ArrayList itemUidList = new ArrayList();
                    ArrayList namesArray = new ArrayList();
                    ArrayList combinedArray = new ArrayList();
                    ArrayList addOnArray = new ArrayList();
                    ArrayList addOnQtyList = new ArrayList();
                    ArrayList addOnNamesArray = new ArrayList();

                    Console.WriteLine("Trying to enter foreach loop in Get Recent Meals");

                    foreach (var m in recentMeals["result"])
                    {
                        //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                        //qtyList.Add(double.Parse(m["qty"].ToString()));
                        //nameList.Add(int.Parse(m["name"].ToString()));
                        //changed 5/17
                        //combinedArray.Add((m["meal_selection"].ToString()));
                        combinedArray.Add((m["meals_selected"].ToString()));
                    }

                    foreach (var m in recentMeals["result"])
                    {
                        addOnArray.Add((m["addon_selection"].ToString()));
                    }

                    if (combinedArray.Count == 0)
                    {
                        //Preferences.Set("isAlreadySelected", false);
                        isAlreadySelected = false;
                    }
                    else
                    {
                        //Preferences.Set("isAlreadySelected", true);
                        isAlreadySelected = true;
                    }
                    //isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                    Console.WriteLine("isAlreadySelected" + isAlreadySelected);

                    Console.WriteLine("Trying to enter for loop in Get Recent Selection");
                    try
                    {
                        for (int i = 0; i < combinedArray.Count; i++)
                        {

                            JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(combinedArray[i].ToString());

                            foreach (JObject config in newobj)
                            {
                                string qty = (string)config["qty"];
                                string name = (string)config["name"];
                                //string price = (string)config["price"];
                                //string mealid = (string)config["item_uid"];
                                namesArray.Add(name);
                                qtyList.Add(qty);
                                Debug.WriteLine("meal updating list name: " + name + " amount: " + qty);
                            }
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("exception caught");
                    }

                    Debug.WriteLine("exception continued");
                    ////source of the crash
                    //for (int i = 0; i < addOnArray.Count; i++)
                    //{

                    //    JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(addOnArray[i].ToString());

                    //    foreach (JObject config in newobj2)
                    //    {
                    //        string qty = (string)config["qty"];
                    //        string name = (string)config["name"];
                    //        //string price = (string)config["price"];
                    //        //string mealid = (string)config["item_uid"];
                    //        addOnNamesArray.Add(name);
                    //        addOnQtyList.Add(qty);
                    //        Debug.WriteLine("add-on updating list name: " + name + " amount: " + qty);
                    //    }
                    //}
                    ////end source


                    for (int i = 0; i < namesArray.Count; i++)
                    {
                        if (namesArray[i].ToString() == "SURPRISE")
                        {
                            isSurprise = true;
                            break;
                        }
                        else if (namesArray[i].ToString() == "SKIP")
                        {
                            isSkip = true;
                            break;
                        }
                        else
                        {
                            isSkip = false;
                            isSurprise = false;
                        }
                    }



                    if (isSkip == true)
                    {
                        selectedDate.fillColor = Color.FromHex("#BBBBBB");
                        selectedDate.status = "Skipped";
                    }
                    else if (isAlreadySelected == true && isSurprise == false)
                    {
                        selectedDate.fillColor = Color.FromHex("#F8BB17");
                        selectedDate.status = "Selected";
                    }
                    else
                    {
                        selectedDate.fillColor = Color.White;
                        selectedDate.status = "Surprise / No Selection";
                    }

                    //Thread.Sleep(3000);
                    await Task.Delay(1000);
                    loadingCount = loadingCount - 1;
                    Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                    if (loadingCount == 0)
                    {
                        //Thread.Sleep(3000);
                        //Task.Delay(30000).Wait();
                        //loadBackground.IsVisible = false;
                        loadingAnim.IsVisible = false;
                        belowMenu.IsVisible = true;
                    }

                    Console.WriteLine("isSurprise value: " + isSurprise + " isSkip value: " + isSkip);
                    return;
                    Console.WriteLine("Trying to enter second for loop in Get Recent Selection");
                    totalMealsCount = 0;

                    //source of the crash moved 
                    for (int i = 0; i < addOnArray.Count; i++)
                    {

                        JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(addOnArray[i].ToString());

                        foreach (JObject config in newobj2)
                        {
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];
                            //string price = (string)config["price"];
                            //string mealid = (string)config["item_uid"];
                            addOnNamesArray.Add(name);
                            addOnQtyList.Add(qty);
                            Debug.WriteLine("add-on updating list name: " + name + " amount: " + qty);
                        }
                    }
                    //end source

                    //resetAll();
                    //for (int i = 0; i < Meals1.Count; i++)
                    //{
                    //    Console.WriteLine("Inside second for loop in Get Recent Selection");
                    //    Meals1[i].MealQuantity = Int32.Parse(qtyList[i].ToString());
                    //    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                    //}

                    //for (int i = 0; i < Meals2.Count; i++)
                    //{
                    //    Console.WriteLine("Inside second for loop in Get Recent Selection");

                    //    Meals2[i].MealQuantity = Int32.Parse(addOnQtyList[i].ToString());
                    //    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                    //}

                    for (int i = 0; i < namesArray.Count; i++)
                    {
                        for (int j = 0; j < Meals1.Count; j++)
                        {
                            if (Meals1[j].MealName == (string)namesArray[i])
                            {
                                //Meals1[j].MealQuantity = Int32.Parse(qtyList[i].ToString());
                                //qtyDict.Add((string)namesArray[i], (string)qtyList[i]);
                                Debug.WriteLine("meal name: " + (string)namesArray[i] + " amount: " + (string)qtyList[i]);
                            }
                        }
                    }

                    //for (int i = 0; i < addOnNamesArray.Count; i++)
                    //{
                    //    for (int j = 0; j < Meals2.Count; j++)
                    //    {
                    //        if (Meals2[j].MealName == (string)addOnNamesArray[i])
                    //        {
                    //            //Meals2[j].MealQuantity = Int32.Parse(addOnQtyList[i].ToString());
                    //            //qtyDict.Add((string)addOnNamesArray[i], (string)addOnQtyList[i]);
                    //            Debug.WriteLine("add-on name: " + (string)addOnNamesArray[i] + " amount: " + (string)addOnQtyList[i]);
                    //        }
                    //    }
                    //}

                    Console.WriteLine("Before set menu call in Get Recent Seleciton");

                    for (int i = 0; i < qtyList.Count; i++)
                    {
                        Console.WriteLine("Inside third for loop in Get Recent Selection");
                        totalMealsCount += Int32.Parse(qtyList[i].ToString());
                    }
                    setMenu();
                    Console.WriteLine("END OF GET RECENT SELECTION");

                }

                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        protected async Task GetRecentSelection2()
        {
            try
            {
                loadingCount++;
                if (loadingAnim.IsVisible != true)
                {
                    loadingAnim.Position = TimeSpan.Zero;
                    Debug.WriteLine("check buffer prog: " + loadingAnim.BufferingProgress.ToString());
                    loadingAnim.Play();
                    //loadBackground.IsVisible = true;
                    belowMenu.IsVisible = false;
                    loadingAnim.IsVisible = true;
                }
                

                Console.WriteLine("INSIDE GetRecentSelection #2");
                var request = new HttpRequestMessage();
                //commented out for id vs uid

                string purchaseID = Preferences.Get("purchId", "");
                //recommented out 5/16/21
                //string purchaseID = Preferences.Get("purchUid", "");
                string date = Preferences.Get("dateSelected", "");
                string userID = (string)Application.Current.Properties["user_id"];
                string halfUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected_specific?customer_uid=" + userID;
                string urlSent = halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date;
                Console.WriteLine("URL ENDPOINT TRYING TO BE REACHED:" + urlSent);
                request.RequestUri = new Uri(halfUrl + "&purchase_id=" + purchaseID + "&menu_date=" + date);
                request.Method = HttpMethod.Get;
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                Console.WriteLine("Trying to enter if statement in Get Recent Selection");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content = response.Content;
                    var userString = await content.ReadAsStringAsync();
                    JObject recentMeals = JObject.Parse(userString);
                    this.NewPlan.Clear();

                    ArrayList qtyList = new ArrayList();
                    //ArrayList nameList = new ArrayList();
                    //ArrayList itemUidList = new ArrayList();
                    ArrayList namesArray = new ArrayList();
                    ArrayList combinedArray = new ArrayList();
                    ArrayList addOnArray = new ArrayList();
                    ArrayList addOnNamesArray = new ArrayList();
                    ArrayList addOnQtyList = new ArrayList();

                    Console.WriteLine("Trying to enter foreach loop in Get Recent Meals");

                    foreach (var m in recentMeals["result"])
                    {
                        //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                        //qtyList.Add(double.Parse(m["qty"].ToString()));
                        //nameList.Add(int.Parse(m["name"].ToString()));
                        combinedArray.Add((m["combined_selection"].ToString()));
                    }

                    foreach (var m in recentMeals["result"])
                    {
                        //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                        //qtyList.Add(double.Parse(m["qty"].ToString()));
                        //nameList.Add(int.Parse(m["name"].ToString()));
                        addOnArray.Add((m["addon_selection"].ToString()));
                    }

                    if (combinedArray.Count == 0)
                    {
                        //Preferences.Set("isAlreadySelected", false);
                        isAlreadySelected = false;
                    }
                    else
                    {
                        //Preferences.Set("isAlreadySelected", true);
                        isAlreadySelected = true;
                    }
                    //isAlreadySelected = Preferences.Get("isAlreadySelected", true);
                    Console.WriteLine("isAlreadySelected" + isAlreadySelected);

                    Console.WriteLine("Trying to enter for loop in Get Recent Selection");
                    for (int i = 0; i < combinedArray.Count; i++)
                    {

                        JArray newobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(combinedArray[i].ToString());

                        foreach (JObject config in newobj)
                        {
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];
                            //string price = (string)config["price"];
                            //string mealid = (string)config["item_uid"];

                            namesArray.Add(name);
                            qtyList.Add(qty);
                        }
                    }

                    for (int i = 0; i < addOnArray.Count; i++)
                    {

                        JArray newobj2 = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(addOnArray[i].ToString());

                        foreach (JObject config in newobj2)
                        {
                            string qty = (string)config["qty"];
                            string name = (string)config["name"];
                            //string price = (string)config["price"];
                            //string mealid = (string)config["item_uid"];

                            addOnNamesArray.Add(name);
                            addOnQtyList.Add(qty);
                        }
                    }

                    Console.WriteLine("Trying to enter second for loop in Get Recent Selection");
                    totalMealsCount = 0;
                    //resetAll();
                    //for (int i = 0; i < Meals1.Count; i++)
                    //{
                    //    Console.WriteLine("Inside second for loop in Get Recent Selection");

                    //    Meals1[i].MealQuantity = Int32.Parse(qtyList[i].ToString());
                    //    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                    //}

                    //for (int i = 0; i < Meals2.Count; i++)
                    //{
                    //    Console.WriteLine("Inside second for loop in Get Recent Selection");

                    //    Meals2[i].MealQuantity = Int32.Parse(addOnQtyList[i].ToString());
                    //    //totalMealsCount += Int32.Parse(qtyList[i].ToString());

                    //}

                    for (int i = 0; i < namesArray.Count; i++)
                    {
                        for (int j = 0; j < Meals1.Count; j++)
                        {
                            if (Meals1[j].MealName == (string)namesArray[i])
                            {
                                //Meals1[j].MealQuantity = Int32.Parse(qtyList[i].ToString());
                                //qtyDict.Add((string)namesArray[i], (string)qtyList[i]);
                                Debug.WriteLine("meal name: " + (string)namesArray[i] + " amount: " + (string)qtyList[i]);
                            }
                        }
                    }

                    for (int i = 0; i < addOnNamesArray.Count; i++)
                    {
                        for (int j = 0; j < Meals2.Count; j++)
                        {
                            if (Meals2[j].MealName == (string)addOnNamesArray[i])
                            {
                                //Meals2[j].MealQuantity = Int32.Parse(addOnQtyList[i].ToString());
                                //qtyDict.Add((string)addOnNamesArray[i], (string)addOnQtyList[i]);
                                Debug.WriteLine("add-on name: " + (string)addOnNamesArray[i] + " amount: " + (string)addOnQtyList[i]);
                            }
                        }
                    }

                    Console.WriteLine("Before set menu call in Get Recent Seleciton 2");

                    for (int i = 0; i < qtyList.Count; i++)
                    {
                        Console.WriteLine("Inside third for loop in Get Recent Selection");
                        totalMealsCount += Int32.Parse(qtyList[i].ToString());
                    }
                    setMenu();
                    Console.WriteLine("END OF GET RECENT SELECTION #2");

                }

                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                //Thread.Sleep(3000);
                await Task.Delay(1000);
                loadingCount = loadingCount - 1;
                Debug.WriteLine("loadingCount = " + loadingCount.ToString());
                if (loadingCount == 0)
                {
                    //Thread.Sleep(3000);
                    //Task.Delay(30000).Wait();
                    //loadBackground.IsVisible = false;
                    loadingAnim.IsVisible = false;
                    belowMenu.IsVisible = true;
                }

                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        private void resetBttn_Clicked(object sender, EventArgs e)
        {

            for (int i = 0; i < Meals1.Count; i++)
            {
                if (Meals1[i].MealQuantity > 0)
                {
                    Meals1[i].MealQuantity = 0;
                }

            }

            //int indexOfMealPlanSelected = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
            int indexOfMealPlanSelected = -1;
            //int selectedIndex = ((ArrayList)dropDownList.ItemsSource).IndexOf(dropDownText.Text);
            foreach (var plan in namesColl)
            {
                indexOfMealPlanSelected++;
                if (plan.name == dropDownText.Text)
                {
                    break;
                }
            }
            Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
            Preferences.Set("purchUid", purchUidArray[indexOfMealPlanSelected].ToString());
            Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

            //string s = SubscriptionPicker.SelectedItem.ToString();
            string s = dropDownText.Text;
            s = s.Substring(0, 2);
            Preferences.Set("total", int.Parse(s));
            totalCount.Text = Preferences.Get("total", 0).ToString();
            Preferences.Set("origMax", int.Parse(s));
        }

        //start of menu functions
        void clickedOpenMenu(object sender, EventArgs e)
        {
            openedMenu.IsVisible = true;
        }

        void clickedCloseMenu(object sender, EventArgs e)
        {
            openedMenu.IsVisible = false;
        }

        async void clickedStripeTest(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new StripeTesting(), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedLanding(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(first, last, email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealPlan(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MealPlans(first, last, email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSelect(System.Object sender, System.EventArgs e)
        {
            if (Preferences.Get("canChooseSelect", false) == false)
            {
                try
                {
                    WebClient client4 = new WebClient();
                    var content2 = client4.DownloadString(Constant.AlertUrl);
                    var obj = JsonConvert.DeserializeObject<AlertsObj>(content2);

                    await DisplayAlert(obj.result[25].title, obj.result[25].message, obj.result[25].responses);
                }
                catch
                {
                    await DisplayAlert("Error", "please purchase a meal plan first", "OK");
                }
            }
            else
            {
                Zones[] zones = new Zones[] { };
                await Navigation.PushAsync(new Select(zones, first, last, email), false);
                //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
            }
        }

        async void clickedSubscription(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(first, last, email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSubHistory(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionHistory(first, last, email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        void xButtonClicked(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            baaPopUpGrid.IsVisible = false;
        }

        void clickedBecomeAmb(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = true;
            baaPopUpGrid.IsVisible = true;
        }

        void clickedCreateAmb(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (AmbEmailEntry.Text != null && AmbEmailEntry.Text != "")
                {
                    createAmb newAmb = new createAmb();
                    newAmb.code = AmbEmailEntry.Text.Trim();
                    var createAmbSerializedObj = JsonConvert.SerializeObject(newAmb);
                    var content = new StringContent(createAmbSerializedObj, Encoding.UTF8, "application/json");
                    var client = new HttpClient();
                    var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/brandAmbassador/create_ambassador", content);
                    Console.WriteLine("RESPONSE TO CREATE_AMBASSADOR   " + response.Result);
                    Console.WriteLine("CREATE JSON OBJECT BEING SENT: " + createAmbSerializedObj);
                    fade.IsVisible = false;
                    baaPopUpGrid.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }

        void clickedLogout(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties["platform"] = "GUEST";
            Application.Current.Properties.Remove("time_stamp");
            //Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }
        //end of menu functions

        void clickedClosePopUp(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            popUpFrame.IsVisible = false;
        }

        void showPopUp(string title, string message)
        {
            fade.IsVisible = true;
            scroll.ScrollToAsync(0, -50, true);
            popUpHeader.Text = title;
            popUpBody.Text = message;
            popUpFrame.IsVisible = true;
        }

        //async Task CheckVersion()
        //{
        //    Debug.WriteLine("before getting latest");
        //    var isLatest = await CrossLatestVersion.Current.IsUsingLatestVersion();
        //    Debug.WriteLine("after getting latest");

        //    if (!isLatest)
        //    {
        //        Debug.WriteLine("not latest version");
        //        await DisplayAlert("Mealsfor.Me\nhas gotten even better!", "Please visit the App Store to get the latest version.", "OK");
        //        await CrossLatestVersion.Current.OpenAppInStore();
        //    }
        //    else
        //    {
        //        Debug.WriteLine("current version");
        //        restOfConstructor();
        //        //GetBusinesses();

        //        //CartTotal.Text = CheckoutPage.total_qty.ToString();
        //    }
        //}

        void restOfConstructor()
        {
            try
            {
                getFavorites();
                GetMealPlans();
                //Task.Delay(1000).Wait();
                setDates();
                getUserMeals();
                setMenu();

                var width = DeviceDisplay.MainDisplayInfo.Width;
                var height = DeviceDisplay.MainDisplayInfo.Height;
                checkPlatform(height, width);
            }
            catch (Exception ex)
            {
                Generic gen = new Generic();
                gen.parseException(ex.ToString());
            }
        }
    }
}
