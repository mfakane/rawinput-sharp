using System;
using System.Linq;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class RawInputDigitizer : RawInputHid
    {
        public static readonly HidUsageAndPage UsageContactCount = new HidUsageAndPage(0x0D, 0x54);

        public int MaxContactCount => Reader.ValueSets.SelectMany(x => x).FirstOrDefault(x => x.LinkUsageAndPage == UsageAndPage && x.UsageAndPage == UsageContactCount)?.MaxValue ?? 1;

        public RawInputDigitizer(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
            : base(device, deviceInfo)
        {
            if (!IsSupported(deviceInfo.Hid.UsageAndPage)) throw new ArgumentException($"UsagePage and Usage {deviceInfo.Hid.UsageAndPage} is not supported as a digitizer.", nameof(deviceInfo));
        }

        public static bool IsSupported(HidUsageAndPage usageAndPage) =>
            usageAndPage.UsagePage == 0x0D;
    }
}
