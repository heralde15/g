using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UtilClient.Config;
using Terraria.UtilClient.Config.Types;
using Terraria.UtilClient.Config.Types.Structures;
using Terraria.UtilClient.Localization;

namespace Terraria.UtilClient.UI.Elements;

public class UIServerListItem : AWorldListItem
{
	private Asset<Texture2D> _dividerTexture;

	private Asset<Texture2D> _innerPanelTexture;

	private UIElement _worldIcon;

	private UIText _buttonLabel;

	private UIText _deleteButtonLabel;

	private Asset<Texture2D> _buttonFavoriteActiveTexture;

	private Asset<Texture2D> _buttonFavoriteInactiveTexture;

	private Asset<Texture2D> _buttonPlayTexture;

	private Asset<Texture2D> _buttonDeleteTexture;

	private UIImageButton _playButton;

	private UIImageButton _favoriteButton;

	private UIImageButton _deleteButton;

	private int _orderInList;

	private bool _canBePlayed;

	private SavedServerData _serverData;

	private Color panelColor;

	public bool IsFavorite => _serverData.IsFavorite;

	public bool IsLanServer => _serverData.IsLocal;

	public UIServerListItem(SavedServerData data, int orderInList)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		_canBePlayed = true;
		panelColor = Color.White;
		base._002Ector();
		_orderInList = orderInList;
		_serverData = data;
		_data = new WorldFileData();
		_serverData.CopyToWorldFileData(_data);
		panelColor = Color.White;
		if (IsLanServer)
		{
			panelColor = Utils.ShiftBlueToCyanTheme(panelColor);
		}
		LoadTextures();
		InitializeAppearance();
		_worldIcon = GetIconElement();
		_worldIcon.OnLeftDoubleClick += PlayGame;
		Append(_worldIcon);
		if (_serverData.DefeatedMoonlord)
		{
			UIImage element = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/IconCompletion", (AssetRequestMode)1))
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				Top = new StyleDimension(-10f, 0f),
				Left = new StyleDimension(-3f, 0f),
				IgnoresMouseInteraction = true
			};
			_worldIcon.Append(element);
		}
		if (GetIcons().Count >= 2 && !_serverData.ZenithWorld)
		{
			UIImage element2 = new UIImage(Main.Assets.Request<Texture2D>("Images/UI/IconMixedSeed", (AssetRequestMode)1))
			{
				HAlign = 0f,
				VAlign = 1f,
				Top = StyleDimension.FromPixels(0f),
				Left = StyleDimension.FromPixels(0f)
			};
			_worldIcon.Append(element2);
		}
		float num = 4f;
		UIImageButton uIImageButton = new UIImageButton(_buttonPlayTexture);
		uIImageButton.VAlign = 1f;
		uIImageButton.Left.Set(num, 0f);
		uIImageButton.OnLeftClick += PlayGame;
		base.OnLeftDoubleClick += PlayGame;
		uIImageButton.OnMouseOver += PlayMouseOver;
		uIImageButton.OnMouseOut += ButtonMouseOut;
		_playButton = uIImageButton;
		Append(uIImageButton);
		num += 24f;
		UIImageButton uIImageButton2 = new UIImageButton(IsFavorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture);
		uIImageButton2.VAlign = 1f;
		uIImageButton2.Left.Set(num, 0f);
		if (!IsLanServer)
		{
			uIImageButton2.OnLeftClick += FavoriteButtonClick;
			uIImageButton2.OnMouseOver += FavoriteMouseOver;
			uIImageButton2.OnMouseOut += ButtonMouseOut;
		}
		uIImageButton2.SetVisibility(1f, IsFavorite ? 0.8f : 0.4f);
		_favoriteButton = uIImageButton2;
		Append(uIImageButton2);
		UIImageButton uIImageButton6 = new UIImageButton(_buttonDeleteTexture)
		{
			VAlign = 1f,
			HAlign = 1f
		};
		if (!IsLanServer)
		{
			if (!IsFavorite)
			{
				uIImageButton6.OnLeftClick += DeleteButtonClick;
			}
			uIImageButton6.OnMouseOver += DeleteMouseOver;
			uIImageButton6.OnMouseOut += DeleteMouseOut;
		}
		_deleteButton = uIImageButton6;
		Append(uIImageButton6);
		num += 28f;
		_buttonLabel = new UIText("");
		_buttonLabel.VAlign = 1f;
		_buttonLabel.Left.Set(num, 0f);
		_buttonLabel.Top.Set(-3f, 0f);
		Append(_buttonLabel);
		_deleteButtonLabel = new UIText("");
		_deleteButtonLabel.VAlign = 1f;
		_deleteButtonLabel.HAlign = 1f;
		_deleteButtonLabel.Left.Set(-30f, 0f);
		_deleteButtonLabel.Top.Set(-3f, 0f);
		Append(_deleteButtonLabel);
		uIImageButton.SetSnapPoint("Play", orderInList);
		uIImageButton2.SetSnapPoint("Favorite", orderInList);
		uIImageButton6.SetSnapPoint("Delete", orderInList);
	}

	private void LoadTextures()
	{
		_dividerTexture = Main.Assets.Request<Texture2D>("Images/UI/Divider", (AssetRequestMode)1);
		_innerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground", (AssetRequestMode)1);
		_buttonFavoriteActiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteActive", (AssetRequestMode)1);
		_buttonFavoriteInactiveTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonFavoriteInactive", (AssetRequestMode)1);
		_buttonPlayTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonPlay", (AssetRequestMode)1);
		_buttonDeleteTexture = Main.Assets.Request<Texture2D>("Images/UI/ButtonDelete", (AssetRequestMode)1);
	}

	private void InitializeAppearance()
	{
		Height.Set(96f, 0f);
		Width.Set(0f, 1f);
		SetPadding(6f);
		SetColorsToNotHovered();
	}

	public void SetOrderInList(int orderInList)
	{
		_orderInList = orderInList;
		_playButton.SetSnapPoint("Play", _orderInList);
		_favoriteButton.SetSnapPoint("Favorite", _orderInList);
		_deleteButton.SetSnapPoint("Delete", _orderInList);
	}

	private void SetColorsToHovered()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		BackgroundColor = new Color(73, 94, 171);
		BorderColor = new Color(89, 116, 213);
		if (!_canBePlayed)
		{
			BorderColor = new Color(150, 150, 150) * 1f;
			BackgroundColor = Color.Lerp(BackgroundColor, new Color(120, 120, 120), 0.5f) * 1f;
		}
		if (IsLanServer)
		{
			BorderColor = Utils.ShiftBlueToCyanTheme(BorderColor);
			BackgroundColor = Utils.ShiftBlueToCyanTheme(BackgroundColor);
		}
	}

	private void SetColorsToNotHovered()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		BackgroundColor = new Color(63, 82, 151) * 0.7f;
		BorderColor = new Color(89, 116, 213) * 0.7f;
		if (!_canBePlayed)
		{
			BorderColor = new Color(127, 127, 127) * 0.7f;
			BackgroundColor = Color.Lerp(new Color(63, 82, 151), new Color(80, 80, 80), 0.5f) * 0.7f;
		}
		if (IsLanServer)
		{
			BorderColor = Utils.ShiftBlueToCyanTheme(BorderColor);
			BackgroundColor = Utils.ShiftBlueToCyanTheme(BackgroundColor);
		}
	}

	private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		if (IsFavorite)
		{
			_buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
		}
		else
		{
			_buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
		}
	}

	private void PlayMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		_buttonLabel.SetText(Language.GetTextValue("UI.Play"));
	}

	private void DeleteMouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		if (!IsLanServer)
		{
			if (IsFavorite)
			{
				_deleteButtonLabel.SetText(Language.GetTextValue("UI.CannotDeleteFavorited"));
			}
			else
			{
				_deleteButtonLabel.SetText(Language.GetTextValue("UI.Delete"));
			}
		}
	}

	private void DeleteMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		_deleteButtonLabel.SetText("");
	}

	private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement)
	{
		_buttonLabel.SetText("");
	}

	private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
	{
		SavedServersConfig config = UConfigExtensions.get_Current<SavedServersConfig>();
		for (int i = 0; i < config.SavedServers.Count; i++)
		{
			if (config.SavedServers[i] == _serverData)
			{
				SoundEngine.PlaySound(10);
				Main.selectedWorld = i;
				Main.menuMode = -999;
				break;
			}
		}
	}

	private void PlayGame(UIMouseEvent evt, UIElement listeningElement)
	{
		if (listeningElement == evt.Target && !TryMovingToRejectionMenuIfNeeded(_serverData.GameMode))
		{
			SoundEngine.PlaySound(10);
			Main.clrInput();
			Main.GetInputText("");
			Main.autoPass = false;
			Netplay.ListenPort = _serverData.ServerEndpoint.Port;
			Main.getIP = _serverData.ServerEndpoint.Host;
			Netplay.SetRemoteIPAsync(Main.getIP, Main.StartClientGameplay);
			Main.menuMode = 14;
			Main.statusText = Language.GetTextValue("Net.ConnectingTo", Main.getIP);
		}
	}

	private bool TryMovingToRejectionMenuIfNeeded(int worldGameMode)
	{
		if (!GameModeID.IsValid(worldGameMode))
		{
			SoundEngine.PlaySound(10);
			Main.statusText = Language.GetTextValue("UI.WorldCannotBeLoadedBecauseItHasAnInvalidGameMode");
			Main.menuMode = 1000000;
			return true;
		}
		return false;
	}

	private void FavoriteButtonClick(UIMouseEvent evt, UIElement listeningElement)
	{
		_serverData.ToggleFavorite();
		((UIImageButton)evt.Target).SetImage(IsFavorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture);
		((UIImageButton)evt.Target).SetVisibility(1f, IsFavorite ? 0.8f : 0.4f);
		if (IsFavorite)
		{
			_buttonLabel.SetText(Language.GetTextValue("UI.Unfavorite"));
		}
		else
		{
			_buttonLabel.SetText(Language.GetTextValue("UI.Favorite"));
		}
		if (base.Parent != null && base.Parent.Parent is UIList uIList)
		{
			uIList.UpdateOrder();
		}
	}

	public override int CompareTo(object obj)
	{
		if (obj is UIServerListItem UIServerListItem)
		{
			return _orderInList.CompareTo(UIServerListItem._orderInList);
		}
		return base.CompareTo(obj);
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		base.MouseOver(evt);
		SetColorsToHovered();
	}

	public override void MouseOut(UIMouseEvent evt)
	{
		base.MouseOut(evt);
		SetColorsToNotHovered();
	}

	private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Draw(_innerPanelTexture.Value, position, (Rectangle?)new Rectangle(0, 0, 8, _innerPanelTexture.Height()), panelColor);
		spriteBatch.Draw(_innerPanelTexture.Value, new Vector2(position.X + 8f, position.Y), (Rectangle?)new Rectangle(8, 0, 8, _innerPanelTexture.Height()), panelColor, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), (SpriteEffects)0, 0f);
		spriteBatch.Draw(_innerPanelTexture.Value, new Vector2(position.X + width - 8f, position.Y), (Rectangle?)new Rectangle(16, 0, 8, _innerPanelTexture.Height()), panelColor);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSelf(spriteBatch);
		CalculatedStyle innerDimensions = GetInnerDimensions();
		CalculatedStyle dimensions = _worldIcon.GetDimensions();
		float num = dimensions.X + dimensions.Width;
		Color color = Color.White;
		string worldName = _serverData.ServerName;
		Utils.DrawBorderString(spriteBatch, worldName, new Vector2(num + 6f, dimensions.Y - 2f), color);
		spriteBatch.Draw(_dividerTexture.Value, new Vector2(num, innerDimensions.Y + 21f), (Rectangle?)null, panelColor, 0f, Vector2.Zero, new Vector2((GetDimensions().X + GetDimensions().Width - num) / 8f, 1f), (SpriteEffects)0, 0f);
		Vector2 vector = new Vector2(num + 6f, innerDimensions.Y + 29f);
		float num2 = 100f;
		if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
		{
			num2 = 135f;
		}
		DrawPanel(spriteBatch, vector, num2);
		GetDifficulty(out var expertText, out var gameModeColor);
		float x = FontAssets.MouseText.Value.MeasureString(expertText).X;
		float x2 = num2 * 0.5f - x * 0.5f;
		Utils.DrawBorderString(spriteBatch, expertText, vector + new Vector2(x2, 3f), gameModeColor);
		vector.X += num2 + 5f;
		float num3 = 150f;
		if (!GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive)
		{
			num3 += 40f;
		}
		if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
		{
			num3 -= 15f;
		}
		DrawPanel(spriteBatch, vector, num3);
		string textValue = Language.GetTextValue("UI.WorldSizeFormat", _serverData.GetWorldSizeName());
		float x3 = FontAssets.MouseText.Value.MeasureString(textValue).X;
		float x4 = num3 * 0.5f - x3 * 0.5f;
		Utils.DrawBorderString(spriteBatch, textValue, vector + new Vector2(x4, 3f), Color.White);
		vector.X += num3 + 5f;
		float num4 = innerDimensions.X + innerDimensions.Width - vector.X;
		DrawPanel(spriteBatch, vector, num4);
		string textValue2 = _serverData.ServerEndpoint.ToString();
		if (IsLanServer)
		{
			textValue2 = ULang.UIVal("LanGame", _serverData.PlayerCount, _serverData.MaxPlayers);
		}
		float x5 = FontAssets.MouseText.Value.MeasureString(textValue2).X;
		float x6 = num4 * 0.5f - x5 * 0.5f;
		Utils.DrawBorderString(spriteBatch, textValue2, vector + new Vector2(x6, 3f), Color.White);
		vector.X += num4 + 5f;
	}
}
