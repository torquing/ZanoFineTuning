using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using LibZano;

namespace ZanoFineTuning.Core
{
    public static class Z
    {

        static Dictionary<uint, String> listens = new Dictionary<uint, string>();

        public static bool Initialised { get; private set; }

        public static uint Handle { get; private set; }
        
        public static Library.SerialNumber SerialNumber = new Library.SerialNumber();

        public static bool CanTick = true;

        public static bool IgnoreConnecting = false;

        public static int LastBattery = 0;

        private static ulong BatteryTimer = 0;

        public static LibZano.Version FirmwareVersion;

        public static void Listen(uint symbol, String evt)
        {
            listens[symbol] = evt;
        }

        public static void Unlisten(uint symbol)
        {
            listens.Remove(symbol);
        }

        public static void UnlistenAll()
        {
            listens.Clear();
        }
        
        public static void Initialise()
        {
            if (Initialised == false)
            {
                Library.Initialise("Torquing-MyZano-1.0.0");

                String path = U.GetApplicationDataPath();
                Library.SetPath(
                    Library.kPathVideo | Library.kPathPicture | Library.kPathAudio | Library.kPathData |
                    Library.kPathTemp, path);
                
                Console.WriteLine(path);
                

                Handle = Library.Obtain("192.168.0.1", 10001);
                Initialised = true;
                A.Trigger("libzano_initialised");
            }
        }

        public static void Tick()
        {
            if (Initialised && CanTick)
            {
                A.Trigger("tick");

                LibZano.Library.GetFirmwareVersion(Handle, ref FirmwareVersion);
                
                if (Library.Time() >= BatteryTimer)
                {
                    BatteryTimer = Library.Time() + 1000;
                    Console.WriteLine("Booted = {0}, Connected = {1}", IsBooted, IsConnected);
                    if (IsBooted && IsConnected)
                    {
                        Frames.SensorsBatteryGet(Handle);
                    }
                }

                Library.Tick();

                Frame frame = new Frame();

                while (Library.Receive(ref frame) == 1)
                {
                    switch (frame.Type)
                    {
                        case Frame.kSendFrame:
                        {
                            String evt;
                            if (listens.TryGetValue(frame.Reference, out evt))
                            {
                                T.Trigger(evt, frame);
                            }

                            if (frame.Reference == Symbols.kSensorsBatteryGet)
                            {
                                Z.LastBattery = frame.Get(Symbols.kCharge);
                                Console.WriteLine("Battery = {0}", Z.LastBattery);
                                A.Trigger("battery", Z.LastBattery);
                            }
                        }
                            break;
                        case Frame.kConnectFrame:
                            {
                                Console.WriteLine("Connect Frame");
                            }
                            break;
                        case Frame.kDisconnectFrame:
                            {
                                Console.WriteLine("Disconnect Frame");
                            }
                            break;
                        case Frame.kFlyStateFrame:
                            {
                                Console.WriteLine("Fly State Frame");
                            }
                            break;
                        case Frame.kFirmware:
                            {
                                Console.WriteLine("Firmware Frame");
                            }
                            break;
                        case Frame.kServerConnectivity:
                            {
                                Console.WriteLine("Server Connectivity Frame");
                            }
                            break;
                    }
                }

                Library.Status status;

                while ((status = Library.GetStatus()) != Library.Status.None)
                {
                    Console.WriteLine("Status = {0}", status);

                    switch (status)
                    {
                        case Library.Status.None:
                            break;
                        case Library.Status.FlashingStarted:
                            Internal.Connect.Trigger("firmware_upgrade_start");
                            A.Trigger("firmware_upgrade_start");
                            break;
                        case Library.Status.FlashingProcessSucessfull:
                            Internal.Connect.Trigger("firmware_upgrade_start_complete");
                            A.Trigger("firmware_upgrade_start_complete");
                            break;
                        case Library.Status.FlashingProcessFailed:
                            Internal.Connect.Trigger("firmware_upgrade_start_failed");
                            A.Trigger("firmware_upgrade_start_failed");
                            break;
                        case Library.Status.FlashingProcessInProgress:
                            Internal.Connect.Trigger("firmware_upgrade_in_progress");
                            A.Trigger("firmware_upgrade_in_progress");
                            break;
                        case Library.Status.ConfigurationStarting:
                            break;
                        case Library.Status.ConfigurationApplied:
                            break;
                        case Library.Status.ConfigurationNotNeeded:
                            break;
                        case Library.Status.ConfigurationDoesNotExist:
                            break;
                        case Library.Status.HelloRequested:
                            break;
                        case Library.Status.HelloSuccess:
                            A.Trigger("has_internet");
                            T.Trigger("has_internet");
                            break;
                        case Library.Status.HelloFailed:
                            A.Trigger("no_internet");
                            T.Trigger("no_internet");
                            break;
                        case Library.Status.HaveServerConnection:
                            A.Trigger("have_server_connection");
                            T.Trigger("have_server_connection");
                            break;
                        case Library.Status.FirmwareDownloadingRequested:
                            break;
                        case Library.Status.FirmwareDownloadedNew:
                            break;
                        case Library.Status.FirmwareDownloadedUpToDate:
                            break;
                        case Library.Status.FirmwareDownloadedNetworkError:
                            break;
                        case Library.Status.ConfigurationRequest:
                            break;
                        case Library.Status.ConfigurationNetworkError:
                            break;
                        case Library.Status.ConfigurationAuthenticationError:
                            break;
                        case Library.Status.ConfigurationNoConfigurationAssociatedWithAccount:
                            break;
                        case Library.Status.ConfigurationDownloaded:
                            A.Trigger("configuration_downloaded");
                            T.Trigger("configuration_downloaded");
                            break;
                        case Library.Status.DeviceConnectionAttempt:
                            break;
                        case Library.Status.DeviceConnectionGotConnection:
                            break;
                        case Library.Status.DeviceConnectionBooted:
                            Internal.Connect.Trigger("booted");
                            A.Trigger("booted");
                            
                            break;
                        case Library.Status.DeviceConnectionInRecovery:
                            break;
                        case Library.Status.DeviceConnectionHeldStill:
                            Internal.Connect.Trigger("not_still");
                            A.Trigger("not_still");
                            break;
                        case Library.Status.DeviceConnectionBootFailure:
                            break;
                        case Library.Status.DeviceConnectionNeedToUpdate:
                            Internal.Connect.Trigger("need_to_update");
                            A.Trigger("need_to_update");
                            break;
                        case Library.Status.DeviceConnectionConnected:
                            Library.GetSerialNumber(Handle, ref SerialNumber);
                            Internal.Connect.Trigger("connected");
                            A.Trigger("connected");
                            BatteryTimer = Library.Time() + 1000;
                            LastBattery = 0;
                            break;
                        case Library.Status.DeviceConnectionNotConnected:
                            LastBattery = 0;
                            Internal.Connect.Trigger("not_connected");
                            A.Trigger("not_connected");
                            break;
                        case Library.Status.DeviceConnectionBadUpdate:
                            Internal.Connect.Trigger("bad_update");
                            A.Trigger("bad_update");
                            break;
                        case Library.Status.DeviceConnectionDroneNotAssociatedWithAccount:
                            Internal.Connect.Trigger("drone_not_associated");
                            A.Trigger("drone_not_associated");
                            break;
                        case Library.Status.DeviceConnectionPeekConnected:
                            A.Trigger("peek_connected");
                            break;
                        case Library.Status.DeviceDisconnectionDisconnected:
                            LastBattery = 0;
                            T.Trigger("disconnected");
                            A.Trigger("disconnected");
                            break;
                        case Library.Status.ServerConnectivityLoginSuccess:
                            A.Trigger("logged_in");
                            T.Trigger("logged_in");
                            break;
                        case Library.Status.ServerConnectivityLoginFailure:
                            A.Trigger("not_logged_in");
                            T.Trigger("not_logged_in");
                            break;
                        case Library.Status.FirmwareConfigurationPersistanceSaveRequested:
                            A.Trigger("config_server_save_started");
                            T.Trigger("config_server_save_started");
                            break;
                        case Library.Status.FirmwareConfigurationPersistanceSaveCompleted:
                            A.Trigger("config_server_save_complete");
                            T.Trigger("config_server_save_complete");
                            break;
                        case Library.Status.FirmwareConfigurationPersistanceSaveFailure:
                            A.Trigger("config_server_save_failed");
                            T.Trigger("config_server_save_failed");
                            break;
                    }
                }
            }
        }

