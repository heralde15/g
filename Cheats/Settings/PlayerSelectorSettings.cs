using Newtonsoft.Json;
using Terraria.UtilClient.Common.Utility.Game;

namespace Terraria.UtilClient.Cheats.Settings;

public abstract class PlayerSelectorSettings<TSelector> : USettings, IPlayerSelectorSettings where TSelector : PlayerSelector, new()
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public TSelector PlayerSelector { get; private set; } = new TSelector();

	[JsonIgnore]
	public PlayerSelector SelectorUntyped => PlayerSelector;
}
