using System;
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

namespace Terraria.UtilClient.UI.Elements;

public class UIDescriptiveItem : UIPanel
{
	public readonly Func<string> Name;

	public readonly Func<string> Description;

	private Asset<Texture2D> _dividerTexture;

	private Asset<Texture2D> _innerPanelTexture;

	private UIText _hoverInfoLabel;

	private ItemTooltip _preparedTooltip;

	protected Func<bool> DisableCondition;

	protected Func<string> DisplayText;

	public UIDescriptiveItem(string nameKey, Func<string> descriptionKey, Func<string> getValue = null, Func<bool> disableCon = null)
	{
		Name = () => Language.GetTextValue(nameKey);
		Description = descriptionKey;
		DisplayText = getValue ?? new Func<string>(EmptyString);
		DisableCondition = disableCon ?? new Func<bool>(FalseBool);
		SetupInner();
	}

	public UIDescriptiveItem(Func<string> name, Func<string> descriptionKey, Func<string> getValue = null, Func<bool> disableCon = null)
	{
		Name = name;
		Description = descriptionKey;
		DisplayText = getValue ?? new Func<string>(EmptyString);
		DisableCondition = disableCon ?? new Func<bool>(FalseBool);
		SetupInner();
	}

	private void SetupInner()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Height.Set(26f, 0f);
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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		string name = Name();
		string rightSideText = DisplayText();
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
		FontAssets.MouseText.Value.MeasureString(name);
		Color color = Color.White;
		Color color2 = Color.Gold;
		if (DisableCondition())
		{
			color = Color.DarkGray;
			color2 = Color.DarkGray;
		}
		Vector2 leftOff = new Vector2(num2, vector.Y - 2f);
		Vector2 righOff = new Vector2(vector.X + innerDimensions.Width - FontAssets.MouseText.Value.MeasureString(rightSideText).X - num2, vector.Y - 2f);
		Utils.DrawBorderString(spriteBatch, name, innerDimensions.Position() + leftOff, color);
		Utils.DrawBorderString(spriteBatch, rightSideText, innerDimensions.Position() + righOff, color2);
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
		UpdateTooltip();
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
	}

	internal void UpdateTooltip()
	{
		if (Description != null)
		{
			_ = _preparedTooltip;
			string item = FontAssets.ItemStack.Value.CreateWrappedText(Description().Replace("\n", " "), 480f, Language.ActiveCulture.CultureInfo);
			List<string> list = new List<string> { item };
			_preparedTooltip = ItemTooltip.FromHardcodedText(list.ToArray());
		}
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

	private string EmptyString()
	{
		return string.Empty;
	}

	private bool FalseBool()
	{
		return false;
	}
}
