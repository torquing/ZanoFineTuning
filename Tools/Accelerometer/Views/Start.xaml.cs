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
using System.Windows.Threading;
using ZanoFineTuning.Core;
using ZanoFineTuning.Tools.Accelerometer.Views.Controls;

namespace ZanoFineTuning.Tools.Accelerometer.Views
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {

        private DispatcherTimer demoTimer;
        private int demoCount;
        private SolidColorBrush demoComplete, demoSelected;
        private bool demoSwitch;

        public Start()
        {
            InitializeComponent();
            FlipView.HideControlButtons();
            demoTimer = new DispatcherTimer();
            demoTimer.Interval = new TimeSpan(0,0,0,0,40);
            demoTimer.Tick += updateDemo;

            demoComplete = new SolidColorBrush(Color.FromRgb(0x64, 0xfc, 0x80));
            demoSelected = new SolidColorBrush(Color.FromRgb(0xfa, 0xb8, 0x00));
        }
        
        private void StartCalibration_OnClick(object sender, RoutedEventArgs e)
        {
            T.Trigger("connect");
        }
        
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PreviousTab.IsSelected)
            {
                Refresh();
            }
        }

        void Refresh()
        {
            Recordings.Children.Clear();
            foreach (var recording in G.Recordings)
            {
                var rc = new RecordingControl();
                rc.Id.Content = recording.Id.ToString();
                rc.SerialNumber.Content = Z.SerialNumberToString(recording.SerialNumber);
                rc.DateTime.Content = recording.DateTime.ToString();
                rc.Apply.Click += (sender, args) =>
                {
                    T.Trigger("apply_recording", recording);
                };

                Recordings.Children.Add(rc);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StopDemo();
        }

        void StartDemo()
        {
            if (demoTimer.IsEnabled)
                return;

            DemoProgress.maximum = K.MinReadings;
            DemoProgress.minimum = 0;
            DemoProgress.Value = 0;
            DemoAnimation.SetSide();

            demoTimer.Start();
            demoSwitch = false;
            DemoAxis.BorderBrush = demoSelected;
        }

        void StopDemo()
        {
            if (demoTimer.IsEnabled == false)
                return;

            DemoProgress.Value = 0;
            DemoAnimation.SetSide();

            demoTimer.Stop();
            demoSwitch = false;
            DemoAxis.BorderBrush = demoSelected;
        }

        private void updateDemo(object sender, EventArgs eventArgs)
        {
            if (demoCount++ > K.MinReadings)
            {
                if (demoSwitch == false)
                {
                    demoSwitch = true;
                    DemoAnimation.PlaySideToSideCCW();
                    DemoAxis.BorderBrush = demoComplete;
                }
                else
                {
                    if (DemoAnimation.IsAnimating == false)
                    {
                        demoSwitch = false;

                        demoCount = 0;
                        DemoProgress.Value = 0;
                        DemoAxis.BorderBrush = demoSelected;
                    }
                }
            }
            else
            {
                DemoProgress.Value = demoCount;
            }
        }

        private void DoNext(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex = FlipView.SelectedIndex + 1;

            if (FlipView.SelectedIndex == 7)
            {
                StartDemo();
            }
            else
            {
                StopDemo();
            }
        }

        private void DoBack(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex = FlipView.SelectedIndex - 1;
            if (FlipView.SelectedIndex == 7)
            {
                StartDemo();
            }
            else
            {
                StopDemo();
            }
        }

    }
}
