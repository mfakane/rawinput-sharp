# RawInput.Sharp

A simple wrapper library for Raw Input.
Available on .NET Standard 1.1, but works only on Windows.

## NuGet

https://www.nuget.org/packages/RawInput.Sharp/

## Usage

### Acquiring connected devices

```cs
// using Linearstar.Windows.RawInput;

// Get the devices that can be handled with Raw Input.
var devices = RawInputDevice.GetDevices();

// Keyboards will be returned as RawInputKeyboard.
var keyboards = devices.OfType<RawInputKeyboard>();

// Mice will be RawInputMouse.
var mice = devices.OfType<RawInputMouse>();
```

### Reading raw inputs

```cs
protected override void WndProc(ref Message m)
{
    const int WM_INPUT = 0x00FF;

    // You can read inputs by processing the WM_INPUT message.
    if (m.Msg == WM_INPUT)
    {
        // Create an RawInputData from the handle stored in lParam.
        var data = RawInputData.FromHandle(m.LParam);

        // You can identify the source device using Header.DeviceHandle or just Device.
        var sourceDeviceHandle = data.Header.DeviceHandle;
        var sourceDevice = data.Device;

        // The data will be an instance of either RawInputMouseData, RawInputKeyboardData, or RawInputHidData.
        // They contain the raw input data in their properties.
        switch (data)
        {
            case RawInputMouseData mouse:
                Console.WriteLine(mouse.Mouse);
                break;
            case RawInputKeyboardData keyboard:
                Console.WriteLine(keyboard.Keyboard);
                break;
            case RawInputHidData hid:
                Console.WriteLine(hid.Hid);
                break;
        }
    }

    base.WndProc(ref m);
}
```
