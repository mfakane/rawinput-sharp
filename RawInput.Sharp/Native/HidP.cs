using System;
using System.Runtime.InteropServices;

namespace Linearstar.Windows.RawInput.Native;

public static partial class HidP
{
    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetCaps(IntPtr preparsedData, IntPtr capabilities);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetButtonCaps(HidPReportType reportType, IntPtr buttonCaps, ref ushort buttonCapsLength, IntPtr preparsedData);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetValueCaps(HidPReportType reportType, IntPtr valueCaps, ref ushort valueCapsLength, IntPtr preparsedData);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection, IntPtr usageList, ref uint usageLength, IntPtr preparsedData, IntPtr report, uint reportLength);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, IntPtr report, uint reportLength);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, IntPtr report, uint reportLength);

    [LibraryImport("hid")]
    private static partial NtStatus HidP_GetUsageValueArray(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, IntPtr usageValue, ushort usageValueByteLength, IntPtr preparsedData, IntPtr report, uint reportLength);

    public static NtStatus TryGetCaps(IntPtr preparsedData, out HidPCaps capabilities)
    {
        var buffer = AllocZeroed(Marshal.SizeOf<HidPCaps>());

        try
        {
            var result = HidP_GetCaps(preparsedData, buffer);
            capabilities = result == NtStatus.Success
                ? Marshal.PtrToStructure<HidPCaps>(buffer)
                : default;
            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public static NtStatus TryGetCaps(HidPreparsedData preparsedData, out HidPCaps capabilities) =>
        TryGetCaps((IntPtr)preparsedData, out capabilities);

    public static HidPCaps GetCaps(IntPtr preparsedData)
    {
        TryGetCaps(preparsedData, out var capabilities).EnsureSuccess();

        return capabilities;
    }

    public static HidPCaps GetCaps(HidPreparsedData preparsedData) =>
        GetCaps((IntPtr)preparsedData);

    public static NtStatus TryGetButtonCaps(IntPtr preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps)
    {
        var caps = GetCaps(preparsedData);
        var capsCount = reportType switch
        {
            HidPReportType.Input => caps.NumberInputButtonCaps,
            HidPReportType.Output => caps.NumberOutputButtonCaps,
            HidPReportType.Feature => caps.NumberFeatureButtonCaps,
            _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
        };

        var itemSize = Marshal.SizeOf<HidPButtonCaps>();
        var buffer = AllocZeroed(itemSize * capsCount);

        try
        {
            var result = HidP_GetButtonCaps(reportType, buffer, ref capsCount, preparsedData);

            if (result != NtStatus.Success)
            {
                buttonCaps = Array.Empty<HidPButtonCaps>();
                return result;
            }

            buttonCaps = new HidPButtonCaps[capsCount];
            for (var i = 0; i < capsCount; i++)
                buttonCaps[i] = Marshal.PtrToStructure<HidPButtonCaps>(buffer + i * itemSize);

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
    
    public static NtStatus TryGetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps) =>
        TryGetButtonCaps((IntPtr)preparsedData, reportType, out buttonCaps);

    public static HidPButtonCaps[] GetButtonCaps(IntPtr preparsedData, HidPReportType reportType)
    {
        TryGetButtonCaps(preparsedData, reportType, out var buttonCaps).EnsureSuccess();

        return buttonCaps;
    }
    
    public static HidPButtonCaps[] GetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType) =>
        GetButtonCaps((IntPtr)preparsedData, reportType);

    public static NtStatus TryGetValueCaps(IntPtr preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps)
    {
        var caps = GetCaps(preparsedData);
        var capsCount = reportType switch
        {
            HidPReportType.Input => caps.NumberInputValueCaps,
            HidPReportType.Output => caps.NumberOutputValueCaps,
            HidPReportType.Feature => caps.NumberFeatureValueCaps,
            _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
        };

        var itemSize = Marshal.SizeOf<HidPValueCaps>();
        var buffer = AllocZeroed(itemSize * capsCount);

        try
        {
            var result = HidP_GetValueCaps(reportType, buffer, ref capsCount, preparsedData);

            if (result != NtStatus.Success)
            {
                valueCaps = Array.Empty<HidPValueCaps>();
                return result;
            }

            valueCaps = new HidPValueCaps[capsCount];
            for (var i = 0; i < capsCount; i++)
                valueCaps[i] = Marshal.PtrToStructure<HidPValueCaps>(buffer + i * itemSize);

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
    
    public static NtStatus TryGetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps) =>
        TryGetValueCaps((IntPtr)preparsedData, reportType, out valueCaps);

    public static HidPValueCaps[] GetValueCaps(IntPtr preparsedData, HidPReportType reportType)
    {
        TryGetValueCaps(preparsedData, reportType, out var valueCaps).EnsureSuccess();

        return valueCaps;
    }
    
    public static HidPValueCaps[] GetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType) =>
        GetValueCaps((IntPtr)preparsedData, reportType);

    public static unsafe NtStatus TryGetUsages(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength, out ushort[] usageList)
    {
        uint usageCount = 0;

        fixed (byte* reportBuffer = report)
            HidP_GetUsages(reportType, usagePage, linkCollection, IntPtr.Zero, ref usageCount, preparsedData, (IntPtr)reportBuffer, (uint)reportLength);

        usageList = new ushort[usageCount];

        fixed (ushort* usageBuffer = usageList)
        fixed (byte* reportBuffer = report)
            return HidP_GetUsages(reportType, usagePage, linkCollection, (IntPtr)usageBuffer, ref usageCount, preparsedData, (IntPtr)reportBuffer, (uint)reportLength);
    }
    
    public static NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength, out ushort[] usageList) =>
        TryGetUsages((IntPtr)preparsedData, reportType, usagePage, linkCollection, report, reportLength, out usageList);

    public static NtStatus TryGetUsages(IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength, out ushort[] usageList) =>
        TryGetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);

    public static NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength, out ushort[] usageList) =>
        TryGetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);
    
    public static ushort[] GetUsages(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength)
    {
        TryGetUsages(preparsedData, reportType, usagePage, linkCollection, report, reportLength, out var usageList).EnsureSuccess();

        return usageList;
    }
    
    public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength) =>
        GetUsages((IntPtr)preparsedData, reportType, usagePage, linkCollection, report, reportLength);

    public static ushort[] GetUsages(IntPtr preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength) =>
        GetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);

    public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength) =>
        GetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);
    
    public static unsafe NtStatus TryGetUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue)
    {
        fixed (byte* reportBuffer = report)
            return HidP_GetUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, (IntPtr)reportBuffer, (uint)reportLength);
    }

    public static NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetUsageValue((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);
    
    public static NtStatus TryGetUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

    public static NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);
    
    public static int GetUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength)
    {
        TryGetUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static int GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength) =>
        GetUsageValue((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

    public static int GetUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static int GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static unsafe NtStatus TryGetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue)
    {
        fixed (byte* reportBuffer = report)
            return HidP_GetScaledUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedData, (IntPtr)reportBuffer, (uint)reportLength);
    }

    public static NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetScaledUsageValue((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out usageValue);

    public static NtStatus TryGetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);
    
    public static NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
        TryGetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

    public static int GetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength)
    {
        TryGetScaledUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength) =>
        GetScaledUsageValue((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength);

    public static int GetScaledUsageValue(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

    public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);
    
    public static unsafe NtStatus TryGetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue)
    {
        usageValue = new byte[usageValueByteLength];

        fixed (byte* usageValueBuffer = usageValue)
        fixed (byte* reportBuffer = report)
            return HidP_GetUsageValueArray(reportType, usagePage, linkCollection, usage, (IntPtr)usageValueBuffer, usageValueByteLength, preparsedData, (IntPtr)reportBuffer, (uint)reportLength);
    }
    
    public static NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out usageValue);

    public static NtStatus TryGetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength, out usageValue);
    
    public static NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out byte[] usageValue) =>
        TryGetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength, out usageValue);

    public static byte[] GetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength)
    {
        TryGetUsageValueArray(preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out var usageValue).EnsureSuccess();

        return usageValue;
    }
    
    public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength) => 
        GetUsageValueArray((IntPtr)preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength);

    public static byte[] GetUsageValueArray(IntPtr preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength);

    public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
        GetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength);

    static unsafe IntPtr AllocZeroed(int size)
    {
        if (size == 0)
            return IntPtr.Zero;

        var buffer = Marshal.AllocHGlobal(size);
        new Span<byte>((void*)buffer, size).Clear();
        return buffer;
    }
    
    public static void EnsureSuccess(this NtStatus result)
    {
        if (result != NtStatus.Success) throw new InvalidOperationException(result.ToString());
    }
}