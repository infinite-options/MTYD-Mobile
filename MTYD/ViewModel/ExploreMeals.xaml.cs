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
using System.Collections.Generic;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.Threading;
using MTYD.Constants;

//auto-complete
//using DurianCode.PlacesSearchBar;

namespace MTYD.ViewModel
{
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
        public static string userId = "100-000050";
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
        string first = ""; string last = ""; string email = "";
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
        bool withinZones = false;
        WebClient client4 = new WebClient();
        Zones[] passingZones;
        WebClient client = new WebClient();

        // address auto-complete
        public const string GooglePlacesApiAutoCompletePath = "https://maps.googleapis.com/maps/api/place/autocomplete/json?key={0}&input={1}&components=country:us"; //Adding country:us limits results to us
        private static HttpClient _httpClientInstance;
        public static HttpClient HttpClientInstance => _httpClientInstance ?? (_httpClientInstance = new HttpClient());
        private ObservableCollection<AddressAutocomplete> _addresses;

        public ExploreMeals()
        {
            availableDates = new List<Date>();
            dateDict = new Dictionary<string, Date>();
            InitializeComponent();

            //===========================================
            Debug.WriteLine("bar initialization done");

            Preferences.Set("origMax", 0);
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

            /* auto-complete
            search_bar.ApiKey = GooglePlacesApiKey;
            search_bar.Type = PlaceType.Address;
            search_bar.Components = new Components("country:au|country:nz"); // Restrict results to Australia and New Zealand
            search_bar.PlacesRetrieved += Search_Bar_PlacesRetrieved;
            search_bar.TextChanged += Search_Bar_TextChanged;
            search_bar.MinimumSearchText = 2;
            results_list.ItemSelected += Results_List_ItemSelected;
            */

            Debug.WriteLine("finished with constructor");
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                //weekOneMenu.HeightRequest = height / 6.8;

                addOns.FontSize = width / 32;

                //weekOneAddOns.HeightRequest = height / 6.8;

                fade.Margin = new Thickness(0, -height / 3, 0, -height / 3);

                xButton.FontSize = width / 37;
                //addressGrid.Margin = new Thickness(width / 30, height / 8, width / 30, 0);
                addressGrid.HeightRequest = height / 4.5;
                addressGrid.WidthRequest = width / 2.3;

                outerFrame.HeightRequest = height/ 4.5;

                addressHeader.FontSize = width / 35;

                street.HeightRequest = width / 30;
                UnitCity.HeightRequest = width / 24;
                StateZip.HeightRequest = width / 24;

                checkAddressButton.WidthRequest = width / 5;
                checkAddressButton.HeightRequest = width / 20;
                checkAddressButton.CornerRadius = (int)(width / 40);
            }
            else //android
            {

            }
            Debug.WriteLine("checkPlatform done");
        }

        //async void getZones()
        //{
        //    //string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;

        //    var content = client.DownloadString(url3);
        //    var obj = JsonConvert.DeserializeObject<ZonesDto>(content);
        //}


        //async void clickedPfp(System.Object sender, System.EventArgs e)
        //{
        //    await Navigation.PushAsync(new UserProfile(first, last, email), false);
        //}

        //async void clickedMenu(System.Object sender, System.EventArgs e)
        //{
        //    await Navigation.PushAsync(new Menu(first, last, email));
        //}

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

        void clickedCheckAddress(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = true;
            addressGrid.IsVisible = true;
        }

        void clickedX(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            addressGrid.IsVisible = false;
        }

        async void clickedCheck(System.Object sender, System.EventArgs e)
        {
            if (AddressEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your address", "OK");
                return;
            }
            if (CityEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your city", "OK");
                return;
            }
            if (StateEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your state", "OK");
                return;
            }
            if (ZipEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your zipcode", "OK");
                return;
            }
            if (AptEntry.Text == null)
            {
                AptEntry.Text = "";
            }

            // Setting request for USPS API
            XDocument requestDoc = new XDocument(
                new XElement("AddressValidateRequest",
                new XAttribute("USERID", "400INFIN1745"),
                new XElement("Revision", "1"),
                new XElement("Address",
                new XAttribute("ID", "0"),
                new XElement("Address1", AddressEntry.Text.Trim()),
                new XElement("Address2", AptEntry.Text.Trim()),
                new XElement("City", CityEntry.Text.Trim()),
                new XElement("State", StateEntry.Text.Trim()),
                new XElement("Zip5", ZipEntry.Text.Trim()),
                new XElement("Zip4", "")
                     )
                 )
             );
            var url = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML=" + requestDoc;
            Console.WriteLine(url);
            var client2 = new WebClient();
            var response2 = client2.DownloadString(url);

            var xdoc = XDocument.Parse(response2.ToString());
            Console.WriteLine("xdoc begin");
            Console.WriteLine(xdoc);


            string latitude = "0";
            string longitude = "0";
            foreach (XElement element in xdoc.Descendants("Address"))
            {
                if (GetXMLElement(element, "Error").Equals(""))
                {
                    if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(ZipEntry.Text.Trim()) && GetXMLElement(element, "City").Equals(CityEntry.Text.ToUpper().Trim())) // Best case
                    {
                        // Get longitude and latitide because we can make a deliver here. Move on to next page.
                        // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                        //GetAddressLatitudeLongitude();
                        Geocoder geoCoder = new Geocoder();

                        Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                        Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                        Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

                        //used for createaccount endpoint
                        Preferences.Set("user_latitude", latitude);
                        Preferences.Set("user_longitude", longitude);
                        Debug.WriteLine("user latitude: " + latitude);
                        Debug.WriteLine("user longitude: " + longitude);

                        //directSignUp.latitude = latitude;
                        //directSignUp.longitude = longitude;
                        //map.MapType = MapType.Street;
                        //var mapSpan = new MapSpan(position, 0.001, 0.001);

                        //Pin address = new Pin();
                        //address.Label = "Delivery Address";
                        //address.Type = PinType.SearchResult;
                        //address.Position = position;

                        //map.MoveToRegion(mapSpan);
                        //map.Pins.Add(address);

                        //https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/-121.8866517,37.2270928 long,lat
                        //var request3 = new HttpRequestMessage();
                        //Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                        //Debug.WriteLine("latitude: " + latitude + ", longitude: " + longitude);
                        string url3 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/categoricalOptions/" + longitude + "," + latitude;
                        //request3.RequestUri = new Uri(url3);
                        //request3.Method = HttpMethod.Get;
                        //var client3 = new HttpClient();
                        //HttpResponseMessage response3 = await client3.SendAsync(request3);
                        Debug.WriteLine("categorical options url: " + url3);

                        var content = client4.DownloadString(url3);
                        var obj = JsonConvert.DeserializeObject<ZonesDto>(content);

                        //HttpContent content3 = response3.Content;
                        //Console.WriteLine("content: " + content3);
                        //var userString3 = await content3.ReadAsStringAsync();
                        //Debug.WriteLine("userString3: " + userString3);
                        //JObject info_obj3 = JObject.Parse(userString3);
                        if (obj.Result.Length == 0)
                        {
                            withinZones = false;
                        }
                        else
                        {
                            Debug.WriteLine("first business: " + obj.Result[0].business_name);
                            passingZones = obj.Result;
                            withinZones = true;
                        }

                        break;
                    }
                    else if (GetXMLElement(element, "DPVConfirmation").Equals("D"))
                    {
                        //await DisplayAlert("Alert!", "Address is missing information like 'Apartment number'.", "Ok");
                        //return;
                    }
                    else
                    {
                        //await DisplayAlert("Alert!", "Seems like your address is invalid.", "Ok");
                        //return;
                    }
                }
                else
                {   // USPS sents an error saying address not found in there records. In other words, this address is not valid because it does not exits.
                    //Console.WriteLine("Seems like your address is invalid.");
                    //await DisplayAlert("Alert!", "Error from USPS. The address you entered was not found.", "Ok");
                    //return;
                }
            }
            if (latitude == "0" || longitude == "0")
            {
                await DisplayAlert("We couldn't find your address", "Please check for errors.", "OK");
            }
            else if (withinZones == false)
            {
                await DisplayAlert("Sorry", "Address is not within any of our delivery zones.", "OK");
                fade.IsVisible = false;
                addressGrid.IsVisible = false;
            }
            else
            {
                bool subscribe = await DisplayAlert("Valid Address", "Your address is within our zone.", "Sign Up", "Continue Exploring");
                if (subscribe)
                {
                    fade.IsVisible = false;
                    addressGrid.IsVisible = false;
                    await Navigation.PushAsync(new SubscriptionPage("Welcome", "Guest", "welcome@guest.com"));
                }
                else
                {
                    fade.IsVisible = false;
                    addressGrid.IsVisible = false;
                }
            }
        }

        public static string GetXMLElement(XElement element, string name)
        {
            var el = element.Element(name);
            if (el != null)
            {
                return el.Value;
            }
            return "";
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
                //weekOneMenu.ItemsSource = Meals1;
                if (mealCount % 2 != 0)
                    mealCount++;
                weekOneMenu.HeightRequest = 320 * ((mealCount / 2));

                if (addOnCount % 2 != 0)
                    addOnCount++;
                weekOneAddOns.HeightRequest = 280 * ((addOnCount / 2));
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
                    //datePicker.Items.Add(i.Key);
                    //String.Format("MMMM dd, yyyy", i.Key);

                    //testing with carouselView
                    //format of i.Key = yyyy-mm-dd hh-mm-ss
                    //format for DateTime constructor: (year, month, day, hour, minute, second)
                    var dateObj = new DateTime(int.Parse(i.Key.Substring(0, 4)), int.Parse(i.Key.Substring(5, 2)), int.Parse(i.Key.Substring(8, 2)), 0, 0, 0);

                    Date d = new Date();
                    d.dotw = dateObj.ToString("ddd").ToUpper();
                    d.month = dateObj.ToString("MMM");
                    d.day = dateObj.Day.ToString();
                    //if (d.day.Substring(0, 1) == "0")
                    //    d.day = d.day.Substring(1, 2);
                    //for just date use Substring(0, 10)
                    d.fullDateTime = i.Key;
                    d.fillColor = Color.White;
                    d.outlineColor = Color.Black;
                    d.index = index1;
                    index1++;
                    Debug.WriteLine("fullDate: $" + d.fullDateTime + "$");
                    availableDates.Add(d);
                    //dateDict.Add(d.fullDateTime, d);
                }

                Debug.WriteLine("after adding to picker reached");
                dateCarousel.ItemsSource = availableDates;
                //datePicker.SelectedIndex = 0;
                availDateIndex = 0;
                selectedDate = availableDates[0];
                selectedDate.outlineColor = Color.Red;
                text1 = selectedDate.fullDateTime;
                //Debug.WriteLine("date picked: " + text1);
                Preferences.Set("dateSelected", availableDates[0].fullDateTime.Substring(0, 10));
                Console.WriteLine("dateSet: " + Preferences.Get("dateSelected", ""));




                int orig = Preferences.Get("origMax", 0);
                //if (orig != 0)
                //{
                //    totalCount.Text = orig.ToString();

                //}
                //else
                //{
                //    totalCount.Text = "Count";
                //}
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

                //checkDateStatuses();
                //GetRecentSelection();
                //GetRecentSelection2();

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
            selectedDate.outlineColor = Color.Black;
            selectedDate = dateChosen;
            selectedDate.outlineColor = Color.Red;
            //dateChosen.fillColor = Color.LightGray;
            selectedDotw = dateChosen.dotw;
            Debug.WriteLine("dayOfWeek: " + selectedDotw);




            //testing no setMenu();
            //setMenu();

            //weekOneProgress.Progress = 0;


            int orig = Preferences.Get("origMax", 0);
            //if (orig != 0)
            //{
            //    totalCount.Text = orig.ToString();

            //}
            //else
            //{
            //    totalCount.Text = "Count";
            //}
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

            //await GetRecentSelection();
            //GetRecentSelection2();

            //testing setMenu here
            setMenu();


            Console.WriteLine("isAlreadySeleced in planchange" + isAlreadySelected);
            //bool isAlreadySelected = Preferences.Get("isAlreadySelected", true);
            
            //reset the buttons
            //default to surprise if null
        }
        //////////

        // Favorite BUtton
        private async void clickedFavorite(object sender, EventArgs e)
        {
            ImageButton b = (ImageButton)sender;
            if (b.Source.ToString().Equals("File: heartoutline.png"))
            {
                b.Source = "heart.png";
            }
            else
            {
                b.Source = "heartoutline.png";

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

        //highlight each date either light gray, orange, or white
        protected async Task checkDateStatuses()
        {
            foreach (Date d in availableDates)
            {
                Debug.WriteLine("INSIDE checkDateStatuses #1");
                Debug.WriteLine("availableDate passed in: " + d.fullDateTime);
                var request = new HttpRequestMessage();
                string purchaseID = Preferences.Get("purchId", "");
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
                        combinedArray.Add((m["meal_selection"].ToString()));
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
                        selectedDate.fillColor = Color.FromHex("#BBBBBB");
                    else if (isAlreadySelected2 == true && isSurprise2 == false)
                        d.fillColor = Color.FromHex("#FFBA00");
                    else d.fillColor = Color.White;
                    Console.WriteLine("isSurprise value: " + isSurprise2 + " isSkip value: " + isSkip2);


                }
            }
        }

        protected async Task GetRecentSelection()
        {
            Console.WriteLine("INSIDE GetRecentSelection #1");
            var request = new HttpRequestMessage();
            string purchaseID = Preferences.Get("purchId", "");
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
                    combinedArray.Add((m["meal_selection"].ToString()));
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
                    //selectedDate.fillColor = Color.FromHex("#C9C9C9");
                    selectedDate.fillColor = Color.FromHex("#BBBBBB");
                else if (isAlreadySelected == true && isSurprise == false)
                    selectedDate.fillColor = Color.FromHex("#FFBA00");
                else selectedDate.fillColor = Color.White;
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
        }

        protected async Task GetRecentSelection2()
        {
            Console.WriteLine("INSIDE GetRecentSelection #2");
            var request = new HttpRequestMessage();
            string purchaseID = Preferences.Get("purchId", "");
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
        }

        // Auto-complete
        public ObservableCollection<AddressAutocomplete> Addresses
        {
            get => _addresses ?? (_addresses = new ObservableCollection<AddressAutocomplete>());
            set
            {
                if (_addresses != value)
                {
                    _addresses = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _addressText;
        public string AddressText
        {
            get => _addressText;
            set
            {
                if (_addressText != value)
                {
                    _addressText = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task GetPlacesPredictionsAsync()
        {

            // TODO: Add throttle logic, Google begins denying requests if too many are made in a short amount of time

            CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token;

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(GooglePlacesApiAutoCompletePath, Constant.GooglePlacesApiKey, WebUtility.UrlEncode(_addressText))))
            { //Be sure to UrlEncode the search term they enter

                using (HttpResponseMessage message = await HttpClientInstance.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                {
                    if (message.IsSuccessStatusCode)
                    {
                        string json = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

                        PlacesLocationPredictions predictionList = await Task.Run(() => JsonConvert.DeserializeObject<PlacesLocationPredictions>(json)).ConfigureAwait(false);

                        if (predictionList.Status == "OK")
                        {

                            Addresses.Clear();

                            if (predictionList.Predictions.Count > 0)
                            {
                                int rows = 0;
                                foreach (Prediction prediction in predictionList.Predictions)
                                {
                                    if (rows > 2) break; // show maximum 3 addresses
                                    Console.WriteLine(prediction.Description);
                                    string[] predictionSplit = prediction.Description.Split(',');
                                    Addresses.Add(new AddressAutocomplete
                                    {
                                        Address = prediction.Description,
                                        Street = predictionSplit[0],
                                        City = predictionSplit[1],
                                        State = predictionSplit[2],
                                    });
                                    rows++;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(predictionList.Status);
                        }
                    }
                }
            }
        }

        private async void OnAddressChanged(object sender, EventArgs eventArgs)
        {
            if (!string.IsNullOrWhiteSpace(AddressText))
            {
                await GetPlacesPredictionsAsync();
            }
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            addressList.IsVisible = true;
            UnitCity.IsVisible = false;
            StateZip.IsVisible = false;
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            addressList.IsVisible = false;
            UnitCity.IsVisible = true;
            StateZip.IsVisible = true;
        }

        void addressSelected(System.Object sender, System.EventArgs e)
        {
            addressList.IsVisible = false;
            UnitCity.IsVisible = true;
            StateZip.IsVisible = true;

            AddressEntry.Text = ((AddressAutocomplete)addressList.SelectedItem).Street;
            CityEntry.Text = ((AddressAutocomplete)addressList.SelectedItem).City;
            StateEntry.Text = ((AddressAutocomplete)addressList.SelectedItem).State;
            ZipEntry.Text = ((AddressAutocomplete)addressList.SelectedItem).ZipCode;
        }

        //private void resetBttn_Clicked(object sender, EventArgs e)
        //{

        //    for (int i = 0; i < Meals1.Count; i++)
        //    {
        //        if (Meals1[i].MealQuantity > 0)
        //        {
        //            Meals1[i].MealQuantity = 0;
        //        }

        //    }

        //    int indexOfMealPlanSelected = (int)SubscriptionPicker.SelectedIndex;
        //    Preferences.Set("purchId", purchIdArray[indexOfMealPlanSelected].ToString());
        //    Console.WriteLine("Purch Id: " + Preferences.Get("purchId", ""));

        //    string s = SubscriptionPicker.SelectedItem.ToString();
        //    s = s.Substring(0, 2);
        //    Preferences.Set("total", int.Parse(s));
        //    totalCount.Text = Preferences.Get("total", 0).ToString();
        //    Preferences.Set("origMax", int.Parse(s));
        //}
    }
}
