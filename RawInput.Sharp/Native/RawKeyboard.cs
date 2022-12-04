using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWKEYBOARD
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct RawKeyboard
{
    readonly ushort usMakeCode;
    readonly RawKeyboardFlags usFlags;
    readonly ushort usReserverd;
    readonly ushort usVKey;
    readonly uint ulMessage;
    readonly uint ulExtraInformation;

    public int ScanCode => usMakeCode;
    public RawKeyboardFlags Flags => usFlags;
    public int VirutalKey => usVKey;
    public uint WindowMessage => ulMessage;
    public uint ExtraInformation => ulExtraInformation;

    public override string ToString() =>
        $"{{Key: {VirutalKey}, ScanCode: {ScanCode}, Flags: {Flags}}}";
}

/// <summary>
/// RI_KEY_*
/// </summary>
[Flags]
public enum RawKeyboardFlags : ushort
{
    /// <summary>
    /// RI_KEY_MAKE
    /// </summary>
    None = 0,
    /// <summary>
    /// RI_KEY_BREAK
    /// </summary>
    Up = 1,
    /// <summary>
    /// RI_KEY_E0
    /// </summary>
    KeyE0 = 2,
    /// <summary>
    /// RI_KEY_E1
    /// </summary>
    KeyE1 = 4,
}