using Linearstar.Windows.RawInput;
using System.ComponentModel;
using System.Runtime.InteropServices;


var WindowClass = "HelperWindowClass";
var wind_class = new WNDCLASS
{
    lpszClassName = Marshal.StringToHGlobalUni(WindowClass),
    lpfnWndProc = (hWnd, msg, wParam, lParam) =>
    {
        const int WM_INPUT = 0x00FF;

        // You can read inputs by processing the WM_INPUT message.
        if (msg == WM_INPUT)
        {
            // Create an RawInputData from the handle stored in lParam.
            var data = RawInputData.FromHandle(lParam);

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

        // The normal way to quit is sending WM_CLOSE message to the window
        // if (msg == 0x0002) { // WM_DESTORY
        //     PostQuitMessage(0);
        //     return nint.Zero;
        // }

        // handle the messages here
        return DefWindowProc(hWnd, msg, wParam, lParam);
    }
};

ushort classAtom = RegisterClassW(ref wind_class);

if (classAtom == 0)
    throw new Win32Exception();

const uint WS_EX_NOACTIVATE = 0x08000000;
const uint WS_POPUP = 0x80000000;
IntPtr hWnd = CreateWindowExW(
    WS_EX_NOACTIVATE,
    WindowClass,
    "",
    WS_POPUP,
    0, 0, 0, 0,
    IntPtr.Zero,
    IntPtr.Zero,
    IntPtr.Zero,
    IntPtr.Zero
);

if (hWnd == IntPtr.Zero)
    throw new Win32Exception();


// Get the devices that can be handled with Raw Input.
var devices = RawInputDevice.GetDevices();

// register the keyboard device and you can register device which you need like mouse
RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard,
    RawInputDeviceFlags.ExInputSink | RawInputDeviceFlags.NoLegacy, hWnd);


// Message loop
while (GetMessage(out var msg, IntPtr.Zero, 0, 0))
{
    TranslateMessage(msg);
    DispatchMessage(msg);
}


[DllImport("user32.dll")]
static extern bool GetMessage(out IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
[DllImport("user32.dll")]
static extern bool TranslateMessage(in IntPtr lpMsg);
[DllImport("user32.dll")]
static extern IntPtr DispatchMessage(in IntPtr lpMsg);


[DllImport("user32.dll", SetLastError = true)]
static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

[DllImport("user32.dll", SetLastError = true)]
static extern IntPtr CreateWindowExW(uint dwExStyle,
    [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
    [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
    uint dwStyle, int x, int y, int nWidth, int nHeight,
    IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

[DllImport("user32.dll")]
static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);


delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

[StructLayout(LayoutKind.Sequential)]
struct WNDCLASS
{
    public uint style;
    public WndProc lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    public string lpszMenuName;
    public nint lpszClassName;
}
