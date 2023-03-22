using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public abstract class RawInputData
{
    RawInputDevice? device;

    public RawInputHeader Header { get; }

    public RawInputDevice? Device =>
        device ??= Header.DeviceHandle != RawInputDeviceHandle.Zero
            ? RawInputDevice.FromHandle(Header.DeviceHandle)
            : null;

    protected RawInputData(RawInputHeader header)
    {
        Header = header;
    }

    public static RawInputData FromHandle(IntPtr lParam)
        => FromHandle((RawInputHandle)lParam);

    public static RawInputData FromHandle(RawInputHandle rawInput)
    {
        var header = User32.GetRawInputDataHeader(rawInput);

        switch (header.Type)
        {
            case RawInputDeviceType.Mouse:
                return new RawInputMouseData(header, User32.GetRawInputMouseData(rawInput, out _));
            case RawInputDeviceType.Keyboard:
                return new RawInputKeyboardData(header, User32.GetRawInputKeyboardData(rawInput, out _));
            case RawInputDeviceType.Hid:
                return RawInputHidData.Create(header, User32.GetRawInputHidData(rawInput, out _));
            default:
                throw new ArgumentException();
        }
    }

    static unsafe RawInputData ParseRawInputBufferItem(byte* ptr)
    {
        var header = *(RawInputHeader*)ptr;
        var headerSize = MarshalEx.SizeOf<RawInputHeader>();
        var dataPtr = ptr + headerSize;

        // RAWINPUT structure must be aligned by 8 bytes on WOW64
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getrawinputbuffer#remarks
        if (EnvironmentEx.Is64BitProcess && EnvironmentEx.Is64BitOperatingSystem) dataPtr += 8;

        switch (header.Type)
        {
            case RawInputDeviceType.Mouse:
                return new RawInputMouseData(header, *(RawMouse*)dataPtr);
            case RawInputDeviceType.Keyboard:
                return new RawInputKeyboardData(header, *(RawKeyboard*)dataPtr);
            case RawInputDeviceType.Hid:
                return RawInputHidData.Create(header, RawHid.FromPointer(dataPtr));
            default:
                throw new ArgumentException();
        }
    }

    public static unsafe RawInputData[] GetBufferedData(int bufferSize = 8)
    {
        var itemSize = User32.GetRawInputBufferSize();
        if (itemSize == 0) return new RawInputData[0];

        var bytes = new byte[itemSize * bufferSize];

        fixed (byte* bytesPtr = bytes)
        {
            var count = User32.GetRawInputBuffer((IntPtr)bytesPtr, (uint)bytes.Length);
            if (count == 0) return new RawInputData[0];

            var result = new RawInputData[count];

            for (int i = 0, offset = 0; i < result.Length; i++)
            {
                var data = ParseRawInputBufferItem(bytesPtr + offset);

                result[i] = data;
                offset = Align(offset + data.Header.Size);
            }

            return result;
        }
    }

    protected static int Align(int x) => (x + IntPtr.Size - 1) & ~(IntPtr.Size - 1);

    public static void DefRawInputProc(RawInputData[] data) =>
        User32.DefRawInputProc(data.SelectMany(i => i.ToStructure()).ToArray());

    public abstract byte[] ToStructure();
}