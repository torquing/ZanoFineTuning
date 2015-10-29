using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using LibZano;
#if Z_WPF
using System.IO;
#endif

namespace ZanoFineTuning.Core
{
    
    public static class U
    {
        public static bool HasAccountFile()
        {
#if Z_WPF
            return File.Exists(GetApplicationDataPath() + "ACC.account");
#else
            return false;
#endif
        }

        public static void DeleteAccountFile()
        {
            LibZano.ServerConnectivity.Clear();
            
            if (HasAccountFile())
            {
#if Z_WPF
                File.Delete(GetApplicationDataPath() + "ACC.account");
#endif
            }
        }

        public static String GetApplicationDataPath()
        {
#if Z_WPF
            // http://stackoverflow.com/questions/2768020/where-should-i-store-my-application-data
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Zano\\ZanoFineTuning\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
#else
            return String.Empty;
#endif
        }

        public static String GetAccountName()
        {
            return LibZano.ServerConnectivity.Name();
        }

        public static String GetVersionString()
        {
            // ZanoFineTuning v0.0.6
            StringBuilder sb = new StringBuilder(100);
            sb.AppendFormat("ZanoFineTuning v{0}.{1}.{2}", 0, 0, 9);
            sb.Append(", ");

            LibZano.Version version = new LibZano.Version();
            Library.GetLibZanoVersion(ref version);

            sb.AppendFormat("LibZano: {0}.{1}.{2}", version.Major, version.Minor, version.Revision);

            return sb.ToString();
        }

    }

}
