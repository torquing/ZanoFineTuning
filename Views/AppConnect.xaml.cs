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
using ZanoFineTuning.Core.Internal;

namespace ZanoFineTuning.Views
{
    /// <summary>
    /// Interaction logic for AppConnect.xaml
    /// </summary>
    public partial class AppConnect : Page
    {
        public static AppConnect Instance;

        public static String Title;
        public static String Description;
        public static String ButtonCaption;
        public static String Event;
        
        public AppConnect()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            Trigger("connecting", Core.Internal.Connect.Target);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Instance = null;
        }

        private void DoOp(object sender, RoutedEventArgs e)
        {
            Connect.Trigger(Event, Core.Internal.Connect.Target);
        }

        public void Trigger(String name, params object[] args)
        {
            switch (name)
            {
                case "connecting":
                {
                    _Progress.FlashRed();
                    if (args.Length != 0 && args[0] is Library.SerialNumber)
                    {
                        Library.SerialNumber sn = (Library.SerialNumber) args[0];
                        if (Z.IsNullSerialNumber(sn))
                        {
                            Title = "Connecting";
                        }
                        else
                        {
                            Title = String.Format("Connecting to {0}", Z.SerialNumberToString(sn));
                        }
                    }
                    else
                    {
                        Title = "Connecting";
                    }
                    Event = String.Empty;
                }
                break;
                case "connected":
                {
                    _Progress.FlashBlue();
                    Title = "Connected";
                    Event = String.Empty;
                }
                break;
                case "booted":
                {
                    _Progress.FlashBlue();
                    Title = "Establishing Connection";
                    Event = String.Empty;
                }
                break;
                case "unexpected_serial":
                    {
                        _Progress.FlashRed();
                        Title = String.Format("This Zano is not {0}", Z.SerialNumberToString((Library.SerialNumber) args[0]));
                        Description = "Please connect to the requested Zano";
                        ButtonCaption = "Connect";
                        Event = "connect";
                    }
                break;
                case "not_connected":
                {
                    _Progress.FlashRed();
                    Title = "Cannot Connect to Zano";
                    Description = "Please make sure Zano is powered on and this Computer is connected to the Zano WiFi";
                    ButtonCaption = "Connect";
                    Event = "connect";
                }
                break;
                case "not_still":
                {
                    _Progress.FlashRed();
                    Title = "Zano is not still";
                    Description = "Please place it on a firm and level surface then re-connect";
                    ButtonCaption = "Connect";
                    Event = "connect";
                }
                break;
                case "drone_not_associated":
                {
                    _Progress.FlashRed();
                    Title = "This Zano is not associated with this account";
                    Description = "Please login into that account or choose another Zano";
                    ButtonCaption = "Connect";
                    Event = "connect";
                }
                break;
                case "bad_update":
                {
                    _Progress.FlashRed();
                    Title = "Firmware was not applied correctly";
                    Description = "Please contact Zano customer support";
                    ButtonCaption = String.Empty;
                    Event = String.Empty;
                }
                break;
                case "need_to_update":
                {
                    _Progress.FlashRed();
                    Title = "Applying Firmware";
                    Description = "Upgrading your Zano";
                    ButtonCaption = String.Empty;
                    Event = String.Empty;
                }
                break;
                case "firmware_upgrade_start":
                {
                    _Progress.FlashRed();
                    Title = "Applying Firmware";
                    Description = "Upgrading your Zano";
                    ButtonCaption = String.Empty;
                    Event = String.Empty;
                }
                break;
                case "firmware_upgrade_start_complete":
                {
                    _Progress.FlashRed();
                    Title = "Firmware Upgraded";
                    Description = "Upgraded your Zano";
                    ButtonCaption = String.Empty;
                    Event = String.Empty;
                }
                break;
                case "firmware_upgrade_start_failed":
                {
                    _Progress.FlashRed();
                    Title = "Firmware Upgrade Failed";
                    Description = "Upgrade Failed. Please check connection and/or battery";
                    ButtonCaption = "Retry";
                    Event = "connect";
                }
                break;
                case "firmware_upgrade_in_progress":
                    {
                        _Progress.FlashRed();
                        Title = String.Format("Applying Firmware ({0}%)", (int) args[0]);
                        Description = "Upgrading your Zano";
                        ButtonCaption = String.Empty;
                        Event = String.Empty;
                    }
                    break;
            }
            Refresh();
        }

        public void Refresh()
        {
            if (String.IsNullOrEmpty(Event))
            {
                _Title.Content = Title;
                _Description.Visibility = Visibility.Hidden;
                _Op.Visibility = Visibility.Hidden;
            }
            else
            {
                _Title.Content = Title;
                _Description.Text = Description;
                _Op.Content = ButtonCaption;
                _Description.Visibility = Visibility.Visible;
                _Op.Visibility = Visibility.Visible;
            }

        }

    }
}
