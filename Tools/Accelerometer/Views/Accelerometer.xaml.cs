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

namespace ZanoFineTuning.Tools.Accelerometer.Views
{
    /// <summary>
    /// Interaction logic for Accelerometer.xaml
    /// </summary>
    public partial class Accelerometer : Page
    {
        public static Accelerometer Instance = null;
        private SolidColorBrush FilterComplete, FilterSelected, FilterIncomplete;
        private Brush ProgressNormalColour;

        public Accelerometer()
        {
            InitializeComponent();

            FilterComplete = new SolidColorBrush(Color.FromRgb(0x64, 0xfc, 0x80));
            FilterSelected = new SolidColorBrush(Color.FromRgb(0xfa, 0xb8, 0x00));
            FilterIncomplete = new SolidColorBrush(Color.FromRgb(0xdc, 0x31, 0x31));
           // ProgressNormalColour = Progress.Foreground;

            for (int i = 0; i < 6; i++)
            {
                var axisLabel = GetAxisLabel(i);
                axisLabel.BorderBrush = FilterIncomplete;
                axisLabel.Content = "x";
            }
            
            Progress.maximum = Tools.Accelerometer.K.MinReadings - 1;
            Progress.Value = 0;

            Start.Visibility = Visibility.Visible;
            Retry.Visibility = Visibility.Collapsed;
            Save.Visibility = Visibility.Collapsed;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("start");
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("retry");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("save");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            Progress.maximum = Tools.Accelerometer.K.MinReadings - 1;
            Refresh();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }

        public void Restart()
        {
            Start.Visibility = Visibility.Visible;
            Retry.Visibility = Visibility.Collapsed;
            Save.Visibility = Visibility.Collapsed;
        }

        public void Begin()
        {
            Start.Visibility = Visibility.Collapsed;
            Retry.Visibility = Visibility.Visible;
            Save.Visibility = Visibility.Collapsed;
        }

        public void Refresh()
        {
            RefreshProgress();
            bool complete = true;
            for (int i = 0; i < 6; i++)
            {
                RefreshAxis(i);
                if (G.FilteredReadings[i] < Tools.Accelerometer.K.MinReadings)
                    complete = false;
            }

            //Save.Visibility = complete ? Visibility.Visible : Visibility.Hidden;
            Save.Visibility = Visibility.Visible;
            

            if (U.AxisChanged)
            {
                int from = U.AxisToSide(G.LastAxisX, G.LastAxisY, G.LastAxisZ);
                int to = U.AxisToSide(G.AxisX, G.AxisY, G.AxisZ);

                if (from == 0 && to == 1)
                {
                    Animation.PlayUpToSide();
                }

                else if (from == 1 && to == 0)
                {
                    Animation.PlaySideToUp();
                }

                else if (from == 1 && to == 1)
                {
                    int direction = U.AxisXYDirection(G.LastAxisX, G.LastAxisY, G.AxisX, G.AxisY);
                    if (direction == 1)
                        Animation.PlaySideToSideCCW();
                    else
                        Animation.PlaySideToSideACW();
                }

            }

        }

        private void RefreshProgress()
        {
            int axisIndex = U.AxisToIndex(G.AxisX, G.AxisY, G.AxisZ);


            if (axisIndex == 6)
            {
                //Progress.Visibility = Visibility.Hidden;
                //Complete.Visibility = Visibility.Collapsed;
                return;
            }

            int readings = G.FilteredReadings[axisIndex];

            Progress.Value = readings;
            
            Console.WriteLine("{0},{1} => {2}", Progress.minimum, Progress.maximum, Progress.Value);
            return;
/*
            int readings = G.FilteredReadings[axisIndex];

            if (readings >= Tools.Accelerometer.K.MinReadings)
            {
                //Progress.Visibility = Visibility.Collapsed;
                //Complete.Visibility = Visibility.Visible;
            }
            else
            {
                //Progress.Visibility = Visibility.Visible;
                //Complete.Visibility = Visibility.Collapsed;
            }

            switch (U.AccelerometerReadState(readings, Tools.Accelerometer.K.MinReadings))
            {
                case ReadState.NotStarted:
                case ReadState.Starting:
                    {
                        Progress.Value = 0;
                    }
                    break;
                case ReadState.Good:
                case ReadState.Excellent:
                case ReadState.Poor:
                    {
                    }
                    break;
            }*/
        }

        private void RefreshAxis(int axisIndex)
        {
            Label axisLabel = GetAxisLabel(axisIndex);
            if (axisLabel == null)
                return;

            bool isComplete = G.FilteredComplete[axisIndex];

            if (isComplete)
            {
                axisLabel.BorderBrush = FilterComplete;
                axisLabel.Content = String.Empty;
                return;
            }

            int readings = G.FilteredReadings[axisIndex];

            switch (U.AccelerometerReadState(readings, Tools.Accelerometer.K.MinReadings))
            {
                case ReadState.NotStarted:
                    {
                        axisLabel.BorderBrush = FilterIncomplete;
                        axisLabel.Content = U.AxisFaceShorthand(axisIndex);
                    }
                    break;
                case ReadState.Starting:
                    {
                        axisLabel.BorderBrush = FilterIncomplete;
                        axisLabel.Content = U.AxisFaceShorthand(axisIndex);
                    }
                    break;
                case ReadState.Poor:
                    {
                        axisLabel.BorderBrush = FilterSelected;
                        axisLabel.Content = U.AxisFaceShorthand(axisIndex);
                    }
                    break;
                case ReadState.Good:
                case ReadState.Excellent:
                    {
                        axisLabel.BorderBrush = FilterComplete;
                        axisLabel.Content = String.Empty;
                    }
                    break;
            }
        }

        private Label GetAxisLabel(int axisIndex)
        {
            switch (axisIndex)
            {
                case 0:
                    return Axis0;
                case 1:
                    return Axis1;
                case 2:
                    return Axis2;
                case 3:
                    return Axis3;
                case 4:
                    return Axis4;
                case 5:
                    return Axis5;
            }
            return null;
        }

    }
}
