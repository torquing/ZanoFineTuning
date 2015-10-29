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

namespace ZanoFineTuning.Tools.Accelerometer
{
    /// <summary>
    /// Interaction logic for Complete.xaml
    /// </summary>
    public partial class Complete : Page
    {
        public Complete()
        {
            InitializeComponent();

            Header.Content = String.Format("Results for {0}", G.ResultsSerial);

            ConfigValue.Text = G.ResultsValues;
            XValue.Text = G.ResultsXAvg.ToString();
            YValue.Text = G.ResultsYAvg.ToString();
            ZValue.Text = G.ResultsZAvg.ToString();

            XMinValue.Text = G.ResultsXMin.ToString();
            YMinValue.Text = G.ResultsYMin.ToString();
            ZMinValue.Text = G.ResultsZMin.ToString();

            XMaxValue.Text = G.ResultsXMax.ToString();
            YMaxValue.Text = G.ResultsYMax.ToString();
            ZMaxValue.Text = G.ResultsZMax.ToString();

        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("menu");
        }
    }
}
