using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// DEVPROPKEY
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct DevicePropertyKey
{
    /// <summary>
    /// DEVPKEY_NAME
    /// </summary>
    public static readonly DevicePropertyKey Name = new(0xb725f130, 0x47ef, 0x101a, 0xa5, 0xf1, 0x02, 0x60, 0x8c, 0x9e, 0xeb, 0xac, 10);
    /// <summary>
    /// DEVPKEY_Device_Manufacturer
    /// </summary>
    public static readonly DevicePropertyKey DeviceManufacturer = new(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 13);
    /// <summary>
    /// DEVPKEY_Device_FriendlyName
    /// </summary>
    public static readonly DevicePropertyKey DeviceFriendlyName = new(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);

    readonly Guid fmtid;
    readonly int pid;

    public DevicePropertyKey(uint l, ushort w1, ushort w2, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, int pid)
    {
        fmtid = new Guid((int)l, (short)w1, (short)w2, b1, b2, b3, b4, b5, b6, b7, b8);
        this.pid = pid;
    }
}
