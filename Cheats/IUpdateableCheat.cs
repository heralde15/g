namespace Terraria.UtilClient.Cheats;

internal interface IUpdateableCheat
{
	bool ShouldUpdate { get; }

	void Update();
}
