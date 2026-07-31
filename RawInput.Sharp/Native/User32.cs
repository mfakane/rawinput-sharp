using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

public static partial class User32
{
    [LibraryImport("user32", SetLastError = true)]
    private static partial uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);

    [LibraryImport("user32", EntryPoint = "GetRawInputDeviceInfoW", SetLastError = true)]
    private static partial uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, IntPtr pData, ref uint pcbSize);

    [LibraryImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RegisterRawInputDevices(IntPtr pRawInputDevices, uint uiNumDevices, uint cbSize);

    [LibraryImport("user32", SetLastError = true)]
    private static partial uint GetRegisteredRawInputDevices(IntPtr pRawInputDevices, ref uint puiNumDevices, uint cbSize);

    [LibraryImport("user32", SetLastError = true)]
    private static partial uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    [LibraryImport("user32", SetLastError = true)]
    private static partial uint GetRawInputBuffer(IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    [LibraryImport("user32", SetLastError = true)]
    private static partial IntPtr DefRawInputProc(IntPtr paRawInput, int nInput, uint cbSizeHeader);

    public enum RawInputGetBehavior : uint
    {
        Input = 0x10000003,
        Header = 0x10000005,
    }

    public static unsafe RawInputDeviceListItem[] GetRawInputDeviceList()
    {
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceListItem>();

        // Get device count by passing null for pRawInputDeviceList.
        uint deviceCount = 0;
        GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, size);

        // Now, fill the buffer using the device count.
        var devices = new RawInputDeviceListItem[deviceCount];
        fixed (RawInputDeviceListItem* buffer = devices)
            GetRawInputDeviceList((IntPtr)buffer, ref deviceCount, size).EnsureSuccess();

        return devices;
    }

    public static string? GetRawInputDeviceName(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);
          
        // Get the length of the device name first.
        // For RIDI_DEVICENAME, the value in the pcbSize is the character count instead of the byte count.
        uint size = 0;
        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceName, IntPtr.Zero, ref size);

        if (size <= 2) return null;

        var buffer = Marshal.AllocHGlobal(checked((int)size * sizeof(char)));

        try
        {
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceName, buffer, ref size).EnsureSuccess();
            return Marshal.PtrToStringUni(buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public static RawInputDeviceInfo GetRawInputDeviceInfo(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceInfo>();

        var buffer = Marshal.AllocHGlobal((int)size);

        try
        {
            Marshal.WriteInt32(buffer, (int)size);
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceInfo, buffer, ref size).EnsureSuccess();
            return Marshal.PtrToStructure<RawInputDeviceInfo>(buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public static unsafe byte[] GetRawInputDevicePreparsedData(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);

        uint size = 0;
        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.PreparsedData, IntPtr.Zero, ref size);

        if (size == 0) return new byte[0];

        var rt = new byte[size];
        fixed (byte* buffer = rt)
            GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.PreparsedData, (IntPtr)buffer, ref size).EnsureSuccess();

        return rt;
    }

    public static unsafe void RegisterRawInputDevices(params RawInputDeviceRegistration[] devices)
    {
        fixed (RawInputDeviceRegistration* buffer = devices)
            RegisterRawInputDevices((IntPtr)buffer, (uint)devices.Length, (uint)MarshalEx.SizeOf<RawInputDeviceRegistration>()).EnsureSuccess();
    }

    public static unsafe RawInputDeviceRegistration[] GetRegisteredRawInputDevices()
    {
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceRegistration>();

        uint count = 0;
        GetRegisteredRawInputDevices(IntPtr.Zero, ref count, size);

        if (count == 0)
            return Array.Empty<RawInputDeviceRegistration>();

        var rt = new RawInputDeviceRegistration[count];
        fixed (RawInputDeviceRegistration* buffer = rt)
            GetRegisteredRawInputDevices((IntPtr)buffer, ref count, size).EnsureSuccess();

        return rt;
    }

    public static unsafe RawInputHeader GetRawInputDataHeader(RawInputHandle rawInput)
    {
        var hRawInput = RawInputHandle.GetRawValue(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        var size = headerSize;

        RawInputHeader header;
        GetRawInputData(hRawInput, RawInputGetBehavior.Header, (IntPtr)(&header), ref size, headerSize).EnsureSuccess();

        return header;
    }

    public static uint GetRawInputDataSize(RawInputHandle rawInput)
    {
        var hRawInput = RawInputHandle.GetRawValue(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        uint size = 0;

        GetRawInputData(hRawInput, RawInputGetBehavior.Input, IntPtr.Zero, ref size, headerSize);

        return size;
    }

    public static void GetRawInputData(RawInputHandle rawInput, IntPtr ptr, uint size)
    {
        var hRawInput = RawInputHandle.GetRawValue(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();

        GetRawInputData(hRawInput, RawInputGetBehavior.Input, ptr, ref size, headerSize).EnsureSuccess();
    }

    public static unsafe RawMouse GetRawInputMouseData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        var bytes = new byte[size];

        fixed (byte* bytesPtr = bytes)
        {
            GetRawInputData(rawInput, (IntPtr)bytesPtr, size);

            header = *(RawInputHeader*)bytesPtr;

            return *(RawMouse*)(bytesPtr + headerSize);
        }
    }

    public static unsafe RawKeyboard GetRawInputKeyboardData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        var bytes = new byte[size];

        fixed (byte* bytesPtr = bytes)
        {
            GetRawInputData(rawInput, (IntPtr)bytesPtr, size);

            header = *(RawInputHeader*)bytesPtr;

            return *(RawKeyboard*)(bytesPtr + headerSize);
        }
    }

    public static unsafe RawHid GetRawInputHidData(RawInputHandle rawInput, out RawInputHeader header)
    {
        var size = GetRawInputDataSize(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        var bytes = new byte[size];

        fixed (byte* bytesPtr = bytes)
        {
            GetRawInputData(rawInput, (IntPtr)bytesPtr, size);

            header = *(RawInputHeader*)bytesPtr;

            return RawHid.FromPointer(bytesPtr + headerSize);
        }
    }

    public static uint GetRawInputBufferSize()
    {
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        uint size = 0;

        GetRawInputBuffer(IntPtr.Zero, ref size, headerSize);

        return size;
    }

    public static uint GetRawInputBuffer(IntPtr ptr, uint size)
    {
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();

        return GetRawInputBuffer(ptr, ref size, headerSize).EnsureSuccess();
    }

    public static unsafe void DefRawInputProc(byte[] paRawInput)
    {
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();

        fixed (byte* buffer = paRawInput)
            DefRawInputProc((IntPtr)buffer, paRawInput.Length, headerSize);
    }

    public static bool EnsureSuccess(this bool result)
    {
        if (!result) throw new Win32ErrorException();

        return result;
    }

    public static uint EnsureSuccess(this uint result)
    {
        if (result == unchecked((uint)-1)) throw new Win32ErrorException();

        return result;
    }
}