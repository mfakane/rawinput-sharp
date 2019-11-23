using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput
{
    public class RawInputMouseData : RawInputData
    {
        public RawMouse Mouse { get; }

        public RawInputMouseData(RawInputHeader header, RawMouse mouse)
            : base(header) =>
            Mouse = mouse;

        public override unsafe byte[] ToStructure()
        {
            var headerSize = MarshalExtensions.SizeOf<RawInputHeader>();
            var mouseSize = MarshalExtensions.SizeOf<RawMouse>();
            var bytes = new byte[headerSize + mouseSize];

            fixed (byte* bytesPtr = bytes)
            {
                *(RawInputHeader*)bytesPtr = Header;
                *(RawMouse*)(bytesPtr + headerSize) = Mouse;
            }

            return bytes;
        }

        public override string ToString() =>
            $"{{{Header}, {Mouse}}}";
    }
}
