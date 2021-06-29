using LibraryWindow.classes.Api;
using LibraryWindow.classes.Api.Models;
using LibraryWindow.classes.Main;
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
    public partial class LoginPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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


        private string _email;
        public string MyEmail
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string MyPassword
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private bool _remember = false;
        public bool Remember
        {
            get { return _remember; }
            set { _remember = value; OnPropertyChanged(); }
        }


        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void Button_Pressed(object sender, EventArgs e)
        {
            LoginCredentials credentials = new LoginCredentials() { Email = MyEmail, Password = MyPassword, Overwride = false };
            string result = await ApiWrapper.Login(credentials);
            User.Logoutclick = false;

            if (result == "succes")
            {
                if (Remember)
                {
                    RememberMe();
                }
                Navigation.PushModalAsync(new NavigationPage(new MainPage()));


                //LibraryWindow libraryWindow = new LibraryWindow();
                //this.Close();
                //libraryWindow.Show();
            }
            else if (result == "active")
            {
                bool action = await App.Current.MainPage.DisplayAlert($"Opgelet!", "Er is al iemand anders ingelogd op dit account! Als u toch wilt inloggen wordt de ander van uw account afgezet. Let op! Dit kan nadelige gevolgen hebben voor uw account als de persoon die ingelogd is momenteel bezig is met een spel heb je het risico om je inzit kwijt te raken. Wilt u toch inloggen?", "Yes", "No");

                //MessageBoxResult mes = MessageBox.Show("Er is al iemand anders ingelogd op dit account! Als u toch wilt inloggen wordt de ander van uw account afgezet. Let op! Dit kan nadelige gevolgen hebben voor uw account als de persoon die ingelogd is momenteel bezig is met een spel heb je het risico om je inzit kwijt te raken. Wilt u toch inloggen?", "Inloggen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                //if (mes == MessageBoxResult.Yes)
                if (action)
                {
                    credentials = new LoginCredentials() { Email = MyEmail, Password = MyPassword, Overwride = true };
                    result = await ApiWrapper.Login(credentials);
                    if (Remember)
                    {
                        RememberMe();
                    }
                    Navigation.PushModalAsync(new NavigationPage(new MainPage()));


                    //LibraryWindow libraryWindow = new LibraryWindow();
                    //this.Close();
                    //libraryWindow.ShowDialog();
                }
            }
            else
            {
                await DisplayAlert("Alert","Gebruikersnaam of wachtwoord is incorrect.","OK");
            }

            //Navigation.PushModalAsync(new NavigationPage(new MainPage()));
        }

        private async void RememberMe()
        {
            //Properties.Settings.Default.Username = MyEmail;
            //Properties.Settings.Default.Password = MyPassword;
            //Properties.Settings.Default.Save();
            await DisplayAlert("Alert", "Remember me", "OK");

        }

        [Obsolete]
        private void Passreset_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/wachtwoord"));
        }

        [Obsolete]
        private void Register_Pressed(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://stonkscasino.nl/public/register"));
        }
    }
}