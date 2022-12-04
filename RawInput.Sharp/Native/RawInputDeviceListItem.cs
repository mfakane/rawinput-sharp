namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWINPUTDEVICELIST
/// </summary>
public struct RawInputDeviceListItem
{
    /// <summary>
    /// hDevice
    /// </summary>
    public RawInputDeviceHandle Device { get; set; }

    /// <summary>
    /// dwType
    /// </summary>
    public RawInputDeviceType Type { get; set; }
}