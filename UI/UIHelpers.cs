using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.UI;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI;

public static class UIHelpers
{
	public static UICollapsibleGroup CreateGroup(string nameKey, params UIElement[] children)
	{
		UICollapsibleGroup dropDown = new UICollapsibleGroup(nameKey);
		dropDown.Width.Set(0f, 1f);
		dropDown.Height.Set(30f, 0f);
		foreach (UIElement element in children)
		{
			dropDown.Add(element);
		}
		dropDown.OnInitialize();
		return dropDown;
	}

	public static UICollapsibleGroup CreateCompactGroup(string nameKey, params UIElement[] children)
	{
		UICollapsibleGroup dropdown = new UICollapsibleGroup(nameKey)
		{
			HAlign = 0f
		};
		dropdown.Width.Set(0f, 1f);
		dropdown.Height.Set(30f, 0f);
		for (int i = 0; i < children.Length; i++)
		{
			UISortableElement sortable = new UISortableElement(i);
			sortable.Width.Set(0f, 0.975f);
			sortable.Height.Set(30f, 0f);
			sortable.HAlign = 0.5f;
			dropdown.Add(sortable);
			UIElement first = children[i];
			if (first is UISimpleToggle)
			{
				first.Width.Set(6f, 0.4825f);
				sortable.Append(first);
				if (i + 1 < children.Length)
				{
					UIElement second = children[++i];
					second.Width.Set(6f, 0.4825f);
					second.HAlign = 1f;
					sortable.Append(second);
				}
				else
				{
					first.HAlign = (first.GetDimensions().X + 1f) / 2f;
				}
			}
			else
			{
				first.Width.Set(6f, 1f);
				first.HAlign = 0.5f;
				sortable.Append(first);
			}
		}
		dropdown.OnInitialize();
		return dropdown;
	}

	public static TMLHeaderElement CreateHeader(string key)
	{
		TMLHeaderElement tMLHeaderElement = new TMLHeaderElement(key, mouseInteract: true);
		tMLHeaderElement.Width.Set(0f, 1f);
		return tMLHeaderElement;
	}

	public static UIElement CreateCommonBase()
	{
		UIElement uIElement = new UIElement();
		uIElement.Width.Set(0f, 1f);
		uIElement.Height.Set(0f, 1f);
		uIElement.HAlign = 0.5f;
		return uIElement;
	}

	public static UIList CreateCommonUIList()
	{
		UIList uIList = new UIList();
		uIList.Width.Set(-25f, 1f);
		uIList.Height.Set(0f, 1f);
		uIList.VAlign = 1f;
		uIList.ListPadding = 4f;
		uIList.ManualSortMethod = NoSorting;
		return uIList;
	}

	public static UIScrollbar CreateCommonScroller()
	{
		UIScrollbar uIScrollbar = new UIScrollbar();
		uIScrollbar.SetView(1f, 10f);
		uIScrollbar.Height.Set(0f, 1f);
		uIScrollbar.HAlign = 1f;
		uIScrollbar.VAlign = 1f;
		return uIScrollbar;
	}

	public static void NoSorting(List<UIElement> list)
	{
	}
}
