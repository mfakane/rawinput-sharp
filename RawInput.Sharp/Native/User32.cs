using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Linearstar.Windows.RawInput.Native;

public static class User32
{
    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputDeviceList([Out] RawInputDeviceListItem[]? pRawInputDeviceList, ref uint puiNumDevices, uint cbSize);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, IntPtr pData, out uint pcbSize);

    [DllImport("user32", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, StringBuilder pData, in uint pcbSize);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, out RawInputDeviceInfo pData, in uint pcbSize);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputDeviceInfo(IntPtr hDevice, RawInputDeviceInfoBehavior uiBehavior, [Out] byte[] pData, in uint pcbSize);

    [DllImport("user32", SetLastError = true)]
    static extern bool RegisterRawInputDevices(RawInputDeviceRegistration[] pRawInputDevices, uint uiNumDevices, uint cbSize);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRegisteredRawInputDevices([Out] RawInputDeviceRegistration[]? pRawInputDevices, ref uint puiNumDevices, uint cbSize);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);


    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputData(IntPtr hRawInput, RawInputGetBehavior uiBehavior, out RawInputHeader pData, ref uint pcbSize, uint cbSizeHeader);

    [DllImport("user32", SetLastError = true)]
    static extern uint GetRawInputBuffer(IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

    [DllImport("user32", SetLastError = true)]
    static extern IntPtr DefRawInputProc(byte[] paRawInput, int nInput, uint cbSizeHeader);

    public enum RawInputGetBehavior : uint
    {
        Input = 0x10000003,
        Header = 0x10000005,
    }

    public static RawInputDeviceListItem[] GetRawInputDeviceList()
    {
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceListItem>();

        // Get device count by passing null for pRawInputDeviceList.
        uint deviceCount = 0;
        GetRawInputDeviceList(null, ref deviceCount, size);

        // Now, fill the buffer using the device count.
        var devices = new RawInputDeviceListItem[deviceCount];
        GetRawInputDeviceList(devices, ref deviceCount, size).EnsureSuccess();

        return devices;
    }

    public static string? GetRawInputDeviceName(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);
          
        // Get the length of the device name first.
        // For RIDI_DEVICENAME, the value in the pcbSize is the character count instead of the byte count.
        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceName, IntPtr.Zero, out var size);

        if (size <= 2) return null;

        var sb = new StringBuilder((int)size);
        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceName, sb, in size).EnsureSuccess();

        return sb.ToString();
    }

    public static RawInputDeviceInfo GetRawInputDeviceInfo(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceInfo>();

        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.DeviceInfo, out var deviceInfo, in size).EnsureSuccess();

        return deviceInfo;
    }

    public static byte[] GetRawInputDevicePreparsedData(RawInputDeviceHandle device)
    {
        var deviceHandle = RawInputDeviceHandle.GetRawValue(device);

        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.PreparsedData, IntPtr.Zero, out var size);

        if (size == 0) return new byte[0];

        var rt = new byte[size];
        GetRawInputDeviceInfo(deviceHandle, RawInputDeviceInfoBehavior.PreparsedData, rt, in size).EnsureSuccess();

        return rt;
    }

    public static void RegisterRawInputDevices(params RawInputDeviceRegistration[] devices)
    {
        RegisterRawInputDevices(devices, (uint)devices.Length, (uint)MarshalEx.SizeOf<RawInputDeviceRegistration>()).EnsureSuccess();
    }

    public static RawInputDeviceRegistration[] GetRegisteredRawInputDevices()
    {
        var size = (uint)MarshalEx.SizeOf<RawInputDeviceRegistration>();

        uint count = 0;
        GetRegisteredRawInputDevices(null, ref count, size);

        var rt = new RawInputDeviceRegistration[count];
        GetRegisteredRawInputDevices(rt, ref count, size).EnsureSuccess();

        return rt;
    }

    public static RawInputHeader GetRawInputDataHeader(RawInputHandle rawInput)
    {
        var hRawInput = RawInputHandle.GetRawValue(rawInput);
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();
        var size = headerSize;

        GetRawInputData(hRawInput, RawInputGetBehavior.Header, out var header, ref size, headerSize).EnsureSuccess();

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

    public static void DefRawInputProc(byte[] paRawInput)
    {
        var headerSize = (uint)MarshalEx.SizeOf<RawInputHeader>();

        DefRawInputProc(paRawInput, paRawInput.Length, headerSize);
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