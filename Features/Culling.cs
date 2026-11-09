using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Features;

public static class Culling
{
	public static Rectangle ScreenRect => GetScreenRect(3);

	private static Rectangle GetScreenRect(int buffer)
	{
		Camera camera = Main.Camera;
		Vector2 pos = camera.ScaledPosition;
		Vector2 scaledSize = camera.ScaledSize;
		int bufferPixels = buffer * 16;
		int x = (int)pos.X - bufferPixels;
		int y = (int)pos.Y - bufferPixels;
		int width = (int)scaledSize.X + bufferPixels * 2;
		int height = (int)scaledSize.Y + bufferPixels * 2;
		return new Rectangle(x, y, width, height);
	}

	public static bool ShouldCull(Rectangle rect, int fluffPixels = 8)
	{
		if (!TweaksCollectionExtensions.get_Current<PerformanceTweaks>().Culling)
		{
			return false;
		}
		if (Main.gameMenu)
		{
			return false;
		}
		_ = Main.screenPosition;
		_ = Main.Camera.ScaledPosition;
		Rectangle screen = ScreenRect;
		screen.Inflate(fluffPixels, fluffPixels);
		if (!NumericsExtensions.Intersects(screen, rect))
		{
			return true;
		}
		return false;
	}

	public static bool ShouldCull(Vector2 pos, int fluffPixels = 32)
	{
		if (!TweaksCollectionExtensions.get_Current<PerformanceTweaks>().Culling)
		{
			return false;
		}
		if (Main.gameMenu)
		{
			return false;
		}
		_ = Main.screenPosition;
		_ = Main.Camera.ScaledPosition;
		Rectangle screen = ScreenRect;
		screen.Inflate(fluffPixels, fluffPixels);
		if (!screen.Contains(pos.ToPoint()))
		{
			return true;
		}
		return false;
	}

	public static bool ShouldCull(Vector2[] positions, int fluffPixels = 32)
	{
		if (!TweaksCollectionExtensions.get_Current<PerformanceTweaks>().Culling)
		{
			return false;
		}
		if (Main.gameMenu)
		{
			return false;
		}
		_ = Main.screenPosition;
		_ = Main.Camera.ScaledPosition;
		Rectangle screen = ScreenRect;
		screen.Inflate(fluffPixels, fluffPixels);
		for (int i = 0; i < positions.Length; i++)
		{
			if (screen.Contains(positions[i].ToPoint()))
			{
				return false;
			}
		}
		return true;
	}

	public static bool NearCameraSections(Point pos)
	{
		if (!TweaksCollectionExtensions.get_Current<PerformanceTweaks>().Culling)
		{
			return true;
		}
		if (Main.gameMenu)
		{
			return true;
		}
		if (Main.mapFullscreen)
		{
			return true;
		}
		int posX = Netplay.GetSectionX(pos.X);
		int posY = Netplay.GetSectionY(pos.Y);
		Point p = NumericsExtensions[ScreenRect];
		int sX = Netplay.GetSectionX(p.X / 16);
		int sY = Netplay.GetSectionY(p.Y / 16);
		if (posX < sX - 1 || posX > sX + 1 || posY < sY - 1 || posY > sY + 1)
		{
			return false;
		}
		return true;
	}
}
