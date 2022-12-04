using System;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputDigitizerData : RawInputHidData
{
    public RawInputDigitizerContact[] Contacts { get; }

    public RawInputDigitizerData(RawInputHeader header, RawHid hid)
        : base(header, hid)
    {
        if (Device is not RawInputDigitizer digitizer) throw new ArgumentException($"Device specified in the {nameof(header)} was not a valid digitizer.", nameof(header));

        var contactButtonStates = ButtonSetStates.SelectMany(x => x).Where(x => x.Button.LinkUsageAndPage != digitizer.UsageAndPage).ToLookup(x => x.Button.LinkCollection);
        var contactValueStates = ValueSetStates.SelectMany(x => x).Where(x => x.Value.LinkUsageAndPage != digitizer.UsageAndPage).ToLookup(x => x.Value.LinkCollection);
        var contactCount = ValueSetStates.SelectMany(x => x).FirstOrDefault(x => x.Value.LinkUsageAndPage == digitizer.UsageAndPage && x.Value.UsageAndPage == RawInputDigitizer.UsageContactCount)?.CurrentValue ?? 1;

        Contacts = contactButtonStates.Select(buttonStates => new RawInputDigitizerContact(buttonStates, contactValueStates[buttonStates.Key]))
                                      .Take(contactCount)
                                      .ToArray();
    }
}