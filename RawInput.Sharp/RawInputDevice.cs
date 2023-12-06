using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public abstract class RawInputDevice
{
    bool gotAttributes;
    string? productName;
    string? manufacturerName;
    string? serialNumber;

    protected RawInputDeviceInfo DeviceInfo { get; }

    public RawInputDeviceHandle Handle { get; }
    public RawInputDeviceType DeviceType => DeviceInfo.Type;
    public string? DevicePath { get; }

    public string? ManufacturerName
    {
        get
        {
            if (manufacturerName == null) GetAttributesOnce();
            return manufacturerName;
        }
    }

    public string? ProductName
    {
        get
        {
            if (productName == null) GetAttributesOnce();
            return productName;
        }
    }

    public string? SerialNumber
    {
        get
        {
            if (serialNumber == null) GetAttributesOnce();
            return serialNumber;
        }
    }

    public bool IsConnected =>
        DevicePath != null && CfgMgr32.TryLocateDevNode(DevicePath, CfgMgr32.LocateDevNodeFlags.Normal, out _) == ConfigReturnValue.Success;

    public abstract HidUsageAndPage UsageAndPage { get; }
    public abstract int VendorId { get; }
    public abstract int ProductId { get; }

    void GetAttributesOnce()
    {
        if (gotAttributes) return;
        gotAttributes = true;

        if (DevicePath == null) return;
        GetAttributesFromHidD();
        if (manufacturerName == null || productName == null) GetAttributesFromCfgMgr();
    }

    void GetAttributesFromHidD()
    {
        if (DevicePath == null || !HidD.TryOpenDevice(DevicePath, out var device)) return;

        try
        {
            manufacturerName ??= HidD.GetManufacturerString(device);
            productName ??= HidD.GetProductString(device);
            serialNumber ??= HidD.GetSerialNumberString(device);
        }
        finally
        {
            HidD.CloseDevice(device);
        }
    }

    void GetAttributesFromCfgMgr()
    {
        if (DevicePath == null) return;

        var path = DevicePath.Substring(4).Replace('#', '\\');
        if (path.Contains("{")) path = path.Substring(0, path.IndexOf('{') - 1);

        var device = CfgMgr32.LocateDevNode(path, CfgMgr32.LocateDevNodeFlags.Phantom);

        manufacturerName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.DeviceManufacturer);
        productName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.DeviceFriendlyName);
        productName ??= CfgMgr32.GetDevNodePropertyString(device, in DevicePropertyKey.Name);
    }

    protected RawInputDevice(RawInputDeviceHandle device, RawInputDeviceInfo deviceInfo)
    {
        Handle = device;
        DevicePath = User32.GetRawInputDeviceName(device);
        DeviceInfo = deviceInfo;
    }

    public static RawInputDevice FromHandle(RawInputDeviceHandle device)
    {
        var deviceInfo = User32.GetRawInputDeviceInfo(device);

        switch (deviceInfo.Type)
        {
            case RawInputDeviceType.Mouse:
                return new RawInputMouse(device, deviceInfo);
            case RawInputDeviceType.Keyboard:
                return new RawInputKeyboard(device, deviceInfo);
            case RawInputDeviceType.Hid:
                return RawInputDigitizer.IsSupported(deviceInfo.Hid.UsageAndPage)
                    ? new RawInputDigitizer(device, deviceInfo)
                    : new RawInputHid(device, deviceInfo);
            default:
                throw new ArgumentException();
        }
    }

    /// <summary>
    /// Gets available devices that can be handled with Raw Input.
    /// </summary>
    /// <returns>Array of <see cref="RawInputDevice"/>, which contains mouse as a <see cref="RawInputMouse"/>, keyboard as a <see cref="RawInputKeyboard"/>, and any other HIDs as a <see cref="RawInputHid"/>.</returns>
    public static RawInputDevice[] GetDevices()
    {
        var devices = User32.GetRawInputDeviceList();

        return devices.Select(i => FromHandle(i.Device)).ToArray();
    }

    public byte[] GetPreparsedData() =>
        User32.GetRawInputDevicePreparsedData(Handle);

    public static void RegisterDevice(HidUsageAndPage usageAndPage, RawInputDeviceFlags flags, IntPtr hWndTarget) =>
        RegisterDevice(new RawInputDeviceRegistration(usageAndPage, flags, hWndTarget));

    public static void RegisterDevice(params RawInputDeviceRegistration[] devices) =>
        User32.RegisterRawInputDevices(devices);

    public static void UnregisterDevice(HidUsageAndPage usageAndPage) =>
        RegisterDevice(usageAndPage, RawInputDeviceFlags.Remove, IntPtr.Zero);

    public static RawInputDeviceRegistration[] GetRegisteredDevices() =>
        User32.GetRegisteredRawInputDevices();
}