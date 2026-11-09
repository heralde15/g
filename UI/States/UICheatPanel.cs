using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UtilClient.UI.Elements;
using Terraria.UtilClient.UI.InnerStates;

namespace Terraria.UtilClient.UI.States;

public class UICheatPanel : UIState
{
	private const int MaxButtons = 8;

	private UIElement _outerContainer;

	private UIElement _buttonContainer;

	private UIPanel _mainPanel;

	private UIElement _innerWrapper;

	private UIState _innerState;

	private UIState _defaultState;

	private UISimpleButton[] _buttons;

	private UIState[] _tabs;

	private string[] _buttonKeys = new string[8] { "UtilClientUI.PlayersTab", "UtilClientUI.WorldInfoTab", "UtilClientUI.SpawnerTab", "UtilClientUI.CheatsTab", "UtilClientUI.WorldEditTab", "UtilClientUI.CosmeticsTab", "UtilClientUI.ItemEditorTab", "UtilClientUI.SettingsTab" };

	private Color _uiBackgroundColor;

	private bool _reallyMousedOver;

	private int selectedTab;

	public bool IsMousedOver
	{
		get
		{
			if (_mainPanel != null)
			{
				return _mainPanel.IsMouseHovering;
			}
			return false;
		}
	}

	public override void OnInitialize()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		UIElement uIElement = new UIElement();
		uIElement.Width.Set(0f, 0.8f);
		uIElement.MaxWidth.Set(600f, 0f);
		uIElement.Top.Set(180f, 0f);
		uIElement.Height.Set(-180f, 1f);
		uIElement.HAlign = 0.5f;
		_outerContainer = uIElement;
		UIPanel uIPanel = new UIPanel();
		uIPanel.Width.Set(0f, 1f);
		uIPanel.Height.Set(-90f, 1f);
		uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
		_uiBackgroundColor = uIPanel.BackgroundColor;
		_mainPanel = uIPanel;
		uIElement.Append(uIPanel);
		_buttons = PrepareButtons();
		for (int i = 0; i < _buttons.Length; i++)
		{
			_buttons[i].OnLeftMouseDown += ClickTabButton;
			uIPanel.Append(_buttons[i]);
		}
		float height = StyleDimension.FromPixels((float)Math.Ceiling((double)_buttons.Length / 4.0) * 35f - 10f).Pixels;
		_innerWrapper = new UIElement();
		_innerWrapper.Top.Set(height + 12f, 0f);
		_innerWrapper.Width.Set(0f, 1f);
		_innerWrapper.Height.Set(-70f, 1f);
		_innerWrapper.HAlign = 0.5f;
		_defaultState = new InnerPlaceholder();
		_tabs = new UIState[8];
		for (int j = 0; j < _tabs.Length; j++)
		{
			_tabs[j] = _defaultState;
		}
		_tabs[0] = new InnerPlayerList();
		_tabs[1] = new InnerWorldInfo();
		_tabs[2] = new InnerSpawner();
		_tabs[3] = new InnerCheats();
		_tabs[7] = new InnerSettings();
		_innerState = _tabs[0];
		_innerWrapper.Append(_innerState);
		uIPanel.Append(_innerWrapper);
		Append(uIElement);
		selectedTab = 0;
	}

	private void ClickTabButton(UIMouseEvent evt, UIElement listeningElement)
	{
		UIElement[] buttons = _buttons;
		if (buttons.Contains(listeningElement, null))
		{
			buttons = _buttons;
			selectedTab = buttons.IndexOf(listeningElement, null);
			SwitchInnerState();
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);
	}

	public override void Update(GameTime gameTime)
	{
		try
		{
			base.Update(gameTime);
			if (_mainPanel.IsMouseHovering)
			{
				Main.LocalPlayer.mouseInterface = true;
			}
		}
		catch
		{
		}
	}

	private UISimpleButton[] PrepareButtons()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		UISimpleButton[] buttons = new UISimpleButton[8];
		int num = 4;
		for (int i = 0; i < buttons.Length; i++)
		{
			UISimpleButton button = new UISimpleButton(_buttonKeys[i], new Color(73, 94, 171, 255) * 0.9f);
			buttons[i] = button;
			int num2 = i % num;
			int num3 = i / num;
			buttons[i].HAlign = (float)num2 / 3f;
			buttons[i].Top.Set(num3 * 33, 0f);
			buttons[i].Height.Set(30f, 0f);
			buttons[i].Width = StyleDimension.FromPercent(0.98f / (float)num);
			buttons[i].DisableIfCondition = () => DisableBySelectedTab(buttons.IndexOf(button, null));
		}
		return buttons;
	}

	public static void RefreshUI()
	{
	}

	private void SwitchInnerState()
	{
		_innerWrapper.RemoveAllChildren();
		_innerState = _tabs[selectedTab];
		_innerWrapper.Append(_innerState);
	}

	private bool DisableBySelectedTab(int i)
	{
		return i == selectedTab;
	}
}
