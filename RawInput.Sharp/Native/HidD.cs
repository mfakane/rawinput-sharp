using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Linearstar.Windows.RawInput.Native;

public static partial class HidD
{
    [LibraryImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool HidD_GetManufacturerString(IntPtr HidDeviceObject, IntPtr Buffer, uint BufferLength);

    [LibraryImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool HidD_GetProductString(IntPtr HidDeviceObject, IntPtr Buffer, uint BufferLength);

    [LibraryImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool HidD_GetSerialNumberString(IntPtr HidDeviceObject, IntPtr Buffer, uint BufferLength);

    [LibraryImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool HidD_GetPreparsedData(IntPtr HidDeviceObject, out IntPtr PreparsedData);

    [LibraryImport("hid")]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool HidD_FreePreparsedData(IntPtr PreparsedData);

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

    static unsafe string? GetString(IntPtr handle, Func<IntPtr, IntPtr, uint, bool> proc)
    {
        var buf = new byte[256];

        fixed (byte* buffer = buf)
        {
            if (!proc(handle, (IntPtr)buffer, (uint)buf.Length))
                return null;
        }

        var str = Encoding.Unicode.GetString(buf, 0, buf.Length);

        return str.Contains("\0") ? str.Substring(0, str.IndexOf('\0')) : str;
    }
}
