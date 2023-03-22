using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidValueSet : IEnumerable<HidValue>
{
    internal readonly HidReader reader;
    internal readonly HidPValueCaps valueCaps;

    public int ReportId => valueCaps.ReportID;

    public int ReportCount => valueCaps.ReportCount;

    public int ValueCount => valueCaps.Range.UsageMax - valueCaps.Range.UsageMin + 1;

    public ushort UsagePage => valueCaps.UsagePage;

    public ushort UsageMin => valueCaps.Range.UsageMin;

    public ushort UsageMax => valueCaps.Range.UsageMax;

    public HidUsageAndPage LinkUsageAndPage => new(valueCaps.LinkUsagePage, valueCaps.LinkUsage);

    public int LinkCollection => valueCaps.LinkCollection;

    internal HidValueSet(HidReader reader, HidPValueCaps valueCaps)
    {
        this.reader = reader;
        this.valueCaps = valueCaps;
    }

    public HidValueSetState GetStates(ArraySegment<byte> report) =>
        GetStates(report.ToArray(), report.Count);

    public HidValueSetState GetStates(byte[] report, int reportLength) => new(this, report, reportLength);

    public override string ToString() =>
        $"{ReportId}, {LinkCollection}, Link: {{{LinkUsageAndPage}}}, UsagePage: {{{UsagePage}}}, Count: {ValueCount}";

    public IEnumerator<HidValue> GetEnumerator()
    {
        for (var usage = valueCaps.Range.UsageMin; usage <= valueCaps.Range.UsageMax; usage++)
            yield return new HidValue(reader, valueCaps, usage);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}