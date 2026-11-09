using System;
using System.Numerics;
using Terraria.UtilClient.Net;
using Terraria.UtilClient.Net.Protocol.Packets;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class ProjectileHelper
{
	public static Projectile NewProjectileCustom(int type, float ai0, float ai1, float ai2, Vector2 position, Vector2 velocity = default(Vector2), bool centerPos = true, int damage = 0, float knockBack = 0f, Action<Projectile>? modifyProjectile = null, bool sync = true)
	{
		return NewProjectileCustom(type, position, velocity, centerPos, 0, 0f, modifyProjectile, sync, ai0, ai1, ai2);
	}

	public static Projectile NewProjectileCustom(int type, Vector2 position, Vector2 velocity = default(Vector2), bool centerPos = true, int damage = 0, float knockBack = 0f, Action<Projectile>? modifyProjectile = null, bool sync = true, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
	{
		Projectile projectile = Projectile.NewProjectileSetup();
		projectile.SetDefaults(type);
		if (centerPos)
		{
			projectile.Center = position;
		}
		else
		{
			projectile.position = position;
		}
		projectile.owner = Main.myPlayer;
		projectile.velocity = velocity;
		projectile.damage = damage;
		projectile.knockBack = knockBack;
		projectile.ai = new float[3] { ai0, ai1, ai2 };
		projectile.active = true;
		modifyProjectile?.Invoke(projectile);
		if (sync)
		{
			SyncProjectilePacket.FromProjectile(projectile).Send();
		}
		return projectile;
	}
}
