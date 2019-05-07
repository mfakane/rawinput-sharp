using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Linearstar.Windows.RawInput.Native
{
    public static class HidP
    {
        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetCaps(IntPtr preparsedData, out HidPCaps capabilities);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetButtonCaps(HidPReportType reportType, [Out] HidPButtonCaps[] buttonCaps, ref ushort buttonCapsLength, IntPtr preparsedData);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetValueCaps(HidPReportType reportType, [Out] HidPValueCaps[] valueCaps, ref ushort valueCapsLength, IntPtr preparsedData);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetUsages(HidPReportType reportType, ushort usagePage, ushort linkCollection, [Out] ushort[] usageList, ref uint usageLength, IntPtr preparsedData, byte[] report, uint reportLength);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, byte[] report, uint reportLength);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetScaledUsageValue(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, out int usageValue, IntPtr preparsedData, byte[] report, uint reportLength);

        [SuppressUnmanagedCodeSecurity, DllImport("hid")]
        static extern NtStatus HidP_GetUsageValueArray(HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, [Out] byte[] usageValue, ushort usageValueByteLength, IntPtr preparsedData, byte[] report, uint reportLength);


        public static NtStatus TryGetCaps(HidPreparsedData preparsedData, out HidPCaps capabilities)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);

            return HidP_GetCaps(preparsedDataPtr, out capabilities);
        }

        public static HidPCaps GetCaps(HidPreparsedData preparsedData)
        {
            TryGetCaps(preparsedData, out var capabilities).EnsureSuccess();

            return capabilities;
        }

        public static NtStatus TryGetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPButtonCaps[] buttonCaps)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);
            var caps = GetCaps(preparsedData);
            var capsCount = reportType switch
            {
                HidPReportType.Input => caps.NumberInputButtonCaps,
                HidPReportType.Output => caps.NumberOutputButtonCaps,
                HidPReportType.Feature => caps.NumberFeatureButtonCaps,
                _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
            };

            buttonCaps = new HidPButtonCaps[capsCount];

            return HidP_GetButtonCaps(reportType, buttonCaps, ref capsCount, preparsedDataPtr);
        }

        public static HidPButtonCaps[] GetButtonCaps(HidPreparsedData preparsedData, HidPReportType reportType)
        {
            TryGetButtonCaps(preparsedData, reportType, out var buttonCaps).EnsureSuccess();

            return buttonCaps;
        }

        public static NtStatus TryGetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType, out HidPValueCaps[] valueCaps)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);
            var caps = GetCaps(preparsedData);
            var capsCount = reportType switch
            {
                HidPReportType.Input => caps.NumberInputValueCaps,
                HidPReportType.Output => caps.NumberOutputValueCaps,
                HidPReportType.Feature => caps.NumberFeatureValueCaps,
                _ => throw new ArgumentException($"Invalid HidPReportType: {reportType}", nameof(reportType)),
            };

            valueCaps = new HidPValueCaps[capsCount];

            return HidP_GetValueCaps(reportType, valueCaps, ref capsCount, preparsedDataPtr);
        }

        public static HidPValueCaps[] GetValueCaps(HidPreparsedData preparsedData, HidPReportType reportType)
        {
            TryGetValueCaps(preparsedData, reportType, out var valueCaps).EnsureSuccess();

            return valueCaps;
        }

        public static NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength, out ushort[] usageList)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);
            uint usageCount = 0;

            HidP_GetUsages(reportType, usagePage, linkCollection, null, ref usageCount, preparsedDataPtr, report, (uint)reportLength);

            usageList = new ushort[usageCount];

            return HidP_GetUsages(reportType, usagePage, linkCollection, usageList, ref usageCount, preparsedDataPtr, report, (uint)reportLength);
        }

        public static NtStatus TryGetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength, out ushort[] usageList) =>
            TryGetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength, out usageList);

        public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, byte[] report, int reportLength)
        {
            TryGetUsages(preparsedData, reportType, usagePage, linkCollection, report, reportLength, out var usageList).EnsureSuccess();

            return usageList;
        }

        public static ushort[] GetUsages(HidPreparsedData preparsedData, HidPReportType reportType, HidPButtonCaps buttonCaps, byte[] report, int reportLength) =>
            GetUsages(preparsedData, reportType, buttonCaps.UsagePage, buttonCaps.LinkCollection, report, reportLength);

        public static NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);

            return HidP_GetUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedDataPtr, report, (uint)reportLength);
        }

        public static NtStatus TryGetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
            TryGetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

        public static int GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength)
        {
            TryGetUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

            return usageValue;
        }

        public static int GetUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
            GetUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

        public static NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength, out int usageValue)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);

            return HidP_GetScaledUsageValue(reportType, usagePage, linkCollection, usage, out usageValue, preparsedDataPtr, report, (uint)reportLength);
        }

        public static NtStatus TryGetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out int usageValue) =>
            TryGetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength, out usageValue);

        public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, byte[] report, int reportLength)
        {
            TryGetScaledUsageValue(preparsedData, reportType, usagePage, linkCollection, usage, report, reportLength, out var usageValue).EnsureSuccess();

            return usageValue;
        }

        public static int GetScaledUsageValue(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
            GetScaledUsageValue(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, report, reportLength);

        public static NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength, out byte[] usageValue)
        {
            var preparsedDataPtr = HidPreparsedData.GetRawValue(preparsedData);

            usageValue = new byte[usageValueByteLength];

            return HidP_GetUsageValueArray(reportType, usagePage, linkCollection, usage, usageValue, usageValueByteLength, preparsedDataPtr, report, (uint)reportLength);
        }

        public static NtStatus TryGetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength, out byte[] usageValue) =>
            TryGetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength, out usageValue);

        public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, ushort usagePage, ushort linkCollection, ushort usage, ushort usageValueByteLength, byte[] report, int reportLength)
        {
            TryGetUsageValueArray(preparsedData, reportType, usagePage, linkCollection, usage, usageValueByteLength, report, reportLength, out var usageValue).EnsureSuccess();

            return usageValue;
        }

        public static byte[] GetUsageValueArray(HidPreparsedData preparsedData, HidPReportType reportType, HidPValueCaps valueCaps, ushort usage, byte[] report, int reportLength) =>
            GetUsageValueArray(preparsedData, reportType, valueCaps.UsagePage, valueCaps.LinkCollection, usage, (ushort)(valueCaps.BitSize * valueCaps.ReportCount), report, reportLength);

        public static void EnsureSuccess(this NtStatus result)
        {
            if (result != NtStatus.Success) throw new InvalidOperationException(result.ToString());
        }
    }
}
