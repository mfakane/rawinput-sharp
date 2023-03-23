using System;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidButtonState
{
    readonly byte[] report;
    readonly int reportLength;

    public HidButton Button { get; }

    public unsafe bool IsActive
    {
        get
        {
            fixed (void* preparsedData = Button.reader.PreparsedData)
            {
                var activeUsages = HidP.GetUsages((IntPtr)preparsedData, HidPReportType.Input, Button.buttonCaps, report, reportLength);

                return Array.IndexOf(activeUsages, Button.UsageAndPage.Usage) != -1;
            }
        }
    }

    internal HidButtonState(HidButton button, byte[] report, int reportLength)
    {
        Button = button;
        this.report = report;
        this.reportLength = reportLength;
    }

    public override string ToString() =>
        $"Button: {{{Button}}}, IsActive: {IsActive}";
}