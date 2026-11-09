using System;
using System.Threading;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class RunActionCheat : Cheat, IGenCommand
{
	private int _running;

	public bool IsRunning
	{
		get
		{
			return Volatile.Read(in _running) == 1;
		}
		private set
		{
			Volatile.Write(ref _running, value ? 1 : 0);
		}
	}

	public virtual string? CommandOverride => null;

	protected void EnsureRunning(bool needed)
	{
		if (needed && !IsRunning)
		{
			throw new InvalidOperationException("Cheat '" + Name + "' is not running yet.");
		}
		if (!needed && IsRunning)
		{
			throw new InvalidOperationException("Cheat '" + Name + "' is already running.");
		}
	}

	public bool Run()
	{
		if (!Restrictions.TryMeetAll())
		{
			throw new InvalidOperationException("Can't run '" + Name + "' run action cheat without meeting requirements");
		}
		EnsureRunning(needed: false);
		IsRunning = true;
		RunCore();
		return true;
	}

	protected abstract void RunCore();

	protected void FinishRun()
	{
		IsRunning = false;
	}

	protected void HandleException(Exception ex)
	{
		ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error running cheat '" + Name + "'"), ex);
		ChatLogging.LogError($"Cheat '{Name}' failed:\n{ex}");
	}
}
public abstract class RunActionCheat<TSettings> : RunActionCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
