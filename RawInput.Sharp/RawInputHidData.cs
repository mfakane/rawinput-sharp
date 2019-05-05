using System;
using System.Linq;
using System.Runtime.InteropServices;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class RawInputHidData : RawInputData
    {
        public new RawInputHid Device => (RawInputHid)base.Device;

        public RawHid Hid { get; }

        public HidButtonSetState[] ButtonSetStates =>
            Hid.ToHidReports().SelectMany(report => Device.Reader.ButtonSets.Select(x => x.GetStates(report))).ToArray();

        public HidValueSetState[] ValueSetStates =>
            Hid.ToHidReports().SelectMany(report => Device.Reader.ValueSets.Select(x => x.GetStates(report))).ToArray();

        protected RawInputHidData(RawInputHeader header, RawHid hid)
            : base(header) =>
            Hid = hid;

        public static RawInputHidData Create(RawInputHeader header, RawHid hid)
        {
            var device = header.DeviceHandle != RawInputDeviceHandle.Zero ? RawInputDevice.FromHandle(header.DeviceHandle) : null;

            if (device != null && RawInputDigitizer.IsSupported(device.UsageAndPage))
                return new RawInputDigitizerData(header, hid);

            return new RawInputHidData(header, hid);
        }

        public override unsafe byte[] ToStructure()
        {
            var headerSize = Marshal.SizeOf<RawInputHeader>();
            var hid = Hid.ToStructure();
            var bytes = new byte[Align(headerSize + hid.Length)];

            fixed (byte* bytesPtr = bytes)
            {
                *(RawInputHeader*)bytesPtr = Header;

                fixed (byte* hidPtr = hid)
                    Buffer.MemoryCopy(hidPtr, bytesPtr + headerSize, hid.Length, hid.Length);
            }

            return bytes;
        }

        public override string ToString() =>
            $"{{{Header}, {Hid}}}";
    }
}
