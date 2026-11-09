using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Terraria.UtilClient.Common.Helpers;
using Terraria.UtilClient.Managing;

namespace Terraria.UtilClient.Logging;

public sealed class LoggingManager : Manager
{
	public readonly ILog zMod;

	public readonly ILog zModConfig;

	public readonly ILog Net;

	public readonly ILog Socket;

	private readonly ILog Chat;

	public void LogChatMessage(string message, Color messageColor)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Chat.Info((object)((!((Color)(ref messageColor)).Equals(Color.White)) ? messageColor.ToConsoleText(message, isForeground: true) : message));
	}

	private LoggingManager()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		zMod = LogManager.GetLogger("zMod");
		zModConfig = LogManager.GetLogger("config");
		Net = LogManager.GetLogger("net");
		Socket = LogManager.GetLogger("socket");
		Chat = LogManager.GetLogger("chat");
		base._002Ector();
		Hierarchy val = (Hierarchy)LogManager.GetRepository();
		PatternLayout defaultLayout = new PatternLayout
		{
			ConversionPattern = "[%d{HH:mm:ss}] [%t/%level] [%logger]: %m%n"
		};
		((LayoutSkeleton)defaultLayout).ActivateOptions();
		PatternLayout chatLayout = new PatternLayout
		{
			ConversionPattern = "[%d{HH:mm:ss}] [%logger]: %m%n"
		};
		((LayoutSkeleton)chatLayout).ActivateOptions();
		AnsiConsoleAppender ansiConsoleAppender = new AnsiConsoleAppender();
		((AppenderSkeleton)ansiConsoleAppender).Name = "DefaultAppender";
		((AppenderSkeleton)ansiConsoleAppender).Layout = (ILayout)(object)defaultLayout;
		AnsiConsoleAppender defaultAppender = ansiConsoleAppender;
		((AppenderSkeleton)defaultAppender).ActivateOptions();
		ChatConsoleAppender chatConsoleAppender = new ChatConsoleAppender();
		((AppenderSkeleton)chatConsoleAppender).Name = "ChatAppender";
		((AppenderSkeleton)chatConsoleAppender).Layout = (ILayout)(object)chatLayout;
		ChatConsoleAppender chatAppender = chatConsoleAppender;
		((AppenderSkeleton)chatAppender).ActivateOptions();
		val.Root.RemoveAllAppenders();
		val.Root.Level = Level.All;
		val.Root.AddAppender((IAppender)(object)defaultAppender);
		Logger val2 = (Logger)((LoggerRepositorySkeleton)val).GetLogger(((ILoggerWrapper)Chat).Logger.Name);
		val2.Level = Level.All;
		val2.AddAppender((IAppender)(object)chatAppender);
		val2.Additivity = false;
		((LoggerRepositorySkeleton)val).Configured = true;
	}
}
