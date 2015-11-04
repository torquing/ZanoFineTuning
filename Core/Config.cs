using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using LibZano;
#if Z_WPF
using System.IO;
#endif

namespace ZanoFineTuning.Core
{

    public static class Cfg
    {
        static Dictionary<String, double> values = new Dictionary<String, double>(); 

        public static bool ReadConfigFile()
        {
            values.Clear();
#if Z_WPF
            if (File.Exists(U.GetApplicationDataPath() + "Config.txt"))
            {
                var lines = File.ReadAllLines((U.GetApplicationDataPath() + "Config.txt"));
                foreach (var line in lines)
                {
                    var l = line.Trim();
                    if (String.IsNullOrWhiteSpace(l))
                        continue;
                    var p = line.Split('=');
                    if (p.Length != 2)
                        continue;
                    var k = p[0].Trim();
                    double v = 0.0f;
                    if (double.TryParse(p[1], out v))
                    {
                        values.Add(k, v);
                        Console.WriteLine("Config: {0} = {1}", k, v);
                    }
                }

                return true;
            }
            return false;
#else
            return false;
#endif
        }

        public static double GetDouble(String name, double defaultValue)
        {
            double v = 0.0f;
            if (values.TryGetValue(name, out v))
            {
                return v;
            }
            return defaultValue;
        }

        public static int GetInt(String name, int defaultValue)
        {
            double v = 0.0f;
            if (values.TryGetValue(name, out v))
            {
                return (int) v;
            }
            return defaultValue;
        }
    }

}
