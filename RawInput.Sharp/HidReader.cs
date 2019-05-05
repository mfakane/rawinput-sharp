using System.Linq;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class HidReader
    {
        HidPCaps capabilities;

        public byte[] PreparsedData
        {
            get;
        }

        public HidPreparsedData PreparsedDataPtr
        {
            get;
        }

        public int ValueCount => capabilities.NumberInputValueCaps;
        public HidButtonSet[] ButtonSets { get; private set; }
        public HidValueSet[] ValueSets { get; private set; }

        public HidReader(HidPreparsedData preparsedData) =>
            Initialize(PreparsedDataPtr = preparsedData);

        public HidReader(byte[] preparsedData)
        {
            using (var preparsedDataPtr = new HidPreparsedDataPtr(PreparsedData = preparsedData))
                Initialize(preparsedDataPtr);
        }

        void Initialize(HidPreparsedData preparsedData)
        {
            capabilities = HidP.GetCaps(preparsedData);

            var buttonCaps = HidP.GetButtonCaps(preparsedData, HidPReportType.Input);

            ButtonSets = buttonCaps.Select(i => new HidButtonSet(this, i)).ToArray();

            var valueCaps = HidP.GetValueCaps(preparsedData, HidPReportType.Input);

            ValueSets = valueCaps.Select(i => new HidValueSet(this, i)).ToArray();
        }

        internal HidPreparsedDataPtr GetPreparsedData() =>
            PreparsedData == null
                ? new HidPreparsedDataPtr(PreparsedDataPtr)
                : new HidPreparsedDataPtr(PreparsedData);
    }
}
