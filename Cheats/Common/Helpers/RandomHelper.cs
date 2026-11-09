using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Terraria.UtilClient.Common.Helpers;

public static class RandomHelper
{
	extension(Random random)
	{
		public bool NextBool()
		{
			return random.Next(2) == 0;
		}

		public bool NextBool(int consequent)
		{
			return random.Next(consequent) == 0;
		}

		public float NextSingle(float minimum, float maximum)
		{
			return random.NextSingle() * (maximum - minimum) + minimum;
		}

		public Vector2 NextVector2Square(float min, float max)
		{
			return new Vector2((max - min) * (float)random.NextDouble() + min, (max - min) * (float)random.NextDouble() + min);
		}

		public TElement Next<TElement>(IReadOnlyList<TElement> source)
		{
			if (source.Count == 0)
			{
				ThrowHelper.ThrowEmptyCollection(source.GetType());
			}
			return source[random.Next(source.Count)];
		}

		public TElement Next<TElement>(IEnumerable<TElement> source)
		{
			using IEnumerator<TElement> enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				ThrowHelper.ThrowEmptyCollection(source.GetType());
			}
			TElement result = enumerator.Current;
			int count = 2;
			while (enumerator.MoveNext())
			{
				if (random.Next(count) == 0)
				{
					result = enumerator.Current;
				}
				count++;
			}
			return result;
		}
	}

	private static class ThrowHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DoesNotReturn]
		public static void ThrowEmptyCollection(Type type)
		{
			throw new InvalidOperationException($"Cannot get a random element from an empty collection of type {type}.");
		}
	}
}
