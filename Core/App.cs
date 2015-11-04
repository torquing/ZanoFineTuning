using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LibZano;

namespace ZanoFineTuning.Core
{
    public static class A
    {

        [DllImport(Library.kLibraryName, CallingConvention = Library.kCallingConvention, EntryPoint = "Zano_PeekConnect"), SuppressUnmanagedCodeSecurity]
        extern public static int PeekConnect(uint zano);

        private static FSM a = null;
        private static ulong tickTimer = 0;
        private static bool hasInternet = false;
        private static bool hasConnection = false;

        static void Init()
        {
            a = new FSM();
            Tools.Registry.Register();

            a.OnMove("NULL>splash", args => {
                V.Navigate("Splash");
            });

            a.OnMove("splash>login", args =>
            {
                V.Navigate("Login");
            });

            a.OnMove("login_refresh>menu, login>menu, tool>menu", args =>
            {
                Cfg.ReadConfigFile();
                V.Navigate("Menu");
            });
            
            a.OnTrigger("NULL.view_initialised", args =>
            {
                a.Move("splash");
            });

            a.OnTrigger("splash.initialise", args =>
            {
                Z.Initialise();
            });

            a.OnTrigger("splash.libzano_initialised", args =>
            {
                a.Move("login");
            });

            a.OnTrigger("login.login", args =>
            {
                String email = (String)args[0];
                String pass = (String)args[1];
                ServerConnectivity.Credentials(email, pass);
                ServerConnectivity.Refresh();
                a.Move("login_refresh");
            });

            a.OnTrigger("login.logout", args =>
            {
                U.DeleteAccountFile();
            });

            a.OnTrigger("login.continue", args =>
            {
                a.Move("menu");
            });

            a.OnTrigger("login_refresh.logged_in", args =>
            {
                a.Move("menu");
            });

            a.OnTrigger("menu.enter_tool", args =>
            {
                a.Move("tool");
            });

            a.OnTrigger("tool.leave_tool", args =>
            {
                a.Move("menu");
            });

            a.OnTrigger("tool.connect", args =>
            {
                Internal.Connect.Trigger("connect", args);
            });

            a.OnTrigger("menu.tick", args =>
            {
                /*
                if (tickTimer == 0 || Library.Time() >= tickTimer)
                {
                    if (Z.IsConnected == false && Library.IsConnecting(Z.Handle) == 0 && Internal.Connect.IsConnecting == false)
                    {
                        Console.WriteLine("Testing Zano Connection");
                        tickTimer = Library.Time() + 10000;
                        PeekConnect(Z.Handle);
                        if (hasInternet == false)
                        {
                            V.TopBarConnectionTrigger("connecting");
                        }
                    }
                }
                */
            });

            a.OnTrigger("ANY.peek_connected, ANY.connected", args =>
            {
                hasConnection = true;
                Library.GetSerialNumber(Z.Handle, ref Z.SerialNumber);
                V.TopBarConnectionTrigger("connected", Z.LastBattery, Z.SerialNumberToString(Z.SerialNumber));
            });

            a.OnTrigger("ANY.not_connected, ANY.disconnected", args =>
            {
                if (hasConnection)
                {
                    V.TopBarConnectionTrigger("not_connected");
                }
                hasConnection = false;
            });

            a.OnTrigger("ANY.have_server_connection", args =>
            {
                hasInternet = true;
                V.TopBarConnectionTrigger("internet", LibZano.ServerConnectivity.Name());
            });

            a.OnTrigger("ANY.no_internet", args =>
            {
                hasInternet = false;
                if (Z.IsConnected == false)
                {
                    V.TopBarConnectionTrigger("not_conected");
                }
            });

            a.OnTrigger("ANY.battery", args =>
            {
                if (hasConnection)
                {
                    Library.GetSerialNumber(Z.Handle, ref Z.SerialNumber);
                    V.TopBarConnectionTrigger("connected", Z.LastBattery, Z.SerialNumberToString(Z.SerialNumber));
                }
            });
        }

        public static void Trigger(String evt, params object[] args)
        {
            if (a == null)
                Init();
            a.Trigger(evt, args);
        }
        
    }
    

}
