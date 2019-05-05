using System.Linq;
using Linearstar.RawInput.Native;

namespace Linearstar.RawInput
{
    public class RawInputDigitizerData : RawInputHidData
	{
		public RawInputDigitizerContact[] Contacts { get; }

		public RawInputDigitizerData(RawInputHeader header, RawHid hid)
            : base(header, hid)
		{
            var digitizer = (RawInputDigitizer)Device;

            var contactButtonStates = ButtonSetStates.SelectMany(x => x).Where(x => x.Button.LinkUsageAndPage != digitizer.UsageAndPage).ToLookup(x => x.Button.LinkCollection);
            var contactValueStates = ValueSetStates.SelectMany(x => x).Where(x => x.Value.LinkUsageAndPage != digitizer.UsageAndPage).ToLookup(x => x.Value.LinkCollection);
            var contactCount = ValueSetStates.SelectMany(x => x).FirstOrDefault(x => x.Value.LinkUsageAndPage == digitizer.UsageAndPage && x.Value.UsageAndPage == RawInputDigitizer.UsageContactCount)?.CurrentValue ?? 1;

			Contacts = contactButtonStates.Select(buttonStates => new RawInputDigitizerContact(buttonStates, contactValueStates[buttonStates.Key]))
							              .Take(contactCount)
							              .ToArray();
		}
	}
}
