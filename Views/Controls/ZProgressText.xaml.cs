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
    /// Interaction logic for ZProgressText.xaml
    /// </summary>
    public partial class ZProgressText : UserControl
    {
        public ZProgressText()
        {
            InitializeComponent();
        }

        public String Text
        {
            set { _Text.Content = value; }
        }

        public HorizontalAlignment TextAlignment
        {
            set { _StackPanel.HorizontalAlignment = value; }
            get { return _StackPanel.HorizontalAlignment; }
        }
        
    }
}
