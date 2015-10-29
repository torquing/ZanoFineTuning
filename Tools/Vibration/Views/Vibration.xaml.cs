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
using ZanoFineTuning.Views.Controls;

namespace ZanoFineTuning.Tools.Vibration.Views
{
    /// <summary>
    /// Interaction logic for VibrationManual.xaml
    /// </summary>
    public partial class Vibration : Page
    {
        public static Vibration Instance = null;

        public Vibration()
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

            Progress.maximum = K.MaxReadings * 4;
            Progress.minimum = 0;
            Progress.Value = 0;

            Refresh();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;


        }
        
        Label GetLabel(int i)
        {
            switch (i)
            {
                case 0:
                    return Motor0;
                case 1:
                    return Motor1;
                case 2:
                    return Motor2;
                case 3:
                    return Motor3;
            }
            return Motor0;
        }

        public void Refresh()
        {
            for (int i = 0; i < 4; i++)
            {
                Label label = GetLabel(i);

                //if (G.Readings[i] >= K.MaxReadings)
                //{
                //    label.Visibility = Visibility.Visible;
                //    label.Content = String.Format("{0:0.0}", G.AvgRatings[i]);
                //}
                //else
                //{
                //    label.Visibility = Visibility.Hidden;
                //}

                var mr = G.GetMostRecentReading(i);

                if (mr != null && mr.Readings >= K.MaxReadings)
                {
                    label.Visibility = Visibility.Visible;
                    label.Content = String.Format("{0:0.0}", mr.Rating);
                }
                else
                {
                    label.Visibility = Visibility.Hidden;
                }

            }

            //Progress.Value = G.Readings[0] + G.Readings[1] + G.Readings[2] + G.Readings[3];
            Progress.maximum = G.TotalMotorReadings();
            Progress.Value = G.AddedMotorReadings();


            switch (Tools.Vibration.Vibration.Instance.State)
            {
                case "motors":
                {
                    var mr = G.CurrentMotorReading;
                    if (mr != null)
                    {
                    CurrentMotorName.Content = String.Format("{0}. {1}", mr.Motor + 1, U.MotorIdToName(mr.Motor));
                    CurrentMotorRating.Content = String.Format("{0:0.0}", mr.Rating);
                        Debug.Content = String.Format("X: {0:0000} Y:{1:0000}", mr.RangeX, mr.RangeY);
                    }
                    else
                    {
                    Debug.Content = String.Empty;
                    }
                }
                break;
                case "wait":
                {
                    CurrentMotorName.Content = "Waiting for things to settle";
                    CurrentMotorRating.Content = "Waiting";
                    // Debug.Content = String.Format("X: {0:0000}, Y: {1:0000}",  G.RangeX[4], G.RangeY[4]);
                    Debug.Content = String.Format("X: {0:0000} Y:{1:0000}", G.WaitReading.RangeX, G.WaitReading.RangeY);
                }
                break;
                case "stopped":
                {
                    CurrentMotorName.Content = "Stopped";
                    CurrentMotorRating.Content = "Stopped";
                }
                break;
                case "start":
                {
                    CurrentMotorName.Content = "Press Start when Ready";
                    CurrentMotorRating.Content = String.Empty;
                }
                break;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("stop");
            Start.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Collapsed;
            Retry.Visibility = Visibility.Visible;
            Menu.Visibility = Visibility.Visible;
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("test");
            Start.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Visible;
            Start.Visibility = Visibility.Collapsed;
            Menu.Visibility = Visibility.Visible;
        }

        private void Retry_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("retry");
            Start.Visibility = Visibility.Collapsed;
            Stop.Visibility = Visibility.Visible;
            Retry.Visibility = Visibility.Collapsed;
            Menu.Visibility = Visibility.Visible;
        }

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("menu");
        }
    }
}
