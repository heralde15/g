using System;
using System.Numerics;
using System.Reflection;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UtilClient.Logging;
using Terraria.UtilClient.Managing;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Features;

internal class JellyEffect
{
	internal static JellyEffect[] JellyPlayerData = new JellyEffect[255];

	internal static JellyEffect[] JellyNPCData = new JellyEffect[Main.maxNPCs];

	private static bool Drunk;

	private int time;

	private float rotation;

	private float frequency;

	private float R;

	private float fadeOut;

	private SlotId JellyMusicSound;

	internal static bool CanJelly
	{
		get
		{
			if (!Drunk)
			{
				return TweaksCollectionExtensions.get_Current<ZModTweaks>().InvincibleJellyEnabled;
			}
			return true;
		}
	}

	internal static void Initialize()
	{
		for (int i = 0; i < JellyPlayerData.Length; i++)
		{
			JellyPlayerData[i] = new JellyEffect();
		}
		for (int j = 0; j < JellyNPCData.Length; j++)
		{
			JellyNPCData[j] = new JellyEffect();
		}
		if (!Drunk)
		{
			ManagerExtensions.get_Instance<LoggingManager>().zMod.Info((object)"Initialized jelly effect");
		}
	}

	internal static void FullReset()
	{
		Drunk = (bool)(typeof(Main).GetField("CrouchingEnabled", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) ?? ((object)false));
		for (int i = 0; i < JellyPlayerData.Length; i++)
		{
			JellyPlayerData[i].Reset();
		}
		for (int j = 0; j < JellyNPCData.Length; j++)
		{
			JellyNPCData[j].Reset();
		}
	}

	internal static void Update()
	{
		for (int i = 0; i < JellyPlayerData.Length; i++)
		{
			if (!Main.player[i].active || Main.player[i].dead)
			{
				JellyPlayerData[i].Reset();
				Main.player[i].fullRotation = 0f;
			}
			else
			{
				JellyPlayerData[i].UpdateJellySound(Main.player[i].Center);
			}
		}
		for (int j = 0; j < JellyNPCData.Length; j++)
		{
			if (!Main.npc[j].active || Main.npc[j].immortal)
			{
				JellyNPCData[j].Reset();
			}
			else
			{
				JellyNPCData[j].UpdateJellySound(Main.npc[j].Center);
			}
		}
	}

	internal void UpdateJellySound(Vector2 center)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ActiveSound jellySound = SoundEngine.GetActiveSound(JellyMusicSound);
		if (jellySound == null)
		{
			return;
		}
		jellySound.Position = center;
		if (!CanJelly)
		{
			jellySound.Stop();
		}
		if (jellySound.IsPlaying)
		{
			if (time > 0 && fadeOut < 1f)
			{
				fadeOut = 1f;
			}
			if (time <= 0)
			{
				fadeOut -= 0.075f;
			}
			UpdateJellySoundVolume(jellySound);
			if (jellySound.IsPlaying && fadeOut <= 0f)
			{
				jellySound.Stop();
			}
		}
	}

	internal void UpdateJellySoundVolume(ActiveSound jellySound)
	{
		if (jellySound != null)
		{
			float volume = 1f;
			volume = MathHelper.Clamp(1f - (jellySound.Position - Main.Camera.Center).Length() / LegacySoundPlayer.SoundAttenuationDistance, 0f, 1f);
			float musVolume = GetInitialJellySoundVolume();
			jellySound.Sound.Volume = volume * fadeOut * jellySound.Style.Volume * jellySound.Volume * musVolume;
		}
	}

	internal float GetInitialJellySoundVolume()
	{
		float musVolume = (Drunk ? 1f : Main.musicVolume);
		if (!Drunk && !FocusHelper.IsSelectedApplication)
		{
			musVolume = 0f;
		}
		return musVolume;
	}

	internal void DoJellyEffect(Entity e, float damage)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (CanJelly && !(e is NPC { immortal: not false }))
		{
			ActiveSound jellySound = SoundEngine.GetActiveSound(JellyMusicSound);
			if (jellySound == null)
			{
				SoundPlayOverrides overrides = new SoundPlayOverrides
				{
					Volume = GetInitialJellySoundVolume()
				};
				JellyMusicSound = SoundEngine.PlayTrackedSound(SoundID.JellyEffect, e.Center);
				jellySound = SoundEngine.GetActiveSound(JellyMusicSound);
			}
			UpdateJellySoundVolume(jellySound);
			damage /= 10f;
			if (damage < 1f)
			{
				damage = 1f;
			}
			int add = (int)(damage + 60f);
			time += add;
			frequency += (float)add / 300f;
			R += (float)add / 200f;
		}
	}

	internal void Update(Entity e)
	{
		if (!CanJelly || time <= 0)
		{
			return;
		}
		if (time > 900)
		{
			time = 900;
		}
		if (frequency > 3f)
		{
			frequency = 3f;
		}
		if (R > 5f)
		{
			R = 5f;
		}
		if (time > 0)
		{
			if (time % 3 == 0)
			{
				frequency -= 0.01f;
			}
			if (time % 2 == 0)
			{
				R -= 0.01f;
			}
			time--;
			rotation = MathF.Sin((float)time * frequency) * R;
		}
		else
		{
			rotation = 0f;
			frequency = 0.6f;
			R = 0f;
		}
		if (e is NPC nPC)
		{
			nPC.rotation = rotation;
		}
		if (e is Player plr)
		{
			plr.fullRotation = rotation;
			plr.fullRotationOrigin = new Vector2((float)plr.width / 2f, plr.height);
		}
		if (Drunk && time <= 0)
		{
			Reset(stopSound: false);
			DoJellyEffect(e, 900f);
		}
	}

	internal void Reset(bool stopSound = true)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		time = 0;
		rotation = 0f;
		frequency = 0f;
		R = 0f;
		fadeOut = 1f;
		if (stopSound)
		{
			SoundEngine.GetActiveSound(JellyMusicSound)?.Stop();
		}
	}

	public JellyEffect()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		frequency = 0.6f;
		JellyMusicSound = SlotId.Invalid;
		base._002Ector();
	}
}
