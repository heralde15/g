using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using Terraria.DataStructures;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class MiscHelper
{
	public static readonly TimeSpan GameTick = TimeSpan.FromSeconds(1.0 / 60.0);

	[CompilerGenerated]
	private static HashSet<int> _003CDebuffs_003Ek__BackingField;

	public static readonly ImmutableHashSet<int> NonDebuffDebuffs = ImmutableHashSet.Create((ReadOnlySpan<int>)new int[14]
	{
		28, 34, 86, 87, 89, 146, 147, 157, 158, 165,
		194, 199, 215, 332
	});

	public static Vector2 MouseOrMapPosition
	{
		get
		{
			if (Main.mapFullscreen)
			{
				return CalculateMapPosition();
			}
			return Main.MouseWorld;
		}
	}

	public static Point MouseOrMapTilePosition
	{
		get
		{
			if (Main.mapFullscreen)
			{
				return CalculateMapPosition().ToPoint();
			}
			return Main.MouseWorld.ToTileCoordinates();
		}
	}

	public static HashSet<int> Debuffs
	{
		get
		{
			if (_003CDebuffs_003Ek__BackingField == null)
			{
				_003CDebuffs_003Ek__BackingField = (from i in Enumerable.Range(0, Main.debuff.Length)
					where Main.debuff[i] && !NonDebuffDebuffs.Contains(i)
					select i).ToHashSet();
			}
			return _003CDebuffs_003Ek__BackingField;
		}
	}

	public static Vector2 CalculateMapPosition()
	{
		float fsScale = Main.mapFullscreenScale;
		float minZoom = (float)Main.screenWidth / (float)Main.maxTilesX * 0.599f;
		fsScale = Math.Clamp(fsScale, minZoom, 31.2f);
		float num24 = Main.mapFullscreenPos.X;
		float num25 = Main.mapFullscreenPos.Y;
		num24 *= fsScale;
		num25 *= fsScale;
		float num26 = 0f - num24 + (float)Main.screenWidth / 2f;
		float num27 = 0f - num25 + (float)Main.screenHeight / 2f;
		num26 += 10f * fsScale;
		num27 += 10f * fsScale;
		return (Main.MouseScreenUnscaled - new Vector2(num26 - 10f * fsScale, num27 - 10f * fsScale)) / fsScale;
	}

	public static Rectangle CenterRect(Point pos, Size size)
	{
		return new Rectangle(pos.X - size.Width / 2, pos.Y - size.Height / 2, size.Width, size.Height);
	}

	public static Rectangle CenterRect(Point pos, int size)
	{
		return new Rectangle(pos.X - size / 2, pos.Y - size / 2, size, size);
	}

	public static void HandleSuicide(Player localPlayer, string reasonType, int param, int param2)
	{
		switch (reasonType)
		{
		case "p":
		case "plr":
		case "player":
			if (param >= 0 && param < 255)
			{
				localPlayer.KillMe(PlayerDeathReason.ByPlayer(param, param2), 10.0, 0, pvp: true, force: true);
			}
			break;
		case "n":
		case "npc":
			localPlayer.KillMe(PlayerDeathReason.ByNPC(param), 10.0, 0, pvp: true, force: true);
			break;
		case "proj":
		case "projectile":
		{
			int owner = ((param2 >= -1 && param2 < 255) ? param2 : Main.myPlayer);
			localPlayer.KillMe(PlayerDeathReason.ByProjectile(owner, UMain.rand.Next(1, 1001), param), 10.0, 0, pvp: true, force: true);
			break;
		}
		case "o":
		case "other":
			localPlayer.KillMe(PlayerDeathReason.ByOther(param), 10.0, 0, pvp: true, force: true);
			break;
		default:
			localPlayer.KillMe(PlayerDeathReason.LegacyDefault(), 10.0, 0, pvp: true, force: true);
			break;
		}
	}

	public static object? CloneIfMutable(object? value)
	{
		if (value != null)
		{
			if (!(value is IList))
			{
				if (!(value is IDictionary))
				{
					if (value is ICloneable cloneable)
					{
						return cloneable.Clone();
					}
					return value;
				}
				return Activator.CreateInstance(value.GetType(), value);
			}
			return Activator.CreateInstance(value.GetType(), value);
		}
		return null;
	}

	public static List<string> TryToFindSteamPacks()
	{
		List<string> paths = new List<string>();
		if (OperatingSystem.IsWindows())
		{
			string steamPath = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Valve\\Steam", "SteamPath", "") as string;
			if (!string.IsNullOrWhiteSpace(steamPath))
			{
				steamPath = steamPath.Replace('/', Path.DirectorySeparatorChar);
				InlineArray5<string> buffer = default(InlineArray5<string>);
				buffer[0] = steamPath;
				buffer[1] = "steamapps";
				buffer[2] = "workshop";
				buffer[3] = "content";
				buffer[4] = "105600";
				paths.Add(Path.Combine(buffer));
			}
			string programFiles = Environment.GetEnvironmentVariable("PROGRAMFILES");
			string programFilesX86 = Environment.GetEnvironmentVariable("PROGRAMFILES(X86)");
			if (!string.IsNullOrWhiteSpace(programFiles))
			{
				paths.Add(Path.Combine(programFiles, "Steam\\steamapps\\workshop\\content\\105600"));
			}
			if (!string.IsNullOrWhiteSpace(programFilesX86))
			{
				paths.Add(Path.Combine(programFilesX86, "Steam\\steamapps\\workshop\\content\\105600"));
			}
		}
		else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
		{
			string home = Environment.GetEnvironmentVariable("HOME");
			if (!string.IsNullOrWhiteSpace(home))
			{
				InlineArray8<string> buffer2 = default(InlineArray8<string>);
				buffer2[0] = home;
				buffer2[1] = ".local";
				buffer2[2] = "share";
				buffer2[3] = "Steam";
				buffer2[4] = "steamapps";
				buffer2[5] = "workshop";
				buffer2[6] = "content";
				buffer2[7] = "105600";
				string steampath = Path.Combine(buffer2);
				paths.Add(steampath);
				if (!Directory.Exists(steampath))
				{
					InlineArray7<string> buffer3 = default(InlineArray7<string>);
					buffer3[0] = home;
					buffer3[1] = ".steam";
					buffer3[2] = "steam";
					buffer3[3] = "steamapps";
					buffer3[4] = "workshop";
					buffer3[5] = "content";
					buffer3[6] = "105600";
					paths.Add(Path.Combine(buffer3));
				}
				InlineArray10<string> buffer4 = default(InlineArray10<string>);
				buffer4[0] = home;
				buffer4[1] = ".var";
				buffer4[2] = "app";
				buffer4[3] = "com.valvesoftware.Steam";
				buffer4[4] = "data";
				buffer4[5] = "Steam";
				buffer4[6] = "steamapps";
				buffer4[7] = "workshop";
				buffer4[8] = "content";
				buffer4[9] = "105600";
				paths.Add(Path.Combine(buffer4));
				if (OperatingSystem.IsMacOS())
				{
					InlineArray8<string> buffer5 = default(InlineArray8<string>);
					buffer5[0] = home;
					buffer5[1] = "Library";
					buffer5[2] = "Application Support";
					buffer5[3] = "Steam";
					buffer5[4] = "steamapps";
					buffer5[5] = "workshop";
					buffer5[6] = "content";
					buffer5[7] = "105600";
					paths.Add(Path.Combine(buffer5));
				}
			}
		}
		paths.RemoveAll((string item) => !Directory.Exists(item));
		return paths;
	}
}
