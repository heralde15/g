using System;
using System.Threading;

namespace Terraria.UtilClient.Common.Utility;

internal sealed class ActionDisposable(Action action) : IDisposable
{
	private int _disposed;

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) == 0)
		{
			action();
		}
	}
}
