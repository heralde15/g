using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria.UtilClient.Config.JsonConverters;

namespace Terraria.UtilClient.Cheats.Settings;

public interface IRadiusSettings
{
	float Radius { get; set; }

	bool RadiusVisual { get; set; }

	[JsonConverter(typeof(ColorAlphaConverter))]
	Color RadiusColor { get; set; }
}
