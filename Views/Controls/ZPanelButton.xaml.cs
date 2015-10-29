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

namespace ZanoFineTuning.Views.Controls
{
    /// <summary>
    /// Interaction logic for ZPanelButton.xaml
    /// </summary>
    public partial class ZPanelButton : UserControl
    {
        public ZPanelButton()
        {
            InitializeComponent();
        }

        public String Header
        {
            set { _Header.Content = value; }
        }

        public String Description
        {
            set { _Description.Text = value; }
        }
    }
}
