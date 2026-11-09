using System;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class SyncRunActionCheat : RunActionCheat
{
	protected abstract void RunAction();

	protected override void RunCore()
	{
		try
		{
			RunAction();
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error running cheat '" + Name + "'"), ex);
			ChatLogging.LogError($"Cheat '{Name}' failed:\n{ex}");
		}
		finally
		{
			FinishRun();
		}
	}
}
public abstract class SyncRunActionCheat<TSettings> : SyncRunActionCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
