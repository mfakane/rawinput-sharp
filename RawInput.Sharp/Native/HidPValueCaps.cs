using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// HIDP_VALUE_CAPS
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct HidPValueCaps
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

    [FieldOffset(16), MarshalAs(UnmanagedType.U1)]
    public bool HasNull;
    [FieldOffset(18)]
    public ushort BitSize;
    [FieldOffset(20)]
    public ushort ReportCount;
    [FieldOffset(32)]
    public uint UnitsExp;
    [FieldOffset(36)]
    public uint Units;
    [FieldOffset(40)]
    public int LogicalMin;
    [FieldOffset(44)]
    public int LogicalMax;
    [FieldOffset(48)]
    public int PhysicalMin;
    [FieldOffset(52)]
    public int PhysicalMax;

    [FieldOffset(56)]
    public HidPCapsRange Range;
    [FieldOffset(56)]
    public HidPCapsNotRange NotRange;
}