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
using LibZano;
using ZanoFineTuning.Core;
using ZanoFineTuning.Views.Controls;

namespace ZanoFineTuning.Views
{
    /// <summary>
    /// Interaction logic for AppMenu.xaml
    /// </summary>
    public partial class AppMenu : Page
    {
        public static AppMenu Instance;

        public AppMenu()
        {
            InitializeComponent();
            foreach (var t in T.Tools)
            {
                var pb = new ZPanelButton();
                pb.Header = t.Value.title;
                pb.Description = t.Value.description;
                pb.Button.Click += (sender, args) =>
                {
                    T.Set(t.Key);
                };

                if (t.Value.category == "Tools")
                    Tools.Children.Add(pb);
                else if (t.Value.category == "Calibration")
                    Calibration.Children.Add(pb);
                else if (t.Value.category == "Diagnostics")
                    Diagnostics.Children.Add(pb);
            }
        }

        public void Trigger(String name, params object[] args)
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }
    }
}