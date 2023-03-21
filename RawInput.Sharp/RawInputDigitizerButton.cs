using System.Collections.Generic;
using System.Linq;

namespace Linearstar.Windows.RawInput;

class RawInputDigitizerButton
{
    public static readonly HidUsageAndPage UsageInRange = new(0x0D, 0x32);
    public static readonly HidUsageAndPage UsageInvert = new(0x0D, 0x3c);
    public static readonly HidUsageAndPage UsageTipSwitch = new(0x0D, 0x42);
    public static readonly HidUsageAndPage UsageBarrel = new(0x0D, 0x44);
    public static readonly HidUsageAndPage UsageEraser = new(0x0D, 0x45);
    public static readonly HidUsageAndPage UsageConfidence = new(0x0D, 0x47);

    public bool? InRange { get; }
    public bool? IsInverted { get; }
    public bool IsDown { get; }
    public bool? IsBarrel { get; }
    public bool? IsEraser { get; }
    public bool? HasConfidence { get; }

    RawInputDigitizerButton(bool? inRange, bool? isInverted, bool isDown, bool? isBarrel, bool? isEraser, bool? hasConfidence)
    {
        InRange = inRange;
        IsInverted = isInverted;
        IsDown = isDown;
        IsBarrel = isBarrel;
        IsEraser = isEraser;
        HasConfidence = hasConfidence;
    }

    internal static IEnumerable<RawInputDigitizerButton> Read(
        ILookup<HidUsageAndPage, HidButtonState> buttonStatesByLinkCollection)
    {
        var buttonStates = buttonStatesByLinkCollection.ToDictionary(x => x.Key, x => x.ToList());

        while (buttonStates[UsageTipSwitch].Any())
        {
            var inRange = PopState(UsageInRange);
            var isInverted = PopState(UsageInvert);
            var isDown = PopState(UsageTipSwitch) ?? false;
            var isBarrel = PopState(UsageBarrel);
            var isEraser = PopState(UsageEraser);
            var hasConfidence = PopState(UsageConfidence);

            yield return new RawInputDigitizerButton(inRange, isInverted, isDown, isBarrel, isEraser, hasConfidence);
        }

        bool? PopState(HidUsageAndPage usageAndPage)
        {
            if (buttonStates?.TryGetValue(usageAndPage, out var usageStates) != true) return null;
            if (usageStates?.Any() != true) return null;

            var isActive = usageStates[0].IsActive;
            usageStates.RemoveAt(0);

            return isActive;
        }
    }
}