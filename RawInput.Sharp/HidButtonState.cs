using System;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class HidButtonState
    {
        readonly byte[] report;
        readonly int reportLength;

        public HidButton Button { get; }

        public bool IsActive
        {
            get
            {
                using (var preparsedDataPtr = Button.reader.GetPreparsedData())
                {
                    var activeUsages = HidP.GetUsages(preparsedDataPtr, HidPReportType.Input, Button.buttonCaps, report, reportLength);

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
}
