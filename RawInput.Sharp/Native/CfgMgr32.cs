using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

public static class CfgMgr32
{
    [DllImport("cfgmgr32", CharSet = CharSet.Unicode)]
    static extern ConfigReturnValue CM_Locate_DevNode(out IntPtr pdnDevInst, string pDeviceID, LocateDevNodeFlags ulFlags);

    [DllImport("cfgmgr32", CharSet = CharSet.Unicode)]
    static extern ConfigReturnValue CM_Get_DevNode_Property(IntPtr dnDevInst, in DevicePropertyKey propertyKey, out uint propertyType, IntPtr propertyBuffer, ref uint propertyBufferSize, uint ulFlags);

    /// <summary>
    /// CM_LOCATE_DEVNODE_*
    /// </summary>
    [Flags]
    public enum LocateDevNodeFlags : uint
    {
        /// <summary>
        /// CM_LOCATE_DEVNODE_NORMAL
        /// </summary>
        Normal = 0x0,
        /// <summary>
        /// CM_LOCATE_DEVNODE_PHANTOM
        /// </summary>
        Phantom = 0x1,
        /// <summary>
        /// CM_LOCATE_DEVNODE_CANCELREMOVE
        /// </summary>
        CancelRemove = 0x2,
        /// <summary>
        /// CM_LOCATE_DEVNODE_NOVALIDATION
        /// </summary>
        NoValidation = 0x4,
    }

    public static DeviceInstanceHandle LocateDevNode(string devicePath, LocateDevNodeFlags flags)
    {
        TryLocateDevNode(devicePath, flags, out var device).EnsureSuccess();

        return device;
    }

    public static ConfigReturnValue TryLocateDevNode(string devicePath, LocateDevNodeFlags flags, out DeviceInstanceHandle device)
    {
        var result = CM_Locate_DevNode(out var devInst, devicePath, flags);

        device = result == ConfigReturnValue.Success
            ? (DeviceInstanceHandle)devInst
            : DeviceInstanceHandle.Zero;

        return result;
    }

    public static string? GetDevNodePropertyString(DeviceInstanceHandle device, in DevicePropertyKey propertyKey)
    {
        TryGetDevNodePropertyString(device, in propertyKey, out var value);

        return value;
    }

    public static ConfigReturnValue TryGetDevNodePropertyString(DeviceInstanceHandle device, in DevicePropertyKey propertyKey, out string? value)
    {
        var devInst = DeviceInstanceHandle.GetRawValue(device);
        uint size = 0;

        var result = CM_Get_DevNode_Property(devInst, in propertyKey, out _, IntPtr.Zero, ref size, 0);
        if (result != ConfigReturnValue.Success &&
            result != ConfigReturnValue.BufferSmall)
        {
            value = null;
            return result;
        }

        var buffer = Marshal.AllocHGlobal((int)size);

        try
        {
            result = CM_Get_DevNode_Property(devInst, in propertyKey, out _, buffer, ref size, 0);
            if (result != ConfigReturnValue.Success)
            {
                value = null;
                return result;
            }

            value = Marshal.PtrToStringUni(buffer);
            return ConfigReturnValue.Success;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    static void EnsureSuccess(this ConfigReturnValue result)
    {
        if (result != ConfigReturnValue.Success) throw new InvalidOperationException(result.ToString());
    }
}
