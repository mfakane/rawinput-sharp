using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Linearstar.Windows.RawInput.Native;

public static class HidD
{
    [DllImport("hid", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool HidD_GetManufacturerString(IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength);

    [DllImport("hid", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool HidD_GetProductString(IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength);

    [DllImport("hid", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength);

    [DllImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool HidD_GetPreparsedData(IntPtr HidDeviceObject, out IntPtr PreparsedData);

    [DllImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);

    public static HidDeviceHandle OpenDevice(string devicePath)
    {
        var deviceHandle = Kernel32.CreateFile(devicePath, Kernel32.ShareMode.Read | Kernel32.ShareMode.Write, Kernel32.CreateDisposition.OpenExisting);

        return (HidDeviceHandle)deviceHandle;
    }

    public static bool TryOpenDevice(string devicePath, out HidDeviceHandle device)
    {
        if (!Kernel32.TryCreateFile(
                devicePath,
                Kernel32.ShareMode.Read | Kernel32.ShareMode.Write,
                Kernel32.CreateDisposition.OpenExisting,
                out var deviceHandle))
        {
            device = HidDeviceHandle.Zero;
            return false;
        }

        device = (HidDeviceHandle)deviceHandle;
        return true;
    }

    public static void CloseDevice(HidDeviceHandle device)
    {
        var deviceHandle = HidDeviceHandle.GetRawValue(device);

        Kernel32.CloseHandle(deviceHandle);
    }

    public static string? GetManufacturerString(HidDeviceHandle device)
    {
        var deviceHandle = HidDeviceHandle.GetRawValue(device);

        return GetString(deviceHandle, HidD_GetManufacturerString);
    }

    public static string? GetProductString(HidDeviceHandle device)
    {
        var deviceHandle = HidDeviceHandle.GetRawValue(device);

        return GetString(deviceHandle, HidD_GetProductString);
    }

    public static string? GetSerialNumberString(HidDeviceHandle device)
    {
        var deviceHandle = HidDeviceHandle.GetRawValue(device);

        return GetString(deviceHandle, HidD_GetSerialNumberString);
    }

    public static HidPreparsedData GetPreparsedData(HidDeviceHandle device)
    {
        var deviceHandle = HidDeviceHandle.GetRawValue(device);

        HidD_GetPreparsedData(deviceHandle, out var preparsedData);

        return (HidPreparsedData)preparsedData;
    }

    public static void FreePreparsedData(HidPreparsedData preparsedData)
    {
        HidD_FreePreparsedData((IntPtr)preparsedData);
    }

    static string? GetString(IntPtr handle, Func<IntPtr, byte[], uint, bool> proc)
    {
        var buf = new byte[256];

        if (!proc(handle, buf, (uint)buf.Length))
            return null;

        var str = Encoding.Unicode.GetString(buf, 0, buf.Length);

        return str.Contains("\0") ? str.Substring(0, str.IndexOf('\0')) : str;
    }
}
