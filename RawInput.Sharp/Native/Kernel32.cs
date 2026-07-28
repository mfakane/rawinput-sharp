using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

static partial class Kernel32
{
    [LibraryImport("kernel32", EntryPoint = "CreateFileW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateFileCore(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes, CreateDisposition dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    [LibraryImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool CloseHandle(IntPtr hObject);

    [LibraryImport("kernel32", EntryPoint = "GetModuleHandleW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr GetModuleHandleCore(string lpModuleName);

    [LibraryImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    private static partial IntPtr GetProcAddressCore(IntPtr hModule, string procName);

    [LibraryImport("kernel32", EntryPoint = "IsWow64Process", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWow64ProcessCore(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool lpSystemInfo);

    [LibraryImport("kernel32")]
    public static partial IntPtr GetCurrentProcess();

    [LibraryImport("kernel32", EntryPoint = "FormatMessageW", SetLastError = true)]
    private static partial uint FormatMessage(uint dwFlags, IntPtr lpSource, uint dwMessageId, uint dwLanguageId, IntPtr lpBuffer, int nSize, IntPtr Arguments);
        
    const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

    [Flags]
    public enum DesiredAccess : uint
    {
        None,
        Write = 0x40000000,
        Read = 0x80000000
    }

    [Flags]
    public enum ShareMode : uint
    {
        None,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004
    }

    public enum CreateDisposition : uint
    {
        CreateNew = 1,
        CreateAlways,
        OpenExisting,
        OpenAlways,
        TruncateExisting
    }

    public static IntPtr GetModuleHandle(string moduleName)
    {
        var hModule = GetModuleHandleCore(moduleName);
        if (hModule == IntPtr.Zero) throw new Win32ErrorException();

        return hModule;
    }

    public static IntPtr GetProcAddress(IntPtr hModule, string procName)
    {
        var farProc = GetProcAddressCore(hModule, procName);
        if (farProc == IntPtr.Zero) throw new Win32ErrorException();

        return farProc;
    }

    public static bool IsWow64Process(IntPtr hProcess)
    {
        if (!IsWow64ProcessCore(hProcess, out var result)) throw new Win32ErrorException();
            
        return result;
    }

    public static IntPtr CreateFile(
        string fileName,
        ShareMode shareMode,
        CreateDisposition creationDisposition,
        DesiredAccess desiredAccess = DesiredAccess.None,
        IntPtr securityAttributes = default,
        uint flagsAndAttributes = 0,
        IntPtr templateFile = default)
    {
        var handle = CreateFileCore(fileName, desiredAccess, shareMode, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);
        if (handle == new IntPtr(-1)) throw new Win32ErrorException();

        return handle;
    }

    public static bool TryCreateFile(
        string fileName,
        ShareMode shareMode,
        CreateDisposition creationDisposition,
        out IntPtr handle,
        DesiredAccess desiredAccess = DesiredAccess.None,
        IntPtr securityAttributes = default,
        uint flagsAndAttributes = 0,
        IntPtr templateFile = default)
    {
        handle = CreateFileCore(fileName, desiredAccess, shareMode, securityAttributes, creationDisposition, flagsAndAttributes, templateFile);

        return handle != new IntPtr(-1);
    }

    public static unsafe string FormatMessage(int errorCode)
    {
        const int capacity = 255;
        var message = new char[capacity];

        fixed (char* buffer = message)
        {
            var charsWritten = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, (uint)errorCode, 0, (IntPtr)buffer, capacity, IntPtr.Zero);
            if (charsWritten == 0) throw new Win32ErrorException();

            return new string(buffer, 0, (int)charsWritten);
        }
    }
}