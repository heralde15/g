using System;
using Terraria.ID;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class PlayerHelper
{
	extension(Player p)
	{
		public void ImmediatelyRegen(bool broadcast = false)
		{
			if (p.statLife != p.statLifeMax2)
			{
				p.statLife = p.statLifeMax2;
				if (broadcast)
				{
					NetHelper.SyncLocalPlayerHealth();
				}
			}
		}

		public bool BuffExists(int buffType, int timeThreshold = 60)
		{
			if (buffType < 0 || buffType >= BuffID.Count)
			{
				throw new ArgumentOutOfRangeException("buffType");
			}
			int buffIndex = p.FindBuffIndex(buffType);
			if (buffIndex < 0)
			{
				return false;
			}
			int time = p.buffTime[buffIndex];
			if (time > 0)
			{
				return time <= timeThreshold;
			}
			return false;
		}
	}
}
