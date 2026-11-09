namespace Terraria.UtilClient.Cheats.Common;

internal sealed class TickScheduler
{
	private readonly UpdateRateState _rate;

	public uint CurrentTick { get; private set; }

	public TickScheduler(UpdateRateState rate)
	{
		_rate = rate;
	}

	public bool ShouldUpdate()
	{
		CurrentTick++;
		return CurrentTick % _rate.Value == 0;
	}

	public void Reset()
	{
		CurrentTick = 0u;
	}
}
