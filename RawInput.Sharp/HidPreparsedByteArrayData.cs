namespace Linearstar.Windows.RawInput;

public class HidPreparsedByteArrayData : IHidPreparsedData
{
    readonly byte[] preparsedData;

    public HidPreparsedByteArrayData(byte[] preparsedData) => this.preparsedData = preparsedData;

    public ref byte GetPinnableReference() => ref preparsedData[0];
}