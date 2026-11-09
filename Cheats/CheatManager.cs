using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria.UtilClient.Cheats.Types;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Managing.Attributes;

namespace Terraria.UtilClient.Cheats;

[ClientOnlyManager]
[ManagerDependsOn(new Type[] { typeof(LoggingManager) })]
public sealed class CheatManager : Manager
{
	private static readonly char[] IllegalForCheatName = new char[2] { '-', '_' };

	private readonly FrozenDictionary<string, Cheat> _cheatNameLookup;

	private readonly ImmutableArray<Cheat> _allCheats;

	[CompilerGenerated]
	private bool _003CPanicMode_003Ek__BackingField;

	public IReadOnlyList<Cheat> AllCheats => _allCheats;

	public bool PanicMode
	{
		[CompilerGenerated]
		get
		{
			return _003CPanicMode_003Ek__BackingField;
		}
		set
		{
			if (_003CPanicMode_003Ek__BackingField == value)
			{
				return;
			}
			_003CPanicMode_003Ek__BackingField = value;
			if (Main.gameMenu)
			{
				return;
			}
			foreach (Cheat allCheat in _allCheats)
			{
				if (allCheat is ToggleCheat toggleCheat)
				{
					toggleCheat.HandlePanicMode(value);
				}
			}
		}
	}

	public static string CleanCheatName(string name)
	{
		return new string(name.Where((char c) => !IllegalForCheatName.Contains(c) && !char.IsWhiteSpace(c)).ToArray());
	}

	public CheatManager()
	{
		Dictionary<string, Cheat> dict = new Dictionary<string, Cheat>(StringComparer.OrdinalIgnoreCase);
		ImmutableArray<Cheat>.Builder cheatsOrdered = ImmutableArray.CreateBuilder<Cheat>();
		foreach (Cheat cheat in BuiltInCheats.GetAll())
		{
			string cleaned = CleanCheatName(cheat.Name);
			if (dict.TryAdd(cleaned, cheat))
			{
				cheatsOrdered.Add(cheat);
			}
			else
			{
				ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Duplicate cheat name '" + cleaned + "'"));
			}
		}
		_cheatNameLookup = dict.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
		_allCheats = cheatsOrdered.ToImmutable();
	}

	public void TickAllCheats()
	{
		foreach (Cheat allCheat in _allCheats)
		{
			if (allCheat is IUpdateableCheat { ShouldUpdate: not false } updateable)
			{
				updateable.Update();
			}
		}
	}

	public void OnWorldLeave()
	{
		if (!Main.gameMenu)
		{
			throw new InvalidOperationException("In a world");
		}
		foreach (Cheat allCheat in _allCheats)
		{
			allCheat.OnWorldLeave();
		}
		ManagerExtensions.get_Instance<CheatManager>().PanicMode = false;
	}

	public UpdateRates GetUpdateRates()
	{
		UpdateRates result = new UpdateRates();
		foreach (IUpdateCheat cheat in from c in AllCheats.OfType<IUpdateCheat>()
			where c.RateConfigurable
			select c)
		{
			string name = CleanCheatName(((Cheat)cheat).Name);
			(cheat.UpdateRateUnit switch
			{
				UpdateRateUnit.Ticks => result.Ticks, 
				UpdateRateUnit.Milliseconds => result.Milliseconds, 
				_ => throw new ArgumentOutOfRangeException(), 
			})[name] = cheat.UpdateRate;
		}
		return result;
	}

	public void ApplyUpdateRates(UpdateRates rates)
	{
		Apply(rates.Ticks, UpdateRateUnit.Ticks);
		Apply(rates.Milliseconds, UpdateRateUnit.Milliseconds);
	}

	private void Apply(Dictionary<string, uint> rates, UpdateRateUnit expectedUnit)
	{
		foreach (var (name, rate) in rates)
		{
			if (_cheatNameLookup.TryGetValue(name, out var cheat) && cheat is IUpdateCheat { RateConfigurable: not false } updateRateCheat && updateRateCheat.UpdateRateUnit == expectedUnit)
			{
				updateRateCheat.SetUpdateRate(rate);
			}
		}
	}
}
