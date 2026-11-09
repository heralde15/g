using System;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerWorldInfo : UIState
{
	private UIList _uiList;

	public InnerWorldInfo()
	{
		UIElement uIElement = UIHelpers.CreateCommonBase();
		_uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = UIHelpers.CreateCommonScroller();
		_uiList.SetScrollbar(uIScrollbar);
		uIElement.Append(uIScrollbar);
		CreateBasicWorldInfo();
		CreateBossProgressionInfo();
		CreateInvasionInfo();
		CreateSeedInfo();
		uIElement.Append(_uiList);
		Append(uIElement);
	}

	private UIDescriptiveItem CreateDescriptiveItem(string name, Func<string> getValue, Func<bool> disableCon = null)
	{
		UIDescriptiveItem decriptive = new UIDescriptiveItem(name, () => Language.GetTextValue(name + "Description"), getValue, disableCon);
		SetupDescriptiveItem(decriptive);
		return decriptive;
	}

	private UIDescriptiveItem CreateDescriptiveItemWith(string name, Func<string> description, Func<string> getValue, Func<bool> disableCon = null)
	{
		UIDescriptiveItem decriptive = new UIDescriptiveItem(name, description, getValue, disableCon);
		SetupDescriptiveItem(decriptive);
		return decriptive;
	}

	private UIDescriptiveItem CreateDescriptiveItemWith(Func<string> name, Func<string> description, Func<string> getValue, Func<bool> disableCon = null)
	{
		UIDescriptiveItem decriptive = new UIDescriptiveItem(name, description, getValue, disableCon);
		SetupDescriptiveItem(decriptive);
		return decriptive;
	}

	private void SetupDescriptiveItem(UIDescriptiveItem descriptive)
	{
		descriptive.Height.Set(26f, 0f);
		descriptive.Width.Set(0f, 0.975f);
		descriptive.HAlign = 0.5f;
	}

	private void CreateBasicWorldInfo()
	{
		_uiList.Add(UIHelpers.CreateGroup("UtilClientUI.WorldInfoTab", CreateDescriptiveItem("UtilClientUI.WorldName", GetWorldName), CreateDescriptiveItem("UtilClientUI.WorldDifficulty", GetWorldDifficulty), CreateDescriptiveItem("UtilClientUI.WorldEvil", GetWorldEvil), CreateDescriptiveItem("UtilClientUI.WorldSeed", GetWorldSeed, DisableAsRemoteClientIfNoSeed), CreateDescriptiveItemWith("UtilClientUI.Hardmode", GetHardmodeDesc, GetHardmode), CreateDescriptiveItem("UtilClientUI.WorldID", GetWorldID), CreateDescriptiveItem("UtilClientUI.WorldGUID", GetWorldGUID), CreateDescriptiveItem("UtilClientUI.WorldGeneratorVersion", GetWorldGeneratorVersion)));
	}

	private void CreateBossProgressionInfo()
	{
		_uiList.Add(UIHelpers.CreateGroup("UtilClientUI.BossProgressInfo", CreateDescriptiveItemWith(() => NpcName(50), null, () => Defeated(NPC.downedSlimeKing)), CreateDescriptiveItemWith(() => NpcName(4), null, () => Defeated(NPC.downedBoss1)), CreateDescriptiveItemWith(EvilBoss, null, () => Defeated(NPC.downedBoss2)), CreateDescriptiveItemWith(() => NpcName(35), null, () => Defeated(NPC.downedBoss3)), CreateDescriptiveItemWith(() => NpcName(222), null, () => Defeated(NPC.downedQueenBee)), CreateDescriptiveItemWith(() => NpcName(668), null, () => Defeated(NPC.downedDeerclops)), CreateDescriptiveItemWith(() => NpcName(657), null, () => Defeated(NPC.downedQueenSlime)), CreateDescriptiveItemWith(() => NpcName(134), null, () => Defeated(NPC.downedMechBoss1)), CreateDescriptiveItemWith(TheTwins, null, () => Defeated(NPC.downedMechBoss2)), CreateDescriptiveItemWith(() => NpcName(127), null, () => Defeated(NPC.downedMechBoss3)), CreateDescriptiveItemWith(() => NpcName(262), null, () => Defeated(NPC.downedPlantBoss)), CreateDescriptiveItemWith(() => NpcName(245), null, () => Defeated(NPC.downedGolemBoss)), CreateDescriptiveItemWith(() => NpcName(370), null, () => Defeated(NPC.downedFishron)), CreateDescriptiveItemWith(() => NpcName(636), null, () => Defeated(NPC.downedEmpressOfLight)), CreateDescriptiveItemWith(() => NpcName(439), null, () => Defeated(NPC.downedAncientCultist)), CreateDescriptiveItemWith(() => NpcName(396), null, () => Defeated(NPC.downedMoonlord))));
	}

	private void CreateInvasionInfo()
	{
		_uiList.Add(UIHelpers.CreateGroup("UtilClientUI.InvasionInfo", CreateDescriptiveItemWith(() => Invasion(Lang.inter[88].Key), null, () => Defeated(NPC.downedGoblins)), CreateDescriptiveItemWith(() => Invasion(Lang.inter[87].Key), null, () => Defeated(NPC.downedFrost)), CreateDescriptiveItemWith(() => Invasion(Lang.inter[86].Key), null, () => Defeated(NPC.downedPirates)), CreateDescriptiveItemWith(() => Invasion(Lang.inter[85].Key), null, () => Defeated(NPC.downedMartians)), CreateDescriptiveItemWith(() => GetDD2Tier(), null, () => Defeated(DD2Event.DownedInvasionT1)), CreateDescriptiveItemWith(() => GetDD2Tier(2), null, () => Defeated(DD2Event.DownedInvasionT2)), CreateDescriptiveItemWith(() => GetDD2Tier(3), null, () => Defeated(DD2Event.DownedInvasionT3))));
	}

	private void CreateSeedInfo()
	{
		_uiList.Add(UIHelpers.CreateGroup("UtilClientUI.WorldSeeds", CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_Drunk_Title"), null, () => YesOrNo(Main.drunkWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_ForTheWorthy_Title"), null, () => YesOrNo(Main.getGoodWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_NotTheBees_Title"), null, () => YesOrNo(Main.notTheBeesWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_Celebration_Title"), null, () => YesOrNo(Main.tenthAnniversaryWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_TheConstant_Title"), null, () => YesOrNo(Main.dontStarveWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_NoTraps_Title"), null, () => YesOrNo(Main.noTrapsWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_Remix_Title"), null, () => YesOrNo(Main.remixWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_Everything_Title"), null, () => YesOrNo(Main.zenithWorld)), CreateDescriptiveItemWith(() => Language.GetTextValue("UI.Seed_Skyblock_Title"), null, () => YesOrNo(Main.skyblockWorld)), CreateDescriptiveItemWith(() => ULang.UIVal("LowTilesSkyblock", Language.GetTextValue("UI.Seed_Skyblock_Title")), null, () => YesOrNo(WorldGen.Skyblock.lowTiles)), CreateDescriptiveItemWith(() => ULang.UIVal("VampireSeed"), null, () => YesOrNo(Main.vampireSeed)), CreateDescriptiveItemWith(() => ULang.UIVal("InfectedSeed"), null, () => YesOrNo(Main.infectedSeed)), CreateDescriptiveItemWith(() => ULang.UIVal("TeamBasedSpawnsSeed"), null, () => YesOrNo(Main.teamBasedSpawnsSeed)), CreateDescriptiveItemWith(() => ULang.UIVal("DualDungeonsSeed"), null, () => YesOrNo(Main.dualDungeonsSeed))));
	}

	public bool DisableAsRemoteClient()
	{
		return Main.netMode == 1;
	}

	public bool DisableAsRemoteClientIfNoSeed()
	{
		if (Main.netMode == 1)
		{
			return Main.ActiveWorldFileData.SeedText == null;
		}
		return false;
	}

	public string GetWorldName()
	{
		return Main.worldName;
	}

	public string GetWorldDifficulty()
	{
		if (Main.GameMode == 3)
		{
			return Language.GetTextValue("UI.Creative");
		}
		int num = 1;
		switch (Main.GameMode)
		{
		case 1:
			num = 2;
			break;
		case 2:
			num = 3;
			break;
		}
		if (Main.getGoodWorld)
		{
			num++;
		}
		return num switch
		{
			2 => Language.GetTextValue("UI.Expert"), 
			3 => Language.GetTextValue("UI.Master"), 
			4 => Language.GetTextValue("UI.Legendary"), 
			_ => Language.GetTextValue("UI.Normal"), 
		};
	}

	public string GetWorldEvil()
	{
		string evil = Lang.misc[101].Value;
		if (WorldGen.crimson)
		{
			evil = Lang.misc[102].Value;
		}
		return evil;
	}

	public string GetWorldSeed()
	{
		if (DisableAsRemoteClientIfNoSeed())
		{
			return ULang.UIVal("SeedUnavailable");
		}
		return Main.ActiveWorldFileData.Seed.ToString();
	}

	public string GetHardmode()
	{
		return YesOrNo(Main.hardMode);
	}

	public string GetWorldID()
	{
		return Main.worldID.ToString();
	}

	public string GetWorldGUID()
	{
		return Main.ActiveWorldFileData.UniqueId.ToString();
	}

	public string GetWorldGeneratorVersion()
	{
		return Main.ActiveWorldFileData.WorldGeneratorVersion.ToString();
	}

	public string GetHardmodeDesc()
	{
		return ULang.FormatWithNPC("UtilClientUI.HardmodeDescription", 113);
	}

	public string NpcName(int npcId)
	{
		return Lang.GetNPCNameValue(npcId);
	}

	public string Defeated(bool value)
	{
		if (!value)
		{
			return ULang.UIVal("NotDefeated");
		}
		return ULang.UIVal("Defeated");
	}

	public string YesOrNo(bool value)
	{
		if (!value)
		{
			return Lang.menu[105].Value;
		}
		return Lang.menu[104].Value;
	}

	public string EvilBoss()
	{
		if (!WorldGen.crimson)
		{
			return NpcName(13);
		}
		return NpcName(266);
	}

	public string TheTwins()
	{
		return Language.GetTextValue("Enemies.TheTwins");
	}

	public string Invasion(string key)
	{
		return Language.GetTextValue(key);
	}

	public string GetDD2Tier(int tier = 1)
	{
		return Language.GetTextValue("UtilClientUI.DD2EventTier", tier);
	}
}
