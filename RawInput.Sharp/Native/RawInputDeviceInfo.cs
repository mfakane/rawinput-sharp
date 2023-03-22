using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RID_DEVICE_INFO
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public readonly struct RawInputDeviceInfo
{
    [FieldOffset(0)]
    readonly int sbSize;
    [FieldOffset(4)]
    readonly RawInputDeviceType dwType;
    [FieldOffset(8)]
    readonly RawInputMouseInfo mouse;
    [FieldOffset(8)]
    readonly RawInputKeyboardInfo keyboard;
    [FieldOffset(8)]
    readonly RawInputHidInfo hid;
        
    /// <summary>
    /// dwType
    /// </summary>
    public RawInputDeviceType Type => dwType;

    /// <summary>
    /// mouse
    /// </summary>
    public RawInputMouseInfo Mouse => mouse;

    /// <summary>
    /// keyboard
    /// </summary>
    public RawInputKeyboardInfo Keyboard => keyboard;

    /// <summary>
    /// hid
    /// </summary>
    public RawInputHidInfo Hid => hid;
}