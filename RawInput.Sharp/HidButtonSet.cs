using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidButtonSet : IEnumerable<HidButton>
{
    internal readonly HidReader reader;
    internal readonly HidPButtonCaps buttonCaps;

    public int ReportId => buttonCaps.ReportID;

    public int ButtonCount => buttonCaps.Range.UsageMax - buttonCaps.Range.UsageMin + 1;

    public ushort UsagePage => buttonCaps.UsagePage;

    public ushort UsageMin => buttonCaps.Range.UsageMin;

    public ushort UsageMax => buttonCaps.Range.UsageMax;

    public HidUsageAndPage LinkUsageAndPage => new(buttonCaps.LinkUsagePage, buttonCaps.LinkUsage);

    public int LinkCollection => buttonCaps.LinkCollection;

    internal HidButtonSet(HidReader reader, HidPButtonCaps buttonCaps)
    {
        this.reader = reader;
        this.buttonCaps = buttonCaps;
    }

    public HidButtonSetState GetStates(ArraySegment<byte> report) =>
        GetStates(report.ToArray(), report.Count);

    public HidButtonSetState GetStates(byte[] report, int reportLength) =>
        new(this, report, reportLength);

    public override string ToString() =>
        $"{ReportId}, {LinkCollection}, Link: {{{LinkUsageAndPage}}}, UsagePage: {{{UsagePage}}}, Count: {ButtonCount}";

    public IEnumerator<HidButton> GetEnumerator()
    {
        for (var usage = buttonCaps.Range.UsageMin; usage <= buttonCaps.Range.UsageMax; usage++)
            yield return new HidButton(reader, buttonCaps, usage);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}