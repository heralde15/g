using System;
using log4net.Appender;
using log4net.Core;
using Microsoft.Xna.Framework;

namespace Terraria.UtilClient.Logging;

public class AnsiConsoleAppender : ConsoleAppender
{
	public const string Reset = "\u001b[0m";

	protected override bool RequiresLayout => true;

	public static string Foreground(Color color)
	{
		return $"\u001b[38;2;{((Color)(ref color)).R};{((Color)(ref color)).G};{((Color)(ref color)).B}m";
	}

	public static string Background(Color color)
	{
		return $"\u001b[48;2;{((Color)(ref color)).R};{((Color)(ref color)).G};{((Color)(ref color)).B}m";
	}

	protected override void Append(LoggingEvent loggingEvent)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Level level = loggingEvent.Level;
		Color ansiColor = ((level == Level.Fatal) ? Color.Red : ((level == Level.Error) ? Color.IndianRed : ((level == Level.Warn) ? Color.Gold : ((level == Level.Info) ? Color.LimeGreen : Color.White))));
		string value = ((AppenderSkeleton)this).RenderLoggingEvent(loggingEvent);
		Console.Write(Foreground(ansiColor));
		Console.Write(value);
		Console.Write("\u001b[0m");
	}
}
