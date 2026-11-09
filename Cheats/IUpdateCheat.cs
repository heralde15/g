namespace Terraria.UtilClient.Cheats;

internal interface IUpdateCheat
{
	uint UpdateRate { get; }

	bool RateConfigurable { get; }

	UpdateRateUnit UpdateRateUnit { get; }

	void SetUpdateRate(uint rate);
}
