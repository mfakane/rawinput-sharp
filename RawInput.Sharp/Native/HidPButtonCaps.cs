using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// HIDP_BUTTON_CAPS
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct HidPButtonCaps
{
    [FieldOffset(0)]
    public ushort UsagePage;
    [FieldOffset(2)]
    public byte ReportID;
    [FieldOffset(3), MarshalAs(UnmanagedType.U1)]
    public bool IsAlias;
    [FieldOffset(4)]
    public ushort BitField;
    [FieldOffset(6)]
    public ushort LinkCollection;
    [FieldOffset(8)]
    public ushort LinkUsage;
    [FieldOffset(10)]
    public ushort LinkUsagePage;
    [FieldOffset(12), MarshalAs(UnmanagedType.U1)]
    public bool IsRange;
    [FieldOffset(13), MarshalAs(UnmanagedType.U1)]
    public bool IsStringRange;
    [FieldOffset(14), MarshalAs(UnmanagedType.U1)]
    public bool IsDesignatorRange;
    [FieldOffset(15), MarshalAs(UnmanagedType.U1)]
    public bool IsAbsolute;

    [FieldOffset(16), MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    readonly int[] Reserved;
    [FieldOffset(56)]
    public HidPCapsRange Range;
    [FieldOffset(56)]
    public HidPCapsNotRange NotRange;
}