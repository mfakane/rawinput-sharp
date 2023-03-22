using System.Collections;
using System.Collections.Generic;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidButtonSetState : IEnumerable<HidButtonState>
{
    readonly byte[] report;
    readonly int reportLength;

    public HidButtonSet ButtonSet { get; }

    public ushort[] ActiveUsages
    {
        get
        {
            using var preparsedDataPtr = ButtonSet.reader.GetPreparsedData();
            return HidP.GetUsages(preparsedDataPtr, HidPReportType.Input, ButtonSet.buttonCaps, report, reportLength);
        }
    }

    internal HidButtonSetState(HidButtonSet buttonSet, byte[] report, int reportLength)
    {
        ButtonSet = buttonSet;
        this.report = report;
        this.reportLength = reportLength;
    }

    public override string ToString() =>
        $"ButtonSet: {{{ButtonSet}}}, Active: [{string.Join(", ", ActiveUsages)}]";

    public IEnumerator<HidButtonState> GetEnumerator()
    {
        for (var usage = ButtonSet.UsageMin; usage <= ButtonSet.UsageMax; usage++)
            yield return new HidButtonState(new HidButton(ButtonSet.reader, ButtonSet.buttonCaps, usage), report, reportLength);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}