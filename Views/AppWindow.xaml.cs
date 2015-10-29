
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using ZanoFineTuning.Core;

namespace ZanoFineTuning
{
    public partial class AppWindow : MetroWindow
    {
        public static AppWindow Instance { get; private set; }

        private DispatcherTimer tick;

        public AppWindow()
        {
            InitializeComponent();
            tick = new DispatcherTimer();
            tick.Interval = new TimeSpan(0,0,0,0,20);
            tick.Tick += Tick;
        }

        private void Tick(object sender, EventArgs eventArgs)
        {
            Z.Tick();
            T.Trigger("tick");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            A.Trigger("view_initialised");
            V.NavigateTopPanel("TopPanel");
            tick.Start();
        }
        
    }
}
