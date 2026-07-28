using Linearstar.Windows.RawInput;
using System.ComponentModel;
using System.Runtime.InteropServices;
using static NativeMethods;


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

var windowClassBuffer = Marshal.AllocHGlobal(Marshal.SizeOf<WNDCLASS>());
ushort classAtom;

try
{
    Marshal.StructureToPtr(wind_class, windowClassBuffer, false);
    classAtom = RegisterClassW(windowClassBuffer);
}
finally
{
    Marshal.FreeHGlobal(windowClassBuffer);
}

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
while (true)
{
    var result = GetMessage(out var msg, IntPtr.Zero, 0, 0);
    if (result == -1)
        throw new Win32Exception();
    if (result == 0)
        break;

    TranslateMessage(msg);
    DispatchMessage(msg);
}

GC.KeepAlive(wind_class.lpfnWndProc);

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

[StructLayout(LayoutKind.Sequential)]
struct MSG
{
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
    public uint lPrivate;
}

[StructLayout(LayoutKind.Sequential)]
struct POINT
{
    public int x;
    public int y;
}

static partial class NativeMethods
{
    [LibraryImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
    internal static partial int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TranslateMessage(in MSG lpMsg);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    internal static partial IntPtr DispatchMessage(in MSG lpMsg);

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial ushort RegisterClassW(IntPtr lpWndClass);

    [LibraryImport("user32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr CreateWindowExW(uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle, int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [LibraryImport("user32.dll", EntryPoint = "DefWindowProcW")]
    internal static partial IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
}
