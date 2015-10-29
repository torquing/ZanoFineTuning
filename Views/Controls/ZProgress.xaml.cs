using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ZProgress.xaml
    /// </summary>
    public partial class ZProgress : UserControl
    {

        public int maximum, minimum;
        private int progressValue;
        private SolidColorBrush BrushNormal, BrushComplete;

        public ZProgress()
        {
            InitializeComponent();
            
            BrushNormal = new SolidColorBrush(Color.FromRgb(0xfa, 0xb8, 0x00));
            BrushComplete = new SolidColorBrush(Color.FromRgb(0x64, 0xfc, 0x80));


            maximum = 1000;
            minimum  = 0;
            progressValue = minimum;
        }
        
        public int Value
        {
            get
            {
                return progressValue;
            }

            set
            {
                this.progressValue = value;
                Refresh();
            }
        }

        public void Refresh()
        {
            if (progressValue == 0)
            {
                Bar.Width = 0;
            }
            else
            {
                double m = ( (double)progressValue / maximum) * Border.ActualWidth;
                Bar.Width = m;
            Console.WriteLine("{0} out of {1}, Size is = {2} / {3}", progressValue, maximum, m, Border.ActualWidth);
            }


            if (progressValue >= maximum)
            {
                Bar.Fill = BrushComplete;
            }
            else
            {
                Bar.Fill = BrushNormal;
            }
        }

    }
}
