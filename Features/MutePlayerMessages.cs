namespace Terraria.UtilClient.Features;

public static class MutePlayerMessages
{
	private static bool[] players = new bool[256];

	private static void Reset(int index)
	{
		players[index] = false;
	}

	public static bool IsMuted(int playerId)
	{
		if (playerId < 0 || playerId >= 255)
		{
			return false;
		}
		if (!Main.player[playerId].active)
		{
			return false;
		}
		return players[playerId];
	}

	public static void Toggle(int playerId)
	{
		if (playerId >= 0 && playerId < 255 && Main.player[playerId].active)
		{
			players[playerId] = !players[playerId];
		}
	}

	public static bool HandleMessage(int playerId)
	{
		if (!IsMuted(playerId))
		{
			return false;
		}
		_ = Main.player[playerId];
		return true;
	}

	internal static void Tick()
	{
		for (int i = 0; i < 255; i++)
		{
			Player player = Main.player[i];
			if (!player.active || player.whoAmI == Main.myPlayer)
			{
				Reset(i);
			}
		}
	}
}
