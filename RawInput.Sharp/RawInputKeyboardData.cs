using System.Runtime.InteropServices;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class RawInputKeyboardData : RawInputData
    {
        public RawKeyboard Keyboard { get; }

        public RawInputKeyboardData(RawInputHeader header, RawKeyboard keyboard)
            : base(header) =>
            Keyboard = keyboard;

        public override unsafe byte[] ToStructure()
        {
            var headerSize = Marshal.SizeOf<RawInputHeader>();
            var mouseSize = Marshal.SizeOf<RawKeyboard>();
            var bytes = new byte[headerSize + mouseSize];

            fixed (byte* bytesPtr = bytes)
            {
                *(RawInputHeader*)bytesPtr = Header;
                *(RawKeyboard*)(bytesPtr + headerSize) = Keyboard;
            }

            return bytes;
        }

        public override string ToString() =>
            $"{{{Header}, {Keyboard}}}";
    }
}
