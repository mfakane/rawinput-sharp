using System;
using System.Collections.Generic;
using System.Linq;
using Linearstar.Windows.RawInput.Native;

namespace Linearstar.Windows.RawInput;

public class RawInputDigitizerData : RawInputHidData
{
    public int ContactsCount { get; }
    
    public int MaxContactsCount { get; }
    
    public RawInputDigitizerContact[] Contacts { get; }

    public RawInputDigitizerData(RawInputHeader header, RawHid hid)
        : base(header, hid)
    {
        if (Device is not RawInputDigitizer digitizer) throw new ArgumentException($"Device specified in the {nameof(header)} was not a valid digitizer.", nameof(header));

        var contactButtonStates = ButtonSetStates
            .SelectMany(x => x)
            .Where(x => x.Button.LinkUsageAndPage != digitizer.UsageAndPage)
            .ToLookup(x => x.Button.LinkCollection);
        var contactValueStates = ValueSetStates
            .SelectMany(x => x)
            .Where(x => x.Value.LinkUsageAndPage != digitizer.UsageAndPage)
            .ToLookup(x => x.Value.LinkCollection);
        var contactsCountUsages = ValueSetStates
            .SelectMany(x => x)
            .Where(x => x.Value.LinkUsageAndPage == digitizer.UsageAndPage && x.Value.UsageAndPage == RawInputDigitizer.UsageContactCount)
            .ToArray();
        var contactsCount = contactsCountUsages.Select(x => x.CurrentValue).DefaultIfEmpty(1).Max();

        Contacts = EnumerateContacts().ToArray();
        this.ContactsCount = contactsCountUsages.Any() ? contactsCount : Math.Min(Contacts.Length, 1);
        this.MaxContactsCount = contactsCountUsages.Select(x => x.Value.MaxValue).DefaultIfEmpty(1).Max();

        IEnumerable<RawInputDigitizerContact> EnumerateContacts()
        {
            foreach (var buttonStates in contactButtonStates)
            foreach (var button in RawInputDigitizerButton.Read(buttonStates.ToLookup(x => x.Button.UsageAndPage)))
            foreach (var contact in RawInputDigitizerContact.Read(button, contactValueStates[buttonStates.Key]))
                if (buttonStates.Key <= contactsCount)
                    yield return contact;
        }
    }
}