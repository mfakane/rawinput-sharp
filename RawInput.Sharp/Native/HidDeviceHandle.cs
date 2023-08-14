using System;

namespace Linearstar.Windows.RawInput.Native;

public readonly struct HidDeviceHandle : IEquatable<HidDeviceHandle>
{
    readonly IntPtr value;

    public static HidDeviceHandle Zero => (HidDeviceHandle)IntPtr.Zero;

    HidDeviceHandle(IntPtr value) => this.value = value;

    public static IntPtr GetRawValue(HidDeviceHandle handle) => handle.value;

    public static explicit operator HidDeviceHandle(IntPtr value) => new(value);

    public static bool operator ==(HidDeviceHandle a, HidDeviceHandle b) => a.Equals(b);

    public static bool operator !=(HidDeviceHandle a, HidDeviceHandle b) => !a.Equals(b);

    public bool Equals(HidDeviceHandle other) => value.Equals(other.value);

    public override bool Equals(object? obj) =>
        obj is HidDeviceHandle other &&
        Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value.ToString();
}