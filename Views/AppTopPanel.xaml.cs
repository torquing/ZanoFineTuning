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
    /// Interaction logic for AppTopPanel.xaml
    /// </summary>
    public partial class AppTopPanel : Page
    {
        public static AppTopPanel Instance;

        private BackButton backButton;

        public AppTopPanel()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            ClearBackButton();
            Trigger("inactive");
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }

        public void SetBackButton(BackButton backButton)
        {
            if (backButton == null)
            {
                ClearBackButton();
                return;
            }

            this.backButton = backButton;
            BackButton.Visibility = Visibility.Visible;
        }

        public void ClearBackButton()
        {
            backButton = null;
            BackButton.Visibility = Visibility.Hidden;
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (backButton != null)
            {
                A.Trigger(backButton.Event, backButton.Args);
                T.Trigger(backButton.Event, backButton.Args);
            }
        }

        public void Trigger(String state, params object[] args)
        {
            switch (state)
            {
                case "not_connected":
                {
                    Internet.Visibility = Visibility.Hidden;
                    ConnectProcess.Visibility = Visibility.Hidden;
                    ConnectProcess.IsActive = false;
                    Disconnected.Visibility = Visibility.Hidden;
                        Connected.Visibility = Visibility.Hidden;
                        BatteryCharge.Visibility = Visibility.Hidden;
                    Battery0.Visibility = Visibility.Hidden;
                    Battery1.Visibility = Visibility.Hidden;
                    Battery2.Visibility = Visibility.Hidden;
                    Battery3.Visibility = Visibility.Hidden;
                    Battery4.Visibility = Visibility.Hidden;
                    ConnectName.Visibility = Visibility.Hidden;
                }
                break;
                case "connecting":
                {
                    ConnectProcess.Visibility = Visibility.Visible;
                    ConnectProcess.IsActive = true;
                }
                break;
                case "internet":
                {
                    Internet.Visibility = Visibility.Visible;
                    ConnectProcess.Visibility = Visibility.Hidden;
                    ConnectProcess.IsActive = false;
                    Disconnected.Visibility = Visibility.Hidden;
                        Connected.Visibility = Visibility.Hidden;
                        BatteryCharge.Visibility = Visibility.Hidden;
                    Battery0.Visibility = Visibility.Hidden;
                    Battery1.Visibility = Visibility.Hidden;
                    Battery2.Visibility = Visibility.Hidden;
                    Battery3.Visibility = Visibility.Hidden;
                    Battery4.Visibility = Visibility.Hidden;
                    ConnectName.Visibility = Visibility.Visible;
                    ConnectName.Content = args[0].ToString();
                }
                break;
                case "connected":
                {
                    Internet.Visibility = Visibility.Hidden;
                    ConnectProcess.Visibility = Visibility.Hidden;
                    ConnectProcess.IsActive = false;
                    Disconnected.Visibility = Visibility.Hidden;

                        Connected.Visibility = Visibility.Hidden;
                        Battery0.Visibility = Visibility.Hidden;
                        Battery1.Visibility = Visibility.Hidden;
                        Battery2.Visibility = Visibility.Hidden;
                        Battery3.Visibility = Visibility.Hidden;
                        Battery4.Visibility = Visibility.Hidden;

                        int battery = Z.GetSimpleBatteryCharge((int) args[0]);

                    switch (battery)
                    {
                        case Int32.MaxValue:
                        {
                            BatteryCharge.Visibility = Visibility.Visible;
                        }
                        break;
                        case 0:
                        {
                            Connected.Visibility = Visibility.Visible;
                        }
                        break;
                        case 1:
                        {
                            Battery0.Visibility = Visibility.Visible;
                        }
                        break;
                        case 2:
                        {
                            Battery0.Visibility = Visibility.Hidden;
                            Battery1.Visibility = Visibility.Visible;
                        }
                            break;
                        case 3:
                        {
                            Battery2.Visibility = Visibility.Visible;
                        }
                            break;
                        case 4:
                        {
                            Battery3.Visibility = Visibility.Visible;
                        }
                        break;
                        case 5:
                        {
                            Battery4.Visibility = Visibility.Visible;
                        }
                        break;
                    }
                    ConnectName.Visibility = Visibility.Visible;
                    ConnectName.Content = args[1].ToString();
                }
                break;
            }
        }
    }
}
