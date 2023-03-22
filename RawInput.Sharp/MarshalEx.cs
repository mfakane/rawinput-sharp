using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput;

static class MarshalEx
{
    public static int SizeOf<T>() => Marshal.SizeOf(typeof(T));
}