using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Initializers;
using Terraria.UI;
using Terraria.UtilClient.Credits;
using Terraria.UtilClient.UI.States;

namespace Terraria.UtilClient.UI;

public static class ClientUI
{
	public static bool DisplayMainPanel;

	public static UserInterface MainPanel = new UserInterface();

	private static List<TitleLinkButton> UtilClientButtons { get; } = new List<TitleLinkButton>();

	public static bool HoveringOverUI
	{
		get
		{
			if (!DisplayMainPanel)
			{
				return false;
			}
			if (MainPanel.CurrentState is UICheatPanel cheatPanel)
			{
				return cheatPanel.IsMousedOver;
			}
			return false;
		}
	}

	public static void DrawMainPanel()
	{
		if (DisplayMainPanel)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, RasterizerState.CullCounterClockwise, (Effect)null, Main.UIScaleMatrix);
			if (MainPanel.CurrentState == null)
			{
				MainPanel.SetState(new UICheatPanel());
			}
			MainPanel.Update(Main._drawInterfaceGameTime);
			MainPanel.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
		}
	}

	internal static void CreateMenuButtons()
	{
		UtilClientButtons.Clear();
		UtilClientButtons.Add(LinkButtonsInitializer.MakeSimpleButton("TitleLinks.Discord", "https://discord.gg/CnPbbGDgFs", 0));
		UtilClientButtons.Add(CreditsProvider.MenuHeart);
	}

	internal static void DrawMenuButtons()
	{
		float menuButtonsY = 164f;
		if (!Main.UnifiedBranding)
		{
			menuButtonsY = 78f;
		}
		if (Main.showFrameRate)
		{
			menuButtonsY += 25f;
		}
		Vector2 anchorPosition = new Vector2(18f, menuButtonsY);
		Main.UpdateEssenceScale();
		for (int i = 0; i < UtilClientButtons.Count; i++)
		{
			UtilClientButtons[i].Draw(Main.spriteBatch, anchorPosition);
			anchorPosition.X += 30f;
		}
	}
}
