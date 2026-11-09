using System;
using System.Threading;

namespace Terraria.UtilClient.Cheats.Common;

internal sealed class TimerScheduler : IDisposable
{
	private readonly UpdateRateState _rate;

	private readonly Lock _sync = new Lock();

	private Timer? _timer;

	public TimerScheduler(UpdateRateState rate)
	{
		_rate = rate;
	}

	public void Start(Action callback)
	{
		using (_sync.EnterScope())
		{
			if (_timer == null)
			{
				_timer = new Timer(delegate
				{
					callback();
				}, null, 0u, _rate.Value);
			}
		}
	}

	public void Stop()
	{
		using (_sync.EnterScope())
		{
			_timer?.Dispose();
			_timer = null;
		}
	}

	public void UpdateRate()
	{
		using (_sync.EnterScope())
		{
			_timer?.Change(0u, _rate.Value);
		}
	}

	public void Dispose()
	{
		Stop();
	}
}
