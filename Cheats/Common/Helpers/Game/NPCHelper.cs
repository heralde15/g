using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.UtilClient.Cheats.Settings;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class NPCHelper
{
	extension(NPC npc)
	{
		public bool BuffExists(int buffType, int timeThreshold = 60)
		{
			if (buffType < 0 || buffType >= BuffID.Count)
			{
				throw new ArgumentOutOfRangeException("buffType");
			}
			ArgumentOutOfRangeException.ThrowIfNegative(timeThreshold, "timeThreshold");
			int buffIndex = npc.FindBuffIndex(buffType);
			if (buffIndex < 0)
			{
				return false;
			}
			return npc.buffTime[buffIndex] >= timeThreshold;
		}

		public bool MatchesFilter(NPCTypeFilter filter, bool allowInvincible = false, bool allowMultiPart = true)
		{
			if (Main.onlyShimmerOceanWorlds && npc.type == 680 && npc.GivenName.Equals("Slimefors", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (npc.realLife >= 0 && npc.realLife != npc.whoAmI)
			{
				if (!allowMultiPart)
				{
					return false;
				}
				npc = Main.npc[npc.realLife];
			}
			bool isBoss = npc.boss;
			bool isHostile = !npc.friendly;
			bool isTown = npc.townNPC || npc.isLikeATownNPC;
			bool isCritter = npc.friendly && npc.CountsAsACritter;
			bool isOther = !isBoss && !isHostile && !isTown && !isCritter;
			if (filter.HasFlag(NPCTypeFilter.Boss) & isBoss)
			{
				return true;
			}
			if (!allowInvincible && npc.dontTakeDamage)
			{
				return false;
			}
			if (filter.HasFlag(NPCTypeFilter.Hostile) & isHostile)
			{
				return true;
			}
			if (filter.HasFlag(NPCTypeFilter.Town) & isTown)
			{
				return true;
			}
			if (filter.HasFlag(NPCTypeFilter.Critter) & isCritter)
			{
				return true;
			}
			return filter.HasFlag(NPCTypeFilter.Other) & isOther;
		}

		public bool MatchesFilter(INPCFilterSettings settings)
		{
			return npc.MatchesFilter(settings.Filter, settings.IncludeInvincibleNPCs, settings.IncludeMultiPartNPCs);
		}

		public bool IsAlive(bool ignoreDummy = false)
		{
			if (npc.active && npc.life > 0)
			{
				if (!(!npc.immortal & ignoreDummy))
				{
					return npc.type != 488;
				}
				return true;
			}
			return false;
		}
	}

	[Flags]
	public enum NPCTypeFilter : byte
	{
		None = 0,
		Hostile = 1,
		Critter = 2,
		Town = 4,
		Boss = 8,
		Other = 0x10,
		All = Hostile | Critter | Town | Boss | Other
	}

	public static IEnumerable<NPC> GetAllExisting(Func<NPC, bool>? predicate = null, bool mustBeAlive = true)
	{
		bool hasPredicate = predicate != null;
		NPC[] npc = Main.npc;
		foreach (NPC npc2 in npc)
		{
			if (npc2.active && !((npc2.life <= 0) & mustBeAlive) && (!hasPredicate || predicate(npc2)))
			{
				yield return npc2;
			}
		}
	}
}
