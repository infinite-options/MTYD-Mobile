using MTYD.Constants;
using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
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


    public partial class SubscriptionModal : ContentPage
    {
        // CREDENTIALS CLASS
        public class Credentials
        {
            public string key { get; set; }
        }

        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewPlan = new ObservableCollection<Plans>();

        double m1price_f1 = 0.0; double m1price_f2 = 0.0; double m1price_f3 = 0.0; double m2price_f1 = 0.0; double m2price_f2 = 0.0; double m2price_f3 = 0.0;
        double m3price_f1 = 0.0; double m3price_f2 = 0.0; double m3price_f3 = 0.0; double m4price_f1 = 0.0; double m4price_f2 = 0.0; double m4price_f3 = 0.0;
        string m1f1name = "", m1f2name = "", m1f3name = "", m2f1name = "", m2f2name = "", m2f3name = "", m3f1name = "", m3f2name = "", m3f3name = "", m4f1name = "", m4f2name = "", m4f3name = "";
        string m1f1uid = "", m1f2uid = "", m1f3uid = "", m2f1uid = "", m2f2uid = "", m2f3uid = "", m3f1uid = "", m3f2uid = "", m3f3uid = "", m4f1uid = "", m4f2uid = "", m4f3uid = "";

        string socialLogin; string refresh_token; string cc_num; string cc_exp_year; string cc_exp_month; string cc_cvv; string purchase_id, purchase_uid;
        string new_item_id; string customer_id; string itm_business_uid; string cc_zip; string cc_exp_date; string qty; string numMeal;
        string passedAdd, passedUnit, passedCity, passedState, passedZip, phoneNum;
        string chargeId = ""; string delivInstructions, startDeliv;
        string updatedDiscount, updatedDue, updatedTax, updatedTip, updatedService, updatedDelivery, updatedSub, updatedAmb;
        string lati, longi;

        int mealSelected;
        int deliverySelected;
        double[,] discounts;
        double[,] itemPrices;
        String[,] itemNames, itemUids;
        double total;
        WebClient client4 = new WebClient();
        double taxRate = 0;
        double extraCharge = 0;
        bool shouldCharge = false;

        public object NavigationStack { get; private set; }


        public SubscriptionModal(string firstName, string lastName, string email, string social, string token, string num, string expDate, string cvv, string zip, string purchaseID, string purchaseUID, string businessID, string itemID, string customerID, string quantity, string numOfMeals, string add, string unit, string city, string state, string zipDeliv, string delivInstr, string startDeliveryDate, string phoneNumber)
        {
            //purchase id is used for charges, purchase uid is used for refunds (change_purchase)
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            Console.WriteLine("SubscriptionModal entered");
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            Console.WriteLine("next entered");
            socialLogin = social; refresh_token = token; cc_num = num; cc_exp_date = expDate; cc_cvv = cvv; purchase_id = purchaseID; purchase_uid = purchaseUID;
            new_item_id = itemID; customer_id = customerID; cc_zip = zip; itm_business_uid = businessID; qty = quantity; numMeal = numOfMeals;
            passedAdd = add.Trim(); passedUnit = unit.Trim(); passedCity = city.Trim(); passedState = state.Trim(); passedZip = zipDeliv.Trim(); delivInstructions = delivInstr;
            startDeliv = startDeliveryDate; phoneNum = phoneNumber;
            Debug.WriteLine("new_item_id: " + new_item_id);
            Debug.WriteLine("deliv instructions passed in: " + delivInstr);
            Debug.WriteLine("exp date passed: " + cc_exp_date);
            Debug.WriteLine("exp year: " + cc_exp_date.Substring(cc_exp_date.IndexOf("-") - 2, 2));
            cc_exp_year = cc_exp_date.Substring(cc_exp_date.IndexOf("-") - 2, 2);
            cc_exp_month = cc_exp_date.Substring(cc_exp_date.IndexOf("-") + 1, 2);
            Debug.WriteLine("exp month: " + cc_exp_date.Substring(cc_exp_date.IndexOf("-") + 1, 2));
            Debug.WriteLine("Address passed in: " + passedAdd + ", unit: " + passedUnit + ", " + passedCity + ", " + passedState + " " + passedZip);

            Console.WriteLine("next2 entered");

            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            GetPlans();
            Preferences.Set("freqSelected", "");
        }


        protected async Task GetPlans()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/plans?business_uid=200-000002");
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                HttpContent content = response.Content;
                var userString = await content.ReadAsStringAsync();
                JObject plan_obj = JObject.Parse(userString);
                this.NewPlan.Clear();

                ArrayList numMealsList = new ArrayList();
                int i = 0, j = 0;
                discounts = new double[10, 5];
                itemPrices = new double[10, 5];
                itemNames = new String[10, 5];
                itemUids = new String[10, 5];
                foreach (var m in plan_obj["result"])
                {
                    int num_meals = int.Parse(m["num_items"].ToString());
                    if (!numMealsList.Contains(num_meals))
                    {
                        numMealsList.Add(num_meals);
                    }
                    discounts[i, j] = double.Parse(m["delivery_discount"].ToString());
                    itemPrices[i, j] = double.Parse(m["item_price"].ToString());
                    itemNames[i, j] = m["item_name"].ToString();
                    itemUids[i, j] = m["item_uid"].ToString();
                    Debug.WriteLine("received item_uid: " + m["item_uid"].ToString());
                    Debug.WriteLine("item name: " + m["item_name"].ToString());
                    
                    if (j == 4)
                    {
                        i++;
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                }

                meals1Text.Text = numMealsList[4].ToString();
                meals2Text.Text = numMealsList[3].ToString();
                meals3Text.Text = numMealsList[2].ToString();
                meals4Text.Text = numMealsList[1].ToString();
                meals5Text.Text = numMealsList[0].ToString();

                currentMealNum.Text = numMeal;
                currentDelivNum.Text = qty;
                EventArgs ev = new EventArgs();
                if (qty == "1")
                {
                    clickedDeliveryNum(delivery1, ev);
                    currentDiscount.Text = "";
                }
                else if (qty == "2")
                {
                    clickedDeliveryNum(delivery2, ev);
                    currentDiscount.Text = "Save 1%";
                }
                else if (qty == "3")
                {
                    clickedDeliveryNum(delivery3, ev);
                    currentDiscount.Text = "Save 2%";
                }
                else if (qty == "4")
                {
                    clickedDeliveryNum(delivery4, ev);
                    currentDiscount.Text = "Save 5%";
                }
                else if (qty == "5")
                {
                    clickedDeliveryNum(delivery5, ev);
                    currentDiscount.Text = "Save 6%";
                }
                else if (qty == "6")
                {
                    clickedDeliveryNum(delivery6, ev);
                    currentDiscount.Text = "Save 7%";
                }
                else if (qty == "7")
                {
                    clickedDeliveryNum(delivery7, ev);
                    currentDiscount.Text = "Save 8%";
                }
                else if (qty == "8")
                {
                    clickedDeliveryNum(delivery8, ev);
                    currentDiscount.Text = "Save 12%";
                }
                else if (qty == "9")
                {
                    clickedDeliveryNum(delivery9, ev);
                    currentDiscount.Text = "Save 13%";
                }
                else
                {
                    clickedDeliveryNum(delivery10, ev);
                    currentDiscount.Text = "Save 15%";
                }

                if (numMeal == "2")
                    clickedMeals1(meals1, ev);
                else if (numMeal == "3")
                    clickedMeals2(meals2, ev);
                else if (numMeal == "4")
                    clickedMeals3(meals3, ev);
                else if (numMeal == "5")
                    clickedMeals4(meals4, ev);
                else clickedMeals5(meals5, ev);
            }
        }


        void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                //open menu adjustments
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = width / 5;
                menu2.HeightRequest = width / 25;
                menu2.WidthRequest = width / 25;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                heading.WidthRequest = width / 5;
                //heading adjustments

                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                //heading.FontSize = width / 32;
                //heading.Margin = new Thickness(0, 0, 0, 30);
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
            }
            else //android
            {
                //open menu adjustments
                orangeBox2.HeightRequest = height / 2;
                orangeBox2.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox2.CornerRadius = height / 40;
                heading2.WidthRequest = width / 5;
                menu2.HeightRequest = width / 25;
                menu2.WidthRequest = width / 25;
                menu2.Margin = new Thickness(25, 0, 0, 30);
                heading.WidthRequest = width / 5;
                //heading adjustments

                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                //heading.FontSize = width / 45;
                //heading.Margin = new Thickness(0, 0, 0, 40);
                pfp.HeightRequest = width / 25;
                pfp.WidthRequest = width / 25;
                pfp.CornerRadius = (int)(width / 50);
                pfp.Margin = new Thickness(0, 0, 23, 35);
                menu.HeightRequest = width / 30;
                menu.WidthRequest = width / 30;
                menu.Margin = new Thickness(25, 0, 0, 40);
            }

            //common adjustments regardless of platform
        }

        private void clickedMeals1(object sender, EventArgs e)
        {
            meals1.Source = "meal_num_button_orange.png";
            meals2.Source = "meal_num_button_yellow.png";
            meals3.Source = "meal_num_button_yellow.png";
            meals4.Source = "meal_num_button_yellow.png";
            meals5.Source = "meal_num_button_yellow.png";
            Preferences.Set("mealSelected", "1");
            mealSelected = int.Parse(meals1Text.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                //double basePrice_dub = itemPrices[deliverySelected - 1, 6 - mealSelected];
                double discountAmt_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice_dub);
                Preferences.Set("discountAmt", discountAmt_dub);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals2(object sender, EventArgs e)
        {
            meals1.Source = "meal_num_button_yellow.png";
            meals2.Source = "meal_num_button_orange.png";
            meals3.Source = "meal_num_button_yellow.png";
            meals4.Source = "meal_num_button_yellow.png";
            meals5.Source = "meal_num_button_yellow.png";
            Preferences.Set("mealSelected", "2");
            mealSelected = int.Parse(meals2Text.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            meals1.Source = "meal_num_button_yellow.png";
            meals2.Source = "meal_num_button_yellow.png";
            meals3.Source = "meal_num_button_orange.png";
            meals4.Source = "meal_num_button_yellow.png";
            meals5.Source = "meal_num_button_yellow.png";
            Preferences.Set("mealSelected", "3");
            mealSelected = int.Parse(meals3Text.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            meals1.Source = "meal_num_button_yellow.png";
            meals2.Source = "meal_num_button_yellow.png";
            meals3.Source = "meal_num_button_yellow.png";
            meals4.Source = "meal_num_button_orange.png";
            meals5.Source = "meal_num_button_yellow.png";
            Preferences.Set("mealSelected", "4");
            mealSelected = int.Parse(meals4Text.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedMeals5(object sender, EventArgs e)
        {
            meals1.Source = "meal_num_button_yellow.png";
            meals2.Source = "meal_num_button_yellow.png";
            meals3.Source = "meal_num_button_yellow.png";
            meals4.Source = "meal_num_button_yellow.png";
            meals5.Source = "meal_num_button_orange.png";
            Preferences.Set("mealSelected", "5");
            mealSelected = int.Parse(meals5Text.Text.Substring(0, 1));
            if (deliverySelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
            SignUpButton.Text = "CHECK PRICE";
        }

        private void clickedDeliveryNum(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            delivery1.BorderWidth = 0;
            delivery2.BorderWidth = 0;
            delivery3.BorderWidth = 0;
            delivery4.BorderWidth = 0;
            delivery5.BorderWidth = 0;
            delivery6.BorderWidth = 0;
            delivery7.BorderWidth = 0;
            delivery8.BorderWidth = 0;
            delivery9.BorderWidth = 0;
            delivery10.BorderWidth = 0;

            if (btn.Equals(delivery1))
            {
                delivery1.BorderWidth = 2;
                Preferences.Set("freqSelected", "1");
                deliverySelected = 1;
            }
            else if (btn.Equals(delivery2))
            {
                delivery2.BorderWidth = 2;
                Preferences.Set("freqSelected", "2");
                deliverySelected = 2;
            }
            else if (btn.Equals(delivery3))
            {
                delivery3.BorderWidth = 2;
                Preferences.Set("freqSelected", "3");
                deliverySelected = 3;
            }
            else if (btn.Equals(delivery4))
            {
                delivery4.BorderWidth = 2;
                Preferences.Set("freqSelected", "4");
                deliverySelected = 4;
            }
            else if (btn.Equals(delivery5))
            {
                delivery5.BorderWidth = 2;
                Preferences.Set("freqSelected", "5");
                deliverySelected = 5;
            }
            else if (btn.Equals(delivery6))
            {
                delivery6.BorderWidth = 2;
                Preferences.Set("freqSelected", "6");
                deliverySelected = 6;
            }
            else if (btn.Equals(delivery7))
            {
                delivery7.BorderWidth = 2;
                Preferences.Set("freqSelected", "7");
                deliverySelected = 7;
            }
            else if (btn.Equals(delivery8))
            {
                delivery8.BorderWidth = 2;
                Preferences.Set("freqSelected", "8");
                deliverySelected = 8;
            }
            else if (btn.Equals(delivery9))
            {
                delivery9.BorderWidth = 2;
                Preferences.Set("freqSelected", "9");
                deliverySelected = 9;
            }
            else if (btn.Equals(delivery10))
            {
                delivery10.BorderWidth = 2;
                Preferences.Set("freqSelected", "10");
                deliverySelected = 10;
            }
            else
            {
                Preferences.Set("freqSelected", "0");
                deliverySelected = 0;
            }
            if (mealSelected != 0)
            {
                Preferences.Set("item_name", itemNames[deliverySelected - 1, 6 - mealSelected]);
                Preferences.Set("item_uid", itemUids[deliverySelected - 1, 6 - mealSelected]);
                double basePrice = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                double itemPrice = itemPrices[0, 6 - mealSelected];
                double discountAmt = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                total = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * (1 - discounts[deliverySelected - 1, 6 - mealSelected] / 100.0);
                Preferences.Set("basePrice", basePrice);
                Preferences.Set("discountAmt", discountAmt);
                Preferences.Set("itemPrice", itemPrice);
                Discount.Text = discounts[deliverySelected - 1, 6 - mealSelected].ToString() + "%";
            }
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            if (total == 0)
            {
                await DisplayAlert("Warning!", "pick a valid plan to continue", "OK");
                return;
            }

            if (SignUpButton.Text == "CHECK PRICE")
            {
                // Setting request for USPS API
                XDocument requestDoc = new XDocument(
                    new XElement("AddressValidateRequest",
                    new XAttribute("USERID", "400INFIN1745"),
                    new XElement("Revision", "1"),
                    new XElement("Address",
                    new XAttribute("ID", "0"),
                    new XElement("Address1", passedAdd),
                    new XElement("Address2", passedUnit),
                    new XElement("City", passedCity),
                    new XElement("State", passedState),
                    new XElement("Zip5", passedZip),
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
                        if (GetXMLElement(element, "DPVConfirmation").Equals("Y") && GetXMLElement(element, "Zip5").Equals(passedZip) && GetXMLElement(element, "City").Equals(passedCity.ToUpper())) // Best case
                        {
                            // Get longitude and latitide because we can make a deliver here. Move on to next page.
                            // Console.WriteLine("The address you entered is valid and deliverable by USPS. We are going to get its latitude & longitude");
                            //GetAddressLatitudeLongitude();
                            Geocoder geoCoder = new Geocoder();

                            //Debug.WriteLine("$" + AddressEntry.Text.Trim() + "$");
                            //Debug.WriteLine("$" + CityEntry.Text.Trim() + "$");
                            //Debug.WriteLine("$" + StateEntry.Text.Trim() + "$");
                            IEnumerable<Position> approximateLocations = await geoCoder.GetPositionsForAddressAsync(passedAdd + "," + passedCity + "," + passedState);
                            Position position = approximateLocations.FirstOrDefault();

                            latitude = $"{position.Latitude}";
                            longitude = $"{position.Longitude}";


                            lati = latitude;
                            longi = longitude;
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

                            Debug.WriteLine("first business: " + obj.Result[0].business_name);
                            taxRate = obj.Result[0].tax_rate / 100;
                            //deliveryFee = obj.Result[0].delivery_fee;
                            //serviceFee = obj.Result[0].service_fee;
                            //passingZones = obj.Result;
                            //withinZones = true;
                        }

                        break;
                    }
                }




                var request3 = new HttpRequestMessage();
                Debug.WriteLine("subscriptionModal trying to delete this purchase uid: " + purchase_uid.ToString());
                //request2.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/refund_calculator?purchase_uid=" + purchase_id);
                request3.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_uid);
                request3.Method = HttpMethod.Get;
                var client3 = new HttpClient();
                HttpResponseMessage response3 = await client3.SendAsync(request3);

                if (response3.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content3 = response3.Content;
                    var userString3 = await content3.ReadAsStringAsync();
                    JObject refund_obj = JObject.Parse(userString3);

                    //string updatedDiscount, updatedDue, updatedTax, updatedTip, updatedService, updatedDelivery, updatedSub, updatedAmb;
                    Debug.WriteLine("first start" + refund_obj.ToString());
                    Debug.WriteLine("this is what I'm getting: " + refund_obj["refund_amount"].ToString());
                    double amt = Double.Parse(refund_obj["refund_amount"].ToString());
                    double basePrice_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected];
                    //double basePrice_dub = itemPrices[deliverySelected - 1, 6 - mealSelected];
                    double discountAmt_dub = deliverySelected * itemPrices[deliverySelected - 1, 6 - mealSelected] * discounts[deliverySelected - 1, 6 - mealSelected] / 100.0;
                    updatedDiscount = discountAmt_dub.ToString();
                    updatedSub = basePrice_dub.ToString();
                    updatedTax = Math.Round(total * (taxRate), 2).ToString();
                    double newTotal = Math.Round(total * (1 + taxRate), 2);
                    Debug.WriteLine("this is the amount for tax: " + Math.Round(total * (taxRate), 2).ToString());
                    Debug.WriteLine("tax rate from categorical options: " + taxRate.ToString());
                    Debug.WriteLine("newTotal: " + newTotal.ToString());
                    newTotal += Double.Parse(refund_obj["delivery_fee"].ToString());
                    newTotal += Double.Parse(refund_obj["service_fee"].ToString());
                    newTotal += Double.Parse(refund_obj["driver_tip"].ToString());
                    updatedTip = refund_obj["driver_tip"].ToString();
                    updatedService = refund_obj["service_fee"].ToString();
                    updatedDelivery = refund_obj["delivery_fee"].ToString();
                    updatedAmb = refund_obj["ambassador_code"].ToString();
                    newTotal = Math.Round(newTotal, 2);
                    Debug.WriteLine("amt before subtracting: " + amt.ToString());
                    Debug.WriteLine("newTotal before subtracting: " + newTotal.ToString());
                    double currentPrice = total;

                    double correct;
                    //check if the ambassador code discount completely covers the meal plan price
                    if (Double.Parse(refund_obj["ambassador_code"].ToString()) - newTotal > 0)
                        correct = amt;
                    else correct = amt + Double.Parse(refund_obj["ambassador_code"].ToString()) - newTotal;

                    //double correct = amt - newTotal;
                    correct = Math.Round(correct, 2);
                    Debug.WriteLine("correct after subtracting: " + correct.ToString());


                    if (correct < 0)
                    {
                        correct *= -1;
                        await DisplayAlert("Extra Charge", "You will be charged $" + correct.ToString() + " for this plan change.", "OK");
                        extraCharge = correct;
                        updatedDue = extraCharge.ToString();
                        shouldCharge = true;
                    }
                    else
                    {
                        await DisplayAlert("Reimbursement", "You will be reimbursed $" + correct.ToString() + " for this plan change.", "OK");
                        shouldCharge = false;
                    }

                    SignUpButton.Text = "SAVE CHANGES";
                }
            }
            else
            {
                if (shouldCharge == true)
                {
                    string price = total.ToString();
                    Preferences.Set("price", price);

                    Console.WriteLine("Price selected: " + price);

                    PurchaseInfo2 updated = new PurchaseInfo2();

                    //if direct login
                    if (socialLogin == "NULL")
                    {
                        var request2 = new HttpRequestMessage();
                        Console.WriteLine("user_id: " + (string)Xamarin.Forms.Application.Current.Properties["user_id"]);
                        string url2 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                        //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                        //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                        request2.RequestUri = new Uri(url2);
                        //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                        request2.Method = HttpMethod.Get;
                        var client2 = new HttpClient();
                        HttpResponseMessage response2 = await client2.SendAsync(request2);

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            HttpContent content3 = response2.Content;
                            Console.WriteLine("content: " + content3);
                            var userString2 = await content3.ReadAsStringAsync();
                            var info_obj2 = JObject.Parse(userString2);

                            //updated.password = (info_obj2["result"])[0]["password_hashed"].ToString();
                        }
                    }

                    //start of stripe processing if there is an additional charge********************************************

                    var total2 = extraCharge.ToString();
                    var clientHttp = new System.Net.Http.HttpClient();
                    var stripe = new Credentials();
                    if (delivInstructions == "M4METEST" || delivInstructions == "M4ME TEST")
                        stripe.key = Constant.TestPK;
                    else stripe.key = Constant.LivePK;

                    var stripeObj = JsonConvert.SerializeObject(stripe);
                    var stripeContent = new StringContent(stripeObj, Encoding.UTF8, "application/json");
                    var RDSResponse = await clientHttp.PostAsync(Constant.StripeModeUrl, stripeContent);
                    var content = await RDSResponse.Content.ReadAsStringAsync();

                    Debug.WriteLine("key to send JSON: " + stripeObj);
                    Debug.WriteLine("Response from key: " + content);
                    Debug.WriteLine("RDSResponse" + RDSResponse.IsSuccessStatusCode.ToString());
                    if (RDSResponse.IsSuccessStatusCode)
                    {
                        //Carlos original code
                        //if (content != "200")
                        //if (content.Contains("200"))
                        //{
                        //Debug.WriteLine("error encountered");
                        string SK = "";
                        string mode = "";

                        if (stripeObj.Contains("test"))
                        {
                            mode = "TEST";
                            SK = Constant.TestSK;
                        }
                        else if (stripeObj.Contains("live"))
                        {
                            mode = "LIVE";
                            SK = Constant.LiveSK;
                        }
                        //Carlos original code
                        //if (content.Contains("Test"))
                        //{
                        //    mode = "TEST";
                        //    SK = Constant.TestSK;
                        //}
                        //else if (content.Contains("Live"))
                        //{
                        //    mode = "LIVE";
                        //    SK = Constant.LiveSK;
                        //}



                        //trying to implement stripe with payment intent and payment method
                        //GetPaymentIntent newPaymentIntent = new GetPaymentIntent();
                        //newPaymentIntent.currency = "usd";
                        //newPaymentIntent.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                        //newPaymentIntent.business_code = DeliveryEntry.Text.Trim();
                        //newPaymentIntent.item_uid = Preferences.Get("item_uid", "");
                        //newPaymentIntent.num_items = Int32.Parse(Preferences.Get("item_name", "").Substring(0, Preferences.Get("item_name", "").IndexOf(" ")));
                        //newPaymentIntent.num_deliveries = Int32.Parse(Preferences.Get("freqSelected", ""));
                        //newPaymentIntent.delivery_discount = Math.Round(Double.Parse(discountPrice.Text.Substring(discountPrice.Text.IndexOf("$") + 1)) / Double.Parse(subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf("$") + 1)), 0);
                        //PaymentSummary paymentSum = new PaymentSummary();
                        //paymentSum.mealSubPrice = subtotalPrice.Text.Substring(subtotalPrice.Text.IndexOf("$") + 1);
                        //paymentSum.discountAmount = discountPrice.Text.Substring(discountPrice.Text.IndexOf("$") + 1);
                        //paymentSum.addOns = "0.00";
                        //paymentSum.tip = tipPrice.Text.Substring(tipPrice.Text.IndexOf("$") + 1);
                        //paymentSum.serviceFee = serviceFeePrice.Text.Substring(serviceFeePrice.Text.IndexOf("$") + 1);
                        //paymentSum.deliveryFee = deliveryFeePrice.Text.Substring(deliveryFeePrice.Text.IndexOf("$") + 1);
                        //paymentSum.taxRate = tax;
                        //paymentSum.taxAmount = taxPrice.Text.Substring(taxPrice.Text.IndexOf("$") + 1);
                        //paymentSum.ambassadorDiscount = ambassDisc.Text.Substring(ambassDisc.Text.IndexOf("$") + 1);
                        //paymentSum.total = grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf("$") + 1);
                        //paymentSum.subtotal = grandTotalPrice.Text.Substring(grandTotalPrice.Text.IndexOf("$") + 1);
                        //newPaymentIntent.payment_summary = paymentSum;


                        //var paymentIntentJSONString = JsonConvert.SerializeObject(newPaymentIntent);
                        //Console.WriteLine("paymentIntentJSONString" + paymentIntentJSONString);
                        //var content3 = new StringContent(paymentIntentJSONString, Encoding.UTF8, "application/json");
                        //Console.WriteLine("Content: " + content3);
                        //var client3 = new System.Net.Http.HttpClient();
                        //var response3 = await client3.PostAsync("https://huo8rhh76i.execute-api.us-west-1.amazonaws.com/dev/api/v2/createPaymentIntent", content3);
                        //var message3 = await response3.Content.ReadAsStringAsync();
                        //Debug.WriteLine("create payment intent response: " + message3);
                        //string payIntent = message3.Substring(1, message3.IndexOf("secret") - 2);
                        //Debug.WriteLine("only the payment intent: " + message3.Substring(1, message3.IndexOf("secret") - 2));
                        //string secret = message3.Substring(message3.IndexOf("secret") + 7);
                        //secret = secret.Substring(0, secret.IndexOf("\""));
                        //Debug.WriteLine("only the secret: " + secret);
                        //string clientSec = message3.Substring(1);
                        //clientSec = clientSec.Substring(0, clientSec.IndexOf("\""));
                        //Debug.WriteLine("client secret: " + clientSec);

                        //PaymentMethodCard payWithCard = new PaymentMethodCard();

                        Debug.WriteLine("MODE          : " + mode);
                        Debug.WriteLine("STRIPE SECRET : " + SK);

                        //Debug.WriteLine("SK" + SK);
                        StripeConfiguration.ApiKey = SK;

                        //Dictionary<String, Object> card = new Dictionary<string, object>();
                        //card.Add("number", "4242424242424242");
                        //card.Add("exp_month", 4);
                        //card.Add("exp_year", 2022);
                        //card.Add("cvc", "314");
                        //Dictionary<String, Object> param = new Dictionary<string, object>();
                        //param.Add("type", "card");
                        //param.Add("card", card);

                        ////Stripe.PaymentMethodCard
                        //Stripe.PaymentMethod paymentMethod = new Stripe.PaymentMethod();
                        //Stripe.PaymentMethodCard paywith = new Stripe.PaymentMethodCard();
                        ////var req = await stripe.createPaymentMethod();
                        //StripeClient stripeClient = new StripeClient();
                        //string cc_num; string cc_exp_year; string cc_exp_month; string cc_cvv;

                        string CardNo = cc_num.Trim();
                        string expMonth = cc_exp_month.Trim();
                        string expYear = cc_exp_year.Trim();
                        string cardCvv = cc_cvv.Trim();

                        Debug.WriteLine("step 1 reached");
                        // Step 1: Create Card
                        TokenCardOptions stripeOption = new TokenCardOptions();
                        stripeOption.Number = CardNo;
                        stripeOption.ExpMonth = Convert.ToInt64(expMonth);
                        stripeOption.ExpYear = Convert.ToInt64(expYear);
                        stripeOption.Cvc = cardCvv;


                        Debug.WriteLine("step 2 reached");
                        // Step 2: Assign card to token object
                        TokenCreateOptions stripeCard = new TokenCreateOptions();
                        stripeCard.Card = stripeOption;

                        TokenService service = new TokenService();
                        Stripe.Token newToken = service.Create(stripeCard);

                        Debug.WriteLine("step 3 reached");
                        // Step 3: Assign the token to the soruce 
                        var option = new SourceCreateOptions();
                        option.Type = SourceType.Card;
                        option.Currency = "usd";
                        option.Token = newToken.Id;

                        var sourceService = new SourceService();
                        Source source = sourceService.Create(option);

                        //source.ClientSecret = clientSec;
                        //source.Card

                        Debug.WriteLine("step 4 reached");
                        // Step 4: Create customer
                        CustomerCreateOptions customer = new CustomerCreateOptions();
                        //NEED CARDHOLDER NAME
                        customer.Name = cust_firstName + " " + cust_lastName;
                        customer.Email = cust_email.ToLower().Trim();
                        //if (cardDescription.Text == "" || cardDescription.Text == null)
                        customer.Description = "";
                        //else customer.Description = cardDescription.Text.Trim();
                        if (passedUnit == null)
                        {
                            passedUnit = "";
                        }
                        //NEED CARDHOLDER ADDRESS
                        customer.Address = new AddressOptions { City = passedCity.Trim(), Country = Constant.Contry, Line1 = passedAdd.Trim(), Line2 = passedUnit.Trim(), PostalCode = passedZip.Trim(), State = passedState.Trim() };


                        var customerService = new CustomerService();
                        var cust = customerService.Create(customer);

                        Debug.WriteLine("step 5 reached");
                        // Step 5: Charge option
                        var chargeOption = new ChargeCreateOptions();
                        chargeOption.Amount = (long)RemoveDecimalFromTotalAmount(total2);

                        Debug.WriteLine("hopefully correct total: " + total2);
                        chargeOption.Currency = "usd";
                        chargeOption.ReceiptEmail = cust_email.ToLower().Trim();
                        chargeOption.Customer = cust.Id;
                        chargeOption.Source = source.Id;
                        //if (cardDescription.Text == "" || cardDescription.Text == null)
                        chargeOption.Description = "";
                        //else chargeOption.Description = cardDescription.Text.Trim();

                        //chargeOption.Description = cardDescription.Text.Trim();

                        Debug.WriteLine("step 6 reached");
                        // Step 6: charge the customer COMMENTED OUT FOR TESTING, backend already charges stripe so we don't have to do it here
                        var chargeService = new ChargeService();

                        Charge charge = chargeService.Create(chargeOption);
                        //charge.PaymentIntent = (PaymentIntent)payIntent;
                        Debug.WriteLine("charge: " + charge.ToString());
                        Debug.WriteLine("charge id: " + charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3));
                        //chargeId = charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3);
                        if (charge.Status == "succeeded")
                        {
                            chargeId = charge.ToString().Substring(charge.ToString().IndexOf("id") + 3, charge.ToString().IndexOf(">") - charge.ToString().IndexOf("id") - 3);


                            Debug.WriteLine("STRIPE PAYMENT WAS SUCCESSFUL");
                            //end of stripe payment frontend processing

                            //done from checkout button clicked
                        }
                        else
                        {
                            //await Navigation.PopAsync(false);
                            // Fail
                            await DisplayAlert("Ooops", "Payment was not succesfull. Please try again", "OK");
                        }
                    }

                    //end of stripe processing*******************************************************************************************

                    //else updated.password = "NULL";

                    //updated.password = password;
                    //updated.refresh_token = refresh_token;
                    //updated.cc_num = cc_num;
                    List<Item> list1 = new List<Item>();
                    Item item1 = new Item();
                    item1.qty = Preferences.Get("freqSelected", "");
                    item1.name = Preferences.Get("mealSelected", "") + " Meal Plan";
                    //item1.price = Preferences.Get("price", "");
                    item1.price = Preferences.Get("itemPrice", "00.00");
                    item1.item_uid = Preferences.Get("item_uid", "");
                    item1.itm_business_uid = itm_business_uid;
                    list1.Add(item1);
                    updated.items = list1;

                    updatePurchase updatePur = new updatePurchase();
                    updatePur.start_delivery_date = startDeliv;
                    updatePur.purchaseId = purchase_id;
                    updatePur.amount_due = updatedDue;
                    updatePur.amount_discount = updatedDiscount;
                    updatePur.amount_paid = updatedDue;
                    updatePur.coupon_id = "";
                    updatePur.charge_id = chargeId;
                    updatePur.payment_type = "STRIPE";
                    updatePur.cc_num = cc_num;
                    updatePur.cc_exp_date = cc_exp_date;
                    updatePur.cc_cvv = cc_cvv;
                    updatePur.cc_zip = cc_zip;
                    updatePur.taxes = updatedTax;
                    updatePur.tip = updatedTip;
                    updatePur.service_fee = updatedService;
                    updatePur.delivery_fee = updatedDelivery;
                    updatePur.subtotal = updatedSub;
                    updatePur.amb = updatedAmb;
                    updatePur.customer_uid = (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                    updatePur.delivery_first_name = cust_firstName;
                    updatePur.delivery_last_name = cust_lastName;
                    updatePur.delivery_email = cust_email;
                    updatePur.delivery_phone = phoneNum;
                    updatePur.delivery_address = passedAdd;
                    updatePur.delivery_unit = passedUnit;
                    updatePur.delivery_state = passedState;
                    updatePur.delivery_city = passedCity;
                    updatePur.delivery_zip = passedZip;
                    updatePur.delivery_instructions = delivInstructions;
                    updatePur.delivery_longitude = longi;
                    updatePur.delivery_latitude = lati;
                    updatePur.items = list1;
                    updatePur.order_instructions = "";
                    updatePur.purchase_notes = "";

                    var updateJSONString = JsonConvert.SerializeObject(updatePur);
                    Console.WriteLine("updatedJSONString" + updateJSONString);
                    var content2 = new StringContent(updateJSONString, Encoding.UTF8, "application/json");
                    Console.WriteLine("updatedContent: " + content2);
                    /*var request = new HttpRequestMessage();
                    request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                    request.Method = HttpMethod.Post;
                    request.Content = content;*/
                    var client = new HttpClient();
                    Debug.WriteLine("url we are posting to: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/update_pay_pur_mobile");
                    //var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                    var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/update_pay_pur_mobile", content2);
                    // HttpResponseMessage response = await client.SendAsync(request);
                    Console.WriteLine("RESPONSE TO update purchase   " + response.Result);
                    Console.WriteLine("update JSON OBJECT BEING SENT: " + updateJSONString);

                    //var newPaymentJSONString = JsonConvert.SerializeObject(updated);
                    //Console.WriteLine("updatedJSONString" + newPaymentJSONString);
                    //var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                    //Console.WriteLine("updatedContent: " + content2);
                    ///*var request = new HttpRequestMessage();
                    //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                    //request.Method = HttpMethod.Post;
                    //request.Content = content;*/
                    //var client = new HttpClient();
                    //Debug.WriteLine("url we are posting to: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_id);
                    ////var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                    //var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_id, content2);
                    //// HttpResponseMessage response = await client.SendAsync(request);
                    //Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                    //Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                    Console.WriteLine("clickedDone Func ENDED!");

                    Zones[] zones = new Zones[] { };
                    await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email));
                    //old nav
                    //await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
                    //Application.Current.MainPage = new DeliveryBilling();
                    //await NavigationPage.PushAsync(DeliveryBilling());
                }
                else //a refund
                {
                    string price = total.ToString();
                    Preferences.Set("price", price);

                    Console.WriteLine("Price selected: " + price);

                    PurchaseInfo2 updated = new PurchaseInfo2();

                    //if direct login
                    if (socialLogin == "NULL")
                    {
                        var request2 = new HttpRequestMessage();
                        Console.WriteLine("user_id: " + (string)Xamarin.Forms.Application.Current.Properties["user_id"]);
                        string url2 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Xamarin.Forms.Application.Current.Properties["user_id"];
                        //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                        //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                        request2.RequestUri = new Uri(url2);
                        //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                        request2.Method = HttpMethod.Get;
                        var client2 = new HttpClient();
                        HttpResponseMessage response2 = await client2.SendAsync(request2);

                        if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            HttpContent content3 = response2.Content;
                            Console.WriteLine("content: " + content3);
                            var userString2 = await content3.ReadAsStringAsync();
                            var info_obj2 = JObject.Parse(userString2);

                            //updated.password = (info_obj2["result"])[0]["password_hashed"].ToString();
                        }
                    }

                   

                    //else updated.password = "NULL";

                    //updated.password = password;
                    //updated.refresh_token = refresh_token;
                    //updated.cc_num = cc_num;
                    //testing
                    updated.cc_num = cc_num;
                    updated.cc_exp_date = cc_exp_date;
                    //updated.cc_exp_year = cc_exp_year;
                    //updated.cc_exp_month = cc_exp_month;
                    updated.cc_cvv = cc_cvv;
                    updated.purchase_id = purchase_uid;
                    //updated.purchase_id = "400-000019";
                    updated.new_item_id = Preferences.Get("item_uid", "");
                    updated.customer_email = cust_email;
                    updated.cc_zip = cc_zip;
                    updated.start_delivery_date = "";

                    List<Item> list1 = new List<Item>();
                    Item item1 = new Item();
                    item1.qty = Preferences.Get("freqSelected", "");
                    item1.name = Preferences.Get("mealSelected", "") + " Meal Plan";
                    //item1.price = Preferences.Get("price", "");
                    item1.price = Preferences.Get("itemPrice", "00.00");
                    item1.item_uid = updated.new_item_id;
                    item1.itm_business_uid = itm_business_uid;
                    list1.Add(item1);
                    updated.items = list1;



                    var newPaymentJSONString = JsonConvert.SerializeObject(updated);
                    Console.WriteLine("updatedJSONString" + newPaymentJSONString);
                    var content2 = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                    Console.WriteLine("updatedContent: " + content2);
                    /*var request = new HttpRequestMessage();
                    request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                    request.Method = HttpMethod.Post;
                    request.Content = content;*/
                    var client = new HttpClient();
                    Debug.WriteLine("url we are posting to: " + "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_uid);
                    //var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                    var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase/" + purchase_uid, content2);
                    // HttpResponseMessage response = await client.SendAsync(request);
                    Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                    Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                    Console.WriteLine("clickedDone Func ENDED!");

                    Zones[] zones = new Zones[] { };
                    await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email));
                }
            }
        }

            // FUNCTION  3:
            public int RemoveDecimalFromTotalAmount(string amount)
            {
                var stringAmount = "";
                var arrayAmount = amount.ToCharArray();
                for (int i = 0; i < arrayAmount.Length; i++)
                {
                    if ((int)arrayAmount[i] != (int)'.')
                    {
                        stringAmount += arrayAmount[i];
                    }
                }
                System.Diagnostics.Debug.WriteLine(stringAmount);
                return Int32.Parse(stringAmount);
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

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new UserProfile();
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            Console.WriteLine("navigation stack count: " + Navigation.NavigationStack.Count);
            if (Navigation.NavigationStack.Count == 1)
            {
                Xamarin.Forms.Application.Current.MainPage = new MainPage();
            }
            else
            {
                await Navigation.PopAsync(false);
            }
            //await Navigation.PushAsync(new MainPage());
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new Menu();
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

        async void clickedLanding(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedMealPlan(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new MealPlans(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        async void clickedSelect(System.Object sender, System.EventArgs e)
        {
            if (Preferences.Get("canChooseSelect", false) == false)
                DisplayAlert("Error", "please purchase a meal plan first", "OK");
            else
            {
                Zones[] zones = new Zones[] { };
                await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email), false);
                //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
            }
        }

        async void clickedSubscription(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email), false);
            //Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
        }

        void clickedLogout(System.Object sender, System.EventArgs e)
        {
            Xamarin.Forms.Application.Current.Properties.Remove("user_id");
            Xamarin.Forms.Application.Current.Properties["platform"] = "GUEST";
            Xamarin.Forms.Application.Current.Properties.Remove("time_stamp");
            //Application.Current.Properties.Remove("platform");
            Xamarin.Forms.Application.Current.MainPage = new MainPage();
        }
        //end of menu functions
    }
}
