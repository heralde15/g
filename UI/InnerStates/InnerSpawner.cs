using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UtilClient.Common.Helpers.Game;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerSpawner : UIState
{
	private readonly record struct BossConfig(string Icon, float LeftOffset, float TopOffset, BossTier Tier);

	private readonly record struct InvasionConfig(string Icon, float LeftOffset, float TopOffset);

	private readonly record struct SpawnCondition(bool Can, string Reason);

	private enum BossTier
	{
		PreHardmode,
		Hardmode
	}

	private static readonly FrozenDictionary<SpawnHelper.BossID, BossConfig> BossToConfig = new Dictionary<SpawnHelper.BossID, BossConfig>
	{
		{
			SpawnHelper.BossID.KingSlime,
			new BossConfig("NPC_Head_Boss_7", 0f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.EyeOfCthulhu,
			new BossConfig("NPC_Head_Boss_0", 2f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.EaterOfWorlds,
			new BossConfig("NPC_Head_Boss_2", 3f, -1f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.BrainOfCthulhu,
			new BossConfig("NPC_Head_Boss_23", 0f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.QueenBee,
			new BossConfig("NPC_Head_Boss_14", 1f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.Skeletron,
			new BossConfig("NPC_Head_Boss_19", 2f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.Deerclops,
			new BossConfig("NPC_Head_Boss_39", -8f, -5f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.WallOfFlesh,
			new BossConfig("NPC_Head_Boss_22", 0f, 0f, BossTier.PreHardmode)
		},
		{
			SpawnHelper.BossID.QueenSlime,
			new BossConfig("NPC_Head_Boss_38", 1f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.TheTwins,
			new BossConfig("NPC_Head_Boss_15", 0f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.TheDestroyer,
			new BossConfig("NPC_Head_Boss_25", 2f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.SkeletronPrime,
			new BossConfig("NPC_Head_Boss_18", 1.5f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.Plantera,
			new BossConfig("NPC_Head_Boss_11", 0f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.Golem,
			new BossConfig("NPC_Head_Boss_5", 1f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.DukeFishron,
			new BossConfig("NPC_Head_Boss_4", 0f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.EmpressOfLight,
			new BossConfig("NPC_Head_Boss_37", 0f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.LunaticCultist,
			new BossConfig("NPC_Head_Boss_24", 1.5f, 0f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.MoonLord,
			new BossConfig("NPC_Head_Boss_8", -2f, -2f, BossTier.Hardmode)
		},
		{
			SpawnHelper.BossID.Mechdusa,
			new BossConfig("Item_5334", -3.5f, 2f, BossTier.Hardmode)
		}
	}.ToFrozenDictionary();

	private static readonly FrozenDictionary<SpawnHelper.InvasionID, InvasionConfig> InvasionToConfig = new Dictionary<SpawnHelper.InvasionID, InvasionConfig>
	{
		{
			SpawnHelper.InvasionID.GoblinArmy,
			new InvasionConfig("Extra_9", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.SnowLegion,
			new InvasionConfig("Extra_7", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.Pirates,
			new InvasionConfig("Extra_11", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.MartianMadness,
			new InvasionConfig("Extra_10", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.OldOnesArmy,
			new InvasionConfig("Extra_79", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.SolarEclipse,
			new InvasionConfig("UI/Bestiary/Icon_Tags_Shadow", -3f, -2f)
		},
		{
			SpawnHelper.InvasionID.BloodMoon,
			new InvasionConfig("UI/Bestiary/Icon_Tags_Shadow", -3f, -2.8f)
		},
		{
			SpawnHelper.InvasionID.SlimeRain,
			new InvasionConfig("UI/Bestiary/Icon_Tags_Shadow", -4f, -5f)
		},
		{
			SpawnHelper.InvasionID.PumpkinMoon,
			new InvasionConfig("Extra_12", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.FrostMoon,
			new InvasionConfig("Extra_8", 0f, 0f)
		},
		{
			SpawnHelper.InvasionID.LunarEvents,
			new InvasionConfig("UI/Bestiary/Icon_Tags_Shadow", -3f, -3f)
		}
	}.ToFrozenDictionary();

	private static readonly Dictionary<SpawnHelper.BossID, SpawnCondition> BossCachedConditions = new Dictionary<SpawnHelper.BossID, SpawnCondition>();

	private static readonly Dictionary<SpawnHelper.InvasionID, SpawnCondition> InvasionCachedConditions = new Dictionary<SpawnHelper.InvasionID, SpawnCondition>();

	private const float IconSlotWidth = 28f;

	private const float IconPaddingLeft = 4f;

	private static Rectangle GetEventFrame(SpawnHelper.InvasionID id, Texture2D tex)
	{
		return id switch
		{
			SpawnHelper.InvasionID.BloodMoon => tex.Frame(16, 5, 6, 2), 
			SpawnHelper.InvasionID.SlimeRain => tex.Frame(16, 5, 15, 2), 
			SpawnHelper.InvasionID.SolarEclipse => tex.Frame(16, 5, 7, 2), 
			SpawnHelper.InvasionID.LunarEvents => tex.Frame(16, 5, 8, 3), 
			_ => tex.Frame(), 
		};
	}

	public InnerSpawner()
	{
		UIElement uiElement = UIHelpers.CreateCommonBase();
		UIList uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = UIHelpers.CreateCommonScroller();
		uiList.SetScrollbar(uIScrollbar);
		uiElement.Append(uIScrollbar);
		uiList.Add(UIHelpers.CreateGroup(ULang.UIVal("PreHardmodeBosses"), CreateBossList(BossTier.PreHardmode)));
		uiList.Add(UIHelpers.CreateGroup(ULang.UIVal("HardmodeBosses"), CreateBossList(BossTier.Hardmode)));
		uiList.Add(UIHelpers.CreateGroup(ULang.UIVal("EventsInvasions"), CreateEventList()));
		uiElement.Append(uiList);
		Append(uiElement);
	}

	private static UIElement[] CreateBossList(BossTier tier)
	{
		List<UIElement> items = new List<UIElement>();
		Rectangle spawnAllTexture = Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Icon_Tags_Shadow", (AssetRequestMode)1).Value.Frame(16, 5, 15, 3);
		List<KeyValuePair<SpawnHelper.BossID, BossConfig>> tierBosses = BossToConfig.Where((KeyValuePair<SpawnHelper.BossID, BossConfig> p) => p.Value.Tier == tier).ToList();
		IEnumerable<SpawnHelper.BossID> tierBossIds = tierBosses.Select((KeyValuePair<SpawnHelper.BossID, BossConfig> p) => p.Key);
		items.Add(CreateSpawnElement((icon: "UI/Bestiary/Icon_Tags_Shadow", LeftOffset: 0f, TopOffset: 0f), () => ULang.UIVal("ClickToSummonBossAll"), null, delegate
		{
			SpawnAllBosses(tierBossIds);
		}, null, null, spawnAllTexture));
		foreach (var (id, config) in tierBosses)
		{
			items.Add(CreateBossSummonElement(id, config));
		}
		return items.ToArray();
	}

	private static UIElement[] CreateEventList()
	{
		List<UIElement> items = new List<UIElement>();
		foreach (var (id, config) in InvasionToConfig)
		{
			items.Add(CreateInvasionStartElement(id, config));
		}
		return items.ToArray();
	}

	private static UIElement CreateBossSummonElement(SpawnHelper.BossID id, BossConfig config)
	{
		return CreateSpawnElement((icon: config.Icon, LeftOffset: config.LeftOffset, TopOffset: config.TopOffset), () => SpawnHelper.GetBossName(id), delegate
		{
			if (!Main.DrawnThisUpdate && BossCachedConditions.TryGetValue(id, out var value))
			{
				return GetSummonText(value.Can);
			}
			string reason = "";
			bool can = Main.netMode != 1 || SpawnHelper.CanSummonBossMP(id, out reason);
			BossCachedConditions[id] = new SpawnCondition(can, reason);
			return GetSummonText(can);
		}, delegate
		{
			SpawnHelper.SpawnBoss(id);
		}, () => BossCachedConditions.TryGetValue(id, out var value) && !value.Can, () => BossCachedConditions.TryGetValue(id, out var value) ? value.Reason : "");
		static string GetSummonText(bool can)
		{
			if (!can)
			{
				return ULang.UIVal("CantSummonBoss");
			}
			return ULang.UIVal("ClickToSummonBoss");
		}
	}

	private static UIElement CreateInvasionStartElement(SpawnHelper.InvasionID id, InvasionConfig config)
	{
		SpawnHelper.InvasionID invasionID = id;
		bool flag = ((invasionID - 7 <= SpawnHelper.InvasionID.SnowLegion || invasionID == SpawnHelper.InvasionID.LunarEvents) ? true : false);
		float scale = (flag ? 1f : 0.75f);
		Asset<Texture2D> texture = Main.Assets.Request<Texture2D>("Images/" + config.Icon, (AssetRequestMode)1);
		Rectangle frame = GetEventFrame(id, texture.Value);
		return CreateSpawnElement((icon: config.Icon, LeftOffset: config.LeftOffset, TopOffset: config.TopOffset), () => SpawnHelper.GetInvasionName(id), delegate
		{
			if (!Main.DrawnThisUpdate && InvasionCachedConditions.TryGetValue(id, out var value))
			{
				return GetStartText(value.Can);
			}
			string reason = "";
			bool can = Main.netMode != 1 || SpawnHelper.CanStartEvent(id, out reason);
			InvasionCachedConditions[id] = new SpawnCondition(can, reason);
			return GetStartText(can);
		}, delegate
		{
			SpawnHelper.StartInvasion(id);
		}, () => InvasionCachedConditions.TryGetValue(id, out var value) && !value.Can, () => InvasionCachedConditions.TryGetValue(id, out var value) ? value.Reason : "", frame, scale);
		static string GetStartText(bool can)
		{
			if (!can)
			{
				return ULang.UIVal("CantStartEvent");
			}
			return ULang.UIVal("ClickToStartEvent");
		}
	}

	private static UIElement CreateSpawnElement((string icon, float LeftOffset, float TopOffset) entry, Func<string> name, Func<string> getValue, Action onClick, Func<bool>? disableCondition = null, Func<string> description = null, Rectangle? overrideFrame = null, float iconScale = 0.75f)
	{
		Asset<Texture2D> texture = Main.Assets.Request<Texture2D>("Images/" + entry.icon, (AssetRequestMode)1);
		Rectangle frame = overrideFrame ?? texture.Frame();
		UIImageFramed icon = new UIImageFramed(texture, frame);
		icon.Resize(iconScale);
		icon.Left.Set(4f + entry.LeftOffset, 0f);
		icon.Top.Set(entry.TopOffset, 0f);
		UISpawnDescriptiveItem item = new UISpawnDescriptiveItem(name, description, onClick, disableCondition, getValue);
		item.Left.Set(32f, 0f);
		item.Width.Set(-32f, 1f);
		item.Height.Set(26f, 0f);
		UIElement uIElement = new UIElement();
		uIElement.Height.Set(26f, 0f);
		uIElement.Width.Set(0f, 0.975f);
		uIElement.HAlign = 0.5f;
		uIElement.Append(icon);
		uIElement.Append(item);
		return uIElement;
	}

	private static void SpawnAllBosses(IEnumerable<SpawnHelper.BossID> bosses)
	{
		HashSet<SpawnHelper.BossID> ignoreInMechdua = new HashSet<SpawnHelper.BossID>
		{
			SpawnHelper.BossID.TheTwins,
			SpawnHelper.BossID.SkeletronPrime,
			SpawnHelper.BossID.TheDestroyer
		};
		foreach (SpawnHelper.BossID id in bosses)
		{
			if ((!SpecialSeedFeatures.Mechdusa || !ignoreInMechdua.Contains(id)) && (SpecialSeedFeatures.Mechdusa || id != SpawnHelper.BossID.Mechdusa) && (Main.netMode != 1 || SpawnHelper.CanSummonBossMP(id, out var _)))
			{
				SpawnHelper.SpawnBoss(id);
			}
		}
	}
}
