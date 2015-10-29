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

namespace ZanoFineTuning.Tools.Inspect
{
    public static class G
    {
        public static double Yaw;
        public static double Roll;
        public static double Pitch;
        
        public static double AccelX;
        public static double AccelY;
        public static double AccelZ;
    }

    public static class K
    {
    }

    public static class U
    {
        public static double LowPass(double filtered, double now, double d)
        {
            return ((filtered * d) + now) / (d + 1.0);
        }
    }

    class Inspect : FSM
    {
        public static Inspect Instance = new Inspect();

        private Inspect()
        {

            OnTrigger("NULL.started", args =>
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

            OnTrigger("NULL.connect", args =>
            {
                A.Trigger("connect");
                Move("connect");
            });

            OnTrigger("connect.connected", args =>
            {
                Move("NULL");
                V.Navigate("$Inspect");
                Z.Listen(Symbols.kFlightHeadingGet, "yaw_received");
                Z.Listen(Symbols.kFlightPitchGet, "pitch_received");
                Z.Listen(Symbols.kFlightRollGet, "roll_received");
                Z.Listen(Symbols.kSensorsAccelerationGet, "acceleration_received");
            });
            
            OnTrigger("NULL.menu", args =>
            {
                T.Set("NULL");
            });

            OnTrigger("NULL.tick", args =>
            {
                Frames.FlightHeadingGet(Z.Handle);
                Frames.FlightPitchGet(Z.Handle);
                Frames.FlightRollGet(Z.Handle);
                Frames.SensorsAccelerationGet(Z.Handle);
            });

            OnTrigger("NULL.yaw_received", args =>
            {
                Frame frame = (Frame) args[0];
                double yaw = frame.Get(Symbols.kYaw);

                G.Yaw = U.LowPass(G.Yaw, yaw, 10.0);

                if (Views.Inspect.Instance != null)
                {
                    Views.Inspect.Instance.UpdateYaw(G.Yaw / 100.0f);
                }
            });

            OnTrigger("NULL.pitch_received", args =>
            {
                Frame frame = (Frame)args[0];
                double pitch = frame.Get(Symbols.kPitch);

                G.Pitch = U.LowPass(G.Pitch, pitch, 10.0);

                if (Views.Inspect.Instance != null)
                {
                    Views.Inspect.Instance.UpdatePitch(G.Pitch / 100.0f);
                }
            });

            OnTrigger("NULL.roll_received", args =>
            {
                Frame frame = (Frame)args[0];

                double roll = frame.Get(Symbols.kRoll);

                G.Roll = U.LowPass(G.Roll, roll, 10.0);

                if (Views.Inspect.Instance != null)
                {
                    Views.Inspect.Instance.UpdateRoll(G.Roll / 100.0f);
                }
            });

            OnTrigger("NULL.acceleration_received", args =>
            {
                Frame frame = (Frame)args[0];

                int x = frame.Get(Symbols.kX);
                int y = frame.Get(Symbols.kY);
                int z = frame.Get(Symbols.kZ);

                G.AccelX = U.LowPass(G.AccelX, x, 20.0f);
                G.AccelY = U.LowPass(G.AccelY, y, 20.0f);
                G.AccelZ = U.LowPass(G.AccelZ, z, 20.0f);

                if (Views.Inspect.Instance != null)
                {
                    Views.Inspect.Instance.UpdateAccelerometer(x, y, z);
                }
            });
        }
    }
}
