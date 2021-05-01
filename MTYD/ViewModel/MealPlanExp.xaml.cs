using MTYD.Constants;
using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    public partial class MealPlanExp : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> userProfileInfo = new ObservableCollection<Plans>();
        public ObservableCollection<PaymentInfo> NewPlan = new ObservableCollection<PaymentInfo>();
        public static ObservableCollection<MealPlanItem> mealPlanColl = new ObservableCollection<MealPlanItem>();
        PaymentInfo orderInfo;
        ArrayList itemsArray = new ArrayList();
        ArrayList purchIdArray = new ArrayList();
        ArrayList namesArray = new ArrayList();
        ArrayList itemUidArray = new ArrayList();
        JObject info_obj;
        string frequency;
        int lastPickerIndex;
        int chosenIndex;
        bool planChangeCalled = false;
        public bool isAddessValidated = false;
        string chosenPurchUid;
        int currentIndex = -1;
        string currentPlan;
        List<JToken> activePlans = new List<JToken>();
        string addressToPass;
        string unitToPass;
        string cityToPass;
        string stateToPass;
        string zipToPass;
        Address addr;

        public MealPlanExp(string firstName, string lastName, string email)
        {
            mealPlanColl.Clear();
            info_obj = null;
            activePlans.Clear();
            itemsArray.Clear();
            purchIdArray.Clear();
            namesArray.Clear();
            itemUidArray.Clear();
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            addr = new Address();
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            getMealsSelected();
            _ = GetMealPlans();

            //if (namesArray.Count != 0)
            //    planPicker.SelectedIndex = 0;
        }

        public async void getFrequency()
        {
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/next_billing_date?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
            request.RequestUri = new Uri(url);
            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                Console.WriteLine("content: " + content);
                var userString = await content.ReadAsStringAsync();
                var freq_obj = JObject.Parse(userString);
                this.userProfileInfo.Clear();

                frequency = (freq_obj["result"])[planPicker.SelectedIndex]["payment_frequency"].ToString();
                Console.WriteLine("frequency: " + (freq_obj["result"])[planPicker.SelectedIndex]["payment_frequency"].ToString());
                if (frequency == "2")
                {
                    freq.Text = "2 WEEKS";
                    ticketPic.Source = "Discount5.png";
                }
                else if (frequency == "4")
                {
                    freq.Text = "4 WEEKS";
                    ticketPic.Source = "Discount10.png";
                }
                else
                {
                    freq.Text = "WEEKLY";
                    ticketPic.Source = "noDiscount.png";
                }
            }

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
                pfp.HeightRequest = width / 20;
                pfp.WidthRequest = width / 20;
                pfp.CornerRadius = (int)(width / 40);
                //pfp.Margin = new Thickness(0, 0, 23, 27);
                innerGrid.Margin = new Thickness(0, 0, 23, 27);


                if (Preferences.Get("profilePicLink", "") == "")
                {
                    string userInitials = "";
                    if (cust_firstName != "" && cust_firstName != null)
                    {
                        userInitials += cust_firstName.Substring(0, 1);
                    }
                    if (cust_lastName != "" && cust_lastName != null)
                    {
                        userInitials += cust_lastName.Substring(0, 1);
                    }
                    initials.Text = userInitials.ToUpper();
                    initials.FontSize = width / 38;
                }
                else pfp.Source = Preferences.Get("profilePicLink", "");

                menu.HeightRequest = width / 25;
                menu.WidthRequest = width / 25;
                menu.Margin = new Thickness(25, 0, 0, 30);

                mealPlanGrid.Margin = new Thickness(width / 40, 10, width / 40, 5);
                selectPlanFrame.Margin = new Thickness(10, 0, 0, 0);
                selectPlanFrame.Padding = new Thickness(15, 5);
                selectPlanFrame.HeightRequest = height / 55;
                planPicker.FontSize = width / 43;
                //planPicker.VerticalOptions = LayoutOptions.Fill;
                planPicker.HorizontalOptions = LayoutOptions.Fill;
                changeMealPlan.Margin = new Thickness(10, 0, 0, 0);
                changeMealPlan.FontSize = width / 40;
                changeMealPlan.HeightRequest = height / 45;
                changeMealPlan.CornerRadius = (int)height / 90;

                mainGrid.Margin = new Thickness(width / 50);
                mainFrame.CornerRadius = 20;
                innerStack.Margin = new Thickness(width / 100);
                delivery.FontSize = width / 38;
                saveInfo.CornerRadius = (int)(height / 80);
                saveInfo.FontSize = width / 38;

                FName.CornerRadius = 21;
                LName.CornerRadius = 21;
                emailAdd.CornerRadius = 21;
                street.CornerRadius = 21;
                unit.CornerRadius = 21;
                city.CornerRadius = 21;
                state.CornerRadius = 21;
                zipCode.CornerRadius = 21;
                phoneNum.CornerRadius = 21;
                FNameEntry.FontSize = width / 45;
                LNameEntry.FontSize = width / 45;
                emailEntry.FontSize = width / 45;
                AddressEntry.FontSize = width / 45;
                AptEntry.FontSize = width / 45;
                CityEntry.FontSize = width / 45;
                StateEntry.FontSize = width / 45;
                ZipEntry.FontSize = width / 45;
                PhoneEntry.FontSize = width / 45;
                //instructionsEntry.FontSize = width / 45;

                addressList.HeightRequest = width / 5;

                pay.FontSize = width / 38;

                card.FontSize = width / 55;
                cardPic.WidthRequest = width / 10;
                cardNum.FontSize = width / 70;

                freq.FontSize = width / 55;
                ticketPic.WidthRequest = width / 10;
                ticketPic.HeightRequest = width / 10;
            }
            else //android
            {

            }
        }

        //auto-populate the delivery info if the user has already previously entered it
        public async void getMealsSelected()
        {
            Console.WriteLine("fillEntries entered");
            var request = new HttpRequestMessage();
            Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
            string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
            //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
            request.RequestUri = new Uri(url);
            //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                Console.WriteLine("content: " + content);
                var userString = await content.ReadAsStringAsync();

                if (userString.ToString()[0] != '{')
                {
                    Console.WriteLine("no meal plans");
                    Preferences.Set("canChooseSelect", false);
                    return;
                }

                info_obj = JObject.Parse(userString);
                this.userProfileInfo.Clear();
                //Console.WriteLine("info_obj: " + info_obj);

                while (info_obj == null)
                    await Task.Delay(100);
            }
        }

        private async void planChange(object sender, EventArgs e)
        {
            Console.WriteLine("planChange entered");
            planChangeCalled = true;
            selectPlanFrame.BackgroundColor = Color.FromHex("#FF6505");
            coverPickerBorder.BorderColor = Color.FromHex("#FF6505");
            planPicker.TextColor = Color.White;
            planPicker.BackgroundColor = Color.FromHex("#FF6505");

            Console.WriteLine("before frequency " + frequency);
            getFrequency();

            Console.WriteLine("after frequency " + frequency);

            if (info_obj == null)
            {
                while (info_obj == null)
                    await Task.Delay(100);

                if (info_obj != null && (info_obj["result"]).ToString() == "[]")
                {
                    return;
                }
            }
            else
            {
                if ((info_obj["result"]).ToString() == "[]")
                {
                    return;
                }
            }
            //if ((info_obj["result"]).ToString() == "[]")
            //{
            //    return;
            //}

            //old
            //chosenPurchUid = (info_obj["result"])[planPicker.SelectedIndex]["purchase_uid"].ToString();
            chosenPurchUid = purchIdArray[planPicker.SelectedIndex].ToString();
            //chosenPurchUid = purchIdArray[PlanCollectionView.].ToString();
            Debug.WriteLine("selected chosen purch id in plan change: " + chosenPurchUid.ToString());

            //currentIndex = planPicker.SelectedIndex;
            //Debug.WriteLine("current index: " + currentIndex.ToString());
            string tet = planPicker.SelectedItem.ToString();
            Debug.WriteLine("picker text: " + tet);
            currentPlan = planPicker.SelectedItem.ToString();
            currentIndex = planPicker.SelectedIndex;

            FNameEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_first_name"].ToString();

            LNameEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_last_name"].ToString();
            emailEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_email"].ToString();
            AddressEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_address"].ToString();
            AptEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_unit"].ToString();

            if (AptEntry.Text == "NULL")
            {
                AptEntry.Text = "";
            }

            CityEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_city"].ToString();
            StateEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_state"].ToString();
            ZipEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_zip"].ToString();
            PhoneEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_phone_num"].ToString();
            //instructionsEntry.Text = (info_obj["result"])[planPicker.SelectedIndex]["delivery_instructions"].ToString();

            string creditCardNum = (info_obj["result"])[planPicker.SelectedIndex]["cc_num"].ToString();
            cardNum.Text = creditCardNum.Substring(creditCardNum.Length - 2);
            cardNum.Text = "**************" + cardNum.Text;

            string itemsStr = (info_obj["result"])[planPicker.SelectedIndex]["items"].ToString();
            Console.WriteLine("items: " + itemsStr);
            Console.WriteLine("name: " + itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10));
            Console.WriteLine("item_uid: " + itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10));
            //Console.WriteLine("name: " + )

            //orderInfo.customer_uid = (info_obj["result"])[planPicker.SelectedIndex]["customer_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //Console.WriteLine("items" + ((info_obj["result"])[planPicker.SelectedIndex]["items"]).ToString());
            //orderInfo.salt = (info_obj["result"])[planPicker.SelectedIndex]["salt"].ToString();
            //orderInfo.delivery_first_name = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //orderInfo.business_uid = (info_obj["result"])[planPicker.SelectedIndex]["business_uid"].ToString();
            //    public string customer_uid { get; set; }
            //public string business_uid { get; set; }
            //public List<Item> items { get; set; }
            //public string salt { get; set; }
            //public string delivery_first_name { get; set; }
            //public string delivery_last_name { get; set; }
            //public string delivery_email { get; set; }
            //public string delivery_phone { get; set; }
            //public string delivery_address { get; set; }
            //public string delivery_unit { get; set; }
            //public string delivery_city { get; set; }
            //public string delivery_state { get; set; }
            //public string delivery_zip { get; set; }
            //public string delivery_instructions { get; set; }
            //public string delivery_longitude { get; set; }
            //public string delivery_latitude { get; set; }
            //public string order_instructions { get; set; }
            //public string purchase_notes { get; set; }
            //public string amount_due { get; set; }
            //public string amount_discount { get; set; }
            //public string amount_paid { get; set; }
            //public string cc_num { get; set; }
            //public string cc_exp_year { get; set; }
            //public string cc_exp_month { get; set; }
            //public string cc_cvv { get; set; }
            //public string cc_zip { get; set; }


        }

        protected async Task GetMealPlans()
        {
            Console.WriteLine("ENTER GET MEAL PLANS FUNCTION");
            var request = new HttpRequestMessage();
            string userID = (string)Application.Current.Properties["user_id"];
            Console.WriteLine("Inside GET MEAL PLANS: User ID:  " + userID);

            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            Console.WriteLine("GET MEALS PLAN ENDPOINT TRYING TO BE REACHED: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/customer_lplp?customer_uid=" + userID);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();

                if (userString.ToString()[0] != '{')
                {
                    Console.WriteLine("no meal plans");
                    Preferences.Set("canChooseSelect", false);
                    return;
                }

                JObject mealPlan_obj = JObject.Parse(userString);
                this.NewPlan.Clear();

                Console.WriteLine("itemsArray contents:");

                foreach (var m in mealPlan_obj["result"])
                {
                    Console.WriteLine("In first foreach loop of getmeal plans func:");

                    if (m["purchase_status"].ToString() == "ACTIVE")
                    {
                        itemsArray.Add((m["items"].ToString()));
                        purchIdArray.Add((m["purchase_uid"].ToString()));
                        activePlans.Add(m);
                    }
                    else Debug.WriteLine(m["purchase_uid"].ToString() + " was skipped");
                }

                if (purchIdArray.Count == 0 || activePlans.Count == 0)
                {
                    Preferences.Set("canChooseSelect", false);
                }

                lastPickerIndex = purchIdArray.Count - 1;

                Console.WriteLine("size of purchIdArray: " + purchIdArray.Count.ToString());
                for (int i = 0; i < purchIdArray.Count; i++)
                {
                    Console.WriteLine("purchId " + i + ": " + purchIdArray[i]);
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
                        //string price = (string)config["price"];
                        //string mealid = (string)config["item_uid"];
                        name = name.Substring(0, name.IndexOf(" "));
                        name = name + " Meals, ";
                        qty = qty + " Deliveries";
                        //string price = (string)config["price"];
                        //string mealid = (string)config["item_uid"];
                        string purchIdCurrent = purchIdArray[i].ToString().Substring(4);
                        //while (purchIdCurrent.Substring(0, 1) == "0")
                        //    purchIdCurrent = purchIdCurrent.Substring(1);

                        //only includes meal plan name
                        //namesArray.Add(name);

                        //adds purchase uid to front of meal plan name
                        //namesArray.Add(purchIdArray[i].ToString().Substring(4) + " : " + name);
                        namesArray.Add(name + qty + " : " + purchIdCurrent);
                        //only includes meal plan name
                        //namesArray.Add(name);
                        mealPlanColl.Add(
                            new MealPlanItem
                            {
                                Background = Color.White,
                                FontColor = Color.Black,
                                PlanName = name + qty + " : " + purchIdCurrent
                            }
                        );
                        //adds purchase uid to front of meal plan name
                        //namesArray.Add(purchIdArray[i].ToString().Substring(4) + " : " + name);
                        namesArray.Add(name + " : " + purchIdArray[i].ToString().Substring(4));

                        string mealid = (string)config["item_uid"];
                        itemUidArray.Add(mealid);
                    }
                }
                Console.WriteLine("Outside foreach in GetmealsPlan func");
                //Find unique number of meals
                //firstIndex = namesArray[0].ToString();
                //Console.WriteLine("namesArray contents:" + namesArray[0].ToString() + " " + namesArray[1].ToString() + " " + namesArray[2].ToString() + " ");
                planPicker.ItemsSource = namesArray;
                PlanCollectionView.ItemsSource = mealPlanColl;
                Console.WriteLine("namesArray contents:" + namesArray[0].ToString());
                //SubscriptionPicker.Title = namesArray[0];

                if (namesArray.Count != 0)
                    planPicker.SelectedIndex = 0;

                Console.WriteLine("END OF GET MEAL PLANS FUNCTION");
            }
        }

        async void clickedSub(System.Object sender, System.EventArgs e)
        {
            if (planChangeCalled == false)
            {
                DisplayAlert("Invalid Selection", "Please select a meal plan first.", "OK");
                return;
            }

            string itemsStr = activePlans[planPicker.SelectedIndex]["items"].ToString();
            string qty = itemsStr.Substring(itemsStr.IndexOf("qty") + 7);
            qty = qty.Substring(0, qty.IndexOf("\""));
            string numMeal = itemsStr.Substring(itemsStr.IndexOf("name") + 8);
            numMeal = numMeal.Substring(0, numMeal.IndexOf(" "));
            Debug.WriteLine("qty: " + qty);
            string expDate = activePlans[planPicker.SelectedIndex]["cc_exp_date"].ToString();
            //var testing = (info_obj["result"])[1];
            //string zip = testing["cc_zip"].ToString();

            await Navigation.PushAsync(new SubscriptionModal(cust_firstName, cust_lastName, cust_email, activePlans[planPicker.SelectedIndex]["user_social_media"].ToString(), activePlans[planPicker.SelectedIndex]["mobile_refresh_token"].ToString(), activePlans[planPicker.SelectedIndex]["cc_num"].ToString(),
                expDate.Substring(0, 10),
                activePlans[planPicker.SelectedIndex]["cc_cvv"].ToString(), activePlans[planPicker.SelectedIndex]["cc_zip"].ToString(), activePlans[planPicker.SelectedIndex]["purchase_id"].ToString(), activePlans[planPicker.SelectedIndex]["purchase_uid"].ToString(), itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10),
                itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10), activePlans[planPicker.SelectedIndex]["pur_customer_uid"].ToString(), qty, numMeal, AddressEntry.Text, AptEntry.Text, CityEntry.Text, StateEntry.Text, ZipEntry.Text, activePlans[planPicker.SelectedIndex]["delivery_instructions"].ToString(), activePlans[planPicker.SelectedIndex]["start_delivery_date"].ToString(), activePlans[planPicker.SelectedIndex]["delivery_phone_num"].ToString()), false);
        }

        async void clickedInfo(System.Object sender, System.EventArgs e)
        {
            if (planChangeCalled == false)
            {
                DisplayAlert("Invalid Selection", "Please select a meal plan first.", "OK");
                return;
            }

            string itemsStr = activePlans[planPicker.SelectedIndex]["items"].ToString();
            string expDate = activePlans[planPicker.SelectedIndex]["cc_exp_date"].ToString();
            Console.WriteLine("clickedInfo exp date: " + expDate);
            string mealPlan;
            int lengthOfPrice = itemsStr.IndexOf("item_uid") - itemsStr.IndexOf("price") - 13;

            if (Int32.Parse(itemsStr.Substring(itemsStr.IndexOf("name") + 8, 2)) == 5)
            {
                mealPlan = itemsStr.Substring(itemsStr.IndexOf("name") + 8, 11);
            }
            else mealPlan = itemsStr.Substring(itemsStr.IndexOf("name") + 8, 12);



            await Navigation.PushAsync(new OrderInfoModal(cust_firstName, cust_lastName, cust_email, "", activePlans[planPicker.SelectedIndex]["mobile_refresh_token"].ToString(), activePlans[planPicker.SelectedIndex]["cc_num"].ToString(),
                expDate.Substring(0, 4), expDate.Substring(5, 2),
                activePlans[planPicker.SelectedIndex]["cc_cvv"].ToString(), activePlans[planPicker.SelectedIndex]["cc_zip"].ToString(), activePlans[planPicker.SelectedIndex]["purchase_uid"].ToString(), itemsStr.Substring(itemsStr.IndexOf("itm_business_uid") + 20, 10),
                mealPlan, itemsStr.Substring(itemsStr.IndexOf("price") + 9, lengthOfPrice), itemsStr.Substring(itemsStr.IndexOf("item_uid") + 12, 10), activePlans[planPicker.SelectedIndex]["pur_customer_uid"].ToString()), false);
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        void LogOutClick(System.Object sender, System.EventArgs e)
        {
            Application.Current.Properties.Remove("user_id");
            Application.Current.Properties.Remove("time_stamp");
            Application.Current.Properties.Remove("platform");
            Application.Current.MainPage = new MainPage();
        }

        async void clickedSave(System.Object sender, System.EventArgs e)
        {
            if (planChangeCalled == false)
            {
                DisplayAlert("Invalid Selection", "Please select a meal plan first.", "OK");
                return;
            }

            //------------------------validate address-----------------------------//

            if (AddressEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your address", "OK");
            }

            if (CityEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your city", "OK");
            }

            if (StateEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your state", "OK");
            }

            if (ZipEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your zipcode", "OK");
            }

            //if (PhoneEntry.Text == null && PhoneEntry.Text.Length == 10)
            //{
            //    await DisplayAlert("Error", "Please enter your phone number", "OK");
            //}

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

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

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
                await DisplayAlert("We couldn't find your address", "Please check for errors.", "Ok");
            }
            else
            {
                int startIndex = xdoc.ToString().IndexOf("<Address2>") + 10;
                int length = xdoc.ToString().IndexOf("</Address2>") - startIndex;

                string xdocAddress = xdoc.ToString().Substring(startIndex, length);
                //Console.WriteLine("xdoc address: " + xdoc.ToString().Substring(startIndex, length));
                //Console.WriteLine("xdoc end");

                if (xdocAddress != AddressEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing address", "ok");
                    AddressEntry.Text = xdocAddress;
                }

                startIndex = xdoc.ToString().IndexOf("<State>") + 7;
                length = xdoc.ToString().IndexOf("</State>") - startIndex;
                string xdocState = xdoc.ToString().Substring(startIndex, length);

                if (xdocAddress != StateEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing state", "ok");
                    StateEntry.Text = xdocState;
                }

                isAddessValidated = true;
                await DisplayAlert("We validated your address", "Please click on the Sign up button to create your account!", "OK");
                await Application.Current.SavePropertiesAsync();
                //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());
            }

            //----------------------end validate address---------------------------//

            //    public string customer_uid { get; set; }
            //public string business_uid { get; set; }
            //public string salt { get; set; }
            //public string delivery_first_name { get; set; }
            //public string delivery_last_name { get; set; }
            //public string delivery_email { get; set; }
            //public string delivery_phone { get; set; }
            //public string delivery_address { get; set; }
            //public string delivery_unit { get; set; }
            //public string delivery_city { get; set; }
            //public string delivery_state { get; set; }
            //public string delivery_zip { get; set; }
            //public string delivery_instructions { get; set; }
            DeliveryInfo delivery = new DeliveryInfo();

            delivery.purchase_uid = (info_obj["result"])[planPicker.SelectedIndex]["purchase_uid"].ToString();
            delivery.first_name = FNameEntry.Text;
            delivery.last_name = LNameEntry.Text;
            delivery.email = emailEntry.Text;
            delivery.phone = PhoneEntry.Text;
            delivery.address = AddressEntry.Text;
            //delivery.unit = AptEntry.Text;
            delivery.unit = AptEntry.Text;
            delivery.city = CityEntry.Text;
            delivery.state = StateEntry.Text;
            delivery.zip = ZipEntry.Text;

            chosenIndex = planPicker.SelectedIndex;

            var newPaymentJSONString = JsonConvert.SerializeObject(delivery);
            // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
            var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
            Console.WriteLine("Content: " + content2);
            /*var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
            request.Method = HttpMethod.Post;
            request.Content = content;*/
            var client = new HttpClient();
            var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Update_Delivery_Info_Address", content2);
            // HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
            Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
            Console.WriteLine("clickedDone Func ENDED!");

            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
        }

        async void ValidateAddressClick(object sender, System.EventArgs e)
        {

            if (AddressEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your address", "OK");
            }

            if (CityEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your city", "OK");
            }

            if (StateEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your state", "OK");
            }

            if (ZipEntry.Text == null)
            {
                await DisplayAlert("Error", "Please enter your zipcode", "OK");
            }

            //if (PhoneEntry.Text == null && PhoneEntry.Text.Length == 10)
            //{
            //    await DisplayAlert("Error", "Please enter your phone number", "OK");
            //}

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
            var client = new WebClient();
            var response = client.DownloadString(url);

            var xdoc = XDocument.Parse(response.ToString());
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

                        IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(AddressEntry.Text.Trim() + "," + CityEntry.Text.Trim() + "," + StateEntry.Text.Trim());
                        Position position = approximateLocations.FirstOrDefault();

                        latitude = $"{position.Latitude}";
                        longitude = $"{position.Longitude}";

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
                await DisplayAlert("We couldn't find your address", "Please check for errors.", "Ok");
            }
            else
            {
                int startIndex = xdoc.ToString().IndexOf("<Address2>") + 10;
                int length = xdoc.ToString().IndexOf("</Address2>") - startIndex;

                string xdocAddress = xdoc.ToString().Substring(startIndex, length);
                //Console.WriteLine("xdoc address: " + xdoc.ToString().Substring(startIndex, length));
                //Console.WriteLine("xdoc end");

                if (xdocAddress != AddressEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing address", "ok");
                    AddressEntry.Text = xdocAddress;
                }

                startIndex = xdoc.ToString().IndexOf("<State>") + 7;
                length = xdoc.ToString().IndexOf("</State>") - startIndex;
                string xdocState = xdoc.ToString().Substring(startIndex, length);

                if (xdocAddress != StateEntry.Text.ToUpper().Trim())
                {
                    //DisplayAlert("heading", "changing state", "ok");
                    StateEntry.Text = xdocState;
                }

                isAddessValidated = true;
                await DisplayAlert("We validated your address", "Please click on the Sign up button to create your account!", "OK");
                await Application.Current.SavePropertiesAsync();
                //await tagUser(emailEntry.Text.Trim(), ZipEntry.Text.Trim());
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

        async void deleteClicked(object sender, System.EventArgs e)
        {
            var client = new HttpClient();
            string refundAmount = "";

            if (chosenPurchUid != null && chosenPurchUid != "" && currentIndex != -1)
            {
                //get the amount that will be refunded
                var request2 = new HttpRequestMessage();
                Debug.WriteLine("trying to delete: " + chosenPurchUid.ToString());

                //sample (get) endpoint: https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/400-000209
                /*
                sample output: {
                                    "week_remaining": 2,
                                    "refund_amount": 19.68
                                }
                */


                //request2.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/refund_calculator?purchase_uid=" + chosenPurchUid);
                request2.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + chosenPurchUid);
                request2.Method = HttpMethod.Get;
                var client2 = new HttpClient();
                HttpResponseMessage response2 = await client2.SendAsync(request2);
                Debug.WriteLine("response from refund calc: " + response2.ToString());
                if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content2 = response2.Content;
                    var userString2 = await content2.ReadAsStringAsync();
                    JObject refund_obj = JObject.Parse(userString2);

                    Debug.WriteLine("first start" + refund_obj.ToString());
                    Debug.WriteLine("this is what I'm getting: " + refund_obj["refund_amount"].ToString());
                    refundAmount = refund_obj["refund_amount"].ToString();

                }

                bool answer = await DisplayAlert("Delete a Plan", "Are you sure you want to delete this " + currentPlan + "? If yes, you will be refunded $" + refundAmount + ".", "Yes", "No");
                Debug.WriteLine("Answer: " + answer);

                if (answer == true)
                {
                    CancelPlanPost willDelete = new CancelPlanPost();
                    willDelete.purchase_uid = chosenPurchUid;

                    var deleteSerializedObject = JsonConvert.SerializeObject(willDelete);
                    Debug.WriteLine("delete JSON Object to send: " + deleteSerializedObject);

                    var deleteContent = new StringContent(deleteSerializedObject, Encoding.UTF8, "application/json");

                    var clientResponse = await client.PutAsync(Constant.DeletePlanUrl, deleteContent);

                    Debug.WriteLine("Status code from deleting plan: " + clientResponse);
                    //await DisplayAlert("Deleted Plan", currentPlan + " was cancelled and refunded.", "OK");

                    await Navigation.PushAsync(new MealPlans(cust_firstName, cust_lastName, cust_email), false);
                    Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
                }
            }


        }

        // Auto-complete
        private ObservableCollection<AddressAutocomplete> _addresses;
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

        private async void OnAddressChanged(object sender, EventArgs eventArgs)
        {
            addressList.ItemsSource = await addr.GetPlacesPredictionsAsync(AddressEntry.Text);
            //addr.OnAddressChanged(addressList, Addresses, _addressText);
        }

        private void addressEntryFocused(object sender, EventArgs eventArgs)
        {
            addr.addressEntryFocused(addressList, new Grid[] { UnitCityState, ZipPhone });
        }

        private void addressEntryUnfocused(object sender, EventArgs eventArgs)
        {
            addr.addressEntryUnfocused(addressList, new Grid[] { UnitCityState, ZipPhone });
        }

        async void addressSelected(System.Object sender, System.EventArgs e)
        {
            addr.addressSelected(addressList, new Grid[] { UnitCityState, ZipPhone }, AddressEntry, CityEntry, StateEntry, ZipEntry);

        }
    }
}
