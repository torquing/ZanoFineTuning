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

namespace ZanoFineTuning.Tools.VibrationManual.Views
{
    /// <summary>
    /// Interaction logic for VibrationManual.xaml
    /// </summary>
    public partial class VibrationManual : Page
    {
        public static VibrationManual Instance = null;

        public VibrationManual()
        {
            InitializeComponent();
            

        }

        private void OpOnClick(object sender, RoutedEventArgs e)
        {
            short motor0 = (short)(double.Parse(Motor0.Text) * 2048.0);
            short motor1 = (short)(double.Parse(Motor1.Text) * 2048.0);
            short motor2 = (short)(double.Parse(Motor2.Text) * 2048.0);
            short motor3 = (short)(double.Parse(Motor3.Text) * 2048.0);

            if (motor0 < 64 && motor0 >= 0)
                motor0 = 10;

            if (motor1 < 64 && motor1 >= 0)
                motor1 = 10;

            if (motor2 < 64 && motor2 >= 0)
                motor2 = 10;

            if (motor3 < 64 && motor3 >= 0)
                motor3 = 10;
            
            // temp

            byte enableDebug = 1 << 0;
            byte enableSonar = 1 << 1;
            byte enableIr = 1 << 2;
            byte enabledMotors = 1 << 3;

            Frames.SystemDebugSet(Z.Handle, (byte) (enableDebug | enabledMotors), motor0, motor1, motor2, motor3);
            Console.WriteLine("{0} {1} {2} {3}", motor0, motor1, motor2, motor3);
        }

        public void Refresh()
        {
            XValue.Text = String.Format("{0:0}", G.AccelerationNowX);
            YValue.Text = String.Format("{0:0}", G.AccelerationNowY);
            RangeX.Text = String.Format("{0:0}", G.RangeX);
            RangeY.Text = String.Format("{0:0}", G.RangeY);
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
