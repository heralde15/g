using System;
using System.Threading;
using System.Threading.Tasks;

namespace Terraria.UtilClient.Common.Utility;

public static class AsyncActionRunner
{
	private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(8);

	public static async Task<bool> RunAsync(Action syncAction, Action? onFinished = null, Action<Exception>? onError = null)
	{
		if (!(await Semaphore.WaitAsync(0)))
		{
			throw new InvalidOperationException("Too many actions running.");
		}
		try
		{
			await Task.Run(syncAction);
			return true;
		}
		catch (Exception obj)
		{
			onError?.Invoke(obj);
			return false;
		}
		finally
		{
			Semaphore.Release();
			onFinished?.Invoke();
		}
	}

	public static async Task<bool> RunAsync(Func<Task> asyncAction, Action? onFinished = null, Action<Exception>? onError = null)
	{
		if (!(await Semaphore.WaitAsync(0)))
		{
			throw new InvalidOperationException("Too many actions running.");
		}
		try
		{
			await asyncAction();
			return true;
		}
		catch (Exception obj)
		{
			onError?.Invoke(obj);
			return false;
		}
		finally
		{
			Semaphore.Release();
			onFinished?.Invoke();
		}
	}

	public static async Task<bool> RunAsync(Func<CancellationToken, Task> asyncAction, CancellationToken cancellationToken, Action? onFinished = null, Action<Exception>? onError = null)
	{
		if (!(await Semaphore.WaitAsync(0, cancellationToken)))
		{
			throw new InvalidOperationException("Too many actions running.");
		}
		try
		{
			await Task.Run(() => asyncAction(cancellationToken));
			return true;
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			return false;
		}
		catch (Exception obj)
		{
			onError?.Invoke(obj);
			return false;
		}
		finally
		{
			Semaphore.Release();
			onFinished?.Invoke();
		}
	}
}
