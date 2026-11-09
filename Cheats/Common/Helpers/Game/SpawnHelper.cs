using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Cheats.Categories.Misc;
using Terraria.UtilClient.Localization;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class SpawnHelper
{
	public enum BossID : byte
	{
		KingSlime,
		EyeOfCthulhu,
		EaterOfWorlds,
		BrainOfCthulhu,
		QueenBee,
		Skeletron,
		Deerclops,
		WallOfFlesh,
		QueenSlime,
		TheTwins,
		TheDestroyer,
		SkeletronPrime,
		Plantera,
		Golem,
		DukeFishron,
		EmpressOfLight,
		LunaticCultist,
		MoonLord,
		Mechdusa
	}

	public enum InvasionID : byte
	{
		None,
		GoblinArmy,
		SnowLegion,
		Pirates,
		MartianMadness,
		PumpkinMoon,
		FrostMoon,
		SolarEclipse,
		BloodMoon,
		SlimeRain,
		OldOnesArmy,
		LunarEvents
	}

	private static readonly short[][] BossToNPCs = new short[19][]
	{
		new short[1] { 50 },
		new short[1] { 4 },
		new short[1] { 13 },
		new short[1] { 266 },
		new short[1] { 222 },
		new short[1] { 35 },
		new short[1] { 668 },
		new short[1] { 113 },
		new short[1] { 657 },
		new short[2] { 125, 126 },
		new short[1] { 134 },
		new short[1] { 127 },
		new short[1] { 262 },
		new short[1] { 245 },
		new short[1] { 370 },
		new short[1] { 636 },
		new short[1] { 439 },
		new short[1] { 398 },
		new short[4] { 134, 127, 126, 125 }
	};

	private static readonly HashSet<BossID> SPOnlyBosses = new HashSet<BossID>
	{
		BossID.WallOfFlesh,
		BossID.Plantera,
		BossID.EmpressOfLight
	};

	private static readonly HashSet<InvasionID> SPOnlyInvasions = new HashSet<InvasionID> { InvasionID.LunarEvents };

	public static ReadOnlySpan<short> GetNPCs(BossID bossID)
	{
		return BossToNPCs[(uint)bossID];
	}

	public static int GetNetID(InvasionID invasionID)
	{
		return invasionID switch
		{
			InvasionID.GoblinArmy => -1, 
			InvasionID.SnowLegion => -2, 
			InvasionID.Pirates => -3, 
			InvasionID.PumpkinMoon => -4, 
			InvasionID.FrostMoon => -5, 
			InvasionID.SolarEclipse => -6, 
			InvasionID.MartianMadness => -7, 
			InvasionID.BloodMoon => -10, 
			InvasionID.SlimeRain => -19, 
			InvasionID.OldOnesArmy => 0, 
			InvasionID.LunarEvents => 0, 
			_ => throw new UnreachableException(), 
		};
	}

	public static bool CanSummonBossMP(BossID id, out string reason)
	{
		reason = string.Empty;
		if (SPOnlyBosses.Contains(id))
		{
			reason = ULang.UIVal("ExclusiveReason", Lang.menu[12].Value);
			return false;
		}
		List<string> reasons = new List<string>(2);
		ReadOnlySpan<short> nPCs = GetNPCs(id);
		int i;
		for (i = 0; i < nPCs.Length; i++)
		{
			int npcType = nPCs[i];
			if (NPC.AnyNPCs(npcType))
			{
				reason = ULang.UIVal("BossAlreadyPresentReason", Lang.GetNPCNameValue(npcType));
				return false;
			}
		}
		switch (id)
		{
		case BossID.Skeletron:
			if (!NPC.AnyNPCs(54))
			{
				reason = ULang.UIVal("NpcNotPresentReason", Lang.GetNPCNameValue(54));
				return false;
			}
			break;
		case BossID.Golem:
		{
			if (!WorldHelper.FindTileNearPlayer(237, 20, 20, out i, out var _))
			{
				reason = ULang.UIVal("RangeReason", Lang.GetItemNameValue(1292), 20);
				return false;
			}
			break;
		}
		case BossID.LunaticCultist:
			if (!NPC.AnyNPCs(437))
			{
				reason = ULang.UIVal("NpcNotPresentReason", ULang.UIVal("Cultists"));
				return false;
			}
			break;
		case BossID.MoonLord:
			if (!NPC.downedGolemBoss)
			{
				reasons.Add(ULang.UIVal("BossUndefeatedReason", Lang.GetNPCNameValue(245)));
			}
			if (!Main.hardMode)
			{
				reasons.Add(ULang.UIVal("NotHardmodeReason", ULang.UIVal("Hardmode")));
			}
			if (NPC.AnyDanger())
			{
				reasons.Add(ULang.UIVal("NpcDangerReason"));
			}
			if (NPC.AnyoneNearCultists())
			{
				reasons.Add(ULang.UIVal("CultistsPresentReason", ULang.UIVal("Cultists")));
			}
			reason += string.Join(", ", reasons);
			return reasons.Count <= 0;
		case BossID.Mechdusa:
			if (!SpecialSeedFeatures.Mechdusa)
			{
				reason = ULang.UIVal("UnavailableInCurrentWorldReason");
				return false;
			}
			break;
		}
		return true;
	}

	private static bool CheckPlayers()
	{
		return Main.player.Any((Player player) => player.active && player.statLife >= 200);
	}

	public static bool CanStartEvent(InvasionID id, out string reason)
	{
		reason = string.Empty;
		switch (id)
		{
		case InvasionID.GoblinArmy:
		case InvasionID.SnowLegion:
		case InvasionID.Pirates:
		case InvasionID.MartianMadness:
			if (!CheckPlayers())
			{
				reason = ULang.UIVal("NotEnoughPlayersReason");
				return false;
			}
			break;
		case InvasionID.SlimeRain:
		{
			List<string> reasons = new List<string>(3);
			if (Main.raining)
			{
				reasons.Add(ULang.UIVal("RainingReason"));
			}
			if (Main.remixWorld)
			{
				reasons.Add(ULang.UIVal("WorldIsRemixReason", Language.GetTextValue("UI.Seed_Remix_Title")));
			}
			if (!Main.isThereAWorldSurface)
			{
				reasons.Add(ULang.UIVal("NoSurfaceReason"));
			}
			if (Main.slimeRain)
			{
				reasons.Add(ULang.UIVal("AlreadyOngoingReason"));
			}
			reason += string.Join(", ", reasons);
			return reasons.Count <= 0;
		}
		}
		if (Main.netMode == 1)
		{
			return CanStartEventMP(id, out reason);
		}
		return true;
	}

	private static bool CanStartEventMP(InvasionID id, out string reason)
	{
		reason = string.Empty;
		if (SPOnlyInvasions.Contains(id))
		{
			reason = ULang.UIVal("ExclusiveReason", Lang.menu[12].Value);
			return false;
		}
		if (Main.netMode == 0 && id != InvasionID.SlimeRain)
		{
			return true;
		}
		List<string> reasons = new List<string>(2);
		switch (id)
		{
		case InvasionID.PumpkinMoon:
		case InvasionID.FrostMoon:
			if (Main.dayTime)
			{
				reasons.Add(ULang.UIVal("NotRightTime", Language.GetTextValue("Bestiary_Times.NightTime")));
			}
			if (DD2Event.Ongoing)
			{
				reasons.Add(ULang.UIVal("EventPresentReason", Language.GetTextValue("Bestiary_Invasions.OldOnesArmy")));
			}
			break;
		case InvasionID.SolarEclipse:
			if (!Main.dayTime)
			{
				reasons.Add(ULang.UIVal("NotRightTime", Language.GetTextValue("Bestiary_Times.DayTime")));
			}
			if (Main.eclipse)
			{
				reasons.Add(ULang.UIVal("AlreadyOngoingReason"));
			}
			break;
		case InvasionID.BloodMoon:
			if (Main.dayTime)
			{
				reasons.Add(ULang.UIVal("NotRightTime", Language.GetTextValue("Bestiary_Times.NightTime")));
			}
			if (Main.bloodMoon)
			{
				reasons.Add(ULang.UIVal("AlreadyOngoingReason"));
			}
			break;
		case InvasionID.OldOnesArmy:
		{
			int x;
			int y;
			if (DD2Event.Ongoing)
			{
				reasons.Add(ULang.UIVal("AlreadyOngoingReason"));
			}
			else if (!WorldHelper.FindTileNearPlayer(466, 80, 80, out x, out y))
			{
				reasons.Add(ULang.UIVal("RangeReason", Lang.GetItemNameValue(3816), 80));
			}
			break;
		}
		default:
			if (Main.invasionType != 0)
			{
				if ((int)id < (int)Terraria.ID.InvasionID.Count && Main.invasionType == (int)id)
				{
					reason = ULang.UIVal("AlreadyOngoingReason");
				}
				else
				{
					reason = ULang.UIVal("AnyOtherEventPresentReason");
				}
				return false;
			}
			break;
		}
		reason += string.Join(", ", reasons);
		return reasons.Count <= 0;
	}

	public static void SpawnBoss(BossID id)
	{
		int player = Main.myPlayer;
		Vector2 position = GetBossSpawnPosition(id);
		short[] npcTypes = GetNPCs(id).ToArray();
		PlayBossSpawnSound(id);
		if (Main.netMode == 1)
		{
			SpawnBossMultiplayer(id, player, position, npcTypes);
		}
		else
		{
			SpawnBossSingleplayer(id, player, position, npcTypes);
		}
	}

	private static void SpawnBossMultiplayer(BossID id, int player, Vector2 spawnPos, short[] npcTypes)
	{
		switch (id)
		{
		case BossID.DukeFishron:
		{
			Projectile proj = Main.projectile[Projectile.NewProjectile(null, spawnPos, Vector2.Zero, 360, 0, 0f, player)];
			NetMessage.SendData(27, -1, -1, null, proj.whoAmI);
			NetMessage.SendData(61, -1, -1, null, player, 370f);
			proj.Kill();
			return;
		}
		case BossID.Skeletron:
			NetMessage.SendData(51, -1, -1, null, player, 1f);
			return;
		case BossID.Golem:
		{
			if (!WorldHelper.FindTileNearPlayer(237, 20, 20, out var _, out var _))
			{
				return;
			}
			break;
		}
		case BossID.LunaticCultist:
		{
			NPC[] npc = Main.npc;
			foreach (NPC npc2 in npc)
			{
				if (npc2.active && !npc2.immortal && (npc2.type == 438 || npc2.type == 379 || npc2.type == 380))
				{
					ButcherHelpers.ButcherSingleNPC(npc2, auto: true);
				}
			}
			return;
		}
		case BossID.Mechdusa:
			NetMessage.SendData(61, -1, -1, null, Main.myPlayer, -16f);
			return;
		}
		foreach (int npcType in npcTypes)
		{
			NetMessage.SendData(61, -1, -1, null, player, npcType);
		}
	}

	private static void SpawnBossSingleplayer(BossID id, int player, Vector2 position, short[] npcTypes)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		switch (id)
		{
		case BossID.WallOfFlesh:
			NPC.SpawnWOF(position);
			return;
		case BossID.EmpressOfLight:
			NPC.SpawnBoss((int)position.X, (int)position.Y, npcTypes[0], Main.myPlayer);
			return;
		case BossID.Golem:
		case BossID.DukeFishron:
		case BossID.MoonLord:
		{
			int npcIndex = NPC.NewNPC(NPC.GetBossSpawnSource(player), (int)position.X, (int)position.Y, npcTypes[0]);
			ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[npcIndex].GetTypeNetName()), ChatColors.BossOrEvent);
			return;
		}
		case BossID.Mechdusa:
			ForceSpawnMechQueen(position);
			return;
		}
		foreach (int npcType in npcTypes)
		{
			NPC.SpawnOnPlayer(player, npcType);
		}
	}

	private static Vector2 GetBossSpawnPosition(BossID id)
	{
		Player player = Main.LocalPlayer;
		Vector2 center = Main.LocalPlayer.Center / 16f;
		Vector2 tilePos = center;
		switch (id)
		{
		case BossID.EmpressOfLight:
			tilePos.Y -= UMain.rand.Next(16, 20);
			break;
		case BossID.DukeFishron:
			if (BuiltInCheats.Freecam.Enabled)
			{
				TileDrawing.GetScreenDrawArea(useOffscreenRange: false, out var _, out var startX, out var endX, out var startY, out var endY);
				tilePos = new Vector2(UMain.rand.Next(startX, endX), UMain.rand.Next(startY, endY));
			}
			if (player.spectating >= 0)
			{
				player = Main.player[player.spectating];
				tilePos = player.Center / 16f;
			}
			tilePos += new Vector2(UMain.rand.Next(30, 40) * player.direction, -UMain.rand.Next(10, 15));
			break;
		case BossID.MoonLord:
			tilePos.Y -= 16f;
			break;
		default:
			return center;
		}
		return tilePos * 16f;
	}

	private static void ForceSpawnMechQueen(Vector2 center)
	{
		int targetPlayer = Main.myPlayer;
		NPC.mechQueen = -2;
		NPC.SpawnOnPlayer(targetPlayer, 127);
		NPC obj = Main.npc[NPC.mechQueen = NPC.FindFirstNPC(127)];
		IEntitySource spawnSource = NPC.GetBossSpawnSource(targetPlayer);
		obj.Center = center;
		int centerX = (int)center.X;
		int centerY = (int)center.Y;
		NPC.NewNPC(spawnSource, centerX, centerY, 125, 1);
		NPC.NewNPC(spawnSource, centerX, centerY, 126, 1);
		int destroyerSlot = NPC.NewNPC(spawnSource, centerX, centerY, 134, 1);
		NPC.NewNPC(spawnSource, centerX, centerY, 139, 1, 0f, 0f, destroyerSlot, -1f);
		NPC.NewNPC(spawnSource, centerX, centerY, 139, 1, 0f, 0f, destroyerSlot, 1f);
	}

	public static void StartInvasion(InvasionID id)
	{
		PlayInvasionStartSound(id);
		if (Main.netMode == 1)
		{
			StartInvasionMultiplayer(id);
		}
		else
		{
			StartInvasionSingleplayer(id);
		}
	}

	private static void StartInvasionMultiplayer(InvasionID id)
	{
		if (id == InvasionID.OldOnesArmy)
		{
			if (WorldHelper.FindTileNearPlayer(466, 80, 80, out var crystalX, out var crystalY))
			{
				NetMessage.SendData(113, -1, -1, null, crystalX, crystalY);
			}
		}
		else
		{
			NetMessage.SendData(61, -1, -1, null, Main.myPlayer, GetNetID(id));
		}
	}

	private static void StartInvasionSingleplayer(InvasionID id)
	{
		switch (id)
		{
		case InvasionID.GoblinArmy:
		case InvasionID.SnowLegion:
		case InvasionID.Pirates:
		case InvasionID.MartianMadness:
			InvasionReset();
			Main.StartInvasion((int)id);
			break;
		case InvasionID.PumpkinMoon:
			Main.NewText(Lang.misc[31].Value, 50, byte.MaxValue, 130);
			Main.startPumpkinMoon();
			break;
		case InvasionID.FrostMoon:
			Main.NewText(Lang.misc[34].Value, 50, byte.MaxValue, 130);
			Main.startSnowMoon();
			break;
		case InvasionID.SolarEclipse:
			Main.eclipse = true;
			Main.NewText(Lang.misc[Main.remixWorld ? 106 : 20].Value, 50, byte.MaxValue, 130);
			break;
		case InvasionID.BloodMoon:
			Main.bloodMoon = true;
			if (Main.GetMoonPhase() == MoonPhase.Empty)
			{
				Main.moonPhase = 5;
			}
			Main.NewText(Lang.misc[8].Value, 50, byte.MaxValue, 130);
			break;
		case InvasionID.SlimeRain:
			Main.slimeRain = false;
			Main.slimeRainTime = 0.0;
			Main.StartSlimeRain();
			Main.slimeWarningTime = 1;
			break;
		case InvasionID.OldOnesArmy:
			DD2Event.ForceSummonCrystal(Main.myPlayer, Main.LocalPlayer.UnmountedCenter.ToPoint());
			break;
		case InvasionID.LunarEvents:
			WorldGen.TriggerLunarApocalypse();
			break;
		}
		static void InvasionReset()
		{
			Main.invasionType = 0;
			Main.invasionSize = 0;
			Main.invasionDelay = 0;
			Main.invasionX = Main.spawnTileX - 1;
			Main.invasionWarn = 2;
		}
	}

	public static string GetBossName(BossID id)
	{
		if (id == BossID.Mechdusa)
		{
			return ULang.UIVal("Mechdusa");
		}
		if (GetNPCs(id).Length < 1)
		{
			return string.Empty;
		}
		List<string> names = new List<string>(2);
		ReadOnlySpan<short> nPCs = GetNPCs(id);
		for (int i = 0; i < nPCs.Length; i++)
		{
			int npcType = nPCs[i];
			switch (npcType)
			{
			case 125:
			case 126:
				return Language.GetTextValue("Enemies.TheTwins");
			case 398:
				names.Add(Language.GetTextValue("Enemies.MoonLord"));
				break;
			default:
				names.Add(Lang.GetNPCNameValue(npcType));
				break;
			}
		}
		return string.Join(", ", names);
	}

	public static string GetInvasionName(InvasionID id)
	{
		return id switch
		{
			InvasionID.GoblinArmy => Lang.inter[88].Value, 
			InvasionID.SnowLegion => Lang.inter[87].Value, 
			InvasionID.Pirates => Lang.inter[86].Value, 
			InvasionID.MartianMadness => Lang.inter[85].Value, 
			InvasionID.OldOnesArmy => Language.GetTextValue("Bestiary_Invasions.OldOnesArmy"), 
			InvasionID.SolarEclipse => Language.GetTextValue("Bestiary_Events.Eclipse"), 
			InvasionID.BloodMoon => Language.GetTextValue("Bestiary_Events.BloodMoon"), 
			InvasionID.SlimeRain => Language.GetTextValue("Bestiary_Events.SlimeRain"), 
			InvasionID.PumpkinMoon => Language.GetTextValue("Bestiary_Invasions.PumpkinMoon"), 
			InvasionID.FrostMoon => Language.GetTextValue("Bestiary_Invasions.FrostMoon"), 
			InvasionID.LunarEvents => ULang.UIVal("LunarApocalypse"), 
			_ => string.Empty, 
		};
	}

	public static void PlayBossSpawnSound(BossID id)
	{
		switch (id)
		{
		case BossID.QueenBee:
			SoundEngine.PlaySound(SoundID.Item173);
			break;
		default:
			SoundEngine.PlaySound(15, -1, -1, 0);
			break;
		case BossID.WallOfFlesh:
		case BossID.DukeFishron:
		case BossID.EmpressOfLight:
			break;
		}
	}

	public static void PlayInvasionStartSound(InvasionID id)
	{
		if (id != InvasionID.OldOnesArmy)
		{
			SoundEngine.PlaySound(15, -1, -1, 0);
		}
	}
}
