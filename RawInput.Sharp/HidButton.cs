using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidButton
{
    internal readonly HidReader reader;
    internal readonly HidPButtonCaps buttonCaps;

    public int ReportId => buttonCaps.ReportID;

    public HidUsageAndPage UsageAndPage { get; }

    public HidUsageAndPage LinkUsageAndPage => new(buttonCaps.LinkUsagePage, buttonCaps.LinkUsage);

    public int LinkCollection => buttonCaps.LinkCollection;

    internal HidButton(HidReader reader, HidPButtonCaps buttonCaps, ushort usage)
    {
        this.reader = reader;
        this.buttonCaps = buttonCaps;
        UsageAndPage = new HidUsageAndPage(buttonCaps.UsagePage, usage);
    }

    public HidButtonState GetState(ArraySegment<byte> report) =>
        GetState(report.ToArray(), report.Count);

    public HidButtonState GetState(byte[] report, int reportLength) =>
        new(this, report, reportLength);

    public override string ToString() =>
        $"{ReportId}, {LinkCollection}, Link: {{{LinkUsageAndPage}}}, Usage: {{{UsageAndPage}}}";
}