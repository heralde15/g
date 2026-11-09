using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class Cheat
{
	protected bool SettingsLoaded;

	public abstract string Name { get; }

	public string PrefsName => CheatManager.CleanCheatName(Name);

	public abstract CheatCategory Category { get; }

	public virtual IReadOnlyList<IRestriction> Restrictions { get; protected init; } = Array.Empty<IRestriction>();

	public virtual bool IncludeInUI => true;

	internal virtual USettings UntypedSettings => null;

	internal void LoadFromPrefs(Preferences prefs)
	{
		try
		{
			UntypedSettings?.Load(prefs, PrefsName);
			LoadFromPrefsCustom(prefs);
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Failed to load '" + Name + "' cheat settings"), ex);
			return;
		}
		if (SettingsLoaded)
		{
			try
			{
				OnReload();
			}
			catch (Exception ex2)
			{
				ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Error during '" + Name + "' cheat reload"), ex2);
			}
		}
		SettingsLoaded = true;
	}

	internal void SaveToPrefs(Preferences prefs)
	{
		try
		{
			UntypedSettings?.Save(prefs, PrefsName);
			SaveToPrefsCustom(prefs);
		}
		catch (Exception ex)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Error((object)("Failed to save '" + Name + "' cheat settings"), ex);
		}
	}

	protected virtual void LoadFromPrefsCustom(Preferences prefs)
	{
	}

	protected virtual void SaveToPrefsCustom(Preferences prefs)
	{
	}

	protected virtual void OnReload()
	{
	}

	internal void OnWorldLeave()
	{
		if (UntypedSettings is IPlayerSelectorSettings selectorSettings)
		{
			selectorSettings.SelectorUntyped.Reset();
		}
		OnWorldLeaveCustom();
	}

	protected virtual void OnWorldLeaveCustom()
	{
	}
}
public abstract class Cheat<TSettings> : Cheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
