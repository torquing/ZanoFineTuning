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

namespace ZanoFineTuning.Tools.ResetFirmware.Views
{
    /// <summary>
    /// Interaction logic for ResetFirmware.xaml
    /// </summary>
    public partial class ResetFirmware : Page
    {
        public ResetFirmware()
        {
            InitializeComponent();
        }

        private void DoCancel(object sender, RoutedEventArgs e)
        {
            T.Trigger("cancel");
        }

        private void DoFlash(object sender, RoutedEventArgs e)
        {
            T.Trigger("flash");
        }
    }
}
