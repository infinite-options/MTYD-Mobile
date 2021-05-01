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
//using Xamarin.CommunityToolkit;

namespace MTYD.ViewModel
{
    //==========================================
    // CARLOS CLASS FOR PROGRESS BAR
    //public class Origin : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    //    public Thickness margin { get; set; }
    //    public Thickness update
    //    {
    //        get { return margin; }
    //        set
    //        {
    //            margin = value;
    //            PropertyChanged(this, new PropertyChangedEventArgs("margin"));
    //        }
    //    }
    //    public string mealsLeft { get; set; }
    //    public string barLabel
    //    {
    //        get { return mealsLeft; }
    //        set
    //        {
    //            mealsLeft = value;
    //            PropertyChanged(this, new PropertyChangedEventArgs("mealsLeft"));
    //        }
    //    }

    //}
    //==========================================




    public partial class ExploreMeals : ContentPage
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
        //public static string userId = (string)Application.Current.Properties["user_id"];
        //private string postUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selection?customer_uid=" + userId;
        private const string menuUrl = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/upcoming_menu";
        //private string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + userId;
        //private const string userMeals = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=100-000001";
        private static Dictionary<string, string> qtyDict = new Dictionary<string, string>();
        private static Dictionary<string, string> qtyDict_addon = new Dictionary<string, string>();
        private static List<MealInformation> mealsSaved = new List<MealInformation>();
        private static int mealsAllowed;
        public int count;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
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
        //List<ImageButton> plates;

        WebClient client = new WebClient();

