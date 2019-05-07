using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Linearstar.Windows.RawInput.Native
{
    public static class HidD
    {
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes, CreateDisposition dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [SuppressUnmanagedCodeSecurity, DllImport("kernel32", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        enum DesiredAccess : uint
        {
            None,
            Write = 0x40000000,
            Read = 0x80000000
        }

        [Flags]
        enum ShareMode : uint
        {
            None,
            Read = 0x00000001,
            Write = 0x00000002,
            Delete = 0x00000004
        }

        enum CreateDisposition : uint
        {
            CreateNew = 1,
            CreateAlways,
            OpenExisting,
            OpenAlways,
            TruncateExisting
        }

        [SuppressUnmanagedCodeSecurity, DllImport("hid", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool HidD_GetManufacturerString(IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength);

        [SuppressUnmanagedCodeSecurity, DllImport("hid", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool HidD_GetProductString(IntPtr HidDeviceObject, [Out] byte[] Buffer, uint BufferLength);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool HidD_GetPreparsedData(IntPtr HidDeviceObject, out IntPtr PreparsedData);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        [return: MarshalAs(UnmanagedType.U1)]
        static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);

        public static HidDeviceHandle OpenDevice(string devicePath)
        {
            var deviceHandle = CreateFile(devicePath, DesiredAccess.None, ShareMode.Read | ShareMode.Write, IntPtr.Zero, CreateDisposition.OpenExisting, 0, IntPtr.Zero);
            if (deviceHandle == new IntPtr(-1)) throw new Win32Exception();

            return (HidDeviceHandle)deviceHandle;
        }

        public static bool TryOpenDevice(string devicePath, out HidDeviceHandle device)
        {
            var deviceHandle = CreateFile(devicePath, DesiredAccess.None, ShareMode.Read | ShareMode.Write, IntPtr.Zero, CreateDisposition.OpenExisting, 0, IntPtr.Zero);

            if (deviceHandle == new IntPtr(-1))
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

            CloseHandle(deviceHandle);
        }

        public static string GetManufacturerString(HidDeviceHandle device)
        {
            var deviceHandle = HidDeviceHandle.GetRawValue(device);
            
            return GetString(deviceHandle, HidD_GetManufacturerString);
        }

        public static string GetProductString(HidDeviceHandle device)
        {
            var deviceHandle = HidDeviceHandle.GetRawValue(device);

            return GetString(deviceHandle, HidD_GetProductString);
        }

        public static HidPreparsedData GetPreparsedData(HidDeviceHandle device)
        {
            var deviceHandle = HidDeviceHandle.GetRawValue(device);

            HidD_GetPreparsedData(deviceHandle, out var preparsedData);

            return (HidPreparsedData)preparsedData;
        }

        public static void FreePreparsedData(HidPreparsedData preparsedData)
        {
            var preparsedDataPointer = HidPreparsedData.GetRawValue(preparsedData);

            HidD_FreePreparsedData(preparsedDataPointer);
        }

        static string GetString(IntPtr handle, Func<IntPtr, byte[], uint, bool> proc)
        {
            var buf = new byte[256];

            if (!proc(handle, buf, (uint)buf.Length))
                return null;

            var str = Encoding.Unicode.GetString(buf);

            return str.Contains("\0") ? str.Substring(0, str.IndexOf('\0')) : str;
        }
    }
}
