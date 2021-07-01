﻿using LibraryWindow.classes.Api;
using LibraryWindow.classes.Main;
using LibraryWindow.Classes.Slotmachine;
using LibraryWindow.Classes.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LibraryWindow.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlotmachinePage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Username
        {
            get { return User.Username; }
        }

        public int Tokens
        {
            get { return User.Tokens; }
        }

        public SlotmachinePage()
        {
            InitializeComponent();
            BindingContext = this;
            Account();

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                OnPropertyChanged("Tokens");
                try
                {
                    ApiWrapper.GetUserInfo();
                }
                catch
                {
                    DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                    Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
                return true;
            });
        }

        private async void Account()
        {
            try
            {
                bool result = await ApiWrapper.GetUserInfo();
                OnPropertyChanged("Username");
                OnPropertyChanged("Tokens");
                if (!result)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
            }
        }

        private async void Uitloggen_Pressed(object sender, EventArgs e)
        {
            try
            {
                bool logout = await User.LogoutAsync();
                if (logout)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new LoginPage()));
                }
            }
            catch
            {
                await DisplayAlert("Check internet", "Mogelijk is de internetverbinding verbroken.", "OK");
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




        private const string _sender = "Slotmachine";

        private Slotmachine _slotmachine = new Slotmachine();

        public Slotmachine Slotmachine
        {
            get { return _slotmachine; }
            set { _slotmachine = value; OnPropertyChanged(); }
        }


        private bool _allowedToClick = true;

        public bool AllowedToClick
        {
            get { return _allowedToClick; }
            set { _allowedToClick = value; OnPropertyChanged(); }
        }

        private async void BtnStart_Pressed(object sender, EventArgs e)
        {
            DisableButtons();

            await Slotmachine.Activate();

            int winnings = Slotmachine.CheckWin();
            if (winnings > 0)
            {
                //MessageBox.Show($"u heeft {winnings} gewonnen");
                await ApiWrapper.UpdateTokens(winnings, _sender);
            }
            Check();
        }
        private void DisableButtons()
        {
            AllowedToClick = false;
        }
        public void Check()
        {
            if (User.Tokens < 100)
            {
                AllowedToClick = false;
            }
            else
            {
                AllowedToClick = true;
            }
        }
    }
}