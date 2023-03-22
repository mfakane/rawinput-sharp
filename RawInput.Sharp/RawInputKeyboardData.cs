using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputKeyboardData : RawInputData
{
    public RawKeyboard Keyboard { get; }

    public RawInputKeyboardData(RawInputHeader header, RawKeyboard keyboard)
        : base(header) =>
        Keyboard = keyboard;

    public override unsafe byte[] ToStructure()
    {
        var headerSize = MarshalEx.SizeOf<RawInputHeader>();
        var mouseSize = MarshalEx.SizeOf<RawKeyboard>();
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