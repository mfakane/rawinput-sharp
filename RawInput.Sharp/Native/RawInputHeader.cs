using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWINPUTHEADER
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct RawInputHeader
{
    readonly RawInputDeviceType dwType;
    readonly int dwSize;
    readonly RawInputDeviceHandle hDevice;
    readonly IntPtr wParam;

    public RawInputDeviceType Type => dwType;
    public int Size => dwSize;
    public RawInputDeviceHandle DeviceHandle => hDevice;
    public IntPtr WParam => wParam;

    public override string ToString() =>
        $"{{{Type}: {DeviceHandle}, WParam: {WParam}}}";
}