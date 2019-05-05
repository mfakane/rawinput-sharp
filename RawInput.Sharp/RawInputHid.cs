using System;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class RawInputHid : RawInputDevice
    {
        readonly Lazy<HidReader> hidReader;

        public override HidUsageAndPage UsageAndPage => DeviceInfo.Hid.UsageAndPage;

        public override int VendorId => DeviceInfo.Hid.VendorId;

        public override int ProductId => DeviceInfo.Hid.ProductId;

        public int Version => DeviceInfo.Hid.VersionNumber;

        public HidReader Reader => hidReader.Value;

        public RawInputHid(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
            : base(device, deviceInfo)
        {
            if (deviceInfo.Type != RawInputDeviceType.Hid) throw new ArgumentException($"Device type must be {RawInputDeviceType.Hid}.", nameof(deviceInfo));

            hidReader = new Lazy<HidReader>(() => new HidReader(GetPreparsedData()));
        }
    }
}
