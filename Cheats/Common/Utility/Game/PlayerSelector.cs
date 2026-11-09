using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Terraria.UtilClient.Common.Utility.Game;

public abstract class PlayerSelector
{
	private readonly HashSet<Player> _selectedPlayers = new HashSet<Player>(ReferenceEqualityComparer.Instance);

	[JsonIgnore]
	public bool IncludeAllPlayers { get; set; }

	[JsonProperty]
	public virtual bool AllIncludesSelf { get; set; }

	public bool IsPlayerSelected(int slot)
	{
		ThrowIfInvalidSlot(slot);
		if (IncludeAllPlayers && (AllIncludesSelf || slot != Main.myPlayer))
		{
			return true;
		}
		Player player = Main.player[slot];
		if (player.active)
		{
			return _selectedPlayers.Contains(player);
		}
		return false;
	}

	public IEnumerable<Player> GetSelectedPlayers()
	{
		if (IncludeAllPlayers)
		{
			Player[] player = Main.player;
			foreach (Player player2 in player)
			{
				if (player2.active && (!player2.IsLocal || AllIncludesSelf))
				{
					yield return player2;
				}
			}
			yield break;
		}
		Filter();
		foreach (Player selectedPlayer in _selectedPlayers)
		{
			yield return selectedPlayer;
		}
	}

	public bool TryAddPlayer(int slot)
	{
		ThrowIfInvalidSlot(slot);
		Player player = Main.player[slot];
		if (!player.active)
		{
			return false;
		}
		return _selectedPlayers.Add(player);
	}

	public bool TryRemovePlayer(int slot)
	{
		ThrowIfInvalidSlot(slot);
		Player player = Main.player[slot];
		if (!player.active)
		{
			return false;
		}
		return _selectedPlayers.Remove(player);
	}

	internal void Reset()
	{
		IncludeAllPlayers = false;
		_selectedPlayers.Clear();
	}

	private void Filter()
	{
		_selectedPlayers.RemoveWhere((Player player) => !player.active || !Main.player.Contains(player, null));
	}

	private static void ThrowIfInvalidSlot(int slot)
	{
		if (Main.player.IndexInRange(slot))
		{
			return;
		}
		throw new IndexOutOfRangeException($"Player slot {slot} is out of range ({Main.player.Length})");
	}
}
