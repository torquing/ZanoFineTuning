using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LibZano;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools.VibrationManual
{
    public static class G
    {
        public static double AccelerationNowX = 0;
        public static double AccelerationNowY = 0;
        public static double FilteredAccelerationX = 0;
        public static double FilteredAccelerationY = 0;
        public static double FilteredAccelerationMinX = 0;
        public static double FilteredAccelerationMinY = 0;
        public static double FilteredAccelerationMaxX = 0;
        public static double FilteredAccelerationMaxY = 0;
        public static double RangeX = 0;
        public static double RangeY = 0;

    }

    public static class K
    {
        public static double D = 50.0f;
    }

    public static class U
    {
        public static double LowPass(double filtered, double now, double d)
        {
            return ((filtered * d) + now) / (d + 1.0);
        }
    }

    class VibrationManual : FSM
    {
        public static VibrationManual Instance = new VibrationManual();

        private VibrationManual()
        {

            OnTrigger("NULL.started", args =>
            {
                Trigger("connect");
            });

            OnTrigger("NULL.connect", args =>
            {
                A.Trigger("connect");
                Move("connect");
            });

            OnTrigger("connect.connected", args =>
            {
                Move("start");
                Z.Listen(Symbols.kSensorsRawAccelerationGet, "received_acceleration");
            });


            OnMove("connect>start", args =>
            {
                V.Navigate("$VibrationManual");
                Move("motors");
            });

            OnTrigger("motors.tick", args =>
            {
                Frames.SensorsRawAccelerationGet(Z.Handle);
            });

            OnTrigger("motors.received_acceleration", args =>
            {
                Frame f = (Frame)args[0];
                G.AccelerationNowX = f.Get(Symbols.kX);
                G.AccelerationNowY = f.Get(Symbols.kY);

                G.FilteredAccelerationX = U.LowPass(G.FilteredAccelerationX, G.AccelerationNowX, K.D);
                G.FilteredAccelerationY = U.LowPass(G.FilteredAccelerationY, G.AccelerationNowY, K.D);

                double hpX = G.AccelerationNowX - G.FilteredAccelerationX;
                double hpY = G.AccelerationNowY - G.FilteredAccelerationY;


                if (hpX > G.FilteredAccelerationMaxX)
                    G.FilteredAccelerationMaxX = hpX;

                if (hpY > G.FilteredAccelerationMaxY)
                    G.FilteredAccelerationMaxY = hpY;

                if (hpX < G.FilteredAccelerationMinX)
                    G.FilteredAccelerationMinX = hpX;

                if (hpY < G.FilteredAccelerationMinY)
                    G.FilteredAccelerationMinY = hpY;

                G.FilteredAccelerationMaxX *= 30.0 / 31.0;
                G.FilteredAccelerationMaxY *= 30.0 / 31.0;

                G.FilteredAccelerationMinX *= 30.0 / 31.0;
                G.FilteredAccelerationMinY *= 30.0 / 31.0;

                double rangeX = G.FilteredAccelerationMaxX - G.FilteredAccelerationMinX;
                double rangeY = G.FilteredAccelerationMaxY - G.FilteredAccelerationMinY;

                G.RangeX = U.LowPass(G.RangeX, rangeX, 10.0);
                G.RangeY = U.LowPass(G.RangeY, rangeY, 10.0);

                Views.VibrationManual.Instance.Refresh();

            });

        }



    }
}
