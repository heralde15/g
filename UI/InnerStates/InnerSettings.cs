using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerSettings : UIState
{
	private readonly UIList _uiList;

	public InnerSettings()
	{
		UIElement uIElement = UIHelpers.CreateCommonBase();
		_uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = UIHelpers.CreateCommonScroller();
		_uiList.SetScrollbar(uIScrollbar);
		uIElement.Append(uIScrollbar);
		AddSettings();
		uIElement.Append(_uiList);
		Append(uIElement);
	}

	private void AddSettings()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Color color = new Color(73, 94, 171, 255) * 0.9f;
		Dictionary<TweaksCategory, List<UIElement>> categoryElements = new Dictionary<TweaksCategory, List<UIElement>>();
		TweaksCategory[] values = Enum.GetValues<TweaksCategory>();
		foreach (TweaksCategory tweaksCategory in values)
		{
			TweaksCollection tweaksCollection = ManagerExtensions.get_Instance<TweaksManager>().GetCategory(tweaksCategory);
			FieldInfo[] fields = tweaksCollection.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				if (!(field.FieldType != typeof(bool)))
				{
					if (!categoryElements.TryGetValue(tweaksCollection.Category, out var list))
					{
						list = (categoryElements[tweaksCollection.Category] = new List<UIElement>());
					}
					list.Add(CreateToggle(field, tweaksCollection.Category, color));
				}
			}
		}
		foreach (var (category, elements) in categoryElements)
		{
			string name = ULang.UIVal($"{category}Options");
			_uiList.Add(UIHelpers.CreateGroup(name, elements.ToArray()));
		}
	}

	private static UISimpleToggle CreateToggle(FieldInfo field, TweaksCategory tweaksCategory, Color color)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		UISimpleToggle uISimpleToggle = new UISimpleToggle("UtilClientOption." + field.Name, color, () => (bool)field.GetValue(ManagerExtensions.get_Instance<TweaksManager>().GetCategory(tweaksCategory)), delegate(bool state)
		{
			field.SetValue(ManagerExtensions.get_Instance<TweaksManager>().GetCategory(tweaksCategory), state);
		}, null, "UtilClientOptionDescription." + field.Name);
		ChatManager.GetStringSize(FontAssets.ItemStack.Value, field.Name, new Vector2(0.8f));
		uISimpleToggle.HAlign = 0.5f;
		uISimpleToggle.Height.Set(30f, 0f);
		uISimpleToggle.Width.Set(0f, 0.975f);
		uISimpleToggle.SetPadding(6f);
		return uISimpleToggle;
	}

	public bool DisableAsRemoteClient()
	{
		return Main.netMode != 1;
	}
}
