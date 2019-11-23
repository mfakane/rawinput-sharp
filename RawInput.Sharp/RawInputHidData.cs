using System;
using System.Linq;
#if NET45
using System.Runtime.CompilerServices;
#endif
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput
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
            var headerSize = MarshalExtensions.SizeOf<RawInputHeader>();
            var hid = Hid.ToStructure();
            var bytes = new byte[Align(headerSize + hid.Length)];

            fixed (byte* bytesPtr = bytes)
            {
                *(RawInputHeader*)bytesPtr = Header;

                fixed (byte* hidPtr = hid)
                {
#if NET45
                    Unsafe.CopyBlock(hidPtr, bytesPtr + headerSize, (uint)hid.Length);
#else
                    Buffer.MemoryCopy(hidPtr, bytesPtr + headerSize, (uint)hid.Length, (uint)hid.Length);
#endif
                }
                
            }

            return bytes;
        }

        public override string ToString() =>
            $"{{{Header}, {Hid}}}";
    }
}
