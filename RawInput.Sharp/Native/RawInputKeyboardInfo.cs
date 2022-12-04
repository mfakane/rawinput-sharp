using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RID_DEVICE_INFO_KEYBOARD
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct RawInputKeyboardInfo
{
    readonly int dwType;
    readonly int dwSubType;
    readonly int dwKeyboardMode;
    readonly int dwNumberOfFunctionKeys;
    readonly int dwNumberOfIndicators;
    readonly int dwNumberOfKeysTotal;

    /// <summary>
    /// dwType
    /// </summary>
    public int KeyboardType => dwType;

    /// <summary>
    /// dwSubType
    /// </summary>
    public int KeyboardSubType => dwSubType;

    /// <summary>
    /// dwKeyboardMode
    /// </summary>
    public int KeyboardMode => dwKeyboardMode;

    /// <summary>
    /// dwNumberOfFunctionKeys
    /// </summary>
    public int FunctionKeyCount => dwNumberOfFunctionKeys;

    /// <summary>
    /// dwNumberOfIndicators
    /// </summary>
    public int IndicatorCount => dwNumberOfIndicators;

    /// <summary>
    /// dwNumberOfKeysTotal
    /// </summary>
    public int TotalKeyCount => dwNumberOfKeysTotal;
}