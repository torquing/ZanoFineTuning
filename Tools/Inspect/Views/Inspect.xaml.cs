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

namespace ZanoFineTuning.Tools.Inspect.Views
{
    /// <summary>
    /// Interaction logic for Inspect.xaml
    /// </summary>
    public partial class Inspect : Page
    {
        public static Inspect Instance;

        public Inspect()
        {
            InitializeComponent();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            T.Trigger("menu");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;

            Library.SerialNumber serialNb = new Library.SerialNumber();

            StringBuilder sn = new StringBuilder(16);
            Library.GetSerialNumber(Z.Handle, ref serialNb);

            for (int i = 0; i < 8; i++)
            {
                sn.AppendFormat("{0:x2}", serialNb.Part[i]);
            }

            Header.Content = String.Format("Results for {0}", sn.ToString());

            LibZano.Version version = new LibZano.Version();
            Library.GetFirmwareVersion(Z.Handle, ref version);

            VersionHardware.Text = version.Hardware.ToString();
            VersionMajor.Text = version.Major.ToString();
            VersionMinor.Text = version.Minor.ToString();
            VersionRevision.Text = version.Revision.ToString();

            XValue.Text = Library.GetAccelOffsetConfiguration_X(Z.Handle).ToString();
            YValue.Text = Library.GetAccelOffsetConfiguration_Y(Z.Handle).ToString();
            ZValue.Text = Library.GetAccelOffsetConfiguration_Z(Z.Handle).ToString();

            XMinValue.Text = Library.GetAccelOffsetConfiguration_MinX(Z.Handle).ToString();
            YMinValue.Text = Library.GetAccelOffsetConfiguration_MinY(Z.Handle).ToString();
            ZMinValue.Text = Library.GetAccelOffsetConfiguration_MinZ(Z.Handle).ToString();

            XMaxValue.Text = Library.GetAccelOffsetConfiguration_MaxX(Z.Handle).ToString();
            YMaxValue.Text = Library.GetAccelOffsetConfiguration_MaxY(Z.Handle).ToString();
            ZMaxValue.Text = Library.GetAccelOffsetConfiguration_MaxZ(Z.Handle).ToString();
        }

        public void UpdateYaw(double yaw)
        {
            YawValue.Text = String.Format("{0:0.0}", yaw);
        }

        public void UpdateRoll(double roll)
        {
            RollValue.Text = String.Format("{0:0.0}", roll);
        }

        public void UpdatePitch(double pitch)
        {
            PitchValue.Text = String.Format("{0:0.0}", pitch);
        }

        public void UpdateAccelerometer(int x, int y, int z)
        {
            AccelXValue.Text = x.ToString();
            AccelYValue.Text = y.ToString();
            AccelZValue.Text = z.ToString();
        }
    }
}
