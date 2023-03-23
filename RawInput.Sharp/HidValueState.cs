using System;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidValueState
{
    readonly byte[] report;
    readonly int reportLength;

    public HidValue Value { get; }

    public unsafe int CurrentValue
    {
        get
        {
            fixed (void* preparsedData = Value.reader.PreparsedData)
                return HidP.GetUsageValue((IntPtr)preparsedData, HidPReportType.Input, Value.valueCaps, Value.UsageAndPage.Usage, report, reportLength);
        }
    }

    public unsafe int? ScaledValue
    {
        get
        {
            fixed (void* preparsedData = Value.reader.PreparsedData)
                return HidP.TryGetScaledUsageValue((IntPtr)preparsedData, HidPReportType.Input, Value.valueCaps, Value.UsageAndPage.Usage, report, reportLength, out var value) == NtStatus.Success
                    ? value
                    : null;
        }
    }

    public bool HasValue
    {
        get
        {
            if (!Value.CanBeNull) return true;

            var currentValue = CurrentValue;

            return currentValue >= Value.MinValue && currentValue <= Value.MaxValue;
        }
    }

    internal HidValueState(HidValue value, byte[] report, int reportLength)
    {
        Value = value;
        this.report = report;
        this.reportLength = reportLength;
    }

    public override string ToString() =>
        $"Value: {{{Value}}}, CurrentValue: {CurrentValue}";
}