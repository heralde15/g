using System;
using System.Collections.Generic;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class WorldHelper
{
	public static IEnumerable<(Tile tile, int x, int y)> EnumerateLoadedTiles(Func<Tile, int, int, bool> filter)
	{
		Tilemap tiles = Main.tile;
		for (int sectionX = 0; sectionX < Main.maxSectionsX; sectionX++)
		{
			for (int sectionY = 0; sectionY < Main.maxSectionsY; sectionY++)
			{
				if (!Main.sectionManager.SectionLoaded(sectionX, sectionY))
				{
					continue;
				}
				int startX = sectionX * 200;
				int startY = sectionY * 150;
				for (int x = startX; x < startX + 200; x++)
				{
					for (int y = startY; y < startY + 150; y++)
					{
						Tile tile = tiles[x, y];
						if (filter(tile, x, y))
						{
							yield return (tile: tile, x: x, y: y);
						}
					}
				}
			}
		}
	}

	public static IEnumerable<Chest> GetAllChests(Func<Chest, bool>? filter = null)
	{
		bool hasFilter = filter != null;
		Chest[] chest = Main.chest;
		foreach (Chest chest2 in chest)
		{
			if (chest2 != null && (!hasFilter || filter(chest2)))
			{
				yield return chest2;
			}
		}
	}

	public static int SectionCountLoaded(bool loaded = true)
	{
		if (Main.sectionManager == null)
		{
			return 0;
		}
		int count = 0;
		for (int i = 0; i < Main.maxSectionsX; i++)
		{
			for (int j = 0; j < Main.maxSectionsY; j++)
			{
				if (Main.sectionManager.SectionLoaded(i, j) == loaded)
				{
					count++;
				}
			}
		}
		return count;
	}

	public static bool FindTileNearPlayer(int tileType, int radiusX, int radiusY, out int x, out int y)
	{
		Player localPlayer = Main.LocalPlayer;
		int centerX = (int)(localPlayer.Center.X / 16f);
		int centerY = (int)(localPlayer.Center.Y / 16f);
		for (int ix = centerX - radiusX; ix < centerX + radiusX; ix++)
		{
			for (int iy = centerY - radiusY; iy < centerY + radiusY; iy++)
			{
				if (WorldGen.InWorld(ix, iy))
				{
					Tile tile = Main.tile[ix, iy];
					if (tile.HasTile && tile.TileType == tileType)
					{
						x = ix;
						y = iy;
						return true;
					}
				}
			}
		}
		x = 0;
		y = 0;
		return false;
	}
}
