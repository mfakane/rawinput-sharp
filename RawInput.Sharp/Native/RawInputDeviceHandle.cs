using System;

namespace Linearstar.Windows.RawInput.Native;

/// <summary>
/// HANDLE
/// </summary>
public readonly struct RawInputDeviceHandle : IEquatable<RawInputDeviceHandle>
{
    readonly IntPtr value;

    public static RawInputDeviceHandle Zero => (RawInputDeviceHandle)IntPtr.Zero;

    RawInputDeviceHandle(IntPtr value) => this.value = value;

    public static IntPtr GetRawValue(RawInputDeviceHandle handle) => handle.value;

    public static explicit operator RawInputDeviceHandle(IntPtr value) => new(value);

    public static bool operator ==(RawInputDeviceHandle a, RawInputDeviceHandle b) => a.Equals(b);

    public static bool operator !=(RawInputDeviceHandle a, RawInputDeviceHandle b) => !a.Equals(b);

    public bool Equals(RawInputDeviceHandle other) => value.Equals(other.value);

    public override bool Equals(object? obj) =>
        obj is RawInputDeviceHandle other &&
        Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value.ToString();
}