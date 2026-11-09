using System;

namespace Terraria.UtilClient.Cheats.Types;

public sealed class SimpleToggleCheat : ToggleCheat
{
	public override string Name { get; }

	public override CheatCategory Category { get; }

	public override bool SaveEnabled { get; }

	protected override bool PanicModeAffected { get; }

	public override string? CommandOverride { get; }

	public SimpleToggleCheat(string name, CheatCategory category, bool saveEnabled = true, bool panicModeAffected = false, string? commandOverride = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name, "name");
		Name = name;
		Category = category;
		SaveEnabled = saveEnabled;
		PanicModeAffected = panicModeAffected;
		CommandOverride = commandOverride;
	}
}
