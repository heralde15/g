using System.Numerics;
using Terraria.GameContent.Drawing;
using Terraria.UtilClient.Common.Helpers.Game;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Features;

public static class DebuffParticles
{
	public static void SpawnIfNecessary(int type, Entity e)
	{
		if (!Main.dedServ && TweaksCollectionExtensions.get_Current<VisualTweaks>().DebuffDisplayParticle && MiscHelper.Debuffs.Contains(type) && (!(e is NPC n) || (n.active && n.life > 0 && !n.HasBuff(type))) && (!(e is Player p) || (p.active && !p.dead && !p.ghost && p.FindBuffIndex(type) == -1)))
		{
			Vector2 randomOffset = new Vector2(UMain.rand.Next(e.width * -1, e.width + 1), UMain.rand.Next(-10, 11));
			ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.BuffDisplay, new ParticleOrchestraSettings
			{
				PositionInWorld = e.Top + randomOffset,
				MovementVector = new Vector2(0f, 1f),
				UniqueInfoPiece = type
			});
		}
	}
}
