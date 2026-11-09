using System;
using Terraria.IO;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class ToggleCheat : Cheat, IGenCommand
{
	private bool _enabled;

	public bool Enabled
	{
		get
		{
			if (_enabled)
			{
				return !ManagerExtensions.get_Instance<CheatManager>().PanicMode;
			}
			return false;
		}
	}

	protected virtual bool PanicModeAffected => true;

	public virtual bool SaveEnabled => true;

	public virtual string? CommandOverride => null;

	public bool GetCanBeEnabled()
	{
		if (!Restrictions.TryMeetAll())
		{
			return SaveEnabled;
		}
		return true;
	}

	protected override void LoadFromPrefsCustom(Preferences prefs)
	{
		if (SaveEnabled)
		{
			_enabled = prefs.Get(base.PrefsName, defaultValue: false);
		}
	}

	protected override void SaveToPrefsCustom(Preferences prefs)
	{
		if (SaveEnabled)
		{
			prefs.Put(base.PrefsName, _enabled);
		}
	}

	protected sealed override void OnWorldLeaveCustom()
	{
		if (!SaveEnabled && Enabled)
		{
			Toggle();
		}
	}

	public void Toggle()
	{
		SetEnabled(!_enabled);
	}

	public void HandlePanicMode(bool active)
	{
		if (PanicModeAffected)
		{
			if (active && _enabled)
			{
				OnDisabled();
			}
			if (!active && _enabled)
			{
				OnEnabled();
			}
		}
	}

	private void HandleToggled()
	{
		if (!PanicModeAffected || !ManagerExtensions.get_Instance<CheatManager>().PanicMode)
		{
			if (_enabled)
			{
				OnEnabled();
			}
			else
			{
				OnDisabled();
			}
		}
	}

	internal void SetEnabled(bool value)
	{
		if (_enabled == value)
		{
			return;
		}
		if (value && !Restrictions.TryMeetAll())
		{
			if (!SaveEnabled)
			{
				throw new InvalidOperationException("Can't enable '" + Name + "' toggle cheat without meeting requirements");
			}
			ChatLogging.LogInfo("Warning: enabling '" + Name + "' toggle cheat without meeting requirements.");
		}
		_enabled = value;
		HandleToggled();
	}

	protected virtual void OnEnabled()
	{
	}

	protected virtual void OnDisabled()
	{
	}
}
public abstract class ToggleCheat<TSettings> : ToggleCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
