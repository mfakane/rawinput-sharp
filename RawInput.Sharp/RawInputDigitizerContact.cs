using System.Collections.Generic;
using System.Linq;

namespace Linearstar.Windows.RawInput;

public class RawInputDigitizerContact
{
    public static readonly HidUsageAndPage UsageX = new(0x01, 0x30);
    public static readonly HidUsageAndPage UsageY = new(0x01, 0x31);
    public static readonly HidUsageAndPage UsagePressure = new(0x0D, 0x30);
    public static readonly HidUsageAndPage UsageWidth = new(0x0D, 0x48);
    public static readonly HidUsageAndPage UsageHeight = new(0x0D, 0x49);
    public static readonly HidUsageAndPage UsageIdentifier = new(0x0D, 0x51);

    readonly RawInputDigitizerButton button;

    public RawInputDigitizerContactKind Kind
    {
        get
        {
            if (button.IsEraser == true) return RawInputDigitizerContactKind.Eraser;

            if (button.IsDown)
                return button.HasConfidence == true
                    ? RawInputDigitizerContactKind.Finger
                    : RawInputDigitizerContactKind.Pen;
            
            return button.InRange == true
                ? RawInputDigitizerContactKind.Hover
                : RawInputDigitizerContactKind.None;
        }
    }

    public int X { get; }
    public int Y { get; }
    public int MinX { get; }
    public int MinY { get; }
    public int MaxX { get; }
    public int MaxY { get; }
    public int? Pressure { get; }
    public int? MaxPressure { get; }
    public bool? IsInverted => button.IsInverted;
    public bool? IsButtonDown => button.IsDown;
    public int? Width { get; }
    public int? Height { get; }
    public int? Identifier { get; }

    RawInputDigitizerContact(
        RawInputDigitizerButton button,
        HidValueState x,
        HidValueState y,
        HidValueState? pressure,
        HidValueState? width,
        HidValueState? height,
        HidValueState? identifier)
    {
        this.button = button;
        X = x.CurrentValue;
        Y = y.CurrentValue;
        MinX = x.Value.MinValue;
        MinY = y.Value.MinValue;
        MaxX = x.Value.MaxValue;
        MaxY = y.Value.MaxValue;
        Pressure = pressure?.CurrentValue;
        MaxPressure = pressure?.Value.MaxValue;
        Width = width?.CurrentValue;
        Height = height?.CurrentValue;
        Identifier = identifier?.CurrentValue;
    }

    internal static IEnumerable<RawInputDigitizerContact> Read(
        RawInputDigitizerButton button,
        IEnumerable<HidValueState> valueStatesPerLinkCollection)
    {
        var valueStates = valueStatesPerLinkCollection
            .ToLookup(x => x.Value.UsageAndPage)
            .ToDictionary(x => x.Key, x => x.ToList());

        while (valueStates[UsageX].Any())
        {
            var x = PopState(UsageX);
            var y = PopState(UsageY);

            if (x == null || y == null) break;
            
            var pressure = PopState(UsagePressure);
            var width = PopState(UsageWidth);
            var height = PopState(UsageHeight);
            var identifier = PopState(UsageIdentifier);

            yield return new RawInputDigitizerContact(
                button,
                x,
                y,
                pressure,
                width,
                height,
                identifier
            );
        }

        HidValueState? PopState(HidUsageAndPage usageAndPage)
        {
            if (valueStates?.TryGetValue(usageAndPage, out var usageStates) != true) return null;
            if (usageStates?.Any() != true) return null;

            var state = usageStates[0];
            usageStates.RemoveAt(0);

            return state;
        }
    }
    
    public override string ToString() =>
        "{" + string.Join(", ", new[]
        {
            $"X: {X}/{MaxX}",
            $"Y: {Y}/{MaxY}",
            $"Kind: {Kind}",
            Pressure.HasValue ? $"Pressure: {Pressure}/{MaxPressure}" : null,
            IsInverted.HasValue ? $"Inverted: {IsInverted}" : null,
            IsButtonDown.HasValue ? $"Button: {IsButtonDown}" : null,
            Width.HasValue ? $"Width: {Width}" : null,
            Height.HasValue ? $"Height: {Height}" : null,
            Identifier.HasValue ? $"Identifier: {Identifier}" : null,
        }.Where(i => i != null)) + "}";
}