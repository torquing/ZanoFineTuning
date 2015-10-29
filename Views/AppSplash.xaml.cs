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
    /// Interaction logic for AppSplash.xaml
    /// </summary>
    public partial class AppSplash : Page
    {
        public AppSplash()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            A.Trigger("initialise");
        }
    }
}
