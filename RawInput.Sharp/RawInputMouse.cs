using System;
using System.Globalization;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputMouse : RawInputDevice
{
    public override HidUsageAndPage UsageAndPage => HidUsageAndPage.Mouse;

    public override int VendorId =>
        DevicePath?.Contains("VID_") == true
            ? int.Parse(DevicePath.Substring(DevicePath.IndexOf("VID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public override int ProductId =>
        DevicePath?.Contains("PID_") == true
            ? int.Parse(DevicePath.Substring(DevicePath.IndexOf("PID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public int Id => DeviceInfo.Mouse.Id;
    public int ButtonCount => DeviceInfo.Mouse.ButtonCount;
    public int SampleRate => DeviceInfo.Mouse.SampleRate;
    public bool HasHorizontalWheel => DeviceInfo.Mouse.HasHorizontalWheel;

    internal RawInputMouse(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
        : base(device, deviceInfo)
    {
        if (deviceInfo.Type != RawInputDeviceType.Mouse) throw new ArgumentException($"Device type must be {RawInputDeviceType.Mouse}.", nameof(deviceInfo));
    }
}