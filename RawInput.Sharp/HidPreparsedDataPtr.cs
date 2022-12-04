using System;
using System.Runtime.InteropServices;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidPreparsedDataPtr : SafeHandle
{
    readonly GCHandle? gcHandle;

    public HidPreparsedDataPtr(HidPreparsedData handle)
        : base(IntPtr.Zero, true) =>
        this.handle = HidPreparsedData.GetRawValue(handle);

    public HidPreparsedDataPtr(byte[] preparsedData)
        : base(IntPtr.Zero, true)
    {
        gcHandle = GCHandle.Alloc(preparsedData, GCHandleType.Pinned);
        handle = gcHandle.Value.AddrOfPinnedObject();
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        gcHandle?.Free();

        return true;
    }

    public static implicit operator HidPreparsedData(HidPreparsedDataPtr ptr) =>
        (HidPreparsedData)ptr.DangerousGetHandle();
}