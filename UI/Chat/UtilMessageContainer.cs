using Terraria.UI.Chat;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.UI.Chat;

internal sealed class UtilMessageContainer(string prefix, int expireTime = -1) : ChatMessageContainer
{
	public string Prefix { get; } = prefix;

	public int TimeLeft { get; private set; } = expireTime;

	public void UpdateExpireTime()
	{
		if (TweaksCollectionExtensions.get_Current<ZModTweaks>().ClientChatLogsExpire && TimeLeft > 0)
		{
			TimeLeft--;
		}
	}
}
