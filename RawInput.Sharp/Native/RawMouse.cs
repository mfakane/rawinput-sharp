using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWMOUSE
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct RawMouse
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

/// <summary>
/// MOUSE_*
/// </summary>
[Flags]
public enum RawMouseFlags : ushort
{
    /// <summary>
    /// MOUSE_MOVE_RELATIVE
    /// </summary>
    None = 0,
    /// <summary>
    /// MOUSE_MOVE_ABSOLUTE
    /// </summary>
    MoveAbsolute = 1,
    /// <summary>
    /// MOUSE_VIRTUAL_DESKTOP
    /// </summary>
    VirtualDesktop = 2,
    /// <summary>
    /// MOUSE_ATTRIBUTES_CHANGED
    /// </summary>
    AttributesChanged = 4,
}

/// <summary>
/// RI_MOUSE_*
/// </summary>
[Flags]
public enum RawMouseButtonFlags : ushort
{
    None,
    /// <summary>
    /// RI_MOUSE_LEFT_BUTTON_DOWN
    /// </summary>
    LeftButtonDown = 0x0001,
    /// <summary>
    /// RI_MOUSE_LEFT_BUTTON_UP
    /// </summary>
    LeftButtonUp = 0x0002,
    /// <summary>
    /// RI_MOUSE_RIGHT_BUTTON_DOWN
    /// </summary>
    RightButtonDown = 0x0004,
    /// <summary>
    /// RI_MOUSE_RIGHT_BUTTON_UP
    /// </summary>
    RightButtonUp = 0x0008,
    /// <summary>
    /// RI_MOUSE_MIDDLE_BUTTON_DOWN
    /// </summary>
    MiddleButtonDown = 0x0010,
    /// <summary>
    /// RI_MOUSE_MIDDLE_BUTTON_UP
    /// </summary>
    MiddleButtonUp = 0x0020,
    /// <summary>
    /// RI_MOUSE_BUTTON_4_DOWN
    /// </summary>
    Button4Down = 0x0040,
    /// <summary>
    /// RI_MOUSE_BUTTON_4_UP
    /// </summary>
    Button4Up = 0x0080,
    /// <summary>
    /// RI_MOUSE_BUTTON_5_DOWN
    /// </summary>
    Button5Down = 0x0100,
    /// <summary>
    /// RI_MOUSE_BUTTON_5_UP
    /// </summary>
    Button5Up = 0x0200,
    /// <summary>
    /// RI_MOUSE_WHEEL
    /// </summary>
    MouseWheel = 0x0400,
    /// <summary>
    /// RI_MOUSE_HWHEEL
    /// </summary>
    MouseHorizontalWheel = 0x0800,
}