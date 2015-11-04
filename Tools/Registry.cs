using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZanoFineTuning.Core;

namespace ZanoFineTuning.Tools
{
    static class Registry
    {
        public static void Register()
        {
            T.Add(Accelerometer.Accelerometer.Instance, "accelerometer", "Accelerometer Calibration", "Calibrate the accelerometer of a Zano so it's drift is reduced during flight", "Calibration");
            // T.Add(VibrationManual.VibrationManual.Instance, "vibrationmanual", "VibrationManual", "Manual Dev Tool");
            T.Add(Inspect.Inspect.Instance, "inspect", "About my Zano", "Look at your Zano Firmware and Calibration Information", "Diagnostics");
            T.Add(Vibration.Vibration.Instance, "vibration", "Propeller Vibration", "Calculate Propeller Vibration to reduce pitch and roll angles for trimming", "Diagnostics");
            T.Add(ResetFirmware.ResetFirmware.Instance, "resetfirmware", "Clear Firmware", "Clear the firmware and stored settings from the Zano", "Tools");
            T.Add(Pressure.Pressure.Instance, "pressure", "Pressure", "...", "Diagnostics");
        }
    }
}
