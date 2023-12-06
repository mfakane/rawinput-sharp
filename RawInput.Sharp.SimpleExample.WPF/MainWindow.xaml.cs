using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using Linearstar.Windows.RawInput;

namespace RawInput.Sharp.SimpleExample.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object? sender, EventArgs e)
        {
            var windowInteropHelper = new WindowInteropHelper(this);
            var hwnd = windowInteropHelper.Handle;

            // Get the devices that can be handled with Raw Input.
            var devices = RawInputDevice.GetDevices();

            // register the keyboard device and you can register device which you need like mouse
            RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard,
                RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, hwnd);

            HwndSource source = HwndSource.FromHwnd(hwnd);
            source.AddHook(Hook);
        }

        private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            const int WM_INPUT = 0x00FF;

            // You can read inputs by processing the WM_INPUT message.
            if (msg == WM_INPUT)
            {
                // Create an RawInputData from the handle stored in lParam.
                var data = RawInputData.FromHandle(lparam);

                // You can identify the source device using Header.DeviceHandle or just Device.
                var sourceDeviceHandle = data.Header.DeviceHandle;
                var sourceDevice = data.Device;

                // The data will be an instance of either RawInputMouseData, RawInputKeyboardData, or RawInputHidData.
                // They contain the raw input data in their properties.
                switch (data)
                {
                    case RawInputMouseData mouse:
                        Debug.WriteLine(mouse.Mouse);
                        break;
                    case RawInputKeyboardData keyboard:
                        Debug.WriteLine(keyboard.Keyboard);
                        break;
                    case RawInputHidData hid:
                        Debug.WriteLine(hid.Hid);
                        break;
                }
            }

            return IntPtr.Zero;
        }
    }
}
