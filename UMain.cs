using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.IO;
using Terraria.Map;
using Terraria.Net;
using Terraria.ObjectData;
using Terraria.Testing;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Chat.Commands;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Common.Helpers.Game;
using Terraria.UtilClient.Features;
using Terraria.UtilClient.Hooks;
using Terraria.UtilClient.Input;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;
using Terraria.UtilClient.UI;
using Terraria.UtilClient.UI.States;

namespace Terraria.UtilClient;

public static class UMain
{
	[ThreadStatic]
	private static Random? _rand;

	public static readonly CoreManager MainManager = new CoreManager();

	public const string ClientName = "zMod";

	public const string LegacyClientName = "tClient";

	public const string ClientRelease = "BETA";

	public static readonly ImmutableArray<string> DangerousServers = ImmutableCollectionsMarshal.AsImmutableArray(new string[1] { "teeria" });

	public static readonly string SavePath = Path.Combine(Main.SavePath, "zMod");

	internal static readonly Stopwatch TimeSinceWorldEntry = new Stopwatch();

	internal static readonly Stopwatch TimeSinceLastMemClear = new Stopwatch();

	public static bool Initialized { get; private set; }

	public static bool InTheMiddleOfReloading { get; private set; }

	public static bool HasLoadedAWorld { get; private set; }

	public static Random rand => _rand ?? (_rand = new Random());

	internal static void Initialize()
	{
		if (!Initialized)
		{
			JellyEffect.Initialize();
			UtilHooks.AttachAllHooks();
			Initialized = true;
		}
	}

	internal static void PartialReload(UtilMessage? message = null)
	{
		InTheMiddleOfReloading = true;
		try
		{
			MainManager.Reload<CheatManager>();
			MainManager.Reload<TMLKeybindManager>();
			UtilClientCommandProcessor.Reload();
			Main.instance.LoadSettings();
		}
		finally
		{
			InTheMiddleOfReloading = false;
			message?.Reply(ULang.CommandVal("Reload"));
		}
	}

	internal static void OnWorldEntry()
	{
		if (ClientUI.MainPanel.CurrentState == null)
		{
			ClientUI.MainPanel.SetState(new UICheatPanel());
		}
		if (!HasLoadedAWorld)
		{
			HasLoadedAWorld = true;
		}
		if (GameStateData.OnDangerousServer)
		{
			ChatLogging.LogInfo(ULang.CommonVal("RiskyServerWarning", Netplay.ServerIPText, "zMod"));
		}
		TimeSinceWorldEntry.Restart();
		TileObjectData.ChestsOnTablesHack();
	}

	internal static void OnDisconnect()
	{
		ChatHelper.ClearDelayedMessagesCache();
		GameStateData.PrevWorldData = null;
		GameStateData.RequestedSections = -1;
		GameStateData.CurrentServerType = GameStateData.ServerType.Unknown;
	}

	internal static void DoUpdate(GameTime gameTime)
	{
		if (!Main.gameMenu)
		{
			ManagerExtensions.get_Instance<TMLKeybindManager>().CheckKeybinds();
			ManagerExtensions.get_Instance<CheatManager>().TickAllCheats();
			JellyEffect.Update();
			CheckMemoryUsage();
		}
	}

	internal static void DoUpdateInWorld()
	{
	}

	internal static void OnWorldChange()
	{
		Main.ToggleGameplayUpdates(state: false);
		Ping.Reset();
		WorldGen.worldCleared = false;
		Main.cloudBGAlpha = ((Main.cloudBGActive >= 1f) ? 1f : 0f);
		Main.windSpeedCurrent = Main.windSpeedTarget;
		Cloud.resetClouds();
		Main.cloudAlpha = Main.maxRaining;
		if (Main.mapEnabled)
		{
			PreWorldMapChange();
		}
		WorldGen.clearWorld();
		if (Main.mapEnabled)
		{
			PostWorldMapChange();
		}
		TimeSinceWorldEntry.Restart();
		SeedCracker.TryCrackSeed();
		ChatLogging.LogInfo("Entered new world");
	}

