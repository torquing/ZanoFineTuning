using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LibZano;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools.Vibration
{

    public class MotorReading
    {
        public double X;
        public double Y;
        public double MinX, MinY;
        public double MaxX, MaxY;
        public double RangeX, RangeY;
        public double RawRating, Rating;
        public int Readings;
        public int Motor;
        public double DirectionX;
        public double DirectionY;
        
        public MotorReading(int motor)
        {
            Motor = motor;
            DirectionX = K.GetMotorDirectionX(motor);
            DirectionY = K.GetMotorDirectionY(motor);
        }

        public static void BuildFor4Motors()
        {
            G.MotorReadingsTotal = 4;
            G.MoveReadingsOver();
            G.MotorReadings.Add(new MotorReading(0));
            G.MotorReadings.Add(new MotorReading(1));
            G.MotorReadings.Add(new MotorReading(2));
            G.MotorReadings.Add(new MotorReading(3));
        }

        public static void BuildForMotors(params int[] motors)
        {
            G.MotorReadingsTotal = motors.Length;
            G.MoveReadingsOver();
            foreach (var m in motors)
            {
                G.MotorReadings.Add(new MotorReading(m));
            }
        }

    }

    public static class G
    {

        public static List<MotorReading> OldReadings = new List<MotorReading>(4); 
        public static List<MotorReading> MotorReadings = new List<MotorReading>(4);
        public static int MotorReadingsTotal = 0;
        public static MotorReading[] LatestMotorReadings = new MotorReading[4];
        public static MotorReading CurrentMotorReading = null;
        public static MotorReading WaitReading = new MotorReading(0);
        
        /*
        public static double[] AccelerationNowX = new double[5];
        public static double[] AccelerationNowY = new double[5];
        public static double[] FilteredAccelerationX = new double[5];
        public static double[] FilteredAccelerationY = new double[5];
        public static double[] FilteredAccelerationMinX = new double[5];
        public static double[] FilteredAccelerationMinY = new double[5];
        public static double[] FilteredAccelerationMaxX = new double[5];
        public static double[] FilteredAccelerationMaxY = new double[5];
        public static double[] RangeX = new double[5];
        public static double[] RangeY = new double[5];
        public static double[] Rating = new double[5];
        public static double[] AllRatings = new double[5];
        public static double[] AvgRatings = new double[5];
        public static int[] Readings = new int[5];
        public static int MotorIndex = 0;
        */

        public static double MovementDirectionX;
        public static double MovementDirectionY;
        public static double MovementLength;

        public static void MoveReadingsOver()
        {
            foreach (var mr in MotorReadings)
            {
                OldReadings.Add(mr);
            }
            MotorReadings.Clear();
            if (CurrentMotorReading != null)
            {
                OldReadings.Add(CurrentMotorReading);
                CurrentMotorReading = null;
            }
        }

        public static MotorReading PopMotorReading()
        {
            if (CurrentMotorReading != null)
            {
                OldReadings.Add(CurrentMotorReading);
                CurrentMotorReading = null;
            }

            if (MotorReadings.Count == 0)
            {
                return null;
            }

            CurrentMotorReading = MotorReadings[0];
            MotorReadings.RemoveAt(0);
            return CurrentMotorReading;
        }

        public static MotorReading GetMostRecentReading(int motor)
        {
            MotorReading last = null;
            foreach (var m in OldReadings)
            {
                if (m.Motor == motor)
                    last = m;
            }
            return last;
        }

        public static MotorReading GetCurrentReading(int motor)
        {
            if (CurrentMotorReading != null && CurrentMotorReading.Motor == motor)
                return CurrentMotorReading;
            foreach (var m in MotorReadings)
            {
                if (m.Motor == motor)
                    return m;
            }
            return null;
        }

        public static int AddedMotorReadings()
        {
            int value = 0;
            if (CurrentMotorReading != null)
            {
                value += CurrentMotorReading.Readings;
            }
            foreach (var m in MotorReadings)
            {
                value += m.Readings;
            }
            foreach (var m in LatestMotorReadings)
            {
                if (m != null)
                {
                    value += m.Readings;
                }
            }
            return value;
        }

        public static int TotalMotorReadings()
        {
            return G.MotorReadingsTotal * K.MaxReadings;
        }

    }

    public static class K
    {
        public static double SpeedModifier = 1.0;
        public static double D = 50.0f;
        public static int MaxReadings = (int) (300 * SpeedModifier);
        public static int WaitReadings = (int) (50 * SpeedModifier);
        public static short MotorSpeed = 614;
        public static int WaitRange = (int) (175 * (1.0 / SpeedModifier));
        public static int DiscardUnder = (int) (100 * SpeedModifier);
        public static int NullMotor = Int32.MaxValue;

        public static double GetMotorDirectionX(int motorId)
        {
            switch (motorId)
            {
                case 0:
                    return -1.0;
                case 1:
                    return -1.0;
                case 2:
                    return +1.0;
                case 3:
                    return +1.0;
            }
            return 0;
        }

        public static double GetMotorDirectionY(int motorId)
        {
            switch (motorId)
            {
                case 0:
                    return -1.0;
                case 1:
                    return +1.0;
                case 2:
                    return +1.0;
                case 3:
                    return -1.0;
            }
            return 0.0;
        }

    }

    public static class U
    {
        public static double LowPass(double filtered, double now, double d)
        {
            return ((filtered * d) + now) / (d + 1.0);
        }
        
        /*
        public static void ResetReadings(int index)
        {
            G.AccelerationNowX[index] = 0;
            G.AccelerationNowY[index] = 0;
            G.FilteredAccelerationX[index] = 0;
            G.FilteredAccelerationY[index] = 0;
            G.FilteredAccelerationMinX[index] = 0;
            G.FilteredAccelerationMinY[index] = 0;
            G.FilteredAccelerationMaxX[index] = 0;
            G.FilteredAccelerationMaxY[index] = 0;
            G.RangeX[index] = 0;
            G.RangeY[index] = 0;
            G.Rating[index] = 0;
            G.AllRatings[index] = 0;
            G.AvgRatings[index] = 0;
            G.Readings[index] = 0;
        }
        */

        public static void CalculateVibration(Frame f, MotorReading r)
        {
            //G.AccelerationNowX[index] = f.Get(Symbols.kX);
            double x = f.Get(Symbols.kX);
            
            //G.AccelerationNowY[index] = f.Get(Symbols.kY);
            double y = f.Get(Symbols.kY);

            //G.FilteredAccelerationX[index] = U.LowPass(G.FilteredAccelerationX[index], G.AccelerationNowX[index], K.D);
            r.X = U.LowPass(r.X, x, K.D);

            //G.FilteredAccelerationY[index] = U.LowPass(G.FilteredAccelerationY[index], G.AccelerationNowY[index], K.D);
            r.Y = U.LowPass(r.Y, y, K.D);
            
            //if (G.Readings[index] == 0)
            if (r.Readings == 0)
            {
                //G.FilteredAccelerationX[index] = G.AccelerationNowX[index];
                r.X = x;

                //G.FilteredAccelerationY[index] = G.AccelerationNowY[index];
                r.Y = y;
            }

            //double hpX = G.AccelerationNowX[index] - G.FilteredAccelerationX[index];
            double hpX = x - r.X;

            //double hpY = G.AccelerationNowY[index] - G.FilteredAccelerationY[index];
            double hpY = y - r.Y;

            //double D = G.Readings[index] - K.DiscardUnder;
            double D = r.Readings - K.DiscardUnder;


            if (D < 0)
            {
                D = 0;
            }

            //if (D > 30.0)
            if (D > 45.0)
            {
                // D = 30.0;
                D = 45.0;
            }

            //if (hpX > G.FilteredAccelerationMaxX[index])
            //    G.FilteredAccelerationMaxX[index] = hpX;
            if (hpX > r.MaxX)
                r.MaxX = hpX;
            
            //if (hpY > G.FilteredAccelerationMaxY[index])
            //    G.FilteredAccelerationMaxY[index] = hpY;
            if (hpY > r.MaxY)
                r.MaxY = hpY;
            
            //if (hpX < G.FilteredAccelerationMinX[index])
            //    G.FilteredAccelerationMinX[index] = hpX;
            if (hpX < r.MinX)
                r.MinX = hpX;

            //if (hpY < G.FilteredAccelerationMinY[index])
            //    G.FilteredAccelerationMinY[index] = hpY;
            if (hpY < r.MinY)
                r.MinY = hpY;

            //if (G.Readings[index] >= K.DiscardUnder || G.MotorIndex == 4)
            if (r.Readings >= K.DiscardUnder || r == G.WaitReading)
            {
                //G.FilteredAccelerationMaxX[index] *= (D / (D+1.0));
                r.MaxX *= (D/(D + 1.0));

                //G.FilteredAccelerationMaxY[index] *= (D / (D+1.0));
                r.MaxY *= (D/(D + 1.0));

                //G.FilteredAccelerationMinX[index] *= (D / (D+1.0));
                r.MinX *= (D/(D + 1.0));

                //G.FilteredAccelerationMinY[index] *= (D / (D+1.0));
                r.MinY *= (D/(D + 1.0));
            }

            //if (G.Readings[index] >= K.DiscardUnder)
            if (r.Readings >= K.DiscardUnder)
            {
                
                //double rangeX = G.FilteredAccelerationMaxX[index] - G.FilteredAccelerationMinX[index];
                double rangeX = r.MaxX - r.MinX;

                //double rangeY = G.FilteredAccelerationMaxY[index] - G.FilteredAccelerationMinY[index];
                double rangeY = r.MaxY - r.MinY;

                //G.RangeX[index] = U.LowPass(G.RangeX[index], rangeX, 10.0);
                r.RangeX = U.LowPass(r.RangeX, rangeX, 10.0);

                //G.RangeY[index] = U.LowPass(G.RangeY[index], rangeY, 10.0);
                r.RangeY = U.LowPass(r.RangeX, rangeY, 10.0);
                
                //G.Rating[index] = 5.0 - ((((G.RangeX[index] + G.RangeY[index])*0.5)-200.0)/70.0);
                r.RawRating = 5.0 - ((((r.RangeX + r.RangeY)*0.5)-200.0)/70.0);

                //if (G.Rating[index] > 5.0)
                if (r.RawRating > 5.0)
                    //G.Rating[index] = 5.0;
                    r.RawRating = 5.0;
                //else if (G.Rating[index] < 0.0)
                else if (r.RawRating < 0.0)
                    //G.Rating[index] = 0.0;
                    r.RawRating = 0.0;
                
                ///// G.AllRatings[index] += G.Rating[index];
                //G.AvgRatings[index] = U.LowPass(G.AvgRatings[index], G.Rating[index], 10.0);  //G.AllRatings[index]/G.Readings[index];
                //r.Rating = U.LowPass(r.Rating, r.RawRating, 10.0);
                r.Rating = U.LowPass(r.Rating, r.RawRating, 20.0);

            }


        }

        public static void SetMotors(short motor0, short motor1, short motor2, short motor3)
        {
            byte enableDebug = 1 << 0;
            //byte enableSonar = 1 << 1;
            //byte enableIr = 1 << 2;
            byte enabledMotors = 1 << 3;
            
            Frames.SystemDebugSet(Z.Handle, (byte)(enableDebug | enabledMotors), motor0, motor1, motor2, motor3);
        }

        public static String MotorIdToName(int motor)
        {
            switch (motor)
            {
                case 0:
                    return "Back Left";
                case 1:
                    return "Front Left";
                case 2:
                    return "Front Right";
                case 3:
                    return "Back Right";
            }
            return String.Empty;
        }
    }

    class Vibration : FSM
    {
        public static Vibration Instance = new Vibration();

        private Vibration()
        {

            OnTrigger("NULL.started", args =>
            {
                V.Navigate("$Start");
                /*
                if (Z.IsConnected)
                {
                    Move("connect");
                    Trigger("connected");
                }
                else
                {
                    Trigger("connect");
                }
                */
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

            OnMove("connect>start, complete>start, stopped>start", args =>
            {
                V.Navigate("$Vibration");
            });

            OnTrigger("motors.tick, wait.tick", args =>
            {
                Frames.SensorsRawAccelerationGet(Z.Handle);
            });

            OnTrigger("motors.test, wait.test, start.test", args =>
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    U.ResetReadings(i);
                //}
                //G.MotorIndex = 0;
                Trigger("build_test");

                Move("wait");
            });

            OnTrigger("motors.stop, wait.stop, start.stop, stopped.stop", args =>
            {
                U.SetMotors(10, 10, 10, 10);
                Move("stopped");
                Views.Vibration.Instance.Refresh();
            });

            OnTrigger("wait.received_acceleration", args =>
            {
                Frame f = (Frame)args[0];
                //U.CalculateVibration(f, 4);
                U.CalculateVibration(f, G.WaitReading);

                //G.Readings[4]++;
                G.WaitReading.Readings++;

                //if (G.RangeX[4] < K.WaitRange && G.RangeY[4] < K.WaitRange && G.Readings[4] >= K.WaitReadings)
                if (G.WaitReading.RangeX < K.WaitRange && G.WaitReading.RangeY < K.WaitRange && G.WaitReading.Readings >= K.WaitReadings)
                {
                    byte enableDebug = 1 << 0;
                    byte enabledMotors = 1 << 3;
                    
                    int motor = G.CurrentMotorReading.Motor;

                    short motor0 = (short)((motor == 0) ? K.MotorSpeed : 10);
                    short motor1 = (short)((motor == 1) ? K.MotorSpeed : 10);
                    short motor2 = (short)((motor == 2) ? K.MotorSpeed : 10);
                    short motor3 = (short)((motor == 3) ? K.MotorSpeed : 10);

                    U.SetMotors(motor0, motor1, motor2, motor3);
                    //U.ResetReadings(4);
                    G.WaitReading = new MotorReading(K.NullMotor);

                    Move("motors");
                }

                Views.Vibration.Instance.Refresh();
            });

            OnTrigger("motors.received_acceleration", args =>
            {
                //  G.Readings[G.MotorIndex]++;
                G.CurrentMotorReading.Readings++;

                //if (G.Readings[G.MotorIndex] >= K.MaxReadings)
                if (G.CurrentMotorReading.Readings >= K.MaxReadings)
                {
                    Views.Vibration.Instance.Refresh();

                    //G.MotorIndex++;
                    var mr = G.PopMotorReading();

                    //if (G.MotorIndex >= 4)
                    if (mr == null)
                    {
                        U.SetMotors(10, 10, 10, 10);
                        Move("complete");
                    }
                    else
                    {
                        U.SetMotors(10, 10, 10, 10);
                        //G.Readings[4] = 0;
                        //U.ResetReadings(4);
                        G.WaitReading = new MotorReading(K.NullMotor);
                        Move("wait");
                    }
                    return;
                }

                Frame f = (Frame)args[0];
                //U.CalculateVibration(f, G.MotorIndex);
                U.CalculateVibration(f, G.CurrentMotorReading);

                Views.Vibration.Instance.Refresh();
            });

            OnMove("motors>complete", args =>
            {
                /*
                double[] x = new double[4];
                double[] y = new double[4];

                x[0] = -1.0 * G.AvgRatings[0];
                y[0] = -1.0 * G.AvgRatings[0];

                x[1] = -1.0 * G.AvgRatings[1];
                y[1] = +1.0 * G.AvgRatings[1];

                x[2] = +1.0 * G.AvgRatings[2];
                y[2] = +1.0 * G.AvgRatings[2];

                x[3] = +1.0 * G.AvgRatings[3];
                y[3] = -1.0 * G.AvgRatings[3];

                double sumX = x[0] + x[1] + x[2] + x[3];
                double sumY = y[0] + y[1] + y[2] + y[3];

                G.MovementLength = Math.Sqrt(sumX*sumX + sumY*sumY);
                G.MovementDirectionX = sumX / G.MovementLength;
                G.MovementDirectionY = sumY / G.MovementLength;
                */

                double dirX = 0.0, dirY = 0.0;
                for (int i = 0; i < 3; i++)
                {
                    var mr = G.GetMostRecentReading(i);
                    G.LatestMotorReadings[i] = mr;
                    if (mr != null)
                    {
                        dirX += mr.DirectionX*mr.Rating;
                        dirY += mr.DirectionY*mr.Rating;
                    }
                }

                G.MovementLength = Math.Sqrt(dirX*dirX + dirY*dirY);
                G.MovementDirectionX = dirX/G.MovementLength;
                G.MovementDirectionY = dirY/G.MovementLength;

                V.Navigate("$Complete");
            });

            OnTrigger("stopped.retry", args =>
            {

                //for (int i = 0; i < 5; i++)
                //{
                //    U.ResetReadings(i);
                //}
                //G.MotorIndex = 0;
                Trigger("build_test");


                Move("start");
                Trigger("test");
            });

            OnTrigger("complete.retry", args =>
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    U.ResetReadings(i);
                //}
                //G.MotorIndex = 0;
                Trigger("build_test");


                Move("start");
            });

            OnTrigger("started.menu, start.menu, connect.menu, stopped.menu, motors.menu, wait.menu, complete.menu", args =>
            {
                //for (int i = 0; i < 5; i++)
                // {
                //     U.ResetReadings(i);
                // }
                //  G.MotorIndex = 0;
                Trigger("build_test");


                U.SetMotors(10, 10, 10, 10);
                Move("NULL");
                T.Set("NULL");
            });

            OnTrigger("ANY.build_test", arg =>
            {
                G.CurrentMotorReading = null;
                G.OldReadings.Clear();
                MotorReading.BuildFor4Motors(); // Temp for now, until profile is done.
                G.PopMotorReading();
                G.WaitReading = new MotorReading(K.NullMotor);

            });
        }
    }
}
