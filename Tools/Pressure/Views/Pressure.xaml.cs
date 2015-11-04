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

namespace ZanoFineTuning.Tools.Pressure.Views
{
    /// <summary>
    /// Interaction logic for Pressure.xaml
    /// </summary>
    public partial class Pressure : Page
    {

        public static Pressure Instance;

        public Pressure()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;

            Start.Visibility = Visibility.Visible;
            Stop.Visibility = Visibility.Collapsed;
            Retry.Visibility = Visibility.Collapsed;
            Menu.Visibility = Visibility.Visible;

            Progress.maximum = K.MaxReadings;
            Progress.minimum = 0;
            Progress.Value = 0;

            Refresh();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }
        
        public void Refresh()
        {

            double v = 0.0f;

            if (G.CurrentPressureReading != null)
            {
                v = G.CurrentPressureReading.RangeAltitude;
                Progress.maximum = K.MaxReadings;
                Progress.Value = G.CurrentPressureReading.Readings;
            }
            else
            {
                Progress.maximum = K.MaxReadings;
                Progress.Value = 0;
            }

            CurrentAltitudeRange.Content = String.Format("{0:0.000}", v);

            switch (Tools.Pressure.Pressure.Instance.State)
            {
                case "baro":
                {
                    Start.Visibility = Visibility.Collapsed;
                    Stop.Visibility = Visibility.Visible;
                    Retry.Visibility = Visibility.Collapsed;
                    Menu.Visibility = Visibility.Visible;
                }
                break;
                case "complete":
                {
                    Start.Visibility = Visibility.Collapsed;
                    Stop.Visibility = Visibility.Collapsed;
                    Retry.Visibility = Visibility.Visible;
                    Menu.Visibility = Visibility.Visible;
                }
                break;
                case "stopped":
                {
                    Start.Visibility = Visibility.Collapsed;
                    Stop.Visibility = Visibility.Collapsed;
                    Retry.Visibility = Visibility.Visible;
                    Menu.Visibility = Visibility.Visible;
                }
                break;
                case "start":
                {
                    Start.Visibility = Visibility.Visible;
                    Stop.Visibility = Visibility.Collapsed;
                    Retry.Visibility = Visibility.Collapsed;
                    Menu.Visibility = Visibility.Visible;
                }
                break;
            }
            
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
             T.Trigger("stop");
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("test");
        }

        private void Retry_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("retry");
        }

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("menu");
        }
    }
}
