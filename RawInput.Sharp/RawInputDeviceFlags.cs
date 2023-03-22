using System;

namespace Linearstar.Windows.RawInput;

/// <summary>
/// RIDEV_*
/// </summary>
[Flags]
public enum RawInputDeviceFlags
{
    None,
    /// <summary>
    /// RIDEV_REMOVE. Removes the top level collection from the inclusion list.
    /// </summary>
    Remove = 0x1,
    /// <summary>
    /// RIDEV_EXCLUDE. Specifies the top level collections to exclude when reading a complete usage page.
    /// </summary>
    Exclude = 0x10,
    /// <summary>
    /// RIDEV_PAGEONLY. Specifies all devices whose top level collection is from the specified <see cref="RawInputDeviceRegistration.UsagePage"/>.
    /// </summary>
    PageOnly = 0x20,
    /// <summary>
    /// RIDEV_NOLEGACY. Prevents any devices specified by <see cref="RawInputDeviceRegistration.UsagePage"/> or <see cref="RawInputDeviceRegistration.Usage"/> from generating legacy messages.
    /// </summary>
    NoLegacy = 0x30,
    /// <summary>
    /// RIDEV_INPUTSINK. Enables the caller to receive the input even when the caller is not in the foreground. Note that <see cref="RawInputDeviceRegistration.HwndTarget"/> must be specified.
    /// </summary>
    InputSink = 0x100,
    /// <summary>
    /// RIDEV_CAPTUREMOUSE. The mouse button click does not activate the other window.
    /// </summary>
    CaptureMouse = 0x200,
    /// <summary>
    /// RIDEV_NOHOTKEYS. The application-defined keyboard device hotkeys are not handled. This can be specified even if <see cref="NoLegacy"/> is not specified and <see cref="RawInputDeviceRegistration.HwndTarget"/> is <see cref="IntPtr.Zero"/>.
    /// </summary>
    NoHotKeys = 0x200,
    /// <summary>
    /// RIDEV_APPKEYS. The application command keys are handled. This can be specified only if <see cref="NoLegacy"/> is specified for a keyboard device.
    /// </summary>
    AppKeys = 0x400,
    /// <summary>
    /// RIDEV_EXINPUTSINK. Enables the caller to receive input in the background only if the foreground application does not process it.
    /// </summary>
    ExInputSink = 0x1000,
    /// <summary>
    /// RIDEV_DEVNOTIFY. Enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.
    /// </summary>
    DevNotify = 0x2000,
}