using MTYD.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTYD.ViewModel
{
    public partial class SubscriptionModal : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;
        public ObservableCollection<Plans> NewPlan = new ObservableCollection<Plans>();

        double m1price_f1 = 0.0; double m1price_f2 = 0.0; double m1price_f3 = 0.0; double m2price_f1 = 0.0; double m2price_f2 = 0.0; double m2price_f3 = 0.0;
        double m3price_f1 = 0.0; double m3price_f2 = 0.0; double m3price_f3 = 0.0; double m4price_f1 = 0.0; double m4price_f2 = 0.0; double m4price_f3 = 0.0;
        string m1f1name = "", m1f2name = "", m1f3name = "", m2f1name = "", m2f2name = "", m2f3name = "", m3f1name = "", m3f2name = "", m3f3name = "", m4f1name = "", m4f2name = "", m4f3name = "";
        string m1f1uid = "", m1f2uid = "", m1f3uid = "", m2f1uid = "", m2f2uid = "", m2f3uid = "", m3f1uid = "", m3f2uid = "", m3f3uid = "", m4f1uid = "", m4f2uid = "", m4f3uid = "";

        string socialLogin; string refresh_token; string cc_num; string cc_exp_year; string cc_exp_month; string cc_cvv; string purchase_id;
        string new_item_id; string customer_id; string itm_business_uid; string cc_zip; string cc_exp_date;

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

                ArrayList item_price = new ArrayList();
                ArrayList num_items = new ArrayList();
                ArrayList payment_frequency = new ArrayList();
                ArrayList groupArray = new ArrayList();

                double doub;
                foreach (var m in plan_obj["result"])
                {
                    //Console.WriteLine("PARSING DATA FROM DB: ITEM_UID: " + m["item_uid"].ToString());
                    item_price.Add(double.Parse(m["item_price"].ToString()));
                    num_items.Add(int.Parse(m["num_items"].ToString()));
                    payment_frequency.Add(int.Parse(m["payment_frequency"].ToString()));
                    groupArray.Add(int.Parse(m["num_items"].ToString()));
                    groupArray.Add(int.Parse(m["payment_frequency"].ToString()));
                    double.TryParse(m["item_price"].ToString(), out doub);
                    groupArray.Add(doub);
                    groupArray.Add(m["item_name"].ToString());
                    groupArray.Add(m["item_uid"].ToString());
                }
                //Find unique number of meals
                int first = (int)num_items[1];
                int[] numItemsArray = new int[] { first, 0, 0, 0 };
                int index = 1;
                //Fill Unique # of Meals
                for (int i = 2; i < num_items.Count; i++)
                {
                    if (((int)num_items[i] != first) && ((int)num_items[i] != numItemsArray[1]) && ((int)num_items[i] != numItemsArray[2]) && ((int)num_items[i] != numItemsArray[3]))
                    {
                        numItemsArray[index] = (int)num_items[i];
                        index++;
                    }
                }
                meals1.Text = numItemsArray[0].ToString() + " MEALS";
                meals2.Text = numItemsArray[1].ToString() + " MEALS";
                meals3.Text = numItemsArray[2].ToString() + " MEALS";
                meals4.Text = numItemsArray[3].ToString() + " MEALS";

                //Fill Payment Frequency
                int[] payFreqArray = new int[] { (int)payment_frequency[1], 0, 0 };
                index = 1;
                for (int i = 2; i < payment_frequency.Count; i++)
                {
                    if (((int)payment_frequency[i] != payFreqArray[0]) && ((int)payment_frequency[i] != payFreqArray[1]) && ((int)payment_frequency[i] != payFreqArray[2]))
                    {
                        payFreqArray[index] = (int)payment_frequency[i];
                        index++;
                    }
                }
                Array.Sort(payFreqArray, 0, 3);

                payOp1.Text = payFreqArray[0].ToString();

                if (payOp1.Text == "1")
                    payOp1.Text = "WEEKLY";
                else payOp1.Text = payFreqArray[0].ToString() + " WEEKS";

                payOp2.Text = payFreqArray[1].ToString() + " WEEKS";
                payOp3.Text = payFreqArray[2].ToString() + " WEEKS";
                //cat1.Text = catArray[0];
                //VenueCatListView.ItemsSource = VenueCat;
                int m1 = numItemsArray[0];
                int m2 = numItemsArray[1];
                int m3 = numItemsArray[2];
                int m4 = numItemsArray[3];
                int p1 = payFreqArray[0];
                int p2 = payFreqArray[1];
                int p3 = payFreqArray[2];

                Console.WriteLine("START OF GET PLANS FUNCTION");
                for (int i = 5; i < (groupArray.Count) - 4; i += 5)
                {
                    if ((int)groupArray[i] == m1 && (int)groupArray[i + 1] == p1)
                    {
                        m1price_f1 = (double)groupArray[i + 2];
                        m1f1name = (string)groupArray[i + 3];
                        m1f1uid = (string)groupArray[i + 4];

                        if (new_item_id == m1f1uid)
                        {
                            Preferences.Set("freqSelected", "1");
                            Preferences.Set("mealSelected", "1");
                            TotalPrice.Text = "$" + m1price_f1.ToString();
                            meals1.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m1 && (int)groupArray[i + 1] == p2)
                    {
                        m1price_f2 = (double)groupArray[i + 2];
                        m1f2name = (string)groupArray[i + 3];
                        m1f2uid = (string)groupArray[i + 4];

                        if (new_item_id == m1f2uid)
                        {
                            Preferences.Set("freqSelected", "2");
                            Preferences.Set("mealSelected", "1");
                            TotalPrice.Text = "$" + m1price_f2.ToString();
                            meals1.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m1 && (int)groupArray[i + 1] == p3)
                    {
                        m1price_f3 = (double)groupArray[i + 2];
                        m1f3name = (string)groupArray[i + 3];
                        m1f3uid = (string)groupArray[i + 4];

                        if (new_item_id == m1f3uid)
                        {
                            Preferences.Set("freqSelected", "3");
                            Preferences.Set("mealSelected", "1");
                            TotalPrice.Text = "$" + m1price_f3.ToString();
                            meals1.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton2.Opacity = 0.3;
                        }
                    }
                    //
                    else if ((int)groupArray[i] == m2 && (int)groupArray[i + 1] == p1)
                    {
                        m2price_f1 = (double)groupArray[i + 2];
                        m2f1name = (string)groupArray[i + 3];
                        m2f1uid = (string)groupArray[i + 4];

                        if (new_item_id == m2f1uid)
                        {
                            Preferences.Set("freqSelected", "1");
                            Preferences.Set("mealSelected", "2");
                            TotalPrice.Text = "$" + m2price_f1.ToString();
                            meals2.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m2 && (int)groupArray[i + 1] == p2)
                    {
                        m2price_f2 = (double)groupArray[i + 2];
                        m2f2name = (string)groupArray[i + 3];
                        m2f2uid = (string)groupArray[i + 4];

                        if (new_item_id == m2f2uid)
                        {
                            Preferences.Set("freqSelected", "2");
                            Preferences.Set("mealSelected", "2");
                            TotalPrice.Text = "$" + m2price_f2.ToString();
                            meals2.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m2 && (int)groupArray[i + 1] == p3)
                    {
                        m2price_f3 = (double)groupArray[i + 2];
                        m2f3name = (string)groupArray[i + 3];
                        m2f3uid = (string)groupArray[i + 4];

                        if (new_item_id == m2f3uid)
                        {
                            Preferences.Set("freqSelected", "3");
                            Preferences.Set("mealSelected", "2");
                            TotalPrice.Text = "$" + m2price_f3.ToString();
                            meals2.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton2.Opacity = 0.3;
                        }
                    }
                    //
                    else if ((int)groupArray[i] == m3 && (int)groupArray[i + 1] == p1)
                    {
                        m3price_f1 = (double)groupArray[i + 2];
                        m3f1name = (string)groupArray[i + 3];
                        m3f1uid = (string)groupArray[i + 4];

                        if (new_item_id == m3f1uid)
                        {
                            Preferences.Set("freqSelected", "1");
                            Preferences.Set("mealSelected", "3");
                            TotalPrice.Text = "$" + m3price_f1.ToString();
                            meals3.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m3 && (int)groupArray[i + 1] == p2)
                    {
                        m3price_f2 = (double)groupArray[i + 2];
                        m3f2name = (string)groupArray[i + 3];
                        m3f2uid = (string)groupArray[i + 4];

                        if (new_item_id == m3f2uid)
                        {
                            Preferences.Set("freqSelected", "2");
                            Preferences.Set("mealSelected", "3");
                            TotalPrice.Text = "$" + m3price_f2.ToString();
                            meals3.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m3 && (int)groupArray[i + 1] == p3)
                    {
                        m3price_f3 = (double)groupArray[i + 2];
                        m3f3name = (string)groupArray[i + 3];
                        m3f3uid = (string)groupArray[i + 4];

                        if (new_item_id == m3f3uid)
                        {
                            Preferences.Set("freqSelected", "3");
                            Preferences.Set("mealSelected", "3");
                            TotalPrice.Text = "$" + m3price_f3.ToString();
                            meals3.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton1.Opacity = 0.3;
                        }
                    }
                    //
                    else if ((int)groupArray[i] == m4 && (int)groupArray[i + 1] == p1)
                    {
                        m4price_f1 = (double)groupArray[i + 2];
                        m4f1name = (string)groupArray[i + 3];
                        m4f1uid = (string)groupArray[i + 4];

                        if (new_item_id == m4f1uid)
                        {
                            Preferences.Set("freqSelected", "1");
                            Preferences.Set("mealSelected", "4");
                            TotalPrice.Text = "$" + m4price_f1.ToString();
                            meals4.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m4 && (int)groupArray[i + 1] == p2)
                    {
                        m4price_f2 = (double)groupArray[i + 2];
                        m4f2name = (string)groupArray[i + 3];
                        m4f2uid = (string)groupArray[i + 4];

                        if (new_item_id == m4f2uid)
                        {
                            Preferences.Set("freqSelected", "2");
                            Preferences.Set("mealSelected", "4");
                            TotalPrice.Text = "$" + m4price_f2.ToString();
                            meals4.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton1.Opacity = 0.3;
                            payButton3.Opacity = 0.3;
                        }
                    }
                    else if ((int)groupArray[i] == m4 && (int)groupArray[i + 1] == p3)
                    {
                        m4price_f3 = (double)groupArray[i + 2];
                        m4f3name = (string)groupArray[i + 3];
                        m4f3uid = (string)groupArray[i + 4];

                        if (new_item_id == m4f3uid)
                        {
                            Preferences.Set("freqSelected", "3");
                            Preferences.Set("mealSelected", "4");
                            TotalPrice.Text = "$" + m4price_f3.ToString();
                            meals4.BackgroundColor = Color.FromHex("#FFBA00");
                            payButton2.Opacity = 0.3;
                            payButton1.Opacity = 0.3;
                        }
                    }
                }
                Console.WriteLine("END OF GET PLANS FUNCTION");
            }
        }


        void checkPlatform(double height, double width)
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

                takeoutGrid.Margin = new Thickness(20, 10, 20, 10);
                takeout.HeightRequest = width / 18;
                takeout.WidthRequest = width / 18;
                deliveryDays.FontSize = width / 38;
                deliveryDays2.FontSize = width / 38;
                numMeals.FontSize = width / 37;
                numMeals.Margin = new Thickness(25, 10, 0, 10);

                meals1.HeightRequest = height / 30;
                meals1.WidthRequest = width / 4;
                meals1.CornerRadius = (int)(height / 60);
                meals1.FontSize = width / 40;
                meals1.Margin = new Thickness(30, 0, 15, 0);
                meals2.HeightRequest = height / 30;
                meals2.WidthRequest = width / 4;
                meals2.CornerRadius = (int)(height / 60);
                meals2.FontSize = width / 40;
                meals2.Margin = new Thickness(30, 0, 15, 0);

                meals3.HeightRequest = height / 30;
                meals3.WidthRequest = width / 4;
                meals3.CornerRadius = (int)(height / 60);
                meals3.FontSize = width / 40;
                meals3.Margin = new Thickness(15, 0, 30, 0);
                meals4.HeightRequest = height / 30;
                meals4.WidthRequest = width / 4;
                meals4.CornerRadius = (int)(height / 60);
                meals4.FontSize = width / 40;
                meals4.Margin = new Thickness(15, 0, 30, 0);

                prepay.Margin = new Thickness(30, 0, 0, 0);
                prepay.FontSize = width / 37;

                payFrame.HeightRequest = height / 12;
                payOp1.FontSize = width / 50;
                payOp2.FontSize = width / 50;
                payOp3.FontSize = width / 50;
                payButton1.HeightRequest = width / 11;
                payButton1.WidthRequest = width / 11;
                payButton1.CornerRadius = (int)(width / 22);
                payButton2.HeightRequest = width / 11;
                payButton2.WidthRequest = width / 11;
                payButton2.CornerRadius = (int)(width / 22);
                payButton3.HeightRequest = width / 11;
                payButton3.WidthRequest = width / 11;
                payButton3.CornerRadius = (int)(width / 22);

                PriceFrame.HeightRequest = height / 30;
                PriceFrame.WidthRequest = width / 6;
                PriceFrame.CornerRadius = 30;
                TotalPrice.FontSize = width / 40;
                SignUpButton.HeightRequest = height / 30;
                SignUpButton.WidthRequest = width / 5;
                SignUpButton.CornerRadius = (int)(height / 60);
                SignUpButton.FontSize = width / 43;
            }
            else //android
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                heading.FontSize = width / 45;
                heading.Margin = new Thickness(0, 0, 0, 40);
                //heading.VerticalOptions = LayoutOptions.Center;
                pfp.HeightRequest = width / 25;
                pfp.WidthRequest = width / 25;
                pfp.CornerRadius = (int)(width / 50);
                pfp.Margin = new Thickness(0, 0, 23, 35);
                menu.HeightRequest = width / 30;
                menu.WidthRequest = width / 30;
                menu.Margin = new Thickness(25, 0, 0, 40);

                takeoutGrid.Margin = new Thickness(20, 10, 20, 10);
                takeout.HeightRequest = width / 22;
                takeout.WidthRequest = width / 22;
                deliveryDays.FontSize = width / 47;
                deliveryDays2.FontSize = width / 47;
                numMeals.FontSize = width / 48;
                numMeals.Margin = new Thickness(25, 10, 0, 10);

                meals1.HeightRequest = height / 33;
                meals1.WidthRequest = width / 4;
                meals1.CornerRadius = (int)(height / 60);
                meals1.FontSize = width / 49;
                meals1.Margin = new Thickness(30, 0, 15, 0);
                meals2.HeightRequest = height / 33;
                meals2.WidthRequest = width / 4;
                meals2.CornerRadius = (int)(height / 60);
                meals2.FontSize = width / 49;
                meals2.Margin = new Thickness(30, 0, 15, 0);

                meals3.HeightRequest = height / 33;
                meals3.WidthRequest = width / 4;
                meals3.CornerRadius = (int)(height / 60);
                meals3.FontSize = width / 49;
                meals3.Margin = new Thickness(15, 0, 30, 0);
                meals4.HeightRequest = height / 33;
                meals4.WidthRequest = width / 4;
                meals4.CornerRadius = (int)(height / 60);
                meals4.FontSize = width / 49;
                meals4.Margin = new Thickness(15, 0, 30, 0);

                prepay.Margin = new Thickness(30, 0, 0, 0);
                prepay.FontSize = width / 48;

                payFrame.HeightRequest = height / 12;
                payFrame.CornerRadius = 105;
                payOp1.FontSize = width / 55;
                payOp2.FontSize = width / 55;
                payOp3.FontSize = width / 55;
                payButton1.HeightRequest = width / 13;
                payButton1.WidthRequest = width / 13;
                payButton1.CornerRadius = (int)(width / 26);
                payButton2.HeightRequest = width / 13;
                payButton2.WidthRequest = width / 13;
                payButton2.CornerRadius = (int)(width / 26);
                payButton3.HeightRequest = width / 13;
                payButton3.WidthRequest = width / 13;
                payButton3.CornerRadius = (int)(width / 26);

                spacer4.HeightRequest = 10;
                PriceFrame.HeightRequest = height / 33;
                PriceFrame.WidthRequest = width / 8;
                PriceFrame.CornerRadius = 33;
                TotalPrice.FontSize = width / 50;
                SignUpButton.HeightRequest = height / 33;
                SignUpButton.WidthRequest = width / 8;
                SignUpButton.CornerRadius = (int)(height / 66);
                SignUpButton.FontSize = width / 50;
            }

            //common adjustments regardless of platform
        }

        public SubscriptionModal(string firstName, string lastName, string email, string social, string token, string num, string expDate, string cvv, string zip, string purchaseID, string businessID, string itemID, string customerID)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            Console.WriteLine("SubscriptionModal entered");
            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;

            Console.WriteLine("next entered");
            socialLogin = social; refresh_token = token; cc_num = num; cc_exp_date = expDate; cc_cvv = cvv; purchase_id = purchaseID;
            new_item_id = itemID; customer_id = customerID; cc_zip = zip; itm_business_uid = businessID;
            Console.WriteLine("next2 entered");

            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
            GetPlans();
            Preferences.Set("freqSelected", "");
        }


        private void clickedMeals1(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            meals1.BackgroundColor = Color.FromHex("#FFBA00");
            meals2.BackgroundColor = Color.FromHex("#FFF0C6");
            meals3.BackgroundColor = Color.FromHex("#FFF0C6");
            meals4.BackgroundColor = Color.FromHex("#FFF0C6");
            Preferences.Set("mealSelected", "1");

            string freq_select = Preferences.Get("freqSelected", "");
            if (freq_select == "1")
            {
                TotalPrice.Text = "$" + m1price_f1.ToString();
                Preferences.Set("item_name", m1f1name);
                Preferences.Set("item_uid", m1f1uid);
                new_item_id = m1f1uid;
            }
            else if (freq_select == "2")
            {
                TotalPrice.Text = "$" + m1price_f2.ToString();
                Preferences.Set("item_name", m1f2name);
                Preferences.Set("item_uid", m1f2uid);
                new_item_id = m1f2uid;
            }
            else if (freq_select == "3")
            {
                TotalPrice.Text = "$" + m1price_f3.ToString();
                Preferences.Set("item_name", m1f3name);
                Preferences.Set("item_uid", m1f3uid);
                new_item_id = m1f3uid;
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }

        }
        private void clickedMeals2(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            meals1.BackgroundColor = Color.FromHex("#FFF0C6");
            meals2.BackgroundColor = Color.FromHex("#FFBA00");
            meals3.BackgroundColor = Color.FromHex("#FFF0C6");
            meals4.BackgroundColor = Color.FromHex("#FFF0C6");
            Preferences.Set("mealSelected", "2");

            string freq_select = Preferences.Get("freqSelected", "");
            if (freq_select == "1")
            {
                TotalPrice.Text = "$" + m2price_f1.ToString();
                Preferences.Set("item_name", m2f1name);
                Preferences.Set("item_uid", m2f1uid);
                new_item_id = m2f1uid;
            }
            else if (freq_select == "2")
            {
                TotalPrice.Text = "$" + m2price_f2.ToString();
                Preferences.Set("item_name", m2f2name);
                Preferences.Set("item_uid", m2f2uid);
                new_item_id = m2f2uid;
            }
            else if (freq_select == "3")
            {
                TotalPrice.Text = "$" + m2price_f3.ToString();
                Preferences.Set("item_name", m2f3name);
                Preferences.Set("item_uid", m2f3uid);
                new_item_id = m2f3uid;
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }
        }

        private void clickedMeals3(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            meals1.BackgroundColor = Color.FromHex("#FFF0C6");
            meals2.BackgroundColor = Color.FromHex("#FFF0C6");
            meals3.BackgroundColor = Color.FromHex("#FFBA00");
            meals4.BackgroundColor = Color.FromHex("#FFF0C6");
            Preferences.Set("mealSelected", "3");

            string freq_select = Preferences.Get("freqSelected", "");
            if (freq_select == "1")
            {
                TotalPrice.Text = "$" + m3price_f1.ToString();
                Preferences.Set("item_name", m3f1name);
                Preferences.Set("item_uid", m3f1uid);
                new_item_id = m3f1uid;
            }
            else if (freq_select == "2")
            {
                TotalPrice.Text = "$" + m3price_f2.ToString();
                Preferences.Set("item_name", m3f2name);
                Preferences.Set("item_uid", m3f2uid);
                new_item_id = m3f2uid;
            }
            else if (freq_select == "3")
            {
                TotalPrice.Text = "$" + m3price_f3.ToString();
                Preferences.Set("item_name", m3f3name);
                Preferences.Set("item_uid", m3f3uid);
                new_item_id = m3f3uid;
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }
        }

        private void clickedMeals4(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            meals1.BackgroundColor = Color.FromHex("#FFF0C6");
            meals2.BackgroundColor = Color.FromHex("#FFF0C6");
            meals3.BackgroundColor = Color.FromHex("#FFF0C6");
            meals4.BackgroundColor = Color.FromHex("#FFBA00");
            Preferences.Set("mealSelected", "4");

            string freq_select = Preferences.Get("freqSelected", "");
            if (freq_select == "1")
            {
                TotalPrice.Text = "$" + m4price_f1.ToString();
                Preferences.Set("item_name", m4f1name);
                Preferences.Set("item_uid", m4f1uid);
                new_item_id = m4f1uid;
            }
            else if (freq_select == "2")
            {
                TotalPrice.Text = "$" + m4price_f2.ToString();
                Preferences.Set("item_name", m4f2name);
                Preferences.Set("item_uid", m4f2uid);
                new_item_id = m4f2uid;
            }
            else if (freq_select == "3")
            {
                TotalPrice.Text = "$" + m4price_f3.ToString();
                Preferences.Set("item_name", m4f3name);
                Preferences.Set("item_uid", m4f3uid);
                new_item_id = m4f3uid;
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }
        }

        private void clickedPayOp1(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            //payButton1.BackgroundColor = Color.FromHex("#FFF0C6");
            //payButton2.BackgroundColor = Color.Transparent;
            //payButton3.BackgroundColor = Color.Transparent;

            payButton1.Opacity = 1;
            payButton2.Opacity = 0.3;
            payButton3.Opacity = 0.3;

            //TryParse(TotalPrice.Text.Substring(1, 5), double val);
            Preferences.Set("freqSelected", "1");
            string meal_select = Preferences.Get("mealSelected", "");
            if (meal_select == "1")
            {
                TotalPrice.Text = "$" + m1price_f1.ToString();
                Preferences.Set("item_name", m1f1name);
                Preferences.Set("item_uid", m1f1uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "2")
            {
                TotalPrice.Text = "$" + m2price_f1.ToString();
                Preferences.Set("item_name", m2f1name);
                Preferences.Set("item_uid", m2f1uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "3")
            {
                TotalPrice.Text = "$" + m3price_f1.ToString();
                Preferences.Set("item_name", m3f1name);
                Preferences.Set("item_uid", m3f1uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "4")
            {
                TotalPrice.Text = "$" + m4price_f1.ToString();
                Preferences.Set("item_name", m4f1name);
                Preferences.Set("item_uid", m4f1uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }
        }

        private void clickedPayOp2(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            //payButton1.BackgroundColor = Color.Transparent;
            //payButton2.BackgroundColor = Color.FromHex("#FFF0C6");
            //payButton3.BackgroundColor = Color.Transparent;

            payButton1.Opacity = 0.3;
            payButton2.Opacity = 1;
            payButton3.Opacity = 0.3;

            Preferences.Set("freqSelected", "2");
            string meal_select = Preferences.Get("mealSelected", "");
            if (meal_select == "1")
            {
                TotalPrice.Text = "$" + m1price_f2.ToString();
                Preferences.Set("item_name", m1f2name);
                Preferences.Set("item_uid", m1f2uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "2")
            {
                TotalPrice.Text = "$" + m2price_f2.ToString();
                Preferences.Set("item_name", m2f2name);
                Preferences.Set("item_uid", m2f2uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "3")
            {
                TotalPrice.Text = "$" + m3price_f2.ToString();
                Preferences.Set("item_name", m3f2name);
                Preferences.Set("item_uid", m3f2uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "4")
            {
                TotalPrice.Text = "$" + m4price_f2.ToString();
                Preferences.Set("item_name", m4f2name);
                Preferences.Set("item_uid", m4f2uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else
            {
                TotalPrice.Text = "$00.00";
            }
        }

        private void clickedPayOp3(object sender, EventArgs e)
        {
            SignUpButton.Text = "CHECK PRICE";
            //payButton1.BackgroundColor = Color.Transparent;
            //payButton2.BackgroundColor = Color.Transparent;
            //payButton3.BackgroundColor = Color.FromHex("#FFF0C6");

            payButton1.Opacity = 0.3;
            payButton2.Opacity = 0.3;
            payButton3.Opacity = 1;

            Preferences.Set("freqSelected", "3");

            string meal_select = Preferences.Get("mealSelected", "");
            if (meal_select == "1")
            {
                TotalPrice.Text = "$" + m1price_f3.ToString();
                Preferences.Set("item_name", m1f3name);
                Preferences.Set("item_uid", m1f3uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "2")
            {
                TotalPrice.Text = "$" + m2price_f3.ToString();
                Preferences.Set("item_name", m2f3name);
                Preferences.Set("item_uid", m2f3uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "3")
            {
                TotalPrice.Text = "$" + m3price_f3.ToString();
                Preferences.Set("item_name", m3f3name);
                Preferences.Set("item_uid", m3f3uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
            else if (meal_select == "4")
            {
                TotalPrice.Text = "$" + m4price_f3.ToString();
                Preferences.Set("item_name", m4f3name);
                Preferences.Set("item_uid", m4f3uid);
                new_item_id = Preferences.Get("item_uid", "");
            }
        }

        private async void clickedDone(object sender, EventArgs e)
        {
            if (TotalPrice.Text == "$ TOTAL" || TotalPrice.Text == "$00.00" || TotalPrice.Text == "$0")
            {
                await DisplayAlert("Warning!", "pick a valid plan to continue", "OK");
                return;
            }

            if (SignUpButton.Text == "CHECK PRICE")
            {
                var request2 = new HttpRequestMessage();
                Debug.WriteLine("subscriptionModal trying to delete this purchase uid: " + purchase_id.ToString());
                request2.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/refund_calculator?purchase_uid=" + purchase_id);
                request2.Method = HttpMethod.Get;
                var client2 = new HttpClient();
                HttpResponseMessage response2 = await client2.SendAsync(request2);

                if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpContent content2 = response2.Content;
                    var userString2 = await content2.ReadAsStringAsync();
                    JObject refund_obj = JObject.Parse(userString2);

                    Debug.WriteLine("first start" + refund_obj["result"][0].ToString());
                    Debug.WriteLine("this is what I'm getting: " + refund_obj["result"][0]["refund_amount"].ToString());
                    double amt = Double.Parse(refund_obj["result"][0]["refund_amount"].ToString());
                    double currentPrice = Double.Parse(TotalPrice.Text.ToString().Substring(1));
                    double correct = amt - currentPrice;

                    if (correct < 0)
                    {
                        correct *= -1;
                        await DisplayAlert("Extra Charge", "You will be charged $" + correct.ToString() + " for this plan change.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Reimbursement", "You will be reimbursed $" + correct.ToString() + " for this plan change.", "OK");
                    }

                    SignUpButton.Text = "SAVE CHANGES";
                }
            }
            else
            {
                int length = (TotalPrice.Text).Length;
                string price = TotalPrice.Text.Substring(1, length - 1);
                Preferences.Set("price", price);

                Console.WriteLine("Price selected: " + price);

                PurchaseInfo2 updated = new PurchaseInfo2();

                //if direct login
                if (socialLogin == "NULL")
                {
                    var request2 = new HttpRequestMessage();
                    Console.WriteLine("user_id: " + (string)Application.Current.Properties["user_id"]);
                    string url2 = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/Profile/" + (string)Application.Current.Properties["user_id"];
                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + (string)Application.Current.Properties["user_id"];
                    //string url = "https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/meals_selected?customer_uid=" + "100-000256";
                    request2.RequestUri = new Uri(url2);
                    //request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/get_delivery_info/400-000453");
                    request2.Method = HttpMethod.Get;
                    var client2 = new HttpClient();
                    HttpResponseMessage response2 = await client2.SendAsync(request2);

                    if (response2.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        HttpContent content2 = response2.Content;
                        Console.WriteLine("content: " + content2);
                        var userString2 = await content2.ReadAsStringAsync();
                        var info_obj2 = JObject.Parse(userString2);

                        updated.password = (info_obj2["result"])[0]["password_hashed"].ToString();
                    }
                }
                else updated.password = "NULL";

                //updated.password = password;
                //updated.refresh_token = refresh_token;
                //updated.cc_num = cc_num;
                //testing
                updated.cc_num = "4242424242424242";
                updated.cc_exp_date = cc_exp_date;
                //updated.cc_exp_year = cc_exp_year;
                //updated.cc_exp_month = cc_exp_month;
                updated.cc_cvv = cc_cvv;
                updated.purchase_id = purchase_id;
                //updated.purchase_id = "400-000019";
                updated.new_item_id = new_item_id;
                updated.customer_email = cust_email;
                updated.cc_zip = cc_zip;
                updated.start_delivery_date = "";

                List<Item2> list1 = new List<Item2>();
                Item2 item1 = new Item2();
                item1.qty = Preferences.Get("freqSelected", "");
                if (Preferences.Get("mealSelected", "") == "1")
                {
                    item1.name = "5 Meal Plan";
                }
                else if (Preferences.Get("mealSelected", "") == "2")
                {
                    item1.name = "10 Meal Plan";
                }
                else if (Preferences.Get("mealSelected", "") == "3")
                {
                    item1.name = "15 Meal Plan";
                }
                else
                {
                    item1.name = "20 Meal Plan";
                }
                item1.price = Preferences.Get("price", "");
                item1.item_uid = updated.new_item_id;
                item1.itm_business_uid = itm_business_uid;
                list1.Add(item1);
                updated.items = list1;

                var newPaymentJSONString = JsonConvert.SerializeObject(updated);
                // Console.WriteLine("newPaymentJSONString" + newPaymentJSONString);
                var content = new StringContent(newPaymentJSONString, Encoding.UTF8, "application/json");
                Console.WriteLine("Content: " + content);
                /*var request = new HttpRequestMessage();
                request.RequestUri = new Uri("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/checkout");
                request.Method = HttpMethod.Post;
                request.Content = content;*/
                var client = new HttpClient();
                //var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase_id", content);
                var response = client.PostAsync("https://ht56vci4v9.execute-api.us-west-1.amazonaws.com/dev/api/v2/change_purchase", content);
                // HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine("RESPONSE TO CHECKOUT   " + response.Result);
                Console.WriteLine("CHECKOUT JSON OBJECT BEING SENT: " + newPaymentJSONString);
                Console.WriteLine("clickedDone Func ENDED!");

                Zones[] zones = new Zones[] { };
                await Navigation.PushAsync(new Select(zones, cust_firstName, cust_lastName, cust_email));
                //old nav
                //await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
                //Application.Current.MainPage = new DeliveryBilling();
                //await NavigationPage.PushAsync(DeliveryBilling());
            }
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new UserProfile();
        }

        async void clickedBack(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopAsync(false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
            //Application.Current.MainPage = new Menu();
        }
    }
}
