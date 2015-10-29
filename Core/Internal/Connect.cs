using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LibZano;

namespace ZanoFineTuning.Core.Internal
{
    public static class Connect
    {
        private static FSM c;

        public static Library.SerialNumber Target = new Library.SerialNumber()
        {
            Part = new byte[8]
        };

        public static bool IsConnecting
        {
            get
            {
                return (c != null && c.State != "NULL")
#if Z_WPF
                       || ZanoFineTuning.Views.AppConnect.Instance != null;
#endif
                    ;
            }
        }

        static void Init()
        {
            c = new FSM();

            c.OnTrigger("NULL.connect", args =>
            {
                if (args.Length != 0 && args[0] is Library.SerialNumber)
                {
                    Target = Z.CopySerialNumber((Library.SerialNumber) args[0]);
                }
                else
                {
                    Target = new Library.SerialNumber();
                    Target.Part = new byte[8];
                }

                V.Navigate("Connect");
                V.ConnectTrigger("connecting", Target);
                Z.Connect();
                c.Move("connect");
            });
            
            c.OnTrigger("connect.connect", args =>
            {
                V.ConnectTrigger("connecting", Target);
                Z.Connect();
            });

            c.OnTrigger("connect.connected", args =>
            {
                if (Target.Part != null && Z.IsNullSerialNumber(Target) == false && Z.IsSerialNumber(Target) == false)
                {
                    V.ConnectTrigger("unexpected_serial", Target);
                    Library.Disconnect(Z.Handle);
                    return;
                }

                V.ConnectTrigger("connected");
                T.Trigger("connected");
            });

            c.OnTrigger("connect.booted", args =>
            {
                V.ConnectTrigger("booted");
            });

            c.OnTrigger("connect.not_connected", args =>
            {
                V.ConnectTrigger("not_connected");
                T.Trigger("not_connected");
            });

            c.OnTrigger("connect.not_still", args =>
            {
                V.ConnectTrigger("not_still");
                T.Trigger("not_still");
            });

            c.OnTrigger("connect.need_to_update", args =>
            {
                V.ConnectTrigger("need_to_update");
                T.Trigger("need_to_update");
            });

            c.OnTrigger("connect.drone_not_associated", args =>
            {
                V.ConnectTrigger("drone_not_associated");
                T.Trigger("drone_not_associated");
            });

            c.OnTrigger("connect.bad_update", args =>
            {
                V.ConnectTrigger("bad_update");
                T.Trigger("bad_update");
            });

            c.OnTrigger("connect.firmware_upgrade_start", args =>
            {
                if (Z.IgnoreConnecting == false)
                    V.ConnectTrigger("firmware_upgrade_start");
                T.Trigger("firmware_upgrade_start");
            });

            c.OnTrigger("connect.firmware_upgrade_in_progress", args =>
            {
                int percentage = LibZano.Library.GetFirmwareUploadProgress(Z.Handle);
                if (Z.IgnoreConnecting == false)
                V.ConnectTrigger("firmware_upgrade_in_progress", percentage);
                T.Trigger("firmware_upgrade_in_progress", percentage);
            });
            
            c.OnTrigger("connect.firmware_upgrade_start_complete", args =>
            {
                if (Z.IgnoreConnecting == false)
                    V.ConnectTrigger("firmware_upgrade_start_complete");
                T.Trigger("firmware_upgrade_start_complete");
            });

            c.OnTrigger("connect.firmware_upgrade_start_failed", args =>
            {
                if (Z.IgnoreConnecting == false)
                    V.ConnectTrigger("firmware_upgrade_start_failed");
                T.Trigger("firmware_upgrade_start_failed");
            });
        }
        
        public static void Trigger(String evt, params object[] args)
        {
            if (c == null)
                Init();
            c.Trigger(evt, args);
        }
        
    }
}