        public static void Connect()
        {
            if (Connecting == false)
            {
                Library.Connect(Handle);
            }
        }

        public static void Disconnected()
        {
            if (IsConnected)
            {
                Library.Disconnect(Handle);
            }
        }

        public static bool IsConnected
        {
            get { return Library.IsConnected(Handle) == 1; }
        }

        public static bool Connecting
        {
            get { return Library.IsConnecting(Handle) == 1; }
        }

        public static bool IsBooted
        {
            get
            {
                // Hack. TODO: In LibZano will have to expose IsBooted to public API.
                return FirmwareVersion.Major >= 1;
            }
        }

        public static String SerialNumberToString(Library.SerialNumber sn)
        {
            StringBuilder sb = new StringBuilder(17);
            for (int i = 0; i < 8; i++)
            {
                sb.AppendFormat("{0:X2}", sn.Part[i]);
            }

            return sb.ToString();
        }

        public static int GetSimpleBatteryCharge(int charge)
        {
            if (charge >= 6700) // Charging via USB
                return Int32.MaxValue;

            if (charge == 0)    // Unknown
                return 0;

            if (charge >= 3900)
                return 5;

            if (charge >= 3600)
                return 4;

            if (charge >= 3300)
                return 3;

            if (charge >= 3000)
                return 2;

            return 1;
        }

        public static Library.SerialNumber SerialNumberFromString(String str)
        {
            Library.SerialNumber sn = new Library.SerialNumber();
            sn.Part = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                sn.Part[i] = byte.Parse(str.Substring(i*2, 2), NumberStyles.HexNumber);
            }
            return sn;
        }

        public static Library.SerialNumber CopySerialNumber(Library.SerialNumber serialNb)
        {
            Library.SerialNumber sn = new Library.SerialNumber();
            sn.Part = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                sn.Part[i] = serialNb.Part[i];
            }
            return sn;
        }

        public static bool IsSerialNumber(Library.SerialNumber cmp)
        {
            return IsSerialNumber(Z.SerialNumber, cmp);
        }

        public static bool IsSerialNumber(Library.SerialNumber lhs, Library.SerialNumber rhs)
        {
            if (lhs.Part == null || rhs.Part == null || lhs.Part.Length != 8  || rhs.Part.Length != 8)
                return false;

            for (int i = 0; i < 8; i++)
            {
                if (lhs.Part[i] != rhs.Part[i])
                    return false;
            }
            return true;
        }

        public static bool IsNullSerialNumber(Library.SerialNumber cmp)
        {
            if (cmp.Part == null || cmp.Part.Length != 8)
                return true;

            for (int i = 0; i < 8; i++)
            {
                if (cmp.Part[i] != 0)
                    return false;
            }
            return true;
        }
    }

}
