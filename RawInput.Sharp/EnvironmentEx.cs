using System;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

static class EnvironmentEx
{
    public static bool Is64BitOperatingSystem
    {
        get
        {
            var isWow64ProcessProc = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("kernel32"), "IsWow64Process");

            return isWow64ProcessProc != IntPtr.Zero
                   && Kernel32.IsWow64Process(Kernel32.GetCurrentProcess());
        }
    }

    public static bool Is64BitProcess => IntPtr.Size == 8;
}