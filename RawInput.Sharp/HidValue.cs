using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidValue
{
    internal readonly HidReader reader;
    internal readonly HidPValueCaps valueCaps;

    public int ReportId => valueCaps.ReportID;

    public int ReportCount => valueCaps.ReportCount;

    public HidUsageAndPage UsageAndPage { get; }

    public HidUsageAndPage LinkUsageAndPage => new(valueCaps.LinkUsagePage, valueCaps.LinkUsage);

    public int LinkCollection => valueCaps.LinkCollection;

    public int MinValue => valueCaps.LogicalMin;

    public int MaxValue => valueCaps.LogicalMax;

    public int MinPhysicalValue => valueCaps.PhysicalMin;

    public int MaxPhysicalValue => valueCaps.PhysicalMax;

    public bool CanBeNull => valueCaps.HasNull;

    internal HidValue(HidReader reader, HidPValueCaps valueCaps, ushort usage)
    {
        this.reader = reader;
        this.valueCaps = valueCaps;
        UsageAndPage = new HidUsageAndPage(valueCaps.UsagePage, usage);
    }

    public HidValueState GetValue(ArraySegment<byte> report) =>
        GetValue(report.ToArray(), report.Count);

    public HidValueState GetValue(byte[] report, int reportLength) => new(this, report, reportLength);

    public override string ToString() =>
        $"{ReportId}, {LinkCollection}, Link: {{{LinkUsageAndPage}}}, Usage: {{{UsageAndPage}}}";
}