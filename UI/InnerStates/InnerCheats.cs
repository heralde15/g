using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Cheats.Types;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Restrictions;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerCheats : UIState
{
	private sealed class UICheatToggle : UISimpleToggle
	{
		private static string GetToggleDisabledText(ToggleCheat toggleCheat)
		{
			if (toggleCheat.Restrictions.TryMeetAllFormatted(out string text))
			{
				throw new UnreachableException();
			}
			if (!toggleCheat.SaveEnabled)
			{
				return text;
			}
			return ULang.RestrictionVal("Restrictions", text);
		}

		internal UICheatToggle(ToggleCheat toggleCheat, Color color)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector(toggleCheat.Name, color, () => toggleCheat.Enabled, toggleCheat.SetEnabled, () => !toggleCheat.Restrictions.TryMeetAll(), "UtilClientCheatDesc." + toggleCheat.GetType().Name, () => GetToggleDisabledText(toggleCheat));
			Height.Set(30f, 0f);
			SetPadding(6f);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			DrawDisabledIndicator(spriteBatch, GetDimensions());
		}
	}

	private sealed class UICheatRunButton : UISimpleButton
	{
		private static void RunClick(RunActionCheat runActionCheat)
		{
			SoundEngine.PlaySound(runActionCheat.IsRunning ? 10 : 11, -1, -1, 0);
		}

		private static string GetRunDisabledText(RunActionCheat runActionCheat)
		{
			if (!runActionCheat.Restrictions.TryMeetAllFormatted(out string text))
			{
				return text;
			}
			if (runActionCheat is SyncRunActionCheat && runActionCheat.IsRunning)
			{
				return "";
			}
			throw new UnreachableException();
		}

		internal UICheatRunButton(RunActionCheat runActionCheat, Color color)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector(runActionCheat.Name, color, delegate
			{
				try
				{
					if (!runActionCheat.IsRunning)
					{
						runActionCheat.Run();
					}
					else if (runActionCheat is AsyncRunActionCheat asyncRunActionCheat)
					{
						asyncRunActionCheat.Stop();
					}
				}
				catch (Exception ex)
				{
					ManagerExtensions.get_Instance<LoggingManager>().zMod.Info((object)"Cheat Run Button ex", ex);
				}
			}, () => !runActionCheat.Restrictions.TryMeetAll() || (runActionCheat is SyncRunActionCheat && runActionCheat.IsRunning), "UtilClientCheatDesc." + runActionCheat.GetType().Name, () => GetRunDisabledText(runActionCheat));
			HAlign = 0.5f;
			Height.Set(30f, 0f);
			SetPadding(6f);
			base.OnClick += delegate
			{
				RunClick(runActionCheat);
			};
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			DrawDisabledIndicator(spriteBatch, GetDimensions());
		}
	}

	private sealed class UIPlayerSelectorCheat : UISimpleToggle
	{
		private static string GetDisabledText(Cheat cheat)
		{
			if (cheat.Restrictions.TryMeetAllFormatted(out string text))
			{
				throw new UnreachableException();
			}
			return text;
		}

		public UIPlayerSelectorCheat(Cheat cheat, IPlayerSelectorSettings settings, Color color)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector(cheat.Name, color, () => settings.SelectorUntyped.IncludeAllPlayers, delegate(bool value)
			{
				settings.SelectorUntyped.IncludeAllPlayers = value;
			}, () => !cheat.Restrictions.TryMeetAll(), "UtilClientCheatDesc." + cheat.GetType().Name, () => GetDisabledText(cheat));
			Height.Set(30f, 0f);
			SetPadding(6f);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			DrawDisabledIndicator(spriteBatch, GetDimensions());
		}
	}

	private readonly UIList _uiList;

	public InnerCheats()
	{
		UIElement uIElement = UIHelpers.CreateCommonBase();
		_uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = UIHelpers.CreateCommonScroller();
		_uiList.SetScrollbar(uIScrollbar);
		uIElement.Append(uIScrollbar);
		AddCheats();
		uIElement.Append(_uiList);
		Append(uIElement);
	}

	private void AddCheats()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Color color = new Color(73, 94, 171, 255) * 0.9f;
		Dictionary<CheatCategory, List<UIElement>> categoryElements = new Dictionary<CheatCategory, List<UIElement>>();
		foreach (Cheat cheat in ManagerExtensions.get_Instance<CheatManager>().AllCheats)
		{
			if (cheat.IncludeInUI)
			{
				if (!categoryElements.TryGetValue(cheat.Category, out var list))
				{
					list = (categoryElements[cheat.Category] = new List<UIElement>());
				}
				UIElement uIElement = ((cheat is ToggleCheat toggleCheat) ? new UICheatToggle(toggleCheat, color) : ((cheat is RunActionCheat runActionCheat) ? new UICheatRunButton(runActionCheat, color) : ((cheat == null || !(cheat.UntypedSettings is IPlayerSelectorSettings settings) || !(cheat is IUpdateCheat)) ? HandleUnsupportedCheat(cheat) : new UIPlayerSelectorCheat(cheat, settings, color))));
				UIElement element = uIElement;
				if (element != null)
				{
					list.Add(element);
				}
			}
		}
		foreach (var (category, elements) in categoryElements.OrderBy((KeyValuePair<CheatCategory, List<UIElement>> kvp) => (int)kvp.Key))
		{
			string name = ULang.UIVal($"{category}Cheats");
			_uiList.Add(UIHelpers.CreateCompactGroup(name, elements.ToArray()));
		}
		static UIElement? HandleUnsupportedCheat(Cheat cheat2)
		{
			Type type = cheat2.GetType();
			string typeName = type.BaseType?.Name ?? type.Name;
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Warn((object)$"Cheat UI does not support the cheat '{cheat2.Name}': {typeName}. Skipping");
			return null;
		}
	}

	public bool DisableAsRemoteClient()
	{
		return Main.netMode != 1;
	}
}
