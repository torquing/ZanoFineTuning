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

namespace ZanoFineTuning.Tools.Vibration.Views
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
            FlipView.HideControlButtons();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (PreviousTab.IsSelected)
            //{
            //    Refresh();
            //}
        }

        void Refresh()
        {
            //Recordings.Children.Clear();
            /*
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
            */
        }

        private void DoNext(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex = FlipView.SelectedIndex + 1;
            
        }

        private void DoBack(object sender, RoutedEventArgs e)
        {
            FlipView.SelectedIndex = FlipView.SelectedIndex - 1;
        }

        private void DoStartTest(object sender, RoutedEventArgs e)
        {
            T.Trigger("connect");

        }
    }
}
