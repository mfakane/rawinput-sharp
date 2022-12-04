namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RIDI_*
/// </summary>
public enum RawInputDeviceInfoBehavior : uint
{
    /// <summary>
    /// RIDI_PREPARSEDDATA
    /// </summary>
    PreparsedData = 0x20000005,
    /// <summary>
    /// RIDI_DEVICENAME
    /// </summary>
    DeviceName = 0x20000007,
    /// <summary>
    /// RIDI_DEVICEINFO
    /// </summary>
    DeviceInfo = 0x2000000b,
}