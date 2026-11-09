using System;

namespace Terraria.UtilClient.Cheats.Common;

internal sealed class UpdateRateState
{
	public uint Value { get; private set; }

	public bool Configurable { get; }

	public UpdateRateUnit Unit { get; }

	public UpdateRateState(uint defaultRate, UpdateRateUnit unit, bool configurable = true)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(defaultRate, 1u, "defaultRate");
		Value = defaultRate;
		Unit = unit;
		Configurable = configurable;
	}

	public void Set(uint rate)
	{
		if (!Configurable)
		{
			throw new InvalidOperationException("The update rate is not configurable.");
		}
		Value = Math.Max(rate, 1u);
	}
}
