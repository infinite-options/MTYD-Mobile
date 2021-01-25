﻿using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTYD.ViewModel
{
    public partial class Landing : ContentPage
    {
        string cust_firstName; string cust_lastName; string cust_email;

        public Landing(string firstName, string lastName, string email)
        {
            cust_firstName = firstName;
            cust_lastName = lastName;
            cust_email = email;
            InitializeComponent();

            var width = DeviceDisplay.MainDisplayInfo.Width;
            var height = DeviceDisplay.MainDisplayInfo.Height;
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            checkPlatform(height, width);
        }

        public void checkPlatform(double height, double width)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                orangeBox.HeightRequest = height / 2;
                orangeBox.Margin = new Thickness(0, -height / 2.2, 0, 0);
                orangeBox.CornerRadius = height / 40;
                logo.HeightRequest = width / 15;
                logo.Margin = new Thickness(0, 0, 0, 30);
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

                //topGrid.HeightRequest = height / 1.5;
                landingPic.Margin = new Thickness(0, -height / 30, 0, 0);
                //topStack.Margin = new Thickness(0, height / 20, 0, 0);
                mainLogo.HeightRequest = height / 18;
                getStarted.HeightRequest = height / 35;
                getStarted.CornerRadius = (int)(height / 70);
                getStarted.Margin = new Thickness(width / 7, 0);

                searchPic.Margin = new Thickness(width / 5.5, 0);
                first.FontSize = width / 30;
                first.HeightRequest = width / 15;
                first.WidthRequest = width / 15;
                first.CornerRadius = (int)(width / 30);
                first.Margin = new Thickness(0, 0, 3, 0);
                step1.Margin = new Thickness(3, 0, 0, 0);
                step1.FontSize = width / 30;
                sub1.FontSize = width / 45;

                cardPic.Margin = new Thickness(width / 5.5, 0);
                second.FontSize = width / 30;
                second.HeightRequest = width / 15;
                second.WidthRequest = width / 15;
                second.CornerRadius = (int)(width / 30);
                second.Margin = new Thickness(0, 0, 3, 0);
                step2.Margin = new Thickness(3, 0, 0, 0);
                step2.FontSize = width / 30;
                sub2.FontSize = width / 45;

                pickPic.Margin = new Thickness(width / 5.5, 0);
                third.FontSize = width / 30;
                third.HeightRequest = width / 15;
                third.WidthRequest = width / 15;
                third.CornerRadius = (int)(width / 30);
                third.Margin = new Thickness(0, 0, 3, 0);
                step3.Margin = new Thickness(3, 0, 0, 0);
                step3.FontSize = width / 30;
                sub3.FontSize = width / 45;

                delivPic.Margin = new Thickness(width / 5.5, 0);
                fourth.FontSize = width / 30;
                fourth.HeightRequest = width / 15;
                fourth.WidthRequest = width / 15;
                fourth.CornerRadius = (int)(width / 30);
                fourth.Margin = new Thickness(0, 0, 3, 0);
                step4.Margin = new Thickness(3, 0, 0, 0);
                step4.FontSize = width / 30;
                sub4.FontSize = width / 45;

                fade.Margin = new Thickness(0, -height / 3, 0, 0);

                xButton.FontSize = width / 37;
                DiscountGrid.Margin = new Thickness(width / 30, height / 9, width / 30, 0);
                DiscountGrid.HeightRequest = height / 5.2;
                DiscountGrid.WidthRequest = width / 2.3;
                couponGrid.HeightRequest = height / 5.2;
                couponGrid.WidthRequest = width / 2.3;

                outerFrame.HeightRequest = height / 5.2;
                //innerFrame.HeightRequest = (height / 5.2) - 15;
                //innerFrame.WidthRequest = outerFrame.Height - 60;

                discHeader.FontSize = width / 35;

                couponGrid.HeightRequest = height / 30;
                couponImg.HeightRequest = height / 30;
                couponAmt.FontSize = width / 50;
                couponDesc.FontSize = width / 43;

                couponGrid2.HeightRequest = height / 30;
                couponImg2.HeightRequest = height / 30;
                couponAmt2.FontSize = width / 50;
                couponDesc2.FontSize = width / 43;
            }
        }

        async void clickedX(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = false;
            DiscountGrid.IsVisible = false;
        }

        async void clickedDiscounts(System.Object sender, System.EventArgs e)
        {
            fade.IsVisible = true;
            DiscountGrid.IsVisible = true;
        }

        async void clickedPfp(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new UserProfile(cust_firstName, cust_lastName, cust_email), false);
        }

        async void clickedMenu(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Menu(cust_firstName, cust_lastName, cust_email));
        }

        async void clickedStarted(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage(cust_firstName, cust_lastName, cust_email));
        }
    }
}
