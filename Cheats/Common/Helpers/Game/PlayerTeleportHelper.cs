using System.Numerics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Cheats.Categories.Travel;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class PlayerTeleportHelper
{
	extension(Player p)
	{
		public void TeleportOnMap(Vector2 centeredPos)
		{
			MapTPCheat.CheatSettings mTpSettings = BuiltInCheats.MapTP.Settings;
			p.Teleport_Shared(p.CenteredTeleportAndOldPos(centeredPos), noBroadcast: false, mTpSettings.WithEffects);
		}

		public void TeleportToMouse(Vector2 centeredPos)
		{
			MouseTPCheat.CheatSettings mTpSettings = BuiltInCheats.MouseTP.Settings;
			p.Teleport_Shared(p.CenteredTeleportAndOldPos(centeredPos), noBroadcast: false, mTpSettings.WithEffects, mTpSettings.LerpScreen);
		}

		public void TeleportToEntity(Entity e, bool noBroadcast = true, bool withEffect = false, bool screenPan = false)
		{
			p.Teleport(e.Bottom, useBottom: true, withEffect, screenPan, noBroadcast);
		}

		private Vector2 CenteredTeleportAndOldPos(Vector2 centeredPos)
		{
			Vector2 position = p.position;
			p.UnmountedCenter = centeredPos.RoundToWholeNumbers();
			return position;
		}

		private void Teleport(Vector2 targetPos, bool useBottom = false, bool withEffect = false, bool screenPan = false, bool noBroadcast = true)
		{
			if (p.whoAmI == Main.myPlayer)
			{
				Vector2 oldPos = p.position;
				Vector2 newPos = targetPos.RoundToWholeNumbers();
				if (useBottom)
				{
					p.Bottom = newPos;
				}
				else
				{
					p.position = newPos;
				}
				p.Teleport_Shared(oldPos, noBroadcast, withEffect, screenPan);
			}
		}

		private void Teleport_Shared(Vector2 oldPos, bool noBroadcast = true, bool withEffect = false, bool screenPan = false)
		{
			if (p.whoAmI == Main.myPlayer)
			{
				p.gfxOffY = 0f;
				p.netOffset = Vector2.Zero;
				p.velocity = Vector2.Zero;
				PressurePlateHelper.UpdatePlayerPosition(p);
				if (Main.netMode == 1 && !noBroadcast)
				{
					p.SetOrRequestSpectating(-1);
				}
				p.DoUnbreakableWallScan();
				p.fallStart = (int)(p.position.Y / 16f);
				p.HandleScreenPan(oldPos, screenPan);
				float effectScale = MathHelper.Clamp(1f - p.teleportTime * 0.99f, 0.01f, 1f);
				p.ApplyTeleportEffect(effectScale, withEffect);
				p.oldPosition = p.position + p.BlehOldPositionFixer;
				if (Main.netPlayCounter % 420 != 0 && !noBroadcast)
				{
					NetHelper.SyncLocalPlayerControls();
				}
			}
		}

		private void ApplyTeleportEffect(float effectScale, bool withEffect)
		{
			if (withEffect)
			{
				int style = TweaksCollectionExtensions.get_Current<GameplayTweaks>().PreferredTeleportStyle;
				int extraInfo = TweaksCollectionExtensions.get_Current<GameplayTweaks>().ExtraTeleportData;
				Main.TeleportEffect(p.getRect(), style, extraInfo, effectScale, TeleportationSide.Exit, p.position);
				p.teleportTime = 1f;
				p.teleportStyle = style;
			}
		}

		private void HandleScreenPan(Vector2 oldPos, bool screenPan = false)
		{
			if (!BuiltInCheats.Freecam.Enabled && screenPan)
			{
				int lerpSpeedupDelay = ((TweaksCollectionExtensions.get_Current<GameplayTweaks>().PreferredTeleportStyle == 1) ? 10 : 0);
				Main.StartCameraTransitionForPlayerTeleport(oldPos, 0.1f, lerpSpeedupDelay);
			}
			p.UpdateBiomesIfMovedEnoughForBlackFade(oldPos);
			if (Main.mapTime < 5)
			{
				Main.mapTime = 5;
			}
			Main.renderNow = true;
			Main.maxQ = true;
			if (!screenPan)
			{
				Main.renderNow = true;
				NPC.ResetNetOffsets();
				Player.ResetNetOffsets();
				Main.instantBGTransitionCounter = 10f;
				Main.UpdateSceneMetrics();
				Main.SceneState.skipTransitions = true;
				Main.bgDelay = 0f;
				Main.bgStyle = Main.GetPreferredBGStyleForPlayer();
				Cloud.lastCameraCenter = null;
				Main.CurrentPan = Vector2.Zero;
				Main.cameraX = 0f;
				Main.cameraY = 0f;
				Main.cameraLerp = 0f;
				Main.cameraLerpTimer = 0f;
				Main.BlackFadeIn = 0f;
			}
			p.ResetAdvancedShadows();
			for (int i = 0; i < 3; i++)
			{
				p.UpdateSocialShadow();
			}
		}
	}
}
