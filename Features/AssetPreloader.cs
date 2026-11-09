using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace Terraria.UtilClient.Features;

public static class AssetPreloader
{
	private static readonly HashSet<int> IgnoreGoreIds = new HashSet<int> { 0, 507, 537 };

	public static void QueueAssetLoading()
	{
		PreloadAssets(TextureAssets.Cloud);
		PreloadAssets(TextureAssets.Wall.Skip(1));
		PreloadAssets(TextureAssets.Tile);
		PreloadAssets(TextureAssets.Item.Skip(1).Where((Asset<Texture2D> g, int i) => i != 6196));
		PreloadAssets(TextureAssets.Npc.Skip(1));
		PreloadAssets(TextureAssets.Buff.Skip(1));
		PreloadAssets(TextureAssets.Extra);
		PreloadAssets(TextureAssets.Flames);
		PreloadAssets(TextureAssets.GlowMask);
		PreloadAssets(TextureAssets.PlayerHair);
		PreloadAssets(TextureAssets.PlayerHairAlt);
		PreloadAssets(TextureAssets.Gore.Where((Asset<Texture2D> g, int i) => !IgnoreGoreIds.Contains(i)));
		PreloadAssets(TextureAssets.Projectile);
	}

	private static void PreloadAssets<T>(IEnumerable<Asset<T>> assets) where T : class
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (Asset<T> a in assets)
		{
			if (a != null && (int)a.State == 0)
			{
				Main.Assets.Request<T>(a.Name, (AssetRequestMode)1);
			}
		}
	}
}
