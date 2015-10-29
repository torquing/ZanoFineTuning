using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using LibZano;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools.ResetFirmware
{
    public static class G
    {

    }

    public static class K
    {
    }

    public static class U
    {
        [DllImport(Library.kLibraryName, CallingConvention = Library.kCallingConvention, EntryPoint = "Zano_ResetFirmware"), SuppressUnmanagedCodeSecurity]
        extern public static void ResetFirmware(uint handle, int secretCode);
    }

    class ResetFirmware : FSM
    {
        public static ResetFirmware Instance = new ResetFirmware();

        private ResetFirmware()
        {

            OnTrigger("NULL.started", args =>
            {
                V.Navigate("$ResetFirmware");
            });

            OnTrigger("NULL.flash", args =>
            {
                if (Z.IsConnected)
                {
                    Move("connect");
                    Trigger("connected");
                }
                else
                {
                    Trigger("connect");
                }
            });

            OnTrigger("NULL.cancel", args =>
            {
                T.Set("NULL");
            });

            OnTrigger("NULL.connect", args =>
            {
                A.Trigger("connect");
                Move("connect");
            });

            OnTrigger("connect.connected", args =>
            {
                Z.IgnoreConnecting = true;
                V.Navigate("$ResetingFirmware");
                U.ResetFirmware(Z.Handle, 12345);
                Move("reset");
                // T.Set("NULL");
            });

            OnTrigger("reset.firmware_upgrade_start_complete", args =>
            {
                Z.CanTick = false;
#if Z_WPF
                Application.Current.Shutdown();
#endif
            });
            
        }
    }
}
