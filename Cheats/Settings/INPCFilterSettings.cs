using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Terraria.UtilClient.Common.Helpers.Game;

namespace Terraria.UtilClient.Cheats.Settings;

public interface INPCFilterSettings
{
	[JsonConverter(typeof(StringEnumConverter))]
	NPCHelper.NPCTypeFilter Filter { get; set; }

	bool IncludeInvincibleNPCs { get; set; }

	bool IncludeMultiPartNPCs { get; set; }
}
