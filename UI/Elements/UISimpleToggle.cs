using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.UtilClient.UI.Elements;

public class UISimpleToggle : UISimpleButton
{
	private static Asset<Texture2D> _toggleTexture;

	protected Func<bool> GetState { get; }

	protected event Action<bool> ChangeState;

	protected override void LeftClick(UIMouseEvent evt, UIElement listeningElement)
	{
		if (!DisableIfCondition())
		{
			ChangeState(!GetState());
			SoundEngine.PlaySound(GetState() ? 10 : 11);
		}
	}

	public UISimpleToggle(string dispLocKey, Color color, [NotNull] Func<bool> stateGetter, [NotNull] Action<bool> stateChange, Func<bool> disableCondition = null, string hoverLocKey = "", Func<string>? disabledHoverTextGetter = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(dispLocKey, color, null, disableCondition, hoverLocKey, disabledHoverTextGetter);
		ArgumentNullException.ThrowIfNull(stateGetter, "stateGetter");
		ArgumentNullException.ThrowIfNull(stateChange, "stateChange");
		Asset<Texture2D> toggleTexture = _toggleTexture;
		if (toggleTexture == null || !toggleTexture.IsLoaded)
		{
			_toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle", (AssetRequestMode)1);
		}
		GetState = stateGetter;
		ChangeState += stateChange.Invoke;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		base.DrawSelf(spriteBatch);
		DrawToggle(spriteBatch, GetDimensions());
	}

	protected override Color GetPanelColor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Color color = base.GetPanelColor();
		if (GetState())
		{
			color = Color.Lerp(color, Color.Black, 0.3f);
		}
		return color;
	}

	protected override Vector2 GetTextPosition(CalculatedStyle dimensions, string text)
	{
		Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, text, base.TextScale);
		Vector2 position = new Vector2(dimensions.X, dimensions.Y + 2f + 6f);
		ref float x = ref position.X;
		float num = x;
		x = num + base.TextAnchor switch
		{
			HAnchor.Left => 10f, 
			HAnchor.Center => (dimensions.Width - size.X) * 0.5f, 
			HAnchor.Right => dimensions.Width - size.X - 10f - (float)_toggleTexture.Width(), 
			_ => throw new InvalidEnumArgumentException("TextAnchor"), 
		};
		return position;
	}

	private void DrawToggle(SpriteBatch spriteBatch, CalculatedStyle dimensions)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Rectangle source = GetToggleSource();
		Vector2 position = new Vector2(dimensions.X + dimensions.Width - (float)source.Width - 10f, dimensions.Y + 2f + 6f);
		spriteBatch.Draw(_toggleTexture.Value, position, (Rectangle?)source, Color.White);
	}

	private Rectangle GetToggleSource()
	{
		int width = (_toggleTexture.Width() - 2) / 2;
		return new Rectangle(GetState() ? (width + 2) : 0, 0, width, _toggleTexture.Height());
	}
}
