using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Gamepad;
using Terraria.UtilClient.Config;
using Terraria.UtilClient.Config.Types;
using Terraria.UtilClient.Config.Types.Structures;
using Terraria.UtilClient.Features;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.UI.Elements;

namespace Terraria.UtilClient.UI.States;

public class UIServerList : UIState, IHaveBackButtonCommand
{
	private UIList _worldList;

	private UITextPanel<LocalizedText> _backPanel;

	private UITextPanel<LocalizedText> _newPanel;

	private UIPanel _containerPanel;

	private UIScrollbar _scrollbar;

	private bool _isScrollbarAttached;

	private List<Tuple<string, bool>> favoritesCache = new List<Tuple<string, bool>>();

	private bool skipDraw;

	private List<UIServerListItem> _savedServers = new List<UIServerListItem>();

	private List<UIServerListItem> _lanServers = new List<UIServerListItem>();

	private List<SavedServerData> _previousSavedData = new List<SavedServerData>();

	public override void OnInitialize()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		UIElement uIElement = new UIElement();
		uIElement.Width.Set(0f, 0.8f);
		uIElement.MaxWidth.Set(650f, 0f);
		uIElement.Top.Set(220f, 0f);
		uIElement.Height.Set(-220f, 1f);
		uIElement.HAlign = 0.5f;
		UIPanel uIPanel = new UIPanel();
		uIPanel.Width.Set(0f, 1f);
		uIPanel.Height.Set(-110f, 1f);
		uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
		uIElement.Append(uIPanel);
		_containerPanel = uIPanel;
		_worldList = new UIList();
		_worldList.Width.Set(0f, 1f);
		_worldList.Height.Set(0f, 1f);
		_worldList.ListPadding = 5f;
		uIPanel.Append(_worldList);
		_scrollbar = new UIScrollbar();
		_scrollbar.SetView(100f, 1000f);
		_scrollbar.Height.Set(0f, 1f);
		_scrollbar.HAlign = 1f;
		_worldList.SetScrollbar(_scrollbar);
		UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(ULang.UI("Servers"), 0.8f, large: true);
		uITextPanel.HAlign = 0.5f;
		uITextPanel.Top.Set(-40f, 0f);
		uITextPanel.SetPadding(15f);
		uITextPanel.BackgroundColor = new Color(73, 94, 171);
		uIElement.Append(uITextPanel);
		UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, large: true);
		uITextPanel2.Width.Set(-10f, 0.5f);
		uITextPanel2.Height.Set(50f, 0f);
		uITextPanel2.VAlign = 1f;
		uITextPanel2.HAlign = 0f;
		uITextPanel2.Top.Set(-45f, 0f);
		uITextPanel2.OnMouseOver += FadedMouseOver;
		uITextPanel2.OnMouseOut += FadedMouseOut;
		uITextPanel2.OnLeftClick += GoBackClick;
		uIElement.Append(uITextPanel2);
		_backPanel = uITextPanel2;
		UITextPanel<LocalizedText> uITextPanel3 = new UITextPanel<LocalizedText>(ULang.UI("AddServer"), 0.7f, large: true);
		uITextPanel3.CopyStyle(uITextPanel2);
		uITextPanel3.HAlign = 1f;
		uITextPanel3.OnMouseOver += FadedMouseOver;
		uITextPanel3.OnMouseOut += FadedMouseOut;
		uITextPanel3.OnLeftClick += AddServerClick;
		uIElement.Append(uITextPanel3);
		_newPanel = uITextPanel3;
		Append(uIElement);
	}

	public void HandleBackButtonUsage()
	{
		Close();
	}

	private void Close()
	{
		SoundEngine.PlaySound(11);
		Main.OpenCharacterSelectUI();
	}

	public override void Recalculate()
	{
		if (_scrollbar != null)
		{
			if (_isScrollbarAttached && !_scrollbar.CanScroll)
			{
				_containerPanel.RemoveChild(_scrollbar);
				_isScrollbarAttached = false;
				_worldList.Width.Set(0f, 1f);
			}
			else if (!_isScrollbarAttached && _scrollbar.CanScroll)
			{
				_containerPanel.Append(_scrollbar);
				_isScrollbarAttached = true;
				_worldList.Width.Set(-25f, 1f);
			}
		}
		base.Recalculate();
	}

	private void AddServerClick(UIMouseEvent evt, UIElement listeningElement)
	{
		ServerAutoFinder.StopSearching();
		SoundEngine.PlaySound(10);
		Main.menuMode = 13;
	}

	private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
	{
		Close();
	}

	private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		SoundEngine.PlaySound(12);
		((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
	}

	private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
		((UIPanel)evt.Target).BorderColor = Color.Black;
	}

	public override void OnActivate()
	{
		ServerAutoFinder.StartSearching();
		UConfigExtensions.get_Current<SavedServersConfig>().Load();
		UpdateServerList();
		if (PlayerInput.UsingGamepadUI)
		{
			UILinkPointNavigator.ChangePoint(3000 + ((_worldList.Count == 0) ? 1 : 2));
		}
	}

	public override void Update(GameTime gameTime)
	{
		ServerAutoFinder.UpdateResults();
		UpdateServersList_AutoFinder();
		Recalculate();
		base.Update(gameTime);
	}

	private bool LanServersUpdated(List<SavedServerData> lanServers)
	{
		if (_previousSavedData.Count != lanServers.Count)
		{
			return true;
		}
		return lanServers.Where((SavedServerData data, int i) => !data.ServerEndpoint.Host.Equals(_previousSavedData[i].ServerEndpoint.Host)).Any();
	}

	private void UpdateServersList_AutoFinder()
	{
		List<SavedServerData> lanServers = ServerAutoFinder.ResultsToData;
		if (_previousSavedData.Count == 0 || LanServersUpdated(lanServers))
		{
			List<UIServerListItem> lanElements = new List<UIServerListItem>();
			for (int i = 0; i < lanServers.Count; i++)
			{
				lanElements.Add(new UIServerListItem(lanServers[i], i));
			}
			_previousSavedData = lanServers;
			_lanServers = lanElements;
		}
		_worldList.Clear();
		UIList newList = new UIList();
		int num = 0;
		foreach (UIServerListItem lanServer in _lanServers)
		{
			lanServer.SetOrderInList(num++);
			newList.Add(lanServer);
		}
		foreach (UIServerListItem savedServer in _savedServers)
		{
			savedServer.SetOrderInList(num++);
			newList.Add(savedServer);
		}
		int j = 0;
		foreach (UIServerListItem item in newList)
		{
			_ = item;
			_worldList.Add(newList.ElementAt(j++));
		}
	}

	private void UpdateServerList()
	{
		_savedServers.Clear();
		_lanServers.Clear();
		_previousSavedData.Clear();
		_worldList.Clear();
		IOrderedEnumerable<SavedServerData> orderedEnumerable = UConfigExtensions.get_Current<SavedServersConfig>().SavedServers.OrderBy((SavedServerData x) => x, SavedServersConfig.SortComparer);
		int num = 0;
		foreach (SavedServerData item in orderedEnumerable)
		{
			UIServerListItem serverItem = new UIServerListItem(item, num++);
			_savedServers.Add(serverItem);
			_worldList.Add(serverItem);
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		if (skipDraw)
		{
			skipDraw = false;
			return;
		}
		if (UpdateFavoritesCache())
		{
			skipDraw = true;
			Main.MenuUI.Draw(spriteBatch, new GameTime());
		}
		base.Draw(spriteBatch);
		SetupGamepadPoints(spriteBatch);
	}

	private bool UpdateFavoritesCache()
	{
		List<SavedServerData> list = new List<SavedServerData>(UConfigExtensions.get_Current<SavedServersConfig>().SavedServers);
		list.Sort(delegate(SavedServerData x, SavedServerData y)
		{
			if (x.IsFavorite && !y.IsFavorite)
			{
				return -1;
			}
			if (!x.IsFavorite && y.IsFavorite)
			{
				return 1;
			}
			int num = x.ServerEndpoint.Host.AsSpan().CompareTo(y.ServerEndpoint.Host.AsSpan(), StringComparison.OrdinalIgnoreCase);
			return (num == 0) ? string.Compare(x.ServerName, y.ServerName, StringComparison.Ordinal) : num;
		});
		bool flag = false;
		if (!flag && list.Count != favoritesCache.Count)
		{
			flag = true;
		}
		if (!flag)
		{
			for (int i = 0; i < favoritesCache.Count; i++)
			{
				Tuple<string, bool> tuple = favoritesCache[i];
				if (!(list[i].ServerName == tuple.Item1) || list[i].IsFavorite != tuple.Item2)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			favoritesCache.Clear();
			foreach (SavedServerData item in list)
			{
				favoritesCache.Add(Tuple.Create(item.ServerName, item.IsFavorite));
			}
			UpdateServerList();
		}
		return flag;
	}

	private void SetupGamepadPoints(SpriteBatch spriteBatch)
	{
		try
		{
			UILinkPointNavigator.Shortcuts.BackButtonCommand = 7;
			int num = 3000;
			UILinkPointNavigator.SetPosition(num, NumericsExtensions[_backPanel.GetInnerDimensions().ToRectangle()].ToVector2());
			UILinkPointNavigator.SetPosition(num + 1, NumericsExtensions[_newPanel.GetInnerDimensions().ToRectangle()].ToVector2());
			int num2 = num;
			UILinkPoint uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Right = num2 + 1;
			num2 = num + 1;
			uILinkPoint = UILinkPointNavigator.Points[num2];
			uILinkPoint.Unlink();
			uILinkPoint.Left = num2 - 1;
			float num3 = 1f / Main.UIScale;
			Rectangle clippingRectangle = _containerPanel.GetClippingRectangle(spriteBatch);
			Vector2 minimum = clippingRectangle.TopLeft() * num3;
			Vector2 maximum = clippingRectangle.BottomRight() * num3;
			List<SnapPoint> snapPoints = GetSnapPoints();
			for (int i = 0; i < snapPoints.Count; i++)
			{
				if (!snapPoints[i].Position.Between(minimum, maximum))
				{
					snapPoints.Remove(snapPoints[i]);
					i--;
				}
			}
			SnapPoint[,] array = new SnapPoint[_worldList.Count, 3];
			foreach (SnapPoint item in snapPoints.Where((SnapPoint a) => a.Name == "Play"))
			{
				array[item.Id, 0] = item;
			}
			foreach (SnapPoint item2 in snapPoints.Where((SnapPoint a) => a.Name == "Favorite"))
			{
				array[item2.Id, 1] = item2;
			}
			foreach (SnapPoint item3 in snapPoints.Where((SnapPoint a) => a.Name == "Delete"))
			{
				array[item3.Id, 2] = item3;
			}
			num2 = num + 2;
			int[] array2 = new int[_worldList.Count];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = -1;
			}
			for (int k = 0; k < array.GetLength(1); k++)
			{
				int num4 = -1;
				for (int l = 0; l < array.GetLength(0); l++)
				{
					if (array[l, k] != null)
					{
						uILinkPoint = UILinkPointNavigator.Points[num2];
						uILinkPoint.Unlink();
						UILinkPointNavigator.SetPosition(num2, array[l, k].Position);
						if (num4 != -1)
						{
							uILinkPoint.Up = num4;
							UILinkPointNavigator.Points[num4].Down = num2;
						}
						if (array2[l] != -1)
						{
							uILinkPoint.Left = array2[l];
							UILinkPointNavigator.Points[array2[l]].Right = num2;
						}
						uILinkPoint.Down = num;
						if (k == 0)
						{
							UILinkPointNavigator.Points[num].Up = (UILinkPointNavigator.Points[num + 1].Up = num2);
						}
						num4 = num2;
						array2[l] = num2;
						UILinkPointNavigator.Shortcuts.FANCYUI_HIGHEST_INDEX = num2;
						num2++;
					}
				}
			}
			if (PlayerInput.UsingGamepadUI && _worldList.Count == 0 && UILinkPointNavigator.CurrentPoint > 3001)
			{
				UILinkPointNavigator.ChangePoint(3001);
			}
		}
		catch
		{
		}
	}
}
