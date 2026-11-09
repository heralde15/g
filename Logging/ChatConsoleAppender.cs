using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Microsoft.Xna.Framework;
using Terraria.UI.Chat;
using Terraria.UtilClient.Common.Helpers;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Logging;

public sealed class ChatConsoleAppender : ConsoleAppender
{
	protected override bool RequiresLayout => true;

	protected override void Append(LoggingEvent loggingEvent)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		string message = ((AppenderSkeleton)this).RenderLoggingEvent(loggingEvent);
		if (TweaksCollectionExtensions.get_Current<AdvancedTweaks>().RawChatInConsole)
		{
			Console.Write(message);
			return;
		}
		Color defaultColor = Color.White;
		List<TextSnippet> list = ChatManager.ParseMessage(message, defaultColor, noReplace: true);
		StringBuilder logBuilder = new StringBuilder();
		foreach (TextSnippet snippet in list)
		{
			Color color = snippet.Color;
			logBuilder.Append((color == defaultColor) ? snippet.Text : color.ToConsoleText(snippet.Text, isForeground: true, defaultColor));
		}
		Console.Write(logBuilder.ToString());
	}
}
