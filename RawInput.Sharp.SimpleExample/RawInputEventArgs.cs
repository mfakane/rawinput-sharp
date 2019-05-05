using System;
using Linearstar.RawInput;

namespace RawInput.Sharp.SimpleExample
{
    class RawInputEventArgs : EventArgs
    {
        public RawInputEventArgs(RawInputData data)
        {
            Data = data;
        }

        public RawInputData Data { get; }
    }
}
