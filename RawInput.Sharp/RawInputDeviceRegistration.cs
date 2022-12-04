using System;

namespace Linearstar.Windows.RawInput;

public readonly struct RawInputDeviceRegistration
{
    readonly ushort usUsagePage;
    readonly ushort usUsage;
    readonly RawInputDeviceFlags dwFlags;
    readonly IntPtr hwndTarget;

    public ushort UsagePage => usUsagePage;
    public ushort Usage => usUsage;
    public RawInputDeviceFlags Flags => dwFlags;
    public IntPtr HwndTarget => hwndTarget;

    public RawInputDeviceRegistration(HidUsageAndPage usageAndPage, RawInputDeviceFlags flags, IntPtr hWndTarget)
        : this(usageAndPage.UsagePage, usageAndPage.Usage, flags, hWndTarget)
    {
    }

    public RawInputDeviceRegistration(ushort usagePage, ushort usage, RawInputDeviceFlags flags, IntPtr hWndTarget)
    {
        usUsagePage = usagePage;
        usUsage = usage;
        dwFlags = flags;
        hwndTarget = hWndTarget;
    }
}