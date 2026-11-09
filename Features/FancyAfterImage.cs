using System.Drawing;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.UtilClient.Features;

internal class FancyAfterImage
{
	public static bool UpdateWhenPaused;

	public Texture2D ActiveTexture;

	public Rectangle Frame;

	public float Transparency;

	public float DecayRate;

	public Vector2 PositionInWorld;

	public Vector2 Velocity;

	public Color Color;

	public float Scale;

	public bool Flipped;

	public FancyAfterImage(Texture2D activeTexture, Rectangle frame, float transparency, Vector2 pos, Vector2 velocity = default(Vector2), float decayRate = 0.01f, bool mirror = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Color = Color.White;
		Scale = 1f;
		base._002Ector();
		ActiveTexture = activeTexture;
		Frame = frame;
		Transparency = transparency;
		PositionInWorld = pos;
		Velocity = velocity;
		DecayRate = decayRate;
		Flipped = mirror;
	}

	public void Update()
	{
		if (!Main.gamePaused || UpdateWhenPaused)
		{
			PositionInWorld -= Velocity * Main.DrawDeltaTime;
			Transparency -= DecayRate * Main.DrawDeltaTime;
		}
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Transparency < 0f)
		{
			return;
		}
		Rectangle rectangle = new Rectangle((int)Main.screenPosition.X - 128, (int)Main.screenPosition.Y - 128, Main.screenWidth + 128, Main.screenHeight + 128);
		Rectangle imageRect = new Rectangle((int)PositionInWorld.X, (int)PositionInWorld.Y, Frame.Width, Frame.Height);
		if (NumericsExtensions.Intersects(rectangle, imageRect))
		{
			SpriteEffects effects = (SpriteEffects)0;
			if (Flipped)
			{
				effects = (SpriteEffects)1;
			}
			Color color = new Color((int)((Color)(ref Color)).R, (int)((Color)(ref Color)).G, (int)((Color)(ref Color)).B, 255) * Transparency;
			Vector2 drawPosition = new Vector2(PositionInWorld.X - Main.screenPosition.X, PositionInWorld.Y - Main.screenPosition.Y);
			spriteBatch.Draw(ActiveTexture, drawPosition, (Rectangle?)Frame, color, 0f, Vector2.Zero, Scale, effects, 0f);
		}
	}

	public static FancyAfterImage CreateNew(Texture2D texture, Rectangle frame, float transparency, Vector2 pos, Vector2 velocity = default(Vector2), float decayRate = 0.01f, bool mirror = false)
	{
		return new FancyAfterImage(texture, frame, transparency, pos, velocity, decayRate, mirror);
	}
}
