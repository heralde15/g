using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.UtilClient.UI.Elements;

public class TMLHeaderElement : UIElement
{
	private readonly string header;

	private readonly bool mouseInteraction;

	private readonly bool lineless;

	public TMLHeaderElement(string header, bool mouseInteract = false, bool noLine = false)
	{
		this.header = header;
		Vector2 size = ChatManager.GetStringSize(FontAssets.ItemStack.Value, this.header, Vector2.One, 532f);
		Width.Set(0f, 1f);
		Height.Set(size.Y + 6f, 0f);
		mouseInteraction = mouseInteract;
		lineless = noLine;
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.DrawSelf(spriteBatch);
		CalculatedStyle dimensions = GetDimensions();
		float settingsWidth = dimensions.Width + 1f;
		Vector2 position = new Vector2(dimensions.X, dimensions.Y) + new Vector2(8f);
		Color lineColor = Color.LightGray;
		Color textColor = Color.White;
		if (mouseInteraction && base.IsMouseHovering)
		{
			lineColor = Color.Gold;
			textColor = Main.OurFavoriteColor;
		}
		if (!lineless)
		{
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dimensions.X + 10, (int)dimensions.Y + (int)dimensions.Height - 2, (int)dimensions.Width - 20, 1), lineColor);
		}
		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Language.GetTextValue(header), position, textColor, 0f, Vector2.Zero, new Vector2(1f), settingsWidth - 20f);
	}
}
