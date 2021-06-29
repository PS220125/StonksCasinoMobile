﻿using LibraryWindow.Classes;
using LibraryWindow.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using LibraryWindow.Classes;
using LibraryWindow.Classes.Main;
using LibraryWindow.classes.Main;
using LibraryWindow.classes.Api;

namespace LibraryWindow
{
    public partial class MainPage : ContentPage
    {
        MenuButtons _menuButton = new MenuButtons();

        public string Username
        {
            get { return User.Username; }
        }

        public int Tokens
        {
            get { return User.Tokens; }
        }

        bool blnShouldStay = true;
        protected override bool OnBackButtonPressed()
        {
            if (blnShouldStay)
            {
                // Yes, we want to stay.
                return true;
            }
            else
            {
                // It's okay, we can leave.
                base.OnBackButtonPressed();
                return false;
            }
        }

        ObservableCollection<CarouselMenuItem> _carouselMenuItems = new ObservableCollection<Classes.CarouselMenuItem>()
        {
            new CarouselMenuItem(ImageSource.FromResource("LibraryWindow.Images.Main.blackjacklogo.png"),new BlackJackPage()),
            new CarouselMenuItem(ImageSource.FromResource("LibraryWindow.Images.Main.liarsdice.png"),new LiarDicePage()),
            new CarouselMenuItem(ImageSource.FromResource("LibraryWindow.Images.Main.seven.png"),new SlotmachinePage()),
            new CarouselMenuItem(ImageSource.FromResource("LibraryWindow.Images.Main.wheel.png"),new WheelOfFortune()),
        };

        public ObservableCollection<CarouselMenuItem> CarouselMenuItems
        {
            get { return _carouselMenuItems; }
        }

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
            Account();

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                OnPropertyChanged("Tokens");
                ApiWrapper.GetUserInfo();
                return true; // True = Repeat again, False = Stop the timer
            });
        }


        private void ImageButton_Pressed(object sender, EventArgs e)
        {
            ImageButton button = sender as ImageButton;
            CarouselMenuItem carouselMenuItem = button.BindingContext as CarouselMenuItem;
            Navigation.PushAsync(carouselMenuItem.MyContentPage);
        }

        private async void Account()
        {
            bool result = await ApiWrapper.GetUserInfo();
            OnPropertyChanged("Username");
            OnPropertyChanged("Tokens");
            if (!result)
            {
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        private async void Uitloggen_Pressed(object sender, EventArgs e)
        {

            _menuButton.Uitloggen();

            bool logout = await User.LogoutAsync();
            if (logout)
            {
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        [Obsolete]
        private void Store_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/winkel"));
        }

        [Obsolete]
        private void Deposit_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/account-info"));
        }
    }
}