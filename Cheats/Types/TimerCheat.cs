using System;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Common;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class TimerCheat : ToggleCheat, IUpdateCheat, IDisposable
{
	private readonly UpdateRateState _rate;

	private readonly TimerScheduler _scheduler;

	protected abstract uint DefaultUpdateRate { get; }

	public uint UpdateRate => _rate.Value;

	public virtual bool RateConfigurable => false;

	public UpdateRateUnit UpdateRateUnit => UpdateRateUnit.Milliseconds;

	public uint TickCount { get; private set; }

	protected TimerCheat()
	{
		_rate = new UpdateRateState(DefaultUpdateRate, UpdateRateUnit, RateConfigurable);
		_scheduler = new TimerScheduler(_rate);
	}

	protected override void OnEnabled()
	{
		TickCount = 0u;
		_scheduler.Start(UpdateInternal);
		OnEnabledCustom();
	}

	protected override void OnDisabled()
	{
		_scheduler.Stop();
	}

	protected virtual void OnEnabledCustom()
	{
	}

	private void UpdateInternal()
	{
		if (!base.Enabled || Main.gameMenu)
		{
			return;
		}
		try
		{
			OnUpdate();
			TickCount++;
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error during cheat '" + Name + "' tick"), ex);
			SetEnabled(value: false);
			ChatLogging.LogError("Cheat '" + Name + "' was disabled due to an error.");
		}
	}

	public void SetUpdateRate(uint rate)
	{
		_rate.Set(rate);
		_scheduler.UpdateRate();
		TickCount = 0u;
	}

	protected abstract void OnUpdate();

	public void Dispose()
	{
		_scheduler.Dispose();
		GC.SuppressFinalize(this);
	}
}
public abstract class TimerCheat<TSettings> : TimerCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
