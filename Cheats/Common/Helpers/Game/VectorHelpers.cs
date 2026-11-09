using System.Numerics;

namespace Terraria.UtilClient.Common.Helpers.Game;

public static class VectorHelpers
{
	extension(Vector2 v)
	{
		public Vector2 RoundToWholeNumbers()
		{
			return new Vector2((int)v.X, (int)v.Y);
		}

		public bool IsFinite()
		{
			if (float.IsFinite(v.X))
			{
				return float.IsFinite(v.Y);
			}
			return false;
		}
	}
}
