using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Views
{
    /// <summary>
    /// Interaction logic for AppLogin.xaml
    /// </summary>
    public partial class AppLogin : Page
    {

        public static AppLogin Instance;

        public AppLogin()
        {
            InitializeComponent();
            Refresh();
        }

        private void DoLogout(object sender, RoutedEventArgs e)
        {
            A.Trigger("logout");
            Refresh();
        }

        private void DoLogin(object sender, RoutedEventArgs e)
        {
            Trigger("login");
            A.Trigger("login", User.Text, Password.Password);
        }

        private void DoContinue(object sender, RoutedEventArgs e)
        {
            Trigger("connection");
            A.Trigger("continue");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            Version.Content = U.GetVersionString();
        }

        void Refresh()
        {
            if (U.HasAccountFile())
            {
                Login.Visibility = Visibility.Collapsed;
                Logout.Visibility = Visibility.Visible;
                User.Text = U.GetAccountName();
                Password.Password = "<<<PASSWORD>>>";
                Greeting.Content = "Welcome Back!";
                InfoText.Text = String.Empty;
                Progress.Visibility = Visibility.Hidden;

                Continue.Visibility = Visibility.Visible;
                Login.Visibility = Visibility.Collapsed;
                Logout.Visibility = Visibility.Visible;
            }
            else
            {
                Greeting.Content = "Please Login";

                User.Text = String.Empty;
                Password.Password = String.Empty;

                Continue.Visibility = Visibility.Collapsed;
                Login.Visibility = Visibility.Visible;
                Logout.Visibility = Visibility.Collapsed;
                Progress.Visibility = Visibility.Collapsed;
            }

        }

        void Trigger(String evt)
        {
            switch (evt)
            {
                case "login":
                {
                    Password.Visibility = Visibility.Visible;
                    ButtonSection.Visibility = Visibility.Collapsed;
                    User.IsEnabled = false;
                    Password.IsEnabled = false;
                    Progress.Visibility = Visibility.Visible;
                    Progress.Text = "Logging In...";
                }
                break;
                case "connection":
                {
                    Progress.Visibility = Visibility.Visible;
                    ButtonSection.Visibility = Visibility.Collapsed;
                    User.IsEnabled = false;
                    Password.IsEnabled = false;
                    Progress.Visibility = Visibility.Visible;
                    Progress.Content = "Connecting...";
                    InfoText.Text = String.Empty;
                }
                break;
                case "not_logged_in":
                {
                    Progress.Visibility = Visibility.Visible;
                    ButtonSection.Visibility = Visibility.Collapsed;
                    User.IsEnabled = true;
                    Password.IsEnabled = true;
                    Progress.Visibility = Visibility.Hidden;
                    Progress.Content = String.Empty;
                    InfoText.Text = "Please check your account details and internet connection";
                }
                break;
                case "logged_out":
                {
                    Refresh();
                }
               break;
            }

        }

    }
}
