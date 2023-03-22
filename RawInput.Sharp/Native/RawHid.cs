using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// RAWHID
/// </summary>
public struct RawHid
{
    int dwSizeHid;
    int dwCount;
    byte[] rawData;

    public int ElementSize => dwSizeHid;
    public int Count => dwCount;
    public unsafe byte[] RawData => rawData;

    public static unsafe RawHid FromPointer(void* ptr)
    {
        var result = new RawHid();
        var intPtr = (int*)ptr;

        result.dwSizeHid = intPtr[0];
        result.dwCount = intPtr[1];
        result.rawData = new byte[result.ElementSize * result.Count];
        Marshal.Copy(new IntPtr(&intPtr[2]), result.rawData, 0, result.rawData.Length);

        return result;
    }

    public ArraySegment<byte>[] ToHidReports()
    {
        var elementSize = ElementSize;
        var rawDataArray = RawData;

        return Enumerable.Range(0, Count)
                         .Select(x => new ArraySegment<byte>(rawDataArray, elementSize * x, elementSize))
                         .ToArray();
    }
        
    public unsafe byte[] ToStructure()
    {
        var result = new byte[dwSizeHid * dwCount + sizeof(int) * 2];

        fixed (byte* resultPtr = result)
        {
            var intPtr = (int*)resultPtr;

            intPtr[0] = dwSizeHid;
            intPtr[1] = dwCount;
        }

        rawData.CopyTo(result, sizeof(int) * 2);

        return result;
    }

    public override string ToString() =>
        $"{{Count: {Count}, Size: {ElementSize}, Content: {BitConverter.ToString(RawData).Replace("-", " ")}}}";
}