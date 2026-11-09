using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Terraria.UtilClient.Config.JsonConverters;
using Terraria.UtilClient.Rendering;

namespace Terraria.UtilClient.Cheats.Settings;

public interface ITileOverlaySettings
{
	[JsonConverter(typeof(StringEnumConverter))]
	TileOverlays.TileOverlayTexture Texture { get; set; }

	[JsonConverter(typeof(ColorAlphaConverter))]
	Color Color { get; }

	byte OutlineThickness { get; set; }
}
