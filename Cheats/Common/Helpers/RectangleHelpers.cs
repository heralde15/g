using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Terraria.UtilClient.Common.Helpers;

public static class RectangleHelpers
{
	extension(Rectangle rect)
	{
		public bool TryClampToRect(Point point, [NotNullWhen(true)] out Point? clamped, out HorizontalSide? horizontalSide, out VerticalSide? verticalSide)
		{
			Point result = point;
			clamped = null;
			horizontalSide = null;
			verticalSide = null;
			if (result.X < rect.Left)
			{
				result.X = rect.Left;
				horizontalSide = HorizontalSide.Left;
			}
			else if (result.X > rect.Right)
			{
				result.X = rect.Right;
				horizontalSide = HorizontalSide.Right;
			}
			if (result.Y < rect.Top)
			{
				result.Y = rect.Top;
				verticalSide = VerticalSide.Top;
			}
			else if (result.Y > rect.Bottom)
			{
				result.Y = rect.Bottom;
				verticalSide = VerticalSide.Bottom;
			}
			if (result == point)
			{
				return false;
			}
			clamped = result;
			return true;
		}

		public bool TryClampToRect(Point point, [NotNullWhen(true)] out Point? clamped)
		{
			HorizontalSide? horizontalSide;
			VerticalSide? verticalSide;
			return rect.TryClampToRect(point, out clamped, out horizontalSide, out verticalSide);
		}
	}

	public enum HorizontalSide
	{
		Left,
		Right
	}

	public enum VerticalSide
	{
		Top,
		Bottom
	}
}
