namespace Linearstar.Windows.RawInput;

public interface IHidPreparsedData
{
    ref byte GetPinnableReference();
}