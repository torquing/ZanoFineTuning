using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LibZano;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools.Accelerometer
{
    public enum Axis
    {
        XPos,
        YPos,
        ZPos,
        XNeg,
        YNeg,
        ZNeg
    }

    public enum ReadState
    {
        NotStarted,
        Starting,
        Poor,
        Good,
        Excellent
    }

    public class AccelerometerRecording
    {
        public Library.SerialNumber SerialNumber = new Library.SerialNumber();
        public DateTime DateTime = System.DateTime.Now;
        public int Id = 0;

        public int Filtered0 = 0;
        public int Filtered1 = 0;
        public int Filtered2 = 0;
        public int Filtered3 = 0;
        public int Filtered4 = 0;
        public int Filtered5 = 0;


        public AccelerometerRecording()
        {
        }

        public AccelerometerRecording(String line)
        {
            var s = line.Split(':');
            SerialNumber = Z.SerialNumberFromString(s[0]);
            DateTime = DateTime.FromFileTimeUtc(long.Parse(s[1]));
            Id = int.Parse(s[2]);
            Filtered0 = int.Parse(s[3]);
            Filtered1 = int.Parse(s[4]);
            Filtered2 = int.Parse(s[5]);

            Filtered3 = int.Parse(s[6]);
            Filtered4 = int.Parse(s[7]);
            Filtered5 = int.Parse(s[8]);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder(512);
            sb.AppendFormat("{0}:", Z.SerialNumberToString(SerialNumber));
            sb.AppendFormat("{0}:", DateTime.ToFileTimeUtc());
            sb.AppendFormat("{0}:", Id);
            sb.AppendFormat("{0}:", Filtered0);
            sb.AppendFormat("{0}:", Filtered1);
            sb.AppendFormat("{0}:", Filtered2);
            sb.AppendFormat("{0}:", Filtered3);
            sb.AppendFormat("{0}:", Filtered4);
            sb.AppendFormat("{0}", Filtered5);

            return sb.ToString();
        }
    }

    public static class G
    {
        public static uint FetchSymbol = Symbols.kSensorsAccelerationGet;
        public static int QueryTimer = 0;
        public static Axis Axis = Axis.XPos;
        public static double[] FilteredValue = new double[7];
        public static int[] FilteredReadings = new int[7];
        public static bool[] FilteredComplete = new bool[7];
        public static bool Recording = false;
        public static bool RecordingStarted = false;
        public static ulong RecordingTime = 0;
        public static int AxisX = 0;
        public static int AxisY = 0;
        public static int AxisZ = 0;
        public static int LastAxisX = 0;
        public static int LastAxisY = 0;
        public static int LastAxisZ = 0;
        public static double GyroMagnitude = 0;

        public static String ResultsSerial = String.Empty;
        public static short ResultsXMin = 0;
        public static short ResultsYMin = 0;
        public static short ResultsZMin = 0;
        public static short ResultsXMax = 0;
        public static short ResultsYMax = 0;
        public static short ResultsZMax = 0;
        public static short ResultsXAvg = 0;
        public static short ResultsYAvg = 0;
        public static short ResultsZAvg = 0;
        public static String ResultsValues = String.Empty;

        public static bool DidSave = false;
        public static int HighestRecordingId = 0;
        public static List<AccelerometerRecording> Recordings = new List<AccelerometerRecording>(16);
        public static bool SaveRecording = true;
    }

    public static class K
    {
        public const double GyroMaxThreshold = 200;
        public const int MinReadings = 120;
        public const double LowPassD = 25.0;
        public const double LowPassDefaultFilter = 4096.0;
        public const bool SkipTutorial = true;
    }

    public static class U
    {
        public static void LoadRecordings()
        {
            G.Recordings.Clear();
            String path = Core.U.GetApplicationDataPath() + "accelerometer.dat";
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    var r = new AccelerometerRecording(line.Trim());
                    G.Recordings.Add(r);
                    if (G.HighestRecordingId < r.Id)
                    {
                        G.HighestRecordingId = r.Id;
                    }
                }
            }
        }

        public static void SaveRecordings()
        {
            StringBuilder sb = new StringBuilder(512 * G.Recordings.Count);
            foreach (var recording in G.Recordings)
            {
                sb.AppendLine(recording.ToString());
            }
            File.WriteAllText(Core.U.GetApplicationDataPath() + "accelerometer.dat", sb.ToString());
        }

        public static double LowPass(double filtered, double now, double d)
        {
            return ((filtered * d) + now) / (d + 1.0);
        }

        public static ReadState AccelerometerReadState(int readings, int maxReadings)
        {
            if (readings <= 0)
                return ReadState.NotStarted;

            if (readings < maxReadings / 16)
                return ReadState.Starting;

            if (readings < maxReadings)
                return ReadState.Poor;

            if (readings >= maxReadings)
                return ReadState.Good;

            return ReadState.Excellent;
        }

        public static double Dot(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return x0 * x1 + y0 * y1 + z0 * z1;
        }

        public static int DotToInt(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return (int)Math.Round(Dot(x0, y0, z0, x1, y1, z1));
        }

        public static void Normalise(ref double x, ref double y, ref double z)
        {
            double d = Math.Sqrt(Dot(x, y, z, x, y, z));

            double r = 1 / d;
            x = r * x;
            y = r * y;
            z = r * z;
        }

        public static String AxisToString(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return "Forward (X+)";
            else if (x == -1 && y == 0 && z == 0)
                return "Backward (X+)";
            else if (x == 0 && y == 1 && z == 0)
                return "Right (Y+)";
            else if (x == 0 && y == -1 && z == 0)
                return "Left (Y-)";
            else if (x == 0 && y == 0 && z == 1)
                return "Bottom (Z+)";
            else if (x == 0 && y == 0 && z == -1)
                return "Top (Z-)";
            return "Orange";
        }

        public static int AxisToIndex(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return 0;
            else if (x == -1 && y == 0 && z == 0)
                return 1;
            else if (x == 0 && y == 1 && z == 0)
                return 2;
            else if (x == 0 && y == -1 && z == 0)
                return 3;
            else if (x == 0 && y == 0 && z == 1)
                return 4;
            else if (x == 0 && y == 0 && z == -1)
                return 5;
            return 6; // 'Other'
        }

        public static uint AxisToSymbol(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return Symbols.kX;
            else if (x == -1 && y == 0 && z == 0)
                return Symbols.kX;
            else if (x == 0 && y == 1 && z == 0)
                return Symbols.kY;
            else if (x == 0 && y == -1 && z == 0)
                return Symbols.kY;
            else if (x == 0 && y == 0 && z == 1)
                return Symbols.kZ;
            else if (x == 0 && y == 0 && z == -1)
                return Symbols.kZ;
            return Symbols.kX; // 'Other'
        }

        public static String AxisToSymbolName(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return "X";
            else if (x == -1 && y == 0 && z == 0)
                return "X";
            else if (x == 0 && y == 1 && z == 0)
                return "Y";
            else if (x == 0 && y == -1 && z == 0)
                return "Y";
            else if (x == 0 && y == 0 && z == 1)
                return "Z";
            else if (x == 0 && y == 0 && z == -1)
                return "Z";
            return "X?"; // 'Other'
        }

        public static int AxisSign(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return 1;
            else if (x == -1 && y == 0 && z == 0)
                return -1;
            else if (x == 0 && y == 1 && z == 0)
                return 1;
            else if (x == 0 && y == -1 && z == 0)
                return -1;
            else if (x == 0 && y == 0 && z == 1)
                return 1;
            else if (x == 0 && y == 0 && z == -1)
                return -1;
            return 1; // 'Other'
        }


        public static String AxisIndexToName(int i)
        {
            switch (i)
            {
                case 0:
                    return "+X";
                case 1:
                    return "-X";
                case 2:
                    return "+Y";
                case 3:
                    return "-Y";
                case 4:
                    return "+Z";
                case 5:
                    return "-Z";
            }
            return "??";
        }

        public static object AxisFaceShorthand(int axisIndex)
        {
            switch (axisIndex)
            {
                case 0:
                    return "Front";
                case 1:
                    return "Back";
                case 2:
                    return "Right";
                case 3:
                    return "Left";
                case 4:
                    return "Bottom";
                case 5:
                    return "Top";
            }
            return "?";
        }

        public static short MaxToShort(double a, double b)
        {
            if (a < b)
            {
                return (short)b;
            }
            return (short)a;
        }

        public static short MinToShort(double a, double b)
        {
            if (a < b)
            {
                return (short)a;
            }
            return (short)b;
        }

        public static double Average(double a, double b)
        {
            return (a + b) / 2.0;
        }

        public static bool AxisChanged
        {
            get
            {
                int l = AxisToIndex(G.LastAxisX, G.LastAxisY, G.LastAxisZ);
                int n = AxisToIndex(G.AxisX, G.AxisY, G.AxisZ);
                return (n != l);
            }
        }

        public static bool SideChanged
        {
            get
            {
                int l = AxisToSide(G.LastAxisX, G.LastAxisY, G.LastAxisZ);
                int n = AxisToSide(G.AxisX, G.AxisY, G.AxisZ);
                return (n != l);
            }
        }
        public static int AxisToSide(int x, int y, int z)
        {
            if (x == 1 && y == 0 && z == 0)
                return 1;
            else if (x == -1 && y == 0 && z == 0)
                return 1;
            else if (x == 0 && y == 1 && z == 0)
                return 1;
            else if (x == 0 && y == -1 && z == 0)
                return 1;
            else if (x == 0 && y == 0 && z == 1)
                return 0;
            else if (x == 0 && y == 0 && z == -1)
                return 0;
            return -1; // 'Other'
        }

        public static int AxisXYDirection(int lastAxisX, int lastAxisY, int axisX, int axisY)
        {
            int f = AxisToIndex(lastAxisX, lastAxisY, 0);
            int t = AxisToIndex(axisX, axisY, 0);
            if (f == 0)
            {
                return t == 3 ? 1 : -1;
            }
            else if (f == 1)
            {
                return t == 2 ? 1 : -1;
            }
            else if (f == 2)
            {
                return t == 0 ? 1 : -1;
            }
            else if (f == 3)
            {
                return t == 1 ? 1 : -1;
            }
            return 0;
        }

        public static String FormatShort(short value)
        {
            int r = 0;
            if (value < 0)
            {
                r = 32768 - value;
                return String.Format("FFFF{0:X4}", value);
            }
            else
            {
                r = value;
                return String.Format("{0:X8}", value);
            }

        }

        public static void GetAxisGravityVector(double x, double y, double z, ref int xDir, ref int yDir, ref int zDir)
        {
            double xAbs = Math.Abs(x);
            double yAbs = Math.Abs(y);
            double zAbs = Math.Abs(z);

            int axis = 0;

            double maxAngle = Math.Max(Math.Max(xAbs, yAbs), zAbs);

            if (Math.Abs(maxAngle - xAbs) < double.Epsilon)
                axis = 0;
            else if (Math.Abs(maxAngle - yAbs) < double.Epsilon)
                axis = 1;
            else
                axis = 2;
            
            switch (axis)
            {
                case 0:
                    if (x > 0) // Flip because of gravity
                    {
                        xDir = 1;
                        yDir = 0;
                        zDir = 0;
                    }
                    else
                    {
                        xDir = -1;
                        yDir = 0;
                        zDir = 0;
                    }
                 break;
                 case 1:
                    if (y > 0) // Flip because of gravity
                    {
                        xDir = 0;
                        yDir = 1;
                        zDir = 0;
                    }
                    else
                    {
                        xDir = 0;
                        yDir = -1;
                        zDir = 0;
                    }
                    break;
                case 2:
                    if (z > 0)
                    {
                        xDir = 0;
                        yDir = 0;
                        zDir = 1;
                    }
                    else
                    {
                        xDir = 0;
                        yDir = 0;
                        zDir = -1;
                    }
                    break;
            }

        }
    }

    class Accelerometer : FSM
    {
        public static Accelerometer Instance = new Accelerometer();

        private Accelerometer()
        {

            OnTrigger("NULL.started", args =>
            {
                U.LoadRecordings();
                V.Navigate("$Start");
                /*
                if (Z.IsConnected)
                {
                    Move("connect");
                    Move("start");
                }
                else
                {
                    Trigger("connect");
                }*/
            });

            OnTrigger("NULL.start_new", args =>
            {
                for (int i = 0; i < 6; i++)
                {
                    G.FilteredReadings[i] = 0;
                    G.FilteredComplete[i] = false;
                    G.FilteredValue[i] = 0;
                }
            });
            
            OnTrigger("NULL.apply_new", args =>
            {
                G.SaveRecording = true;
                Trigger("connect");
            });

            OnTrigger("NULL.apply_recording", args =>
            {
                if (args.Length != 0 && args[0] is AccelerometerRecording)
                {
                    var recording = (AccelerometerRecording) args[0];
                    G.FilteredReadings[0] = K.MinReadings;
                    G.FilteredValue[0] = recording.Filtered0;
                    G.FilteredComplete[0] = true;

                    G.FilteredReadings[1] = K.MinReadings;
                    G.FilteredValue[1] = recording.Filtered1;
                    G.FilteredComplete[1] = true;

                    G.FilteredReadings[2] = K.MinReadings;
                    G.FilteredValue[2] = recording.Filtered2;
                    G.FilteredComplete[2] = true;


                    G.FilteredReadings[3] = K.MinReadings;
                    G.FilteredValue[3] = recording.Filtered3;
                    G.FilteredComplete[3] = true;

                    G.FilteredReadings[4] = K.MinReadings;
                    G.FilteredValue[4] = recording.Filtered4;
                    G.FilteredComplete[4] = true;

                    G.FilteredReadings[5] = K.MinReadings;
                    G.FilteredValue[5] = recording.Filtered5;
                    G.FilteredComplete[5] = true;

                    G.SaveRecording = false;
                    Trigger("connect", recording.SerialNumber);
                }
            });

            OnTrigger("NULL.connect", args =>
            {
                if (Z.IsConnected == false)
                {
                    A.Trigger("connect", args);
                    Move("connect");
                }
                else
                {
                    Move("connect");
                    Trigger("connected");
                }
            });

            OnTrigger("connect.connected", args =>
            {
                bool completeSet = true;

                for (int i = 0; i < 6; i++)
                {
                    if (G.FilteredComplete[i] == false)
                    {
                        completeSet = false;
                    }
                }

                if (completeSet)
                {
                    Move("axis");
                    Trigger("save");
                }
                else
                {
                    Move("wait_start");
                }

            });

            OnMove("connect>wait_start", args =>
            {
                // Clear
                G.DidSave = false;
                for (int i = 0; i < 6; i++)
                {
                    G.FilteredReadings[i] = 0;
                    G.FilteredComplete[i] = false;
                    G.FilteredValue[i] = 0;
                }

                V.Navigate("$Accelerometer");
            });

            OnTrigger("wait_start.start", args =>
            {
                Move("start");
            });

            OnMove("wait_start>start", args =>
            {
                // Get ZanoHandler to send gyros and raw acceleration frames this way.
                Z.Listen(Symbols.kSensorsGyroGet, "receive_gyro");
                Z.Listen(Symbols.kSensorsRawAccelerationGet, "receive_accelerometer");

                Views.Accelerometer.Instance.Begin();
                Move("axis");
            });

            OnTrigger("axis.retry", args =>
            {
                G.DidSave = false;
                for (int i = 0; i < 6; i++)
                {
                    G.FilteredReadings[i] = 0;
                    G.FilteredComplete[i] = false;
                    G.FilteredValue[i] = 0;
                }
            });

            OnTrigger("axis.save, save.not_sent, connect.save", args =>
            {
                // Compile the readings into maximum and minimum readings
                G.ResultsXMin = U.MinToShort(G.FilteredValue[U.AxisToIndex(1, 0, 0)], G.FilteredValue[U.AxisToIndex(-1, 0, 0)]);
                G.ResultsXMax = U.MaxToShort(G.FilteredValue[U.AxisToIndex(1, 0, 0)], G.FilteredValue[U.AxisToIndex(-1, 0, 0)]);
                G.ResultsYMin = U.MinToShort(G.FilteredValue[U.AxisToIndex(0, 1, 0)], G.FilteredValue[U.AxisToIndex(0, -1, 0)]);
                G.ResultsYMax = U.MaxToShort(G.FilteredValue[U.AxisToIndex(0, 1, 0)], G.FilteredValue[U.AxisToIndex(0, -1, 0)]);
                G.ResultsZMin = U.MinToShort(G.FilteredValue[U.AxisToIndex(0, 0, 1)], G.FilteredValue[U.AxisToIndex(0, 0, -1)]);
                G.ResultsZMax = U.MaxToShort(G.FilteredValue[U.AxisToIndex(0, 0, 1)], G.FilteredValue[U.AxisToIndex(0, 0, -1)]);

                G.ResultsXAvg = (short)(-1 * ((G.ResultsXMin + G.ResultsXMax) / 2));
                G.ResultsYAvg = (short)(-1 * ((G.ResultsYMin + G.ResultsYMax) / 2));
                G.ResultsZAvg = (short)(-1 * ((G.ResultsZMin + G.ResultsZMax) / 2));
                
                // Copied from ConfigAccelOffsetSet
                Frame sendFrame = new Frame(Z.Handle);
                sendFrame.Reference = Symbols.kConfigAccelOffsetSet;
                sendFrame.Type = Frame.kSendFrame;
                unchecked
                {
                    sendFrame.Names[0] = Symbols.kX;
                    sendFrame.Values[0] = (int)G.ResultsXAvg;
                    sendFrame.Names[1] = Symbols.kY;
                    sendFrame.Values[1] = (int)G.ResultsYAvg;
                    sendFrame.Names[2] = Symbols.kZ;
                    sendFrame.Values[2] = (int)G.ResultsZAvg;
                    sendFrame.Names[3] = Symbols.kXmin;
                    sendFrame.Values[3] = (int)G.ResultsXMin;
                    sendFrame.Names[4] = Symbols.kYmin;
                    sendFrame.Values[4] = (int)G.ResultsYMin;
                    sendFrame.Names[5] = Symbols.kZmin;
                    sendFrame.Values[5] = (int)G.ResultsZMin;
                    sendFrame.Names[6] = Symbols.kXmax;
                    sendFrame.Values[6] = (int)G.ResultsXMax;
                    sendFrame.Names[7] = Symbols.kYmax;
                    sendFrame.Values[7] = (int)G.ResultsYMax;
                    sendFrame.Names[8] = Symbols.kZmax;
                    sendFrame.Values[8] = (int)G.ResultsZMax;
                }

                LibZano.Calibration.Set(ref Z.SerialNumber, ref sendFrame);
                Library.Send(ref sendFrame);

                Frames.ConfigSave(Z.Handle);

                StringBuilder sn = new StringBuilder(16);

                Library.SerialNumber serialNb = new Library.SerialNumber();

                Library.GetSerialNumber(Z.Handle, ref serialNb);
                for (int i = 0; i < 8; i++)
                {
                    sn.AppendFormat("{0:x2}", serialNb.Part[i]);
                }

                G.ResultsSerial = sn.ToString();

                StringBuilder sb = new StringBuilder();
                sb.Append(G.ResultsSerial);
                sb.AppendFormat(" {0:X8} 09", Symbols.kConfigAccelOffsetSet);
                sb.AppendFormat(" {0:X8} {1}", Symbols.kX, U.FormatShort(G.ResultsXAvg));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kY, U.FormatShort(G.ResultsYAvg));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kZ, U.FormatShort(G.ResultsZAvg));

                sb.AppendFormat(" {0:X8} {1}", Symbols.kXmin, U.FormatShort(G.ResultsXMin));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kYmin, U.FormatShort(G.ResultsYMin));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kZmin, U.FormatShort(G.ResultsZMin));

                sb.AppendFormat(" {0:X8} {1}", Symbols.kXmax, U.FormatShort(G.ResultsXMax));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kYmax, U.FormatShort(G.ResultsYMax));
                sb.AppendFormat(" {0:X8} {1}", Symbols.kZmax, U.FormatShort(G.ResultsZMax));

                Console.WriteLine(sb.ToString());

                File.WriteAllText(Core.U.GetApplicationDataPath() + "last_accel_results.txt", sb.ToString());

                if (G.SaveRecording)
                {
                    AccelerometerRecording reading = new AccelerometerRecording();
                    reading.SerialNumber = Z.CopySerialNumber(serialNb);
                    G.HighestRecordingId++;
                    reading.Id = G.HighestRecordingId;
                    reading.Filtered0 = (int) G.FilteredValue[0];
                    reading.Filtered1 = (int) G.FilteredValue[1];
                    reading.Filtered2 = (int) G.FilteredValue[2];
                    reading.Filtered3 = (int) G.FilteredValue[3];
                    reading.Filtered4 = (int) G.FilteredValue[4];
                    reading.Filtered5 = (int) G.FilteredValue[5];
                    
                    G.Recordings.Add(reading);
                    U.SaveRecordings();
                }

                G.ResultsValues = sb.ToString();

                //StringBuilder fc = new StringBuilder();
                //var now = DateTime.Now;
                //fc.AppendFormat("{0} {1}{2}", now.ToLongDateString(), now.ToLongTimeString(), Environment.NewLine);
                // fc.AppendFormat("{0}", sb.)

                //fc.AppendLine(DateTime.Now.)

                // The Zano will automatically reboot, so we should switch state and wait for the connected event which
                // is handled by libZano.
                Move("save_to_zano");
                V.Navigate("$SaveToZano");

            });

            OnTrigger("save_to_zano.disconnected", args =>
            {
                Console.WriteLine("Probably Saved");
                Move("save_to_server");
                V.Navigate("$SaveToServer");
                LibZano.ServerConnectivity.ForceRefresh();
            });

            OnTrigger("save_to_server.no_internet", args =>
            {
                Console.WriteLine("Checking for Internet");
                LibZano.ServerConnectivity.ForceRefresh();
            });

            OnTrigger("save_to_server.configuration_downloaded", args =>
            {
                Console.WriteLine("Got for Internet");
                if (G.DidSave == false)
                {
                    G.DidSave = true;
                    Console.WriteLine("Saving to server...");
                    LibZano.Calibration.ApplyToServer(ref Z.SerialNumber, Symbols.kConfigAccelOffsetSet);
                }
            });

            OnTrigger("save_to_server.config_server_save_complete", args =>
            {
                Console.WriteLine("Saved Config");
                V.Navigate("$Complete");
            });

            OnTrigger("save_to_server.config_server_save_failed", args =>
            {
                Console.WriteLine("Not Saved Config");
            });

            OnTrigger("axis.tick", args =>
            {
                Frames.SensorsGyroGet(Z.Handle);

                if (G.Recording)
                {
                    Frames.SensorsRawAccelerationGet(Z.Handle);
                }
            });

            OnTrigger("axis.receive_gyro", args =>
            {

                Frame frame = (Frame)args[0];

                // Get the gyro vector and calculate the magnitude
                double x = frame.Get(Symbols.kX);
                double y = frame.Get(Symbols.kY);
                double z = frame.Get(Symbols.kZ);
                double mag = Math.Sqrt(x * x + y * y + z * z);

                G.GyroMagnitude = mag;

                Console.WriteLine("Gyro Mag = {0}", mag);
                if (mag >= K.GyroMaxThreshold)
                {
                    // If the magnitude is over the threshold stop asking for accelerometer
                    G.Recording = false;
                    G.RecordingTime = 0;
                    
                    int lastAxis = U.AxisToIndex(G.LastAxisX, G.LastAxisY, G.LastAxisZ);

                    if (G.FilteredReadings[lastAxis] < K.MinReadings)
                    {
                        // Was the number of readings not finished? Then reset them
                        G.FilteredComplete[lastAxis] = false;
                        G.FilteredReadings[lastAxis] = 0;
                        G.FilteredValue[lastAxis] = 0;
                    }
                }
                else
                {
                    // Underneath the current threshold

                    if (G.Recording == false)
                    {
                        // Not recording?
                        // Turn it on!
                        G.Recording = true;
                        G.RecordingTime = Library.Time();
                        G.RecordingStarted = true;
                    }
                }

                // Refresh the view
                if (Views.Accelerometer.Instance != null)
                {
                    Views.Accelerometer.Instance.Refresh();
                }
            });

            OnTrigger("axis.receive_accelerometer", args =>
            {

                Frame frame = (Frame)args[0];

                // Get raw accelerometer values
                double x = frame.Get(Symbols.kX);
                double y = frame.Get(Symbols.kY);
                double z = frame.Get(Symbols.kZ);

                int xDir = 0;
                int yDir = 0;
                int zDir = 0;

                U.GetAxisGravityVector(x, y, z, ref xDir, ref yDir, ref zDir);

                /*

                    // Console.WriteLine("{0} {1} {2}", x, y, z);

                    U.Normalise(ref x, ref y, ref z);

                    // Calculate the best axis that the Zano is pointing up as using
                    // three dot products against each of the unit vectors, from that
                    // you have a ~unit vector that is the best direction.
                
                    int xDir = U.DotToInt(x, y, z, 1, 0, 0);
                    int yDir = U.DotToInt(x, y, z, 0, 1, 0);
                    int zDir = U.DotToInt(x, y, z, 0, 0, 1);

                */

                // Store old axis and copy new one over.

                G.LastAxisX = G.AxisX;
                G.LastAxisY = G.AxisY;
                G.LastAxisZ = G.AxisZ;

                G.AxisX = xDir;
                G.AxisY = yDir;
                G.AxisZ = zDir;

                // Now on a different axis?

                if (G.AxisX != G.LastAxisX || G.AxisY != G.LastAxisY || G.AxisZ != G.LastAxisZ)
                {
                    int lastAxis = U.AxisToIndex(G.LastAxisX, G.LastAxisY, G.LastAxisZ);
                    if (G.FilteredReadings[lastAxis] >= K.MinReadings)
                    {
                        // Did all readings? 
                        // Mark old axis as complete
                        G.FilteredComplete[lastAxis] = true;
                    }
                    else
                    {
                        // Didn't complete.?
                        // Reset readings and value, so they can be done again.
                        G.FilteredReadings[lastAxis] = 0;
                        G.FilteredComplete[lastAxis] = false;
                        G.FilteredValue[lastAxis] = 0;
                    }

                    // Say something
                    Console.WriteLine("Changed Axis from {0} to {1}", U.AxisToString(G.LastAxisX, G.LastAxisY, G.LastAxisZ), U.AxisToString(G.AxisX, G.AxisY, G.AxisZ));
                }

                int axisIndex = U.AxisToIndex(G.AxisX, G.AxisY, G.AxisZ);

                // Read and perform a low pass if the number of readings of the current axis
                // has not been met yet.
                if (G.FilteredReadings[axisIndex] < K.MinReadings)
                {

                    // Set Initial Value, which is +-4096 (based on axis) where 4096 is 1G.
                    // Ideally the accelerometer would be +-4096 but due to manufacturing this
                    // can vary, hence the need for calibration!
                    if (G.RecordingStarted)
                    {
                        G.RecordingStarted = false;
                        G.FilteredValue[axisIndex] = K.LowPassDefaultFilter * U.AxisSign(G.AxisX, G.AxisY, G.AxisZ);
                    }

                    // Perform a low pass filter on the value and store.
                    double filtered = G.FilteredValue[axisIndex];
                    double now = frame.Get(U.AxisToSymbol(G.AxisX, G.AxisY, G.AxisZ));

                    G.FilteredValue[axisIndex] = U.LowPass(filtered, now, K.LowPassD);

                    G.FilteredReadings[axisIndex]++;
                }

                // Exceeded reading count? Then it's complete
                if (G.FilteredReadings[axisIndex] >= K.MinReadings)
                {
                    G.FilteredComplete[axisIndex] = true;
                }

                // Update the view
                // V.RefreshAccelerometerDisplay();
                if (Views.Accelerometer.Instance != null)
                {
                    Views.Accelerometer.Instance.Refresh();
                }
                
            });

            OnTrigger("save_to_server.menu", args =>
            {
                Move("NULL");
                T.Set("NULL");
            });

        }





    }
}
