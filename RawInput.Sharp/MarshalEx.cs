using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput;

static class MarshalEx
{
#if NET7_0_OR_GREATER
    public static int SizeOf<T>() => Marshal.SizeOf<T>();
#else
    public static int SizeOf<T>() => Marshal.SizeOf(typeof(T));
#endif
}