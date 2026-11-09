using System;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Common;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class TickingCheat : ToggleCheat, IUpdateCheat, IUpdateableCheat
{
	private readonly UpdateRateState _rate;

	private readonly TickScheduler _scheduler;

	protected abstract uint DefaultUpdateRate { get; }

	public uint UpdateRate => _rate.Value;

	public virtual bool RateConfigurable => false;

	public UpdateRateUnit UpdateRateUnit => UpdateRateUnit.Ticks;

	public virtual bool ShouldUpdate => base.Enabled;

	protected TickingCheat()
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
		if (!ShouldUpdate || !_scheduler.ShouldUpdate())
		{
			return;
		}
		try
		{
			OnUpdate();
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error during cheat '" + Name + "' tick"), ex);
			SetEnabled(value: false);
			ChatLogging.LogError($"Cheat '{Name}' was disabled due to an error:\n{ex}");
		}
	}

	protected override void OnEnabled()
	{
		_scheduler.Reset();
		OnEnabledCustom();
	}

	protected virtual void OnEnabledCustom()
	{
	}

	protected abstract void OnUpdate();
}
public abstract class TickingCheat<TSettings> : TickingCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
