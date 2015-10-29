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

namespace ZanoFineTuning.Tools.Accelerometer.Views.Controls
{
    /// <summary>
    /// Interaction logic for AccAnimatedBox.xaml
    /// </summary>
    public partial class AccAnimatedBox : UserControl
    {
        const int kFrameCount = 16;
        const int kFrameSpeed = 2;

        private BitmapImage[] Frames;
        private DispatcherTimer Timer;

        private int Frame, Begin, End;
        private int FrameIncrement;
        private bool Loop;
        private bool Stopped;


        public AccAnimatedBox()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetCallingAssembly();

            Frames = new BitmapImage[129];

            for (int i = 0; i < 129; i++)
            {
                Frames[i] = LoadBitmapFromResource(String.Format("Views/Resources/ZanoBoxAnimation{0}.png", i));
            }

            Image.Source = Frames[0];

            Frame = 0;
            Begin = 0;
            End = 128;
            FrameIncrement = 1;
            Loop = true;
            Stopped = true;

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / (kFrameCount * kFrameSpeed));
            Timer.Tick += Animate;


        }

        private void Animate(object sender, EventArgs eventArgs)
        {
            if (Stopped)
                return;

            Image.Source = Frames[Frame];

            Frame += FrameIncrement;

            if (Frame > End)
            {
                if (Loop)
                    Frame = Begin;
                else
                    Stopped = true;
            }
            else if (Frame < Begin)
            {
                if (Loop)
                    Frame = End;
                else
                    Stopped = true;
            }

        }

        public bool IsAnimating
        {
            get { return Stopped == false; }
        }

        public bool IsComplete
        {
            get
            {
                if (FrameIncrement == 1)
                    return Frame > End;
                else
                    return Frame < End;
            }
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

        private void AnimatedZanoBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            Timer.Start();
        }

        private void AnimatedZanoBox_OnUnloaded(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
        }

        public void PlayUpToSide()
        {
            Frame = Begin = 0;
            End = Frame + kFrameCount;
            FrameIncrement = 1;
            Loop = false;
            Stopped = false;
        }

        public void PlaySideToUp()
        {
            Frame = Begin = 16;
            End = Frame + kFrameCount;
            FrameIncrement = 1;
            Loop = false;
            Stopped = false;
        }

        public void PlaySideToSideACW()
        {
            Frame = Begin = 65;
            End = Frame + kFrameCount;
            FrameIncrement = 1;
            Loop = false;
            Stopped = false;
        }

        public void PlaySideToSideCCW()
        {
            Frame = Begin = 81;
            End = Frame + kFrameCount;
            FrameIncrement = 1;
            Loop = false;
            Stopped = false;
        }

        public void SetSide()
        {
            Frame = Begin = 81;
            End = Frame + kFrameCount;
            FrameIncrement = 1;
            Loop = false;
            Stopped = true;
            Image.Source = Frames[Frame];
        }


    }
}
