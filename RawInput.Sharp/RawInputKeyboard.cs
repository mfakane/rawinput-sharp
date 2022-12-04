using System;
using System.Globalization;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputKeyboard : RawInputDevice
{
    public override HidUsageAndPage UsageAndPage => HidUsageAndPage.Keyboard;

    public override int VendorId =>
        DevicePath?.Contains("VID_") == true
            ? int.Parse(DevicePath.Substring(DevicePath.IndexOf("VID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public override int ProductId =>
        DevicePath?.Contains("PID_") == true
            ? int.Parse(DevicePath.Substring(DevicePath.IndexOf("PID_", StringComparison.Ordinal) + 4, 4), NumberStyles.HexNumber)
            : 0;

    public int KeyboardType => DeviceInfo.Keyboard.KeyboardType;
    public int KeyboardSubType => DeviceInfo.Keyboard.KeyboardSubType;
    public int KeyboardMode => DeviceInfo.Keyboard.KeyboardMode;
    public int FunctionKeyCount => DeviceInfo.Keyboard.FunctionKeyCount;
    public int IndicatorCount => DeviceInfo.Keyboard.IndicatorCount;
    public int TotalKeyCount => DeviceInfo.Keyboard.TotalKeyCount;

    internal RawInputKeyboard(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
        : base(device, deviceInfo)
    {
        if (deviceInfo.Type != RawInputDeviceType.Keyboard) throw new ArgumentException($"Device type must be {RawInputDeviceType.Keyboard}", nameof(deviceInfo));
    }
}