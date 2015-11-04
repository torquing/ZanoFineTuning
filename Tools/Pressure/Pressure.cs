using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LibZano;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools.Pressure
{

    public class PressureReading
    {
        public double Altitude;
        public double MinAltitude;
        public double MaxAltitude;
        public double RangeAltitude;
        public int Readings;
    }

    public static class G
    {

        public static List<PressureReading> OldReadings = new List<PressureReading>(4);
        public static PressureReading CurrentPressureReading = null;
    }

    public static class K
    {
        public static double SpeedModifier = 1.0;
        public static double D = 50.0f;
        public static int MaxReadings = 300;

        public static void ReadConfig()
        {
            SpeedModifier = Cfg.GetDouble("pressure.modifier", 1.0);
            D = Cfg.GetDouble("pressure.d", 50.0);
            MaxReadings = Cfg.GetInt("pressure.readings", 300);
            MaxReadings = (int) (MaxReadings*SpeedModifier);
        }

    }

    public static class U
    {
        public static double LowPass(double filtered, double now, double d)
        {
            return ((filtered * d) + now) / (d + 1.0);
        }
        
        public static void CalculatePressure(Frame f, PressureReading r)
        {
            //G.AccelerationNowX[index] = f.Get(Symbols.kX);
            double altitude = f.Get(Symbols.kAltitude);
            

            //G.FilteredAccelerationX[index] = U.LowPass(G.FilteredAccelerationX[index], G.AccelerationNowX[index], K.D);
            r.Altitude = U.LowPass(r.Altitude, altitude, K.D);
            

            //if (G.Readings[index] == 0)
            if (r.Readings == 0)
            {
                //G.FilteredAccelerationX[index] = G.AccelerationNowX[index];
                r.Altitude = altitude;
                
            }

            //double hpX = G.AccelerationNowX[index] - G.FilteredAccelerationX[index];
            double hpAltitude = altitude - r.Altitude;
            

            //double D = G.Readings[index] - K.DiscardUnder;
            double D = 35.0f;

            //if (hpX > G.FilteredAccelerationMaxX[index])
            //    G.FilteredAccelerationMaxX[index] = hpX;
            if (hpAltitude > r.MaxAltitude)
                r.MaxAltitude = hpAltitude;
            
            //if (hpX < G.FilteredAccelerationMinX[index])
            //    G.FilteredAccelerationMinX[index] = hpX;
            if (hpAltitude < r.MinAltitude)
                r.MinAltitude = hpAltitude;
            
            //if (G.Readings[index] >= K.DiscardUnder || G.MotorIndex == 4)
//            if (r.Readings >= K.DiscardUnder || r == G.WaitReading)
//            {
                //G.FilteredAccelerationMaxX[index] *= (D / (D+1.0));
                r.MaxAltitude *= (D / (D + 1.0));
            

                //G.FilteredAccelerationMinX[index] *= (D / (D+1.0));
                r.MinAltitude *= (D / (D + 1.0));
            
//            }

            //if (G.Readings[index] >= K.DiscardUnder)
//            if (r.Readings >= K.DiscardUnder)
//            {

                //double rangeX = G.FilteredAccelerationMaxX[index] - G.FilteredAccelerationMinX[index];
                double rangeAltitude = r.MaxAltitude - r.MinAltitude;
            

                //G.RangeX[index] = U.LowPass(G.RangeX[index], rangeX, 10.0);
                r.RangeAltitude = U.LowPass(r.RangeAltitude, rangeAltitude, 10.0);
            
//
//                //G.Rating[index] = 5.0 - ((((G.RangeX[index] + G.RangeY[index])*0.5)-200.0)/70.0);
//                r.RawRating = 5.0 - ((((r.RangeX + r.RangeY) * 0.5) - 200.0) / 70.0);
//
//                //if (G.Rating[index] > 5.0)
//                if (r.RawRating > 5.0)
//                    //G.Rating[index] = 5.0;
//                    r.RawRating = 5.0;
//                //else if (G.Rating[index] < 0.0)
//                else if (r.RawRating < 0.0)
//                    //G.Rating[index] = 0.0;
//                    r.RawRating = 0.0;
//
//                ///// G.AllRatings[index] += G.Rating[index];
//                //G.AvgRatings[index] = U.LowPass(G.AvgRatings[index], G.Rating[index], 10.0);  //G.AllRatings[index]/G.Readings[index];
//                //r.Rating = U.LowPass(r.Rating, r.RawRating, 10.0);
//                r.Rating = U.LowPass(r.Rating, r.RawRating, 20.0);

//            }


        }
        
    }

    class Pressure : FSM
    {
        public static Pressure Instance = new Pressure();

        private Pressure()
        {

            OnTrigger("NULL.started", args =>
            {
                K.ReadConfig();
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
                Move("start");
                Z.Listen(Symbols.kSensorsPressureGet, "received_pressure");
            });

            OnMove("connect>start, complete>start, stopped>start", args =>
            {
                V.Navigate("$Pressure");
            });

            OnTrigger("baro.tick, wait.tick", args =>
            {
                Frames.SensorsPressureGet(Z.Handle);
            });

            OnTrigger("baro.test, wait.test, start.test", args =>
            {
                Trigger("build_test");
                Move("baro");
            });

            OnTrigger("baro.stop, wait.stop, start.stop, stopped.stop", args =>
            {
                Move("stopped");
                Views.Pressure.Instance.Refresh();
            });
            
            OnTrigger("baro.received_pressure", args =>
            {
                Frame f = (Frame)args[0];
                U.CalculatePressure(f, G.CurrentPressureReading);
                G.CurrentPressureReading.Readings++;
                Console.WriteLine("{0} => {1:0000.0}:{2:0000.0}", G.CurrentPressureReading.Readings++, G.CurrentPressureReading.RangeAltitude, G.CurrentPressureReading.Altitude);

                if (G.CurrentPressureReading.Readings >= K.MaxReadings)
                {
                    Move("complete");
                }
                
                Views.Pressure.Instance.Refresh();
            });

            OnMove("baro>complete", args =>
            {
                // Ta Da!
            });

            OnTrigger("stopped.retry", args =>
            {
                Trigger("build_test");
                Move("start");
                Trigger("test");
            });

            OnTrigger("complete.retry", args =>
            {
                Trigger("build_test");
                Move("start");
                Trigger("test");
            });

            OnTrigger("started.menu, start.menu, connect.menu, stopped.menu, baro.menu, wait.menu, complete.menu", args =>
            {
                Trigger("build_test");
                Move("NULL");
                T.Set("NULL");
            });

            OnTrigger("ANY.build_test", arg =>
            {
                if (G.CurrentPressureReading != null && G.CurrentPressureReading.Readings >= K.MaxReadings)
                {
                    G.OldReadings.Add(G.CurrentPressureReading);
                    G.CurrentPressureReading = null;
                }
                
                G.CurrentPressureReading = new PressureReading();
            });
        }
    }
}
