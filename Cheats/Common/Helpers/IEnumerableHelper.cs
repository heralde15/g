using System.Collections.Generic;

namespace Terraria.UtilClient.Common.Helpers;

public static class IEnumerableHelper
{
	public static IEnumerable<TItem> Concat<TItem>(params IEnumerable<TItem>[] collections)
	{
		foreach (IEnumerable<TItem> collection in collections)
		{
			foreach (TItem item in collection)
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<TItem> Concat<TItem>(IEnumerable<IEnumerable<TItem>> collections)
	{
		foreach (IEnumerable<TItem> collection in collections)
		{
			foreach (TItem item in collection)
			{
				yield return item;
			}
		}
	}
}
