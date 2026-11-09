using System.Collections.Generic;

namespace Terraria.UtilClient.Common.Helpers;

public static class CollectionHelper
{
	extension<T>(IList<T> list)
	{
		public void MoveTo(int oldIndex, int newIndex)
		{
			T temp = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, temp);
		}
	}
}
