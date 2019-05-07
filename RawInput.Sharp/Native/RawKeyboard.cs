using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native
{
    /// <summary>
    /// RAWKEYBOARD
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawKeyboard
    {
        readonly ushort usMakeCode;
        readonly RawKeyboardFlags usFlags;
        readonly ushort usReserverd;
        readonly ushort usVKey;
        readonly uint ulMessage;
        readonly uint ulExtraInformation;

        public int ScanCode => usMakeCode;
        public RawKeyboardFlags Flags => usFlags;
        public int VirutalKey => usVKey;
        public uint WindowMessage => ulMessage;
        public uint ExtraInformation => ulExtraInformation;

        public override string ToString() =>
            $"{{Key: {VirutalKey}, ScanCode: {ScanCode}, Flags: {Flags}}}";
    }

    [Flags]
    public enum RawKeyboardFlags : ushort
    {
        Down,
        Up,
        LeftKey,
        RightKey = 4,
    }
}
