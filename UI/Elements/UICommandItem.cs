using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UtilClient.Chat.Commands;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Restrictions;

namespace Terraria.UtilClient.UI.Elements;

public class UICommandItem : UIPanel
{
	public readonly IUtilCommand Command;

	private readonly Asset<Texture2D> _dividerTexture;

	private readonly Asset<Texture2D> _innerPanelTexture;

	private readonly UIText _hoverInfoLabel;

	private ItemTooltip _preparedTooltip;

	public int Order { get; set; }

	public UICommandItem(IUtilCommand command, int order)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		Command = command;
		Order = order;
		Height.Set(30f, 0f);
		Width.Set(0f, 1f);
		SetPadding(6f);
		BorderColor = Color.Transparent;
		BackgroundColor = Color.Transparent;
		_dividerTexture = Main.Assets.Request<Texture2D>("Images/UI/Divider", (AssetRequestMode)1);
		_innerPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground", (AssetRequestMode)1);
		_hoverInfoLabel = new UIText("");
		_hoverInfoLabel.VAlign = 1f;
		_hoverInfoLabel.Left.Set(80f, 0f);
		_hoverInfoLabel.Top.Set(-3f, 0f);
		Append(_hoverInfoLabel);
	}

	protected override void DrawSelf(SpriteBatch spriteBatch)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		string name = Command.Name;
		string text = Command.Description ?? "";
		base.DrawSelf(spriteBatch);
		if (base.IsMouseHovering)
		{
			Item item = Main.DisplayAndGetFakeItem(ItemRarityColor.StrongRed10);
			item.SetNameOverride(name);
			item.ToolTip = _preparedTooltip;
		}
		CalculatedStyle innerDimensions = GetInnerDimensions();
		Vector2 vector = innerDimensions.Position() - innerDimensions.Position();
		float num = 6f;
		float num2 = vector.X + num;
		float num3 = 21f;
		FontAssets.MouseText.Value.MeasureString(name);
		Color color = Color.White;
		Color color2 = Color.Gold;
		if (!Command.Restrictions.TryMeetAll())
		{
			color = Color.DarkGray;
			color2 = Color.DarkGray;
		}
		Utils.DrawBorderString(spriteBatch, name, innerDimensions.Position() + new Vector2(num2 + 6f, vector.Y - 2f), color, 1.1f);
		Utils.DrawBorderString(spriteBatch, text, innerDimensions.Position() + new Vector2(num2 + 6f + 180f + 16f, vector.Y + 2f + num3), color2, 0.8f, 0f, 1f);
	}

	public override int CompareTo(object obj)
	{
		return Order.CompareTo(((UICommandItem)obj).Order);
	}

	public override void MouseOver(UIMouseEvent evt)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.MouseOver(evt);
		BackgroundColor = new Color(76, 90, 149);
		BorderColor = new Color(50, 60, 86);
		_ = _preparedTooltip;
		string item = FontAssets.ItemStack.Value.CreateWrappedText((Command.Description ?? "").Replace("\n", " "), 480f, Language.ActiveCulture.CultureInfo);
		List<string> list = new List<string> { item };
		if (!Command.Restrictions.TryMeetAllFormatted(out string reasonText))
		{
			list.Add(reasonText);
		}
		_preparedTooltip = ItemTooltip.FromHardcodedText(list.ToArray());
	}

	public override void MouseOut(UIMouseEvent evt)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base.MouseOut(evt);
		BackgroundColor = (BorderColor = Color.Transparent);
	}

	public override void LeftClick(UIMouseEvent evt)
	{
		IngameFancyUI.Close();
		Main.drawingPlayerChat = true;
		Main.chatText = "." + Command.Name.ToLower() + " ";
		Main.NewText(ULang.CommandVal("ChatSet", Main.chatText), byte.MaxValue, byte.MaxValue, 0);
		Main.keyCount = 0;
	}

	private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Draw(_innerPanelTexture.Value, position, (Rectangle?)new Rectangle(0, 0, 8, _innerPanelTexture.Height()), Color.White);
		spriteBatch.Draw(_innerPanelTexture.Value, new Vector2(position.X + 8f, position.Y), (Rectangle?)new Rectangle(8, 0, 8, _innerPanelTexture.Height()), Color.White, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), (SpriteEffects)0, 0f);
		spriteBatch.Draw(_innerPanelTexture.Value, new Vector2(position.X + width - 8f, position.Y), (Rectangle?)new Rectangle(16, 0, 8, _innerPanelTexture.Height()), Color.White);
	}
}
