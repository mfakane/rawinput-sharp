using System;
using System.Linq;
#if NET45
using System.Runtime.CompilerServices;
#endif

namespace Linearstar.Windows.RawInput.Native
{
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

            fixed (byte* rawDataPtr = result.rawData)
            {
#if NET45
                Unsafe.CopyBlock(&intPtr[2], rawDataPtr, (uint)result.rawData.Length);
#else
                 Buffer.MemoryCopy(&intPtr[2], rawDataPtr, result.rawData.Length, result.rawData.Length);
#endif

            }

            return result;
        }

        public ArraySegment<byte>[] ToHidReports()
        {
            var elementSize = ElementSize;
            var rawData = RawData;

            return Enumerable.Range(0, Count)
                             .Select(x => new ArraySegment<byte>(rawData, elementSize * x, elementSize))
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

                fixed (byte* rawDataPtr = rawData)
                {
#if NET45
                    Unsafe.CopyBlock(rawDataPtr, &intPtr[2], (uint)rawData.Length);
#else
                    Buffer.MemoryCopy(rawDataPtr, &intPtr[2], rawData.Length, rawData.Length);
#endif

                }
            }

            return result;
        }

        public override string ToString() =>
            $"{{Count: {Count}, Size: {ElementSize}, Content: {BitConverter.ToString(RawData).Replace("-", " ")}}}";
    }
}
