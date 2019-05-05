using System.Collections.Generic;
using System.Linq;

namespace Linearstar.RawInput
{
    public class RawInputDigitizerContact
	{
		public static readonly HidUsageAndPage UsageX = new HidUsageAndPage(0x01, 0x30);
		public static readonly HidUsageAndPage UsageY = new HidUsageAndPage(0x01, 0x31);
		public static readonly HidUsageAndPage UsagePressure = new HidUsageAndPage(0x0D, 0x30);
		public static readonly HidUsageAndPage UsageInRange = new HidUsageAndPage(0x0D, 0x32);
		public static readonly HidUsageAndPage UsageInvert = new HidUsageAndPage(0x0D, 0x3c);
		public static readonly HidUsageAndPage UsageTipSwitch = new HidUsageAndPage(0x0D, 0x42);
		public static readonly HidUsageAndPage UsageBarrel = new HidUsageAndPage(0x0D, 0x44);
		public static readonly HidUsageAndPage UsageEraser = new HidUsageAndPage(0x0D, 0x45);
		public static readonly HidUsageAndPage UsageConfidence = new HidUsageAndPage(0x0D, 0x47);
		public static readonly HidUsageAndPage UsageWidth = new HidUsageAndPage(0x0D, 0x48);
		public static readonly HidUsageAndPage UsageHeight = new HidUsageAndPage(0x0D, 0x49);
		public static readonly HidUsageAndPage UsageIdentifier = new HidUsageAndPage(0x0D, 0x51);

		public RawInputDigitizerContactKind Kind { get; }
		public int X { get; }
		public int Y { get; }
		public int MinX { get; }
		public int MinY { get; }
		public int MaxX { get; }
		public int MaxY { get; }
		public int? Pressure { get; }
		public int? MaxPressure { get; }
		public bool? IsInverted { get; }
		public bool? IsButtonDown { get; }
		public int? Width { get; }
		public int? Height { get; }
		public int? Identifier { get; }

		public RawInputDigitizerContact(IEnumerable<HidButtonState> buttonStates, IEnumerable<HidValueState> valueStates)
		{
			var buttons = buttonStates.ToDictionary(x => x.Button.UsageAndPage, x => x.IsActive);
			var values = valueStates.ToDictionary(x => x.Value.UsageAndPage);

			X = values[UsageX].CurrentValue;
			Y = values[UsageY].CurrentValue;
			MinX = values[UsageX].Value.MinValue;
			MinY = values[UsageY].Value.MinValue;
			MaxX = values[UsageX].Value.MaxValue;
			MaxY = values[UsageY].Value.MaxValue;

			if (values.TryGetValue(UsagePressure, out var pressure))
			{
				Pressure = pressure.CurrentValue;
				MaxPressure = pressure.Value.MaxValue;
			}

			IsInverted = buttons.TryGetValue(UsageInvert, out var invert) ? invert : (bool?)null;
			IsButtonDown = buttons.TryGetValue(UsageBarrel, out var barrel) ? barrel : (bool?)null;

			var isTipDown = buttons[UsageTipSwitch];

			if (buttons.TryGetValue(UsageEraser, out var eraser) && eraser)
				Kind = RawInputDigitizerContactKind.Eraser;
			else
				Kind = isTipDown
					? buttons.TryGetValue(UsageConfidence, out var confidence) && confidence
						? RawInputDigitizerContactKind.Finger
						: RawInputDigitizerContactKind.Pen
					: buttons.TryGetValue(UsageInRange, out var inRange) && inRange
						? RawInputDigitizerContactKind.Hover
						: RawInputDigitizerContactKind.None;

			Width = values.TryGetValue(UsageWidth, out var width) ? width.CurrentValue : (int?)null;
			Height = values.TryGetValue(UsageHeight, out var height) ? height.CurrentValue : (int?)null;
			Identifier = values.TryGetValue(UsageIdentifier, out var identifier) ? identifier.CurrentValue : (int?)null;
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
}
