using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UtilClient.Assets;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Common.Helpers.Game;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.UI.Elements;

internal class UIPlayerListItem : UIPanel
{
	private float ringPoint;

	private const int numSteps = 61;

	private Vector2[] oldPos;

	private Vector2[] oldTangent;

	private VertexStrip _vertexStrip;

	private float opacity;

	private float trial;

	private float animationSpeed;

	private float saturation;

	private Asset<Texture2D> _dividerTexture;

	private Asset<Texture2D> _innerPanelTexture;

	private Asset<Texture2D> _vanillaInnerPanelTexture;

	private Asset<Texture2D> _buttonTrashTexture;

	private Asset<Texture2D> _spectateButtonTexture;

	private Asset<Texture2D> _teamTexture;

	private UICharacter _playerPanel;

	private UIText _buttonLabel;

	public UIImageButton playerItem;

	private UIImageButton _trashButton;

	private UIImageButton _spectateButton;

	private UIImage _clientIcon;

	private UIImage _specOn;

	private UIImageFramed _teamIndicator;

	public int playerIndex;

	public Player OriginalPlayer => Main.player[playerIndex];

	public Player DisplayedPlayer
	{
		get
		{
			if (_playerPanel != null)
			{
				if (_playerPanel._player != null)
				{
					return _playerPanel._player;
				}
				return OriginalPlayer;
			}
			return OriginalPlayer;
		}
		set
		{
			_playerPanel._player = value;
		}
	}

	public bool PlayerActive => Main.player[playerIndex].active;

	public bool PlayerFullyConnected => Main.player[playerIndex].HasFullyConnected;

