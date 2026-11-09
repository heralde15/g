using Newtonsoft.Json;
using Terraria.UtilClient.Common.Utility.Game;

namespace Terraria.UtilClient.Cheats.Settings;

public interface IPlayerSelectorSettings
{
	[JsonIgnore]
	PlayerSelector SelectorUntyped { get; }
}
