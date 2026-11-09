using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.UI;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.InnerStates;

internal class InnerPlayerList : UIState
{
	private UIList _uiList;

	private List<UIElement> _players = new List<UIElement>();

	private List<UIElement> _displayedPlayers = new List<UIElement>();

	public InnerPlayerList()
	{
		UIElement uIElement = UIHelpers.CreateCommonBase();
		_uiList = UIHelpers.CreateCommonUIList();
		UIScrollbar uIScrollbar = new UIScrollbar();
		uIScrollbar.SetView(100f, 1000f);
		uIScrollbar.Height.Set(0f, 1f);
		uIScrollbar.HAlign = 1f;
		uIScrollbar.VAlign = 1f;
		_uiList.SetScrollbar(uIScrollbar);
		uIElement.Append(uIScrollbar);
		uIElement.Append(_uiList);
		CreatePlayerList();
		UpdatePlayerList();
		Append(uIElement);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		_uiList.Clear();
		UpdatePlayerList();
		foreach (UIElement element in _displayedPlayers)
		{
			_uiList.Add(element);
		}
		Recalculate();
		base.Draw(spriteBatch);
	}

	private void CreatePlayerList()
	{
		for (int i = 0; i < Main.player.Length; i++)
		{
			UISortableElement uISortableElement = new UISortableElement(i);
			uISortableElement.HAlign = 0.5f;
			uISortableElement.Width.Set(0f, 1f);
			uISortableElement.Height.Set(70f, 0f);
			if (i == 0)
			{
				uISortableElement.PaddingTop = 6f;
				uISortableElement.Height.Set(76f, 0f);
			}
			UIPlayerListItem listItem = new UIPlayerListItem(i);
			uISortableElement.Append(listItem);
			_players.Add(uISortableElement);
		}
		for (int j = 0; j < _players.Count; j++)
		{
			_displayedPlayers.Add(_players[j]);
		}
	}

	private void UpdatePlayerList()
	{
		_displayedPlayers.Clear();
		bool firstPlayer = true;
		for (int i = 0; i < Main.player.Length; i++)
		{
			Player player = Main.player[i];
			if ((player.active || !TweaksCollectionExtensions.get_Current<ZModTweaks>().PlayerListHideInactive) && !string.IsNullOrEmpty(player.name))
			{
				_displayedPlayers.Add(_players[i]);
				if (firstPlayer)
				{
					_players[i].PaddingTop = 6f;
					_players[i].Height.Set(76f, 0f);
					firstPlayer = false;
				}
				else
				{
					_players[i].PaddingTop = 0f;
					_players[i].Height.Set(70f, 0f);
				}
			}
		}
	}
}
