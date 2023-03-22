using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RID_DEVICE_INFO_MOUSE
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct RawInputMouseInfo
{
    readonly int dwId;
    readonly int dwNumberOfButtons;
    readonly int dwSampleRate;
    [MarshalAs(UnmanagedType.Bool)]
    readonly bool fHasHorizontalWheel;
        
    /// <summary>
    /// dwId
    /// </summary>
    public int Id => dwId;

    /// <summary>
    /// dwNumberOfButtons
    /// </summary>
    public int ButtonCount => dwNumberOfButtons;

    /// <summary>
    /// dwSampleRate
    /// </summary>
    public int SampleRate => dwSampleRate;

    /// <summary>
    /// fHasHorizontalWheel
    /// </summary>
    public bool HasHorizontalWheel => fHasHorizontalWheel;
}