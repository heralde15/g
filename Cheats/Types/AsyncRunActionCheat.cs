using System.Threading;
using System.Threading.Tasks;
using Terraria.UtilClient.Chat;
using Terraria.UtilClient.Cheats.Settings;
using Terraria.UtilClient.Common.Utility;

namespace Terraria.UtilClient.Cheats.Types;

public abstract class AsyncRunActionCheat : RunActionCheat
{
	private CancellationTokenSource? _cts;

	public virtual string StopMessage => "Stopped '" + Name + "' cheat.'";

	protected abstract Task RunAction(CancellationToken cancellationToken);

	protected override void RunCore()
	{
		_cts = new CancellationTokenSource();
		AsyncActionRunner.RunAsync(RunAction, _cts.Token, base.FinishRun, base.HandleException);
	}

	public void Stop(bool announce = true)
	{
		EnsureRunning(needed: true);
		_cts?.Cancel();
		if (announce)
		{
			ChatLogging.LogInfo(StopMessage);
		}
	}
}
public abstract class AsyncRunActionCheat<TSettings> : AsyncRunActionCheat where TSettings : USettings, new()
{
	public TSettings Settings { get; } = new TSettings();

	internal override USettings UntypedSettings => Settings;
}