	public void DrawColorRingCallback(UIElement element, SpriteBatch spriteBatch)
	{
		if (!PlayerActive || !OriginalPlayer.zModUserPrivate || OriginalPlayer.IsLocal)
		{
			return;
		}
		spriteBatch.End();
		spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, (Effect)null, Main.UIScaleMatrix);
		Rectangle dimensions = element.GetDimensions().ToRectangle();
		if (oldPos[0] == Vector2.Zero)
		{
			for (int i = 0; i < oldPos.Length; i++)
			{
				UpdateColorRing(dimensions, forced: true);
			}
		}
		else
		{
			UpdateColorRing(dimensions);
		}
		DrawSpecialSeedRing(dimensions);
		spriteBatch.End();
		spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, (Effect)null, Main.UIScaleMatrix);
	}

	private void UpdateColorRing(Rectangle dimensions, bool forced = false)
	{
		if (!Main.DrawnThisUpdate || forced)
		{
			dimensions.Inflate(-1, -1);
			int num = dimensions.Width * 2 + dimensions.Height * 2;
			float num2 = (float)num / (float)(oldPos.Length - 1);
			ringPoint += num2;
			if (ringPoint >= (float)num)
			{
				ringPoint -= num;
			}
			float num3 = (float)Math.Sqrt(dimensions.Width / 2 * dimensions.Width / 2 + dimensions.Height / 2 * dimensions.Height / 2);
			float num4 = (float)Math.PI * 2f * ringPoint / (float)num;
			Vector2 vector = new Vector2((float)Math.Cos(num4), (float)Math.Sin(num4));
			Vector2 vector2 = vector * num3;
			float num5 = Math.Abs(vector2.X) / ((float)dimensions.Width / 2f);
			float num6 = Math.Abs(vector2.Y) / ((float)dimensions.Height / 2f);
			if (num5 > num6)
			{
				vector2 /= num5;
				vector /= num5;
			}
			else
			{
				vector2 /= num6;
				vector /= num6;
			}
			vector2 += NumericsExtensions[dimensions].ToVector2();
			for (int num7 = oldPos.Length - 1; num7 > 0; num7--)
			{
				oldPos[num7] = oldPos[num7 - 1];
				oldTangent[num7] = oldTangent[num7 - 1];
			}
			oldPos[0] = vector2 - dimensions.TopLeft();
			oldTangent[0] = vector;
		}
	}

	private void DrawSpecialSeedRing(Rectangle dimensions)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
		miscShaderData.UseSaturation(trial);
		miscShaderData.UseOpacity(opacity);
		miscShaderData.UseSpriteTransformMatrix(Main.UIScaleMatrix);
		miscShaderData.Apply();
		Vector2 dimsTopLeft = dimensions.TopLeft();
		float num = 8f;
		int num2 = oldPos.Length;
		_vertexStrip.Reset(num2 * 2);
		int num3 = num2;
		for (int i = 0; i < num2 && !(oldPos[i] == Vector2.Zero); i++)
		{
			Vector2 vector = oldPos[i] + dimsTopLeft;
			float num4 = (float)i / (float)(num3 - 1);
			num4 *= 0.6f;
			Color vertexColor = StripColors(num4);
			float num5 = StripWidth(num4);
			Vector2 vector2 = oldTangent[i] * num;
			Vector3 uvA = new Vector3(num4, num5 / 2f, num5);
			Vector3 uvB = new Vector3(num4, num5 / 2f, num5);
			_vertexStrip.AddVertexPair(vector + vector2, vector, uvA, uvB, vertexColor);
		}
		_vertexStrip.PrepareIndices(includeBacksides: true);
		_vertexStrip.DrawTrail();
		Main.pixelShader.CurrentTechnique.Passes[0].Apply();
		miscShaderData.UseSpriteTransformMatrix(null);
	}

	private Color StripColors(float progressOnStrip)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Color result = Main.hslToRgb((progressOnStrip - Main.GlobalTimeWrappedHourly * animationSpeed) % 1f, saturation, 0.5f);
		((Color)(ref result)).A = 0;
		return result;
	}

	private float StripWidth(float progressOnStrip)
	{
		float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
		return 24f * lerpValue;
	}

	public UIPlayerListItem(int index)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		oldPos = new Vector2[61];
		oldTangent = new Vector2[61];
		_vertexStrip = new VertexStrip();
		opacity = 0.6f;
		animationSpeed = 0.5f;
		saturation = 0.7f;
		base._002Ector();
		BackgroundColor = new Color(63, 82, 151) * 0.7f;
		BorderColor = new Color(89, 116, 213) * 0.7f;
		_dividerTexture = Main.Assets.Request<Texture2D>("Images/UI/Divider", (AssetRequestMode)1);
		_vanillaInnerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground", (AssetRequestMode)1);
		_innerPanelTexture = UtilAssets.InnerPanelBackground2;
		_spectateButtonTexture = Main.Assets.Request<Texture2D>("Images/Inventory_Tick_Off", (AssetRequestMode)1);
		_teamTexture = Main.Assets.Request<Texture2D>("Images/UI/PVP_1", (AssetRequestMode)1);
		Height.Set(70f, 0f);
		Width.Set(0f, 1f);
		SetPadding(6f);
		if (index < 0 || index > 255)
		{
			index = 0;
		}
		playerIndex = index;
		UpdateBackgroundColor();
		Append(_teamIndicator = new UIImageFramed(_teamTexture, _teamTexture.Value.Frame(6))
		{
			HAlign = 1f,
			MarginTop = 1f,
			IgnoresMouseInteraction = true
		});
		UIImageButton uIImageButton6 = new UIImageButton(_spectateButtonTexture)
		{
			HAlign = 0.96f,
			MarginTop = 3f
		};
		UIImage specOn = new UIImage(Main.Assets.Request<Texture2D>("Images/Inventory_Tick_On", (AssetRequestMode)1))
		{
			IgnoresMouseInteraction = true,
			Color = Color.Transparent
		};
		_specOn = specOn;
		uIImageButton6.Append(_specOn);
		uIImageButton6.OnLeftClick += SpectateButtonLeftClick;
		uIImageButton6.OnRightClick += SpectateButtonRightClick;
		_spectateButton = uIImageButton6;
		Append(uIImageButton6);
		UIImage clientIcon = new UIImage(Main.Assets.Request<Texture2D>("Images/Item_2737", (AssetRequestMode)1))
		{
			ImageScale = 0.75f,
			VAlign = -0.3f,
			HAlign = 0.93f,
			PassThroughMouseInteraction = true,
			Color = Color.Transparent
		};
		clientIcon.Color = Color.Lerp(Color.White, Color.LightCyan, 0.5f);
		_clientIcon = clientIcon;
		Append(clientIcon);
		_playerPanel = new UICharacter(Main.player[playerIndex], animated: false, hasBackPanel: true, 1f, useAClone: true);
		_playerPanel.Left.Set(4f, 0f);
		_playerPanel.PrepareAction = UpdateDummy;
		_playerPanel.OnLeftClick += delegate
		{
			if (!OriginalPlayer.IsLocal && PlayerInput.Triggers.JustReleased.MouseLeft)
			{
				Main.LocalPlayer.TeleportToEntity(OriginalPlayer, noBroadcast: false, withEffect: true, screenPan: true);
			}
		};
		_playerPanel.drawsGoldenOutline = OriginalPlayer.IsLocal;
		UIElement emptyElement = new UIElement();
		emptyElement.IgnoresMouseInteraction = (emptyElement.PassThroughMouseInteraction = true);
		emptyElement.Left.Set(8f, 0f);
		emptyElement.Top.Set(4f, 0f);
		emptyElement.Width.Set(52f, 0f);
		emptyElement.Height.Set(52f, 0f);
		emptyElement.OnDraw += DrawColorRingCallback;
		Append(emptyElement);
		Append(_playerPanel);
		base.OnLeftClick += OpenInvView;
	}

	private void UpdateDummy()
	{
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		DisplayedPlayer.CurrentLoadoutIndex = OriginalPlayer.CurrentLoadoutIndex;
		DisplayedPlayer.armor = OriginalPlayer.armor;
		DisplayedPlayer.dye = OriginalPlayer.dye;
		DisplayedPlayer.hideVisibleAccessory = OriginalPlayer.hideVisibleAccessory;
		DisplayedPlayer.Loadouts = OriginalPlayer.Loadouts;
		DisplayedPlayer.miscEquips = OriginalPlayer.miscEquips;
		DisplayedPlayer.miscDyes = OriginalPlayer.miscDyes;
		DisplayedPlayer.hideMisc = OriginalPlayer.hideMisc;
		DisplayedPlayer.team = OriginalPlayer.team;
		DisplayedPlayer.hostile = OriginalPlayer.hostile;
		DisplayedPlayer.difficulty = OriginalPlayer.difficulty;
		DisplayedPlayer.statLife = OriginalPlayer.statLife;
		DisplayedPlayer.statMana = OriginalPlayer.statMana;
		DisplayedPlayer.statLifeMax = OriginalPlayer.statLifeMax;
		DisplayedPlayer.statLifeMax2 = OriginalPlayer.statLifeMax2;
		DisplayedPlayer.statManaMax = OriginalPlayer.statManaMax;
		DisplayedPlayer.statLifeMax2 = OriginalPlayer.statLifeMax2;
		DisplayedPlayer.skinVariant = OriginalPlayer.skinVariant;
		DisplayedPlayer.voiceVariant = OriginalPlayer.voiceVariant;
		DisplayedPlayer.eyeColor = OriginalPlayer.eyeColor;
		DisplayedPlayer.hairColor = OriginalPlayer.hairColor;
		DisplayedPlayer.hair = OriginalPlayer.hair;
		DisplayedPlayer.skinColor = OriginalPlayer.skinColor;
		DisplayedPlayer.shirtColor = OriginalPlayer.shirtColor;
		DisplayedPlayer.underShirtColor = OriginalPlayer.underShirtColor;
		DisplayedPlayer.shoeColor = OriginalPlayer.shoeColor;
		DisplayedPlayer.pantsColor = OriginalPlayer.pantsColor;
		DisplayedPlayer.hairDye = OriginalPlayer.hairDye;
		DisplayedPlayer.hairDyeColor = OriginalPlayer.hairDyeColor;
		DisplayedPlayer.wereWolf = OriginalPlayer.wereWolf && PlayerFullyConnected;
		DisplayedPlayer.hideWolf = OriginalPlayer.hideWolf;
		DisplayedPlayer.forceWerewolf = OriginalPlayer.forceWerewolf && PlayerFullyConnected;
		DisplayedPlayer.merman = OriginalPlayer.merman && PlayerFullyConnected;
		DisplayedPlayer.hideMerman = OriginalPlayer.hideMerman;
		DisplayedPlayer.forceMerman = OriginalPlayer.forceMerman && PlayerFullyConnected;
		DisplayedPlayer.zone1 = OriginalPlayer.zone1;
		DisplayedPlayer.zone2 = OriginalPlayer.zone2;
		DisplayedPlayer.zone3 = OriginalPlayer.zone3;
		DisplayedPlayer.zone4 = OriginalPlayer.zone4;
		DisplayedPlayer.zone5 = OriginalPlayer.zone5;
		DisplayedPlayer.zModUser = OriginalPlayer.zModUser && PlayerFullyConnected;
		_playerPanel.PreparePetProjectiles();
	}

	private void UpdateBackgroundColor()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Color darkenColor = (base.IsMouseHovering ? new Color(175, 175, 175) : new Color(150, 150, 150));
		_ = base.IsMouseHovering;
		BackgroundColor = GetBackgroundColor(base.IsMouseHovering);
		BorderColor = GetBorderColor(base.IsMouseHovering);
		if (!PlayerActive)
		{
			BackgroundColor = BackgroundColor.MultiplyRGB(darkenColor);
			((Color)(ref BackgroundColor)).A = (byte)((float)(int)((Color)(ref BackgroundColor)).A * 0.7f);
			BorderColor = BorderColor.MultiplyRGB(darkenColor);
			((Color)(ref BorderColor)).A = (byte)((float)(int)((Color)(ref BorderColor)).A * 0.7f);
		}
	}

	private Color GetBackgroundColor(bool mouseHover = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Color color = (mouseHover ? Colors.BlueBackgroundHoverColor : Colors.BlueBackgroundColor);
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().PlayerListTeamColors)
		{
			color = Main.teamColor[DisplayedPlayer.team] * (mouseHover ? 0.8f : 0.6f);
		}
		return color;
	}

	private Color GetBorderColor(bool mouseHover = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Color color = (mouseHover ? Colors.BlueBorderHoverColor : Colors.BlueBorderColor);
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().PlayerListTeamColors)
		{
			color = Main.teamColor[DisplayedPlayer.team] * (mouseHover ? 1f : 0.85f);
		}
		return color;
	}

	private Color GetPanelColor()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Color color = Colors.InnerPanelColor;
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().ResourcePackCompatMode)
		{
			color = Color.White;
		}
		byte A = ((Color)(ref color)).A;
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().PlayerListTeamColors)
		{
			color = Main.teamColor[DisplayedPlayer.team] * (PlayerActive ? 0.45f : 0.3f);
		}
		else if (!PlayerActive)
		{
			color *= 0.7f;
		}
		((Color)(ref color)).A = A;
		return color;
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		base.MouseOver(evt);
		_playerPanel.SetAnimated(animated: true);
	}

	public override void MouseOut(UIMouseEvent evt)
	{
		base.MouseOut(evt);
		_playerPanel.SetAnimated(animated: false);
	}

	private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Color color = GetPanelColor();
		Asset<Texture2D> panelTex = _innerPanelTexture;
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().ResourcePackCompatMode)
		{
			panelTex = _vanillaInnerPanelTexture;
		}
		spriteBatch.Draw(panelTex.Value, position, (Rectangle?)new Rectangle(0, 0, 8, panelTex.Height()), color);
		spriteBatch.Draw(panelTex.Value, new Vector2(position.X + 8f, position.Y), (Rectangle?)new Rectangle(8, 0, 8, panelTex.Height()), color, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), (SpriteEffects)0, 0f);
		spriteBatch.Draw(panelTex.Value, new Vector2(position.X + width - 8f, position.Y), (Rectangle?)new Rectangle(16, 0, 8, panelTex.Height()), color);
	}

	public override void Update(GameTime gameTime)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_spectateButton.IgnoresMouseInteraction = !PlayerActive || OriginalPlayer.IsLocal;
			if (Main.LocalPlayer.spectating == playerIndex && Main.LocalPlayer.spectating != Main.myPlayer)
			{
				_specOn.Color = Color.White;
			}
			else
			{
				_specOn.Color = Color.Transparent;
			}
			_teamIndicator.SetFrame(_teamTexture.Frame(6, 1, OriginalPlayer.team));
			_teamIndicator.Color = Color.White;
			if (!PlayerActive)
			{
				_teamIndicator.Color = Color.DarkGray;
			}
			if (_playerPanel.IsMouseHovering && !OriginalPlayer.IsLocal)
			{
				Main.instance.MouseText(ULang.UIVal("LeftClickTeleport"), 0, 0);
			}
			if (_clientIcon.IsMouseHovering && OriginalPlayer.zModUser && !OriginalPlayer.IsLocal && PlayerFullyConnected)
			{
				Main.instance.MouseText(ULang.UIVal("ClientUser", "zMod"), 0, 0);
			}
			if (_spectateButton.IsMouseHovering && !OriginalPlayer.IsLocal && PlayerFullyConnected)
			{
				if (Main.LocalPlayer.spectating == playerIndex)
				{
					_specOn.Color = Color.Transparent;
				}
				else
				{
					_specOn.Color = Color.White;
				}
				Main.instance.MouseText(ULang.UIVal((Main.LocalPlayer.spectating != playerIndex) ? "LeftClickSpectateStart" : "LeftClickSpectateStop"), 0, 0);
			}
			base.Update(gameTime);
		}
		catch
		{
		}
	}

	private void TrashButtonDoubleClick(UIMouseEvent evt, UIElement listeningElement)
	{
	}

	private void SpectateButtonLeftClick(UIMouseEvent evt, UIElement listeningElement)
	{
		SpectatePlayer();
	}

	private void SpectateButtonRightClick(UIMouseEvent evt, UIElement listeningElement)
	{
		SpectatePlayer(silent: true);
	}

	private void SpectatePlayer(bool silent = false)
	{
		if (!PlayerFullyConnected || OriginalPlayer.IsLocal)
		{
			return;
		}
		if (Main.LocalPlayer.spectating == playerIndex)
		{
			Main.LocalPlayer.SetOrRequestSpectating(-1);
			return;
		}
		if (BuiltInCheats.Freecam.Enabled)
		{
			BuiltInCheats.Freecam.Toggle();
		}
		if (silent)
		{
			Main.LocalPlayer.spectating = playerIndex;
		}
		else
		{
			Main.LocalPlayer.SetOrRequestSpectating(playerIndex);
		}
	}

	private string FormatName()
	{
		string name = OriginalPlayer.name;
		if (string.IsNullOrWhiteSpace(name))
		{
			name = $"Player {playerIndex}";
		}
		string namePrefix = string.Empty;
		if (!OriginalPlayer.IsLocal)
		{
			if (!PlayerActive)
			{
				namePrefix = ULang.UIVal("PrefixOffline");
			}
			else if (OriginalPlayer.ConnectionState == Player.JoinState.CurrentlyConnecting)
			{
				namePrefix = ULang.UIVal("PrefixConnecting");
			}
		}
		if (string.IsNullOrWhiteSpace(namePrefix))
		{
			return name;
		}
		InlineArray2<string> buffer = default(InlineArray2<string>);
		buffer[0] = namePrefix;
		buffer[1] = name;
		return string.Join(" ", (ReadOnlySpan<string?>)buffer);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSelf(spriteBatch);
		_playerPanel.drawsGoldenOutline = OriginalPlayer.IsLocal;
		if (OriginalPlayer.IsLocal)
		{
			_spectateButton.Color = Color.Transparent;
			_clientIcon.Color = Color.Transparent;
		}
		else
		{
			_spectateButton.Color = Color.White;
			_clientIcon.Color = Color.Transparent;
			if (OriginalPlayer.zModUser)
			{
				_clientIcon.Color = Color.White;
			}
		}
		if (!PlayerFullyConnected)
		{
			_spectateButton.Color = Color.Transparent;
			_clientIcon.Color = Color.Transparent;
		}
		CalculatedStyle innerDimensions = GetInnerDimensions();
		CalculatedStyle dimensions = _playerPanel.GetDimensions();
		float num = dimensions.X + dimensions.Width;
		UpdateBackgroundColor();
		Color color = (PlayerActive ? Color.White : Color.DarkGray);
		Color color3 = (PlayerActive ? Color.White : Color.DarkGray);
		string text = FormatName();
		Utils.DrawBorderString(spriteBatch, text, new Vector2(num + 6f, dimensions.Y - 2f), color);
		spriteBatch.Draw(_dividerTexture.Value, new Vector2(num, innerDimensions.Y + 21f), (Rectangle?)null, color, 0f, Vector2.Zero, new Vector2((GetDimensions().X + GetDimensions().Width - num - 2f) / 8f, 1f), (SpriteEffects)0, 0f);
		Vector2 vector = new Vector2(num + 6f, innerDimensions.Y + 29f);
		float num2 = 215f;
		if (GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive)
		{
			num2 = 270f;
		}
		Vector2 vector2 = vector;
		DrawPanel(spriteBatch, vector2, num2);
		spriteBatch.Draw(TextureAssets.Heart.Value, vector2 + new Vector2(5f, 2f), color3);
		vector2.X += 10f + (float)TextureAssets.Heart.Width();
		Utils.DrawBorderString(spriteBatch, OriginalPlayer.statLifeMax2 + Language.GetTextValue("GameUI.PlayerLifeMax"), vector2 + new Vector2(0f, 3f), color);
		vector2.X += 80f;
		spriteBatch.Draw(TextureAssets.Mana.Value, vector2 + new Vector2(5f, 2f), color3);
		vector2.X += 10f + (float)TextureAssets.Mana.Width();
		Utils.DrawBorderString(spriteBatch, OriginalPlayer.statManaMax2 + Language.GetTextValue("GameUI.PlayerManaMax"), vector2 + new Vector2(0f, 3f), color);
		vector.X += num2 + 5f;
		Vector2 vector3 = vector;
		float num3 = 140f;
		if (GameCulture.FromCultureName(GameCulture.CultureName.Chinese).IsActive)
		{
			num3 = 80f;
		}
		else if (GameCulture.FromCultureName(GameCulture.CultureName.Russian).IsActive)
		{
			num3 = 160f;
		}
		DrawPanel(spriteBatch, vector3, num3);
		string text2 = "";
		Color color4 = Color.White;
		switch (OriginalPlayer.difficulty)
		{
		case 0:
			text2 = Language.GetTextValue("UI.Softcore");
			break;
		case 1:
			text2 = Language.GetTextValue("UI.Mediumcore");
			color4 = Main.mcColor;
			break;
		case 2:
			text2 = Language.GetTextValue("UI.Hardcore");
			color4 = Main.hcColor;
			break;
		case 3:
			text2 = Language.GetTextValue("UI.Creative");
			color4 = Main.creativeModeColor;
			break;
		}
		if (!PlayerActive)
		{
			color4 = color4.MultiplyRGB(Color.DarkGray);
		}
		vector3 += new Vector2(num3 * 0.5f - FontAssets.MouseText.Value.MeasureString(text2).X * 0.5f, 3f);
		Utils.DrawBorderString(spriteBatch, text2, vector3, color4);
		vector.X += num3 + 5f;
		Vector2 vector4 = vector;
		float num4 = innerDimensions.X + innerDimensions.Width - vector4.X;
		DrawPanel(spriteBatch, vector4, num4);
		string text3 = ULang.UIVal("PlayerIndex", playerIndex);
		vector4 += new Vector2(num4 * 0.5f - FontAssets.MouseText.Value.MeasureString(text3).X * 0.5f, 3f);
		Utils.DrawBorderString(spriteBatch, text3, vector4, color);
	}

	private void OpenInvView(UIMouseEvent evt, UIElement element)
	{
		if (OriginalPlayer.name != "" && !_playerPanel.IsMouseHovering && !_spectateButton.IsMouseHovering)
		{
			SoundEngine.PlaySound(10);
		}
	}
}