        public ExploreMeals()
        {
            //public ObservableCollection<AddressAutocomplete> Add;


            availableDates = new List<Date>();
            dateDict = new Dictionary<string, Date>();
            favDict = new Dictionary<string, bool>();
            //passedZones = zones;
            InitializeComponent();


            //move bar initialization testing
            //==========================================
            // CARLOS PROGRESS BAR INITIALIZATION
            var m = new Origin();
            m.margin = new Thickness(0, 0, 0, 0);
            m.mealsLeft = "";

            

            BarParameters.Add(m);
            MyCollectionView.ItemsSource = BarParameters;
            //===========================================
            Debug.WriteLine("bar initialization done");

            Preferences.Set("origMax", 0);
            //getFavorites();
            //GetMealPlans();
            //Task.Delay(1000).Wait();
            setDates();
            //getUserMeals();
            setMenu();

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            //dateCarousel.PeekAreaInsets = new Thickness((width / 2) - 250, 0);



            //first = firstName;
            //last = lastName;
            //email = userEmail;
            checkPlatform(height, width);
            Preferences.Set("canChooseSelect", true);

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


            Debug.WriteLine("finished with constructor");
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 32;
                heading.Margin = new Thickness(0, 0, 0, 30);


                popUpFrame.Margin = new Thickness(0, height / 10, 0, 0);
                popUpFrame.WidthRequest = width / 2.5;
                popUpFrame.HeightRequest = popUpFrame.WidthRequest * (6/5);
                //popUpHeader.HeightRequest = popUpFrame.HeightRequest / 3;
                popButton1.HeightRequest = popUpFrame.HeightRequest / 7;
                popButton2.HeightRequest = popUpFrame.HeightRequest / 7;
                popButton3.HeightRequest = popUpFrame.HeightRequest / 7;

                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;
                menu.Margin = new Thickness(25, 0, 0, 30);
                

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

                //weekOneAddOns.HeightRequest = height / 6.8;

            }
            else //android
            {
                ////open menu adjustments
                //orangeBox2.HeightRequest = height / 2;
                //orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                //orangeBox2.CornerRadius = height / 40;
                //heading2.WidthRequest = width / 5;
                //menu2.HeightRequest = width / 25;
                //menu2.WidthRequest = width / 25;
                //menu2.Margin = new Thickness(25, 0, 0, 30);
                //heading.WidthRequest = width / 5;
                ////heading adjustments
            }
            Debug.WriteLine("checkPlatform done");
        }

        //async void getZones()
        //{
        //    //string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;

        //    var content = client.DownloadString(url3);
        //    var obj = JsonConvert.DeserializeObject<ZonesDto>(content);
        //}
        

        async void getFavorites()
        {
            //GetFavPost getFav = new GetFavPost();
            //getFav.customer_uid = (string)Application.Current.Properties["user_id"];
            //var getFavSerializedObj = JsonConvert.SerializeObject(getFav);
            //var content4 = new StringContent(getFavSerializedObj, Encoding.UTF8, "application/json");
            //var client3 = new System.Net.Http.HttpClient();
            //var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/get", content4);
            //var message = await response3.Content.ReadAsStringAsync();
            //Debug.WriteLine("RESPONSE TO getfavs   " + response3.ToString());
            //Debug.WriteLine("json object sent:  " + getFavSerializedObj.ToString());
            //Debug.WriteLine("message received:  " + message.ToString());

            ////if there are any favorites stored
            //if (message.Contains("840") == true)
            //{
            //    var data = JsonConvert.DeserializeObject<FavsDto>(message);
            //    Debug.WriteLine("RESPONSE TO getfavs   " + response3.ToString());
            //    Debug.WriteLine("favorites: " + data.result[0].favorites);

            //    string favoritesList = data.result[0].favorites;

            //    while (favoritesList.Contains(",") != false)
            //    {
            //        Debug.WriteLine("favorite: " + favoritesList.Substring(0, favoritesList.IndexOf(",")));
            //        favDict.Add(favoritesList.Substring(0, favoritesList.IndexOf(",")), true);
            //        favoritesList = favoritesList.Substring(favoritesList.IndexOf(",") + 1);
            //    }

            //    Debug.WriteLine("favorite: " + favoritesList);
            //    favDict.Add(favoritesList, true);

            //}
        }

        public async void directLoginButtonClicked(object sender, EventArgs e)
        {
            //Preferences.Set("FromExploreMeals", true);
            //Application.Current.MainPage = new MainPage();
            Application.Current.MainPage = new MainLogin();
        }

        async void clickedSignUp(object sender, EventArgs e)
        {
            Preferences.Set("canChooseSelect", false);
            Application.Current.MainPage = new CarlosSignUp();
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

                        Debug.WriteLine("itemuid: " + obj.Result[i].MealUid + " and meal name: " + obj.Result[i].MealName);

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
                            MealPrice = obj.Result[i].MealPrice,
                            ItemUid = obj.Result[i].MealUid,
                            MealDesc = obj.Result[i].MealDesc,
                            SeeDesc = false,
                            SeeImage = true,
                            HeartSource = source
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
                            MealPrice = obj.Result[i].MealPrice,
                            ItemUid = obj.Result[i].MealUid,
                            MealDesc = obj.Result[i].MealDesc,
                            SeeDesc = false,
                            SeeImage = true,
                            HeartSource = source
                        });

                        addOnCount++;
                        //testing in new location
                        //weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));

                        //weekOneAddOns.ItemsSource = Meals2;

                    }
                }

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

                if (addOnCount % 2 != 0)
                    addOnCount++;
                //weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));
                Debug.WriteLine("mealCount:" + mealCount.ToString());
                Debug.WriteLine("mealCount half:" + ((int)(mealCount / 2)).ToString());
                Debug.WriteLine("height:" + weekOneMenu.HeightRequest.ToString());
                BindingContext = this;

                //testing to make sure the bar is filled
                if (isAlreadySelected == true)
                {
                    Debug.WriteLine("inside isalreadyselected if");
                    BarParameters[0].margin = new Thickness(this.Width, 0, 0, 0);
                    BarParameters[0].update = new Thickness(this.Width, 0, 0, 0);
                }
            }
            catch
            {
                Console.WriteLine("SET MENU IS CRASHING!");
            }
        }

        // Set Dates of Each Label
        private void setDates()
        {
            Debug.WriteLine("setDates entered");
            try
            {
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
                selectedDate.outlineColor = Color.Red;
                text1 = selectedDate.fullDateTime;
                //Debug.WriteLine("date picked: " + text1);
                Preferences.Set("dateSelected", availableDates[0].fullDateTime.Substring(0, 10));
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
                //getUserMeals();

                mealsSaved.Clear();   //New Addition SV
                resetAll(); //New Addition SV

                isSkip = false;
                isSurprise = false;

                if ((string)Application.Current.Properties["platform"] != "GUEST")
                {
                    //checkDateStatuses();
                    //GetRecentSelection();
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
                
            }
            catch
            {
                Console.WriteLine("SET DATA IS CRASHING");
            }

        }


        /////////date change for carousel view
        async private void dateChangeCar(object sender, EventArgs e)
        {
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
            DateTime selected = new DateTime(Int32.Parse(text1.Substring(0, 4)), Int32.Parse(text1.Substring(5, 2)), Int32.Parse(text1.Substring(8, 2)));
            Debug.WriteLine(sender.GetType().ToString());
            Button button1 = (Button)sender;
            Date dateChosen = button1.BindingContext as Date;
            selectedDate.outlineColor = Color.White;
            selectedDate = dateChosen;
            selectedDate.outlineColor = Color.Red;
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
            //getUserMeals();

            mealsSaved.Clear();   //New Addition SV
            resetAll(); //New Addition SV

            isSkip = false;
            isSurprise = false;

            if ((string)Application.Current.Properties["platform"] != "GUEST")
            {
                //await GetRecentSelection();
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
           

            //reset the buttons
            //default to surprise if null
        }
        //////////

        

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
            ImageButton b = (ImageButton)sender;
            MealInfo ms = b.BindingContext as MealInfo;
            //ms.MealQuantity++;
            popUpHeader.Text = "Don’t forget your favorite meals! Click the heart to easily find your favorite meals and get reminders.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);

            //favoriting
            if (b.Source.ToString().Equals("File: leftHeart.png"))
            {
                //favDict.Add(ms.ItemUid, true);
                //UpdateFavPost updateFav = new UpdateFavPost();
                //updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                //updateFav.favorite = ms.ItemUid;
                //var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                //var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                //var client3 = new System.Net.Http.HttpClient();
                ////post endpoint passes in 1 favorite and appends it to the end of the list of favorites for the user
                //var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/post", content4);
                //var message = await response3.Content.ReadAsStringAsync();
                //Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                //Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                //Debug.WriteLine("message received:  " + message.ToString());
                ////b.Source = "heart.png";
                //ms.HeartSource = "leftFilledHeart.png";

            }
            //unfavoriting
            else
            {
                //favDict.Remove(ms.ItemUid);

                ////if there are no favorites to be saved, send in a blank list of favorites
                //if (favDict.Count == 0)
                //{
                //    UpdateFavPost updateFav = new UpdateFavPost();
                //    updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                //    updateFav.favorite = "";
                //    var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                //    var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                //    var client3 = new System.Net.Http.HttpClient();
                //    //overwrites the entire favorites list and only keep the favorite that is passed in
                //    var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                //    var message = await response3.Content.ReadAsStringAsync();
                //    Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                //    Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                //    Debug.WriteLine("message received:  " + message.ToString());
                //}
                //else if (favDict.Count == 1)
                //{
                //    UpdateFavPost updateFav = new UpdateFavPost();
                //    updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                //    updateFav.favorite = favDict.Keys.First();
                //    var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                //    var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                //    var client3 = new System.Net.Http.HttpClient();
                //    //update overwrites the entire favorites list and only keep the favorite that is passed in
                //    var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                //    var message = await response3.Content.ReadAsStringAsync();
                //    Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                //    Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                //    Debug.WriteLine("message received:  " + message.ToString());
                //}
                //else if (favDict.Count > 1)
                //{
                //    UpdateFavPost updateFav = new UpdateFavPost();
                //    updateFav.customer_uid = (string)Application.Current.Properties["user_id"];
                //    updateFav.favorite = favDict.Keys.First();
                //    //var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                //    //var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                //    //var client3 = new System.Net.Http.HttpClient();
                //    ////update overwrites the entire favorites list and only keep the favorite that is passed in
                //    //var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                //    //var message = await response3.Content.ReadAsStringAsync();
                //    //Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                //    //Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                //    //Debug.WriteLine("message received:  " + message.ToString());

                //    foreach (string uid in favDict.Keys)
                //    {
                //        if (uid == favDict.Keys.First())
                //            continue;
                //        else updateFav.favorite += "," + uid;


                //        //UpdateFavPost updateFav2 = new UpdateFavPost();
                //        //updateFav2.customer_uid = (string)Application.Current.Properties["user_id"];
                //        //updateFav2.favorite = uid;
                //        //var updateFavSerializedObj2 = JsonConvert.SerializeObject(updateFav);
                //        //var content2 = new StringContent(updateFavSerializedObj2, Encoding.UTF8, "application/json");
                //        //var client2 = new System.Net.Http.HttpClient();
                //        ////post endpoint passes in 1 favorite and appends it to the end of the list of favorites for the user
                //        //var response2 = await client2.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/post", content2);
                //        //var message2 = await response2.Content.ReadAsStringAsync();
                //        //Debug.WriteLine("RESPONSE TO updatefavs   " + response2.ToString());
                //        //Debug.WriteLine("json object sent:  " + updateFavSerializedObj2.ToString());
                //        //Debug.WriteLine("message received:  " + message2.ToString());
                //    }
                //    var updateFavSerializedObj = JsonConvert.SerializeObject(updateFav);
                //    var content4 = new StringContent(updateFavSerializedObj, Encoding.UTF8, "application/json");
                //    var client3 = new System.Net.Http.HttpClient();
                //    //update overwrites the entire favorites list and only keep the favorite that is passed in
                //    var response3 = await client3.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/favourite_food/update", content4);
                //    var message = await response3.Content.ReadAsStringAsync();
                //    Debug.WriteLine("RESPONSE TO updatefavs   " + response3.ToString());
                //    Debug.WriteLine("json object sent:  " + updateFavSerializedObj.ToString());
                //    Debug.WriteLine("message received:  " + message.ToString());
                //}

                //b.Source = "heartoutline.png";
                //ms.HeartSource = "leftHeart.png";

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
            popUpHeader.Text = "Looks like you're enjoying MealsFor.Me! The + and - buttons help you add / remove meals from your meal plan.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
        }

        private async void clickIncreaseAddOn(object sender, EventArgs e)
        {
            popUpHeader.Text = "Looks like you're enjoying MealsFor.Me! The + and - buttons help you add / remove meals from your meal plan.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
            ////testing reseting the save button when they increase or decrease amount of meals/add-ons
            //surpriseBttn.BackgroundColor = Color.White;
            //surpriseFrame.BackgroundColor = Color.White;
            //surpriseBttn.TextColor = Color.Black;
            //saveBttn.BackgroundColor = Color.White;
            //saveFrame.BackgroundColor = Color.White;
            //saveBttn.TextColor = Color.Black;
            //skipBttn.BackgroundColor = Color.White;
            //skipFrame.BackgroundColor = Color.White;
            //skipBttn.TextColor = Color.Black;

            //Button b = (Button)sender;
            //MealInfo ms = b.BindingContext as MealInfo;
            //ms.MealQuantity++;
            //ms.Background = Color.FromHex("#F8BB17");
        }

        private async void clickDecrease(object sender, EventArgs e)
        {
            popUpHeader.Text = "Looks like you're enjoying MealsFor.Me! The + and - buttons help you add / remove meals from your meal plan.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
            //Button b = (Button)sender;
            //MealInfo ms = b.BindingContext as MealInfo;
            //int count = Preferences.Get("total", 0);
            ////if meal quantity is 0, don't do anything
            //if (count != Preferences.Get("origMax", 0) && ms.MealQuantity != 0)
            //{



            //    //testing reseting the save button when they increase or decrease amount of meals/add-ons
            //    surpriseBttn.BackgroundColor = Color.White;
            //    surpriseFrame.BackgroundColor = Color.White;
            //    surpriseBttn.TextColor = Color.Black;
            //    saveBttn.BackgroundColor = Color.White;
            //    saveFrame.BackgroundColor = Color.White;
            //    saveBttn.TextColor = Color.Black;
            //    skipBttn.BackgroundColor = Color.White;
            //    skipFrame.BackgroundColor = Color.White;
            //    skipBttn.TextColor = Color.Black;
            //    // ======================================
            //    // CARLOS PROGRESS BAR INTEGRATION
            //    var width = this.Width;
            //    var result = this.Width / Preferences.Get("origMax", 0);
            //    Debug.WriteLine("DIVISOR DECREMENT FUNCTION: " + Preferences.Get("origMax", 0));
            //    factor = result;

            //    Debug.WriteLine("FACTOR TO INCREASE OR DECREASE PROGRESS BAR: " + factor);
            //    Debug.WriteLine("WIDTH                                      : " + width);

            //    var currentMargin = BarParameters[0].margin;
            //    var currentLeft = currentMargin.Left;
            //    var newLeft = currentLeft - factor;

            //    Debug.WriteLine("CURRENT LEFT: " + currentLeft);
            //    Debug.WriteLine("NEW LEFT" + newLeft);

            //    currentMargin.Left = newLeft;

            //    BarParameters[0].margin = currentMargin;
            //    BarParameters[0].update = currentMargin;
            //    // ======================================
            //    if (ms.MealQuantity != 0)
            //    {
            //        totalCount.Text = (++count).ToString();
            //        BarParameters[0].mealsLeft = "Please Select " + count.ToString() + " Meals";
            //        mealsLeftLabel.Text = "Please Select " + count.ToString() + " Meals";
            //        BarParameters[0].barLabel = "Please Select " + count.ToString() + " Meals";
            //        Preferences.Set("total", count);
            //        //BarParameters[0].mealsLeft = count.ToString();
            //        ms.MealQuantity--;

            //        if (ms.MealQuantity == 0)
            //            ms.Background = Color.White;

            //        int permCount = Preferences.Get("origMax", 0);
            //        //float adder = 0.0f;
            //        //if (permCount == 5)
            //        //{
            //        //    //adder = 0.2f;
            //        //    weekOneProgress.Progress -= 0.2;
            //        //}
            //        //else if (permCount == 10)
            //        //{
            //        //    //adder = 0.1f;
            //        //    weekOneProgress.Progress -= 0.1;
            //        //}
            //        //else if (permCount == 15)
            //        //{
            //        //    //adder = 0.067f;
            //        //    weekOneProgress.Progress -= 0.067;
            //        //}
            //        //else if (permCount == 20)
            //        //{
            //        //    //adder = 0.05f;
            //        //    weekOneProgress.Progress -= 0.05;
            //        //}

            //        //weekOneProgress.Progress -= 0.1;
            //        // weekOneProgress.Progress -= adder;
            //        //if (weekOneProgress.Progress < 0.3)
            //        //{
            //        //    weekOneProgress.ProgressColor = Color.LightGoldenrodYellow;
            //        //}
            //        //else if (weekOneProgress.Progress >= 0.3 && weekOneProgress.Progress < 0.5)
            //        //{
            //        //    weekOneProgress.ProgressColor = Color.Orange;
            //        //}
            //        //else if (weekOneProgress.Progress >= 0.5 && weekOneProgress.Progress <= 0.7)
            //        //{
            //        //    weekOneProgress.ProgressColor = Color.LightGreen;
            //        //}
            //        //else if (weekOneProgress.Progress >= 0.8)
            //        //{
            //        //    weekOneProgress.ProgressColor = Color.DarkOliveGreen;
            //        //}
            //    }
            //    else { }

            //}
            //else { }
        }

        private async void clickDecreaseAddOn(object sender, EventArgs e)
        {
            popUpHeader.Text = "Looks like you're enjoying MealsFor.Me! The + and - buttons help you add / remove meals from your meal plan.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
            //Button b = (Button)sender;
            //MealInfo ms = b.BindingContext as MealInfo;
            //if (ms.MealQuantity != 0)
            //{
            //    //testing reseting the save button when they increase or decrease amount of meals/add-ons
            //    surpriseBttn.BackgroundColor = Color.White;
            //    surpriseFrame.BackgroundColor = Color.White;
            //    surpriseBttn.TextColor = Color.Black;
            //    saveBttn.BackgroundColor = Color.White;
            //    saveFrame.BackgroundColor = Color.White;
            //    saveBttn.TextColor = Color.Black;
            //    skipBttn.BackgroundColor = Color.White;
            //    skipFrame.BackgroundColor = Color.White;
            //    skipBttn.TextColor = Color.Black;

            //    ms.MealQuantity--;
            //    if (ms.MealQuantity == 0)
            //        ms.Background = Color.White;
            //}
        }

        

        
        private async void saveUserMeals(object sender, EventArgs e)
        {
            popUpHeader.Text = "Save allows you to select your meals up to 3 weeks in advance.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
        }

        private async void skipMealSelection(object sender, EventArgs e)
        {
            popUpHeader.Text = "Not at home or have other plans? Its easy to Skip a delivery and we’ll automatically extend your subscription.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
        }

        private void surprise()
        {
            
        }


        private async void surpriseMealSelection(object sender, EventArgs e)
        {
            popUpHeader.Text = "Surprise means we'll give you an assortment of meals on the specific delivery day.";

            fade.IsVisible = true;
            popUpFrame.IsVisible = true;
            await scroll.ScrollToAsync(0, -50, true);
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

        

        void clickedBack(System.Object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }

        async void clickedSub(System.Object sender, System.EventArgs e)
        {
            //await Navigation.PushAsync(new SubscriptionPage(first, last, email));
            //Application.Current.MainPage = new MainPage();
            Application.Current.Properties["user_id"] = "000-00000";
            Application.Current.Properties["platform"] = "GUEST";
            Preferences.Set("user_latitude", "0.0");
            Preferences.Set("user_longitude", "0.0");
            //Application.Current.MainPage = new NavigationPage(new SubscriptionPage("Welcome", "Guest", "welcome@guest.com"));
            await Navigation.PushAsync(new SubscriptionPage("Welcome", "Guest", "welcome@guest.com"));
        }

        void clickedClosePopUp(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            popUpFrame.IsVisible = false;
        }

        
    }
}
