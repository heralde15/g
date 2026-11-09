using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria.UtilClient.UI.Elements;

public class TitleItemDisplayButton : TitleLinkButton
{
	public string TooltipHoverText;

	public string TooltipHoverShiftText;

	public int ItemType { get; init; }

	public override void Draw(SpriteBatch spriteBatch, Vector2 anchorPosition)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (ItemType >= 0 && ItemType < ItemID.Count)
		{
			float scale = 1f;
			if (ItemID.Sets.ItemIconPulse[ItemType])
			{
				scale = Main.essScale;
			}
			else if (ItemType == 58 || ItemType == 184 || ItemType == 4143)
			{
				scale = Main.essScale * 0.25f + 0.75f;
			}
			Main.instance.LoadItem(ItemType);
			Vector2 vector = TextureAssets.Item[ItemType].Value.Frame().Size();
			Vector2 vector2 = anchorPosition - vector / 2f;
			if (Main.MouseScreen.Between(vector2, vector2 + vector))
			{
				Main.LocalPlayer.mouseInterface = true;
				DrawTooltip();
			}
			ItemSlot.DrawItemIcon(ContentSamples.ItemsByType[ItemType], 0, spriteBatch, anchorPosition, scale, 80f, Color.White, 2f);
		}
	}

	internal override void DrawTooltip()
	{
		Item fakeItem = TitleLinkButton._fakeItem;
		fakeItem.SetDefaults(0);
		string textValue = Language.GetTextValue(TooltipTextKey);
		fakeItem.SetNameOverride(textValue);
		fakeItem.type = 1;
		fakeItem.scale = 0f;
		fakeItem.rare = -12;
		fakeItem.value = -1;
		fakeItem.BestiaryNotes = TooltipHoverText;
		if ((((KeyboardState)(ref Main.keyState)).IsKeyDown((Keys)160) || ((KeyboardState)(ref Main.keyState)).IsKeyDown((Keys)161)) && !string.IsNullOrWhiteSpace(TooltipHoverShiftText))
		{
			fakeItem.BestiaryNotes = TooltipHoverShiftText;
		}
		Main.HoverItem = TitleLinkButton._fakeItem;
		Main.instance.MouseText("", 0, 0);
		Main.mouseText = true;
	}

	internal override void TryClicking()
	{
	}
}
