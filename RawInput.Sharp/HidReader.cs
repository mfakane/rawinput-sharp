using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class HidReader
{
    readonly HidPCaps capabilities;

    public byte[]? PreparsedData { get; }
    public HidPreparsedData PreparsedDataPtr { get; }
    public int ValueCount => capabilities.NumberInputValueCaps;
    public HidButtonSet[] ButtonSets { get; }
    public HidValueSet[] ValueSets { get; }

    public HidReader(HidPreparsedData preparsedData)
    {
        PreparsedDataPtr = preparsedData;
        capabilities = HidP.GetCaps(preparsedData);

        var buttonCaps = HidP.GetButtonCaps(preparsedData, HidPReportType.Input);
        ButtonSets = buttonCaps.Select(i => new HidButtonSet(this, i)).ToArray();
        
        var valueCaps = HidP.GetValueCaps(preparsedData, HidPReportType.Input);
        ValueSets = valueCaps.Select(i => new HidValueSet(this, i)).ToArray();
    }

    public HidReader(byte[] preparsedData)
    {
        using var preparsedDataPtr = new HidPreparsedDataPtr(PreparsedData = preparsedData);
        capabilities = HidP.GetCaps(preparsedDataPtr);

        var buttonCaps = HidP.GetButtonCaps(preparsedDataPtr, HidPReportType.Input);
        ButtonSets = buttonCaps.Select(i => new HidButtonSet(this, i)).ToArray();
        
        var valueCaps = HidP.GetValueCaps(preparsedDataPtr, HidPReportType.Input);
        ValueSets = valueCaps.Select(i => new HidValueSet(this, i)).ToArray();
    }

    internal HidPreparsedDataPtr GetPreparsedData() =>
        PreparsedData == null
            ? new HidPreparsedDataPtr(PreparsedDataPtr)
            : new HidPreparsedDataPtr(PreparsedData);
}