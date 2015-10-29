#if Z_WPF
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ZanoFineTuning.Views;

namespace ZanoFineTuning.Core
{
    public static class V
    {
        public static void Navigate(String name)
        {
            if (name.StartsWith("$"))
            {
                name = String.Format("Tools/{0}/Views/{1}", T.Name, name.Substring(1));
            }
            else
            {
                name = String.Format("Views/App{0}", name);
            }

            var i = AppWindow.Instance;
            var f = i.Frame;
            var uri = new Uri(String.Format("pack://application:,,,/{0}.xaml", name));
            f.RemoveBackEntry();
            f.Navigate(uri);
        }

        public static void NavigateTopPanel(String name)
        {
            if (name.StartsWith("$"))
            {
                name = String.Format("Tools/{0}/Views/{1}", T.Name, name.Substring(1));
            }
            else
            {
                name = String.Format("Views/App{0}", name);
            }

            var i = AppWindow.Instance;
            var f = i.TopPanel;
            var uri = new Uri(String.Format("pack://application:,,,/{0}.xaml", name));
            f.RemoveBackEntry();
            f.Navigate(uri);
        }

        public static void TopBarConnectionTrigger(String evt, params object[] args)
        {
            if (AppTopPanel.Instance != null)
            {
                AppTopPanel.Instance.Trigger(evt, args);
            }
        }

        public static void ConnectTrigger(String evt, params object[] args)
        {
            if (AppConnect.Instance != null)
            {
                AppConnect.Instance.Trigger(evt, args);
            }
        }

        public static void MenuTrigger(string evt, params object[] args)
        {
            if (AppMenu.Instance != null)
            {
                AppMenu.Instance.Trigger(evt, args);
            }
        }

        public static BackButton TopPanelBackButton
        {
            get
            {
                return _topPanelBackButton;
            }
            set
            {
                if (value != null)
                    AppTopPanel.Instance.SetBackButton(value);
                else
                    AppTopPanel.Instance.ClearBackButton();
                _topPanelBackButton = value;
            }
        }

        private static BackButton _topPanelBackButton;

    }
}
#endif


namespace ZanoFineTuning.Core
{
    public class BackButton
    {

        public String Event { get; set; }
        public object[] Args { get; set; }

        public BackButton(String evt, params object[] args)
        {
            
        }

    }
}