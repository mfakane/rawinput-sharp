using System;
using System.Runtime.InteropServices;

namespace Linearstar.RawInput.Native
{
    /// <summary>
    /// RAWMOUSE
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawMouse
    {
        readonly RawMouseFlags usFlags;
        readonly ushort usReserved;
        readonly RawMouseButtonFlags usButtonFlags;
        readonly short usButtonData;
        readonly uint ulRawButtons;
        readonly int lLastX;
        readonly int lLastY;
        readonly uint ulExtraInformation;

        public RawMouseFlags Flags => usFlags;
        public RawMouseButtonFlags Buttons => usButtonFlags;
        public int ButtonData => usButtonData;
        public uint RawButtons => ulRawButtons;
        public int LastX => lLastX;
        public int LastY => lLastY;
        public uint ExtraInformation => ulExtraInformation;

        public override string ToString() =>
            $"{{X: {LastX}, Y: {LastY}, Flags: {Flags}, Buttons: {Buttons}, Data: {ButtonData}}}";
    }

    [Flags]
    public enum RawMouseFlags : ushort
    {
        MoveRelative = 0,
        MoveAbsolute = 1,
        VirtualDesktop = 2,
        AttributesChanged = 4,
    }

    [Flags]
    public enum RawMouseButtonFlags : ushort
    {
        None,
        LeftButtonDown = 0x0001,
        LeftButtonUp = 0x0002,
        RightButtonDown = 0x0004,
        RightButtonUp = 0x0008,
        MiddleButtonDown = 0x0010,
        MiddleButtonUp = 0x0020,
        Button4Down = 0x0040,
        Button4Up = 0x0080,
        Button5Down = 0x0100,
        Button5Up = 0x0200,
        MouseWheel = 0x0400,
        MouseHorizontalWheel = 0x0800,
    }
}
