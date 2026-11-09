using System.Collections.Generic;
using System.Reflection;
using Terraria.UtilClient.Cheats.Categories.Combat;
using Terraria.UtilClient.Cheats.Categories.Exploit;
using Terraria.UtilClient.Cheats.Categories.LocalPlayer;
using Terraria.UtilClient.Cheats.Categories.Misc;
using Terraria.UtilClient.Cheats.Categories.Travel;
using Terraria.UtilClient.Cheats.Categories.Visual;
using Terraria.UtilClient.Cheats.Types;

namespace Terraria.UtilClient.Cheats;

public static class BuiltInCheats
{
	public static readonly GodmodeCheat Godmode = new GodmodeCheat();

	public static readonly SimpleToggleCheat NoFallDamage = new SimpleToggleCheat("No Fall Damage", CheatCategory.LocalPlayer);

	public static readonly SimpleToggleCheat NoKnockback = new SimpleToggleCheat("No Knockback", CheatCategory.LocalPlayer);

	public static readonly SimpleToggleCheat InfiniteBreath = new SimpleToggleCheat("Infinite Breath", CheatCategory.LocalPlayer, saveEnabled: true, panicModeAffected: false, "infbreath");

	public static readonly SimpleToggleCheat IgnoreDamageBlocks = new SimpleToggleCheat("Ignore Damage Blocks", CheatCategory.LocalPlayer);

	public static readonly SimpleToggleCheat IgnoreLiquid = new SimpleToggleCheat("Ignore Liquids", CheatCategory.LocalPlayer);

	public static readonly AntiDebuffCheat AntiDebuff = new AntiDebuffCheat();

	public static readonly SimpleToggleCheat CancelAnyBuff = new SimpleToggleCheat("Cancel Any Buff", CheatCategory.LocalPlayer, saveEnabled: true, panicModeAffected: true);

	public static readonly SimpleToggleCheat InfiniteMana = new SimpleToggleCheat("Infinite Mana", CheatCategory.LocalPlayer, saveEnabled: true, panicModeAffected: false, "infmana");

	public static readonly SimpleToggleCheat UnlimitedSummons = new SimpleToggleCheat("Unlimited Summons", CheatCategory.LocalPlayer);

	public static readonly PreventItemConsumptionCheat PreventItemConsumption = new PreventItemConsumptionCheat();

	public static readonly ForcedEquipsCheat ForcedEquips = new ForcedEquipsCheat();

	public static readonly MountTweaksCheat MountTweaks = new MountTweaksCheat();

	public static readonly ExtendedReachCheat ExtendedReach = new ExtendedReachCheat();

	public static readonly SimpleToggleCheat DisableItemUseChecks = new SimpleToggleCheat("Disable Item Use Checks", CheatCategory.LocalPlayer, saveEnabled: true, panicModeAffected: false, "noitemusechecks");

	public static readonly SimpleToggleCheat TerraDiscs = new SimpleToggleCheat("Terra Discs", CheatCategory.LocalPlayer);

	public static readonly SimpleToggleCheat JourneyMode = new SimpleToggleCheat("Journey Mode", CheatCategory.LocalPlayer);

	public static readonly AutoKillHooksCheat AutoHooks = new AutoKillHooksCheat();

	public static readonly PreventServerModificationCheat PreventServerModification = new PreventServerModificationCheat();

	public static readonly MouseTPCheat MouseTP = new MouseTPCheat();

	public static readonly MapTPCheat MapTP = new MapTPCheat();

	public static readonly NoclipCheat Noclip = new NoclipCheat();

	public static readonly SimpleToggleCheat InfiniteFlight = new SimpleToggleCheat("Infinite Flight", CheatCategory.Travel, saveEnabled: true, panicModeAffected: false, "infflight");

	public static readonly SimpleToggleCheat FastSync = new SimpleToggleCheat("Fast Sync", CheatCategory.Travel, saveEnabled: false);

	public static readonly TPBypassCheat TPBypass = new TPBypassCheat();

	public static readonly SimpleToggleCheat IgnoreCreativeShock = new SimpleToggleCheat("Ignore Creative Shock", CheatCategory.Build);

	public static readonly FullbrightCheat GlobalBrightness = new FullbrightCheat();

	public static readonly TrackPlayersCheat TrackPlayers = new TrackPlayersCheat();

	public static readonly HitboxDisplayCheat HitboxDisplay = new HitboxDisplayCheat();

	public static readonly TileESPCheat TileESP = new TileESPCheat();

	public static readonly TileCursorCheat TileCursor = new TileCursorCheat();

	public static readonly SimpleToggleCheat Freecam = new SimpleToggleCheat("Freecam", CheatCategory.Visual, saveEnabled: false);

	public static readonly SimpleToggleCheat NoWeatherEffects = new SimpleToggleCheat("No Weather Effects", CheatCategory.Visual);

	public static readonly SimpleToggleCheat AlwaysRenderEcho = new SimpleToggleCheat("Always Render Echo", CheatCategory.Visual);

	public static readonly SmoothScrollingChEaT SmoothScrolling = new SmoothScrollingChEaT();

	public static readonly SimpleToggleCheat ReduceNPCSpawnRate = new SimpleToggleCheat("Reduce NPC Spawn Rate", CheatCategory.Combat);

	public static readonly AimbotCheat Aimbot = new AimbotCheat();

	public static readonly NoCombatIFramesCheat NoCombatIFrames = new NoCombatIFramesCheat();

	public static readonly RevealMapCheat RevealMap = new RevealMapCheat();

	public static readonly DeleteWorldItemsCheat DeleteWorldItems = new DeleteWorldItemsCheat();

	public static readonly ButcherNPCsCheat ButcherNPCs = new ButcherNPCsCheat();

	public static readonly AutoDeleteWorldItemsCheat AutoDeleteWorldItems = new AutoDeleteWorldItemsCheat();

	public static readonly AutoButcherNPCsCheat AutoButcherNPCs = new AutoButcherNPCsCheat();

	public static readonly KillOwnedProjectilesCheat KillOwnedProjectiles = new KillOwnedProjectilesCheat();

	public static readonly ParticleAnnoyCheat ParticleAnnoy = new ParticleAnnoyCheat();

	public static readonly AutoFishBotCheat AutoFishBot = new AutoFishBotCheat();

	public static readonly LagPlayersCheat LagPlayers = new LagPlayersCheat();

	public static readonly CrashMobilePlayersCheat CrashMobilePlayers = new CrashMobilePlayersCheat();

	public static readonly AutoCrashMobilePlayersCheat AutoCrashMobilePlayers = new AutoCrashMobilePlayersCheat();

	public static IEnumerable<Cheat> GetAll()
	{
		FieldInfo[] fields = typeof(BuiltInCheats).GetFields(BindingFlags.Static | BindingFlags.Public);
		FieldInfo[] array = fields;
		foreach (FieldInfo field in array)
		{
			if (field.IsInitOnly && field.GetValue(null) is Cheat cheat)
			{
				yield return cheat;
			}
		}
	}
}
