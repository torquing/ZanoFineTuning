using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace ZanoFineTuning.Views.Controls
{
    /// <summary>
    /// Interaction logic for ZConnectingProgress.xaml
    /// </summary>
    public partial class ZConnectingProgress : UserControl
    {
        const int kFrameSpeed = 1;

        private BitmapImage[] Frames;
        private DispatcherTimer Timer;


        private int FrameA, FrameB, FrameIndex;


        public ZConnectingProgress()
        {
            InitializeComponent();
            Frames = new BitmapImage[3];

            Frames[0] = LoadBitmapFromResource("Views/Resources/Zano_Blink_None.png");
            Frames[1] = LoadBitmapFromResource("Views/Resources/Zano_Blink_Red.png");
            Frames[2] = LoadBitmapFromResource("Views/Resources/Zano_Blink_Blue.png");

            FrameA = 0;
            FrameB = 0;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / kFrameSpeed);
            Timer.Tick += Animate;

        }
        private void Animate(object sender, EventArgs e)
        {
            if (FrameA == FrameB)
                return;

            int fr = FrameIndex == 1 ? FrameB : FrameA;
            FrameIndex = FrameIndex == 1 ? 0 : 1;

            _Image.Source = Frames[fr];
        }

        public void FlashNone()
        {
            FrameA = 0;
            FrameB = 0;
            FrameIndex = 1;
            _Image.Source = Frames[FrameA];
        }

        public void FlashRed()
        {
            FrameA = 1;
            FrameB = 0;
            FrameIndex = 1;
            _Image.Source = Frames[FrameA];
        }

        public void FlashBlue()
        {
            FrameA = 2;
            FrameB = 0;
            FrameIndex = 2;
            _Image.Source = Frames[FrameA];
        }

        // http://stackoverflow.com/questions/347614/wpf-image-resources
        public static BitmapImage LoadBitmapFromResource(string pathInApplication, Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }

            if (pathInApplication[0] == '/')
            {
                pathInApplication = pathInApplication.Substring(1);
            }
            return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
        }

        private void ZanoBlinking_OnLoaded(object sender, RoutedEventArgs e)
        {
            Timer.Start();
        }

        private void ZanoBlinking_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
        }

    }
}
