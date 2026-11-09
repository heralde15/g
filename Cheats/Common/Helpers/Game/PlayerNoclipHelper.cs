using System;
using System.Numerics;
using Terraria.GameInput;
using Terraria.UtilClient.Cheats;
using Terraria.UtilClient.Cheats.Categories.Travel;
using Terraria.UtilClient.Input;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class PlayerNoclipHelper
{
	extension(Player plr)
	{
		private NoclipInputStruct NoclipInput()
		{
			NoclipCheat.CheatSettings settings = BuiltInCheats.Noclip.Settings;
			NoclipInputStruct input = default(NoclipInputStruct);
			if (!FocusHelper.AllowInputProcessing || Main.mapFullscreen || plr.spectating != -1)
			{
				return input;
			}
			input.Up = (PlayerInput.Triggers.Current.Up || PlayerInput.Triggers.Current.Jump) && !PlayerInput.Triggers.Current.Down;
			input.Down = PlayerInput.Triggers.Current.Down && !PlayerInput.Triggers.Current.Up && !PlayerInput.Triggers.Current.Jump;
			input.Left = PlayerInput.Triggers.Current.Left && !PlayerInput.Triggers.Current.Right;
			input.Right = PlayerInput.Triggers.Current.Right && !PlayerInput.Triggers.Current.Left;
			float speed = Math.Clamp(settings.Speed, 4f, 100f);
			if (BuiltInCheats.Freecam.Enabled)
			{
				input.Up = (input.Down = (input.Left = (input.Right = false)));
			}
			else if (settings.EnableSpeedControls)
			{
				if (BuiltInKeybinds.NoclipSpeedFast.Current && !BuiltInKeybinds.NoclipSpeedSlow.Current)
				{
					speed *= settings.FastMultiplier;
				}
				else if (BuiltInKeybinds.NoclipSpeedSlow.Current && !BuiltInKeybinds.NoclipSpeedFast.Current)
				{
					speed *= settings.SlowMultiplier;
				}
			}
			input.SpeedMultiplier = Math.Clamp(speed, 2f, 200f);
			return input;
		}

		public void DoNoclipMovement()
		{
			if (!plr.IsLocal)
			{
				return;
			}
			if (plr.mount.CanHover())
			{
				plr.mount.Hover(plr);
			}
			plr.RefreshMovementAbilities();
			plr.stickyBreak = 0;
			plr.sloping = false;
			plr.gravity = 0f;
			NoclipCheat.CheatSettings settings = BuiltInCheats.Noclip.Settings;
			NoclipInputStruct input = plr.NoclipInput();
			Vector2 velocity = plr.velocity;
			float speed = input.SpeedMultiplier;
			if (settings.GhostMovement)
			{
				float accel = speed / 35f;
				ApplyAxisAcc(ref velocity.X, input.Right, input.Left, speed, accel);
				ApplyAxisAcc(ref velocity.Y, input.Down, input.Up, speed, accel);
			}
			else
			{
				velocity = Vector2.Zero;
				if (input.Up)
				{
					velocity.Y -= speed;
				}
				if (input.Down)
				{
					velocity.Y += speed;
				}
				if (input.Left)
				{
					velocity.X -= speed;
				}
				if (input.Right)
				{
					velocity.X += speed;
				}
			}
			plr.velocity = velocity;
			plr.position += velocity;
			plr.fallStart = (int)(plr.position.Y / 16f);
			if (plr.mount.CanGrindRails || plr.mount.Cart)
			{
				plr.fullRotation = 0f;
				plr.fullRotationOrigin = Vector2.Zero;
			}
			if (!BuiltInCheats.Freecam.Enabled)
			{
				int dir = plr.GetDirection();
				bool canChangeDir = (plr.itemAnimation == 0 || plr.inventory[plr.selectedItem].useTurn) && plr.mount.AllowDirectionChange && !plr.sandStorm;
				if (plr.mount.Active && plr.mount.Cart && Math.Sign(velocity.X) != dir)
				{
					canChangeDir = false;
				}
				if (canChangeDir)
				{
					plr.ChangeDir(dir);
				}
			}
		}

		private int GetDirection()
		{
			float x = plr.velocity.X;
			if (!(x < 0f))
			{
				if (x > 0f && plr.controlRight && !plr.controlLeft)
				{
					return 1;
				}
			}
			else if (plr.controlLeft && !plr.controlRight)
			{
				return -1;
			}
			return plr.direction;
		}
	}

	private struct NoclipInputStruct
	{
		public bool Up;

		public bool Down;

		public bool Left;

		public bool Right;

		public float SpeedMultiplier;
	}

	private static void ApplyAxisAcc(ref float vel, bool positive, bool negative, float speed, float accel)
	{
		int dir = 0;
		if (positive)
		{
			dir = 1;
		}
		else if (negative)
		{
			dir = -1;
		}
		if (dir != 0)
		{
			if (Math.Sign(vel) != dir)
			{
				vel *= 0.9f;
			}
			vel += accel * (float)dir;
			vel = Math.Clamp(vel, 0f - speed, speed);
		}
		else
		{
			vel = ((Math.Abs(vel) > accel) ? (vel * 0.9f) : 0f);
		}
	}
}
