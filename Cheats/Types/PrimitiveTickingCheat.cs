using System;
using Terraria.UtilClient.Cheats.Common;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class PrimitiveTickingCheat : Cheat, IUpdateCheat, IUpdateableCheat
{
	private readonly UpdateRateState _rate;

	private readonly TickScheduler _scheduler;

	protected abstract uint DefaultUpdateRate { get; }

	public uint UpdateRate => _rate.Value;

	public virtual bool RateConfigurable => false;

	public UpdateRateUnit UpdateRateUnit => UpdateRateUnit.Ticks;

	public virtual bool ShouldUpdate
	{
		get
		{
			if (!Main.gameMenu)
			{
				return Main.netMode != 2;
			}
			return false;
		}
	}

	protected PrimitiveTickingCheat()
	{
		_rate = new UpdateRateState(DefaultUpdateRate, UpdateRateUnit, RateConfigurable);
		_scheduler = new TickScheduler(_rate);
	}

	public void SetUpdateRate(uint rate)
	{
		_rate.Set(rate);
		_scheduler.Reset();
	}

	public void Update()
	{
		try
		{
			if (ShouldUpdate && Restrictions.TryMeetAll() && _scheduler.ShouldUpdate())
			{
				OnUpdate();
			}
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error during primitive ticking cheat '" + Name + "' tick"), ex);
		}
	}

	protected abstract void OnUpdate();
}
public abstract class PrimitiveTickingCheat<TSettings> : PrimitiveTickingCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