	private static void PreWorldMapChange()
	{
		FileMetadata mapFileMetadata = Main.MapFileMetadata;
		string prevWorldName = GameStateData.PrevWorldName;
		WorldFileData prevWorldFileData = GameStateData.PrevWorldFileData;
		if (GameStateData.PrevWorldData.HasValue)
		{
			WorldMap map = new WorldMap(Main.Map);
			WorldData prevWorldData = GameStateData.PrevWorldData.Value;
			Task.Run(delegate
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				MapHelper.SaveMapOnWorldChange(map, prevWorldFileData, mapFileMetadata, prevWorldName, prevWorldData.WorldId, prevWorldData.MaxTilesY, prevWorldData.MaxTilesX);
				stopwatch.Stop();
				ManagerExtensions.get_Instance<LoggingManager>().zMod.Info((object)$"Saved map of previous world {prevWorldName} in {stopwatch.Elapsed.TotalSeconds:0.000}s.");
			});
		}
	}

	private static void PostWorldMapChange()
	{
		WorldMap map = Main.Map;
		Task.Run(delegate
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			map.LoadOnWorldChange();
			stopwatch.Stop();
			map.AllChanged();
			Main.refreshMap = true;
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Info((object)$"Loaded map of {GameStateData.PrevWorldName} in {stopwatch.Elapsed.TotalSeconds:0.000}s.");
		});
	}

	internal static void DoEarlyUpdate()
	{
		try
		{
			OptionUpdate();
			if (Main.gameMenu)
			{
				ManagerExtensions.get_Instance<CheatManager>().OnWorldLeave();
			}
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)"Error during early update", ex);
		}
	}

	internal static void OptionUpdate()
	{
		if (TweaksCollectionExtensions.get_Current<PerformanceTweaks>().AntiLag)
		{
			AntiLag.Tick();
		}
		if (TweaksCollectionExtensions.get_Current<PerformanceTweaks>().AntiSpamProjectiles || TweaksCollectionExtensions.get_Current<PerformanceTweaks>().AntiSpamSound || TweaksCollectionExtensions.get_Current<PerformanceTweaks>().AntiSpamParticles)
		{
			AntiSpam.Tick();
		}
		MutePlayerMessages.Tick();
	}

	internal static void UpdateSyncRates()
	{
		if (BuiltInCheats.Noclip.Enabled && Main.netPlayCounter % BuiltInCheats.Noclip.Settings.SyncRate == 0 && Main.netPlayCounter % 420 != 0)
		{
			NetHelper.SyncLocalPlayerControls();
		}
		if (BuiltInCheats.Godmode.Enabled && Main.LocalPlayer.hostile && Main.netPlayCounter % 30 == 0 && BuiltInCheats.Godmode.Settings.ImmediateRegen && Main.netPlayCounter % 900 != 0)
		{
			NetMessage.SendData(16, -1, -1, null, Main.myPlayer);
		}
		if (BuiltInCheats.InfiniteMana.Enabled && Main.netPlayCounter % 30 == 0 && Main.netPlayCounter % 900 != 0)
		{
			NetMessage.SendData(40, -1, -1, null, Main.myPlayer);
		}
	}

	internal static void CheckMusicSwap()
	{
		if (TweaksCollectionExtensions.get_Current<AudioTweaks>().UseMusicSwapper && TweaksCollectionExtensions.get_Current<AudioTweaks>().MusicIDSwap.ContainsKey(Main.newMusic))
		{
			Main.newMusic = TweaksCollectionExtensions.get_Current<AudioTweaks>().MusicIDSwap[Main.newMusic];
		}
	}

	public static bool IsDangerousSever(string IP)
	{
		if (string.IsNullOrEmpty(IP))
		{
			return false;
		}
		foreach (string dangerousServer in DangerousServers)
		{
			if (IP.Contains(dangerousServer, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static GameStateData.ServerType TryDetectServer()
	{
		if (Main.netMode == 0)
		{
			Invariant.Assert(condition: false, "TryDeetctServer in singleplayer");
			return GameStateData.ServerType.UtilClient;
		}
		if (Main.ServerSideCharacter)
		{
			return GameStateData.ServerType.TShock;
		}
		if (Netplay.Connection != null && Netplay.Connection.UtilClientServer)
		{
			return GameStateData.ServerType.UtilClient;
		}
		NetMessage.TrySendData(147, -1, -1, null, Main.myPlayer, 67f);
		NetMessage.TrySendData(147, -1, -1, null, Main.myPlayer, Main.LocalPlayer.CurrentLoadoutIndex);
		return GameStateData.ServerType.Unknown;
	}

	public static void CheckMemoryUsage()
	{
	}
}
