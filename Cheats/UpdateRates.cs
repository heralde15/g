using System.Collections.Generic;

namespace Terraria.UtilClient.Cheats;

public sealed class UpdateRates
{
	public Dictionary<string, uint> Ticks { get; } = new Dictionary<string, uint>();

	public Dictionary<string, uint> Milliseconds { get; } = new Dictionary<string, uint>();
}
