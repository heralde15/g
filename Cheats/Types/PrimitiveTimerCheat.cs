using System;
using Terraria.UtilClient.Cheats.Common;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class PrimitiveTimerCheat : Cheat, IUpdateCheat, IDisposable
{
	private readonly UpdateRateState _rate;

	private readonly TimerScheduler _scheduler;

	protected abstract uint DefaultUpdateRate { get; }

	public uint UpdateRate => _rate.Value;

	public virtual bool RateConfigurable => false;

	public UpdateRateUnit UpdateRateUnit => UpdateRateUnit.Milliseconds;

	public uint TickCount { get; private set; }

	protected PrimitiveTimerCheat()
	{
		_rate = new UpdateRateState(DefaultUpdateRate, UpdateRateUnit);
		_scheduler = new TimerScheduler(_rate);
		_scheduler.Start(UpdateInternal);
	}

	private void UpdateInternal()
	{
		if (Main.gameMenu || Main.netMode == 2 || !Restrictions.TryMeetAll())
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
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error during primitive timer cheat '" + Name + "' tick"), ex);
		}
	}

	public void SetUpdateRate(uint rate)
	{
		_rate.Set(rate);
		_scheduler.UpdateRate();
	}

	protected abstract void OnUpdate();

	public void Dispose()
	{
		_scheduler.Dispose();
		GC.SuppressFinalize(this);
	}
}
public abstract class PrimitiveTimerCheat<TSettings> : PrimitiveTimerCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
