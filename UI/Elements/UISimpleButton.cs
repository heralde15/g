using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.UtilClient.UI.Elements;

public class UISimpleButton : UIElement
{
	protected const float TextVerticalOffset = 6f;

	protected const float TextTopPadding = 2f;

	protected const float TextSidePadding = 10f;

	[CompilerGenerated]
	private readonly Color _003CButtonColor_003Ek__BackingField;

	public int ClickSound;

	public Func<bool> DisableIfCondition;

	protected LocalizedText Text { get; }

	protected LocalizedText HoverText { get; }

	protected Func<string>? GetHoverDisabledText { get; }

	public Color ButtonColor
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CButtonColor_003Ek__BackingField;
		}
		[CompilerGenerated]
		init
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CButtonColor_003Ek__BackingField = value;
		}
	}

	public HAnchor TextAnchor { get; init; }

	public Vector2 TextScale { get; init; }

	public bool MuteClickSound { get; init; }

	public event Action OnClick;

	public UISimpleButton(string dispLocKey, Color color, Action onClick = null, Func<bool> disableCondition = null, string hoverLocKey = "", Func<string>? disabledHoverTextGetter = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ClickSound = 10;
		TextAnchor = HAnchor.Center;
		TextScale = new Vector2(0.8f);
		base._002Ector();
		Text = Language.GetText(dispLocKey);
		DisableIfCondition = disableCondition ?? ((Func<bool>)(() => false));
		HoverText = Language.GetText(hoverLocKey);
		GetHoverDisabledText = disabledHoverTextGetter;
		ButtonColor = color;
		OnClick += onClick;
		base.OnUpdate += UpdateHovering;
		base.OnMouseOver += MouseOver;
		base.OnLeftMouseDown += LeftClick;
	}

	protected virtual void LeftClick(UIMouseEvent evt, UIElement listeningElement)
	{
		if (!DisableIfCondition())
		{
			OnClick?.Invoke();
			if (!MuteClickSound)
			{
				SoundEngine.PlaySound(ClickSound);
			}
		}
	}

	private void MouseOver(UIMouseEvent evt, UIElement listeningElement)
	{
		if (!DisableIfCondition())
		{
			SoundEngine.PlaySound(12);
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected void UpdateHovering(UIElement element)
	{
		if (!element.IsMouseHovering)
		{
			return;
		}
		string hoverText;
		if (DisableIfCondition() && GetHoverDisabledText != null)
		{
			hoverText = GetHoverDisabledText();
		}
		else
		{
			if (!HoverText.HasValue)
			{
				return;
			}
			hoverText = HoverText.Value;
		}
		Main.instance.MouseText(hoverText, 0, 0);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSelf(spriteBatch);
		CalculatedStyle dimensions = GetDimensions();
		float panelWidth = dimensions.Width + 1f;
		Utils.DrawSettings2Panel(spriteBatch, dimensions.Position(), panelWidth, GetPanelColor());
		DrawText(spriteBatch, dimensions, Text.Value, GetTextColor(), panelWidth);
	}

	protected void DrawText(SpriteBatch spriteBatch, CalculatedStyle dimensions, string text, Color color, float panelWidth)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, GetTextPosition(dimensions, text), color, 0f, Vector2.Zero, TextScale, panelWidth);
	}

	protected void DrawDisabledIndicator(SpriteBatch spriteBatch, CalculatedStyle dimensions)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (DisableIfCondition())
		{
			Texture2D texture = TextureAssets.Extra[48].Value;
			int emoteId = 11;
			int frame = 1;
			Rectangle texFrame = texture.Frame(8, EmoteBubble.EMOTE_SHEET_VERTICAL_FRAMES, emoteId * 2 % 8 + frame, 1 + emoteId / 4);
			Vector2 position = new Vector2(dimensions.X + dimensions.Width - 10f - 22f, dimensions.Y + 2f + 6f - 4f);
			spriteBatch.Draw(texture, position, (Rectangle?)texFrame, Color.White);
		}
	}

	protected virtual Color GetPanelColor()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (DisableIfCondition())
		{
			return ButtonColor.MultiplyRGBA(new Color(140, 140, 140));
		}
		if (!base.IsMouseHovering)
		{
			return ButtonColor.MultiplyRGBA(new Color(180, 180, 180));
		}
		return ButtonColor;
	}

	protected Color GetTextColor()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Color color = (base.IsMouseHovering ? Color.White : Color.Silver);
		if (!DisableIfCondition() && base.IsMouseHovering)
		{
			color = Color.Lerp(color, Color.White, 0.5f);
		}
		return color;
	}

	protected virtual Vector2 GetTextPosition(CalculatedStyle dimensions, string text)
	{
		Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, text, TextScale);
		Vector2 position = new Vector2(dimensions.X, dimensions.Y + 2f + 6f);
		switch (TextAnchor)
		{
		case HAnchor.Left:
			position.X += 10f;
			break;
		case HAnchor.Center:
			position.X += (dimensions.Width - size.X) * 0.5f;
			break;
		case HAnchor.Right:
			position.X += dimensions.Width - size.X - 10f;
			break;
		default:
			throw new InvalidEnumArgumentException("TextAnchor");
		}
		return position;
	}
}
