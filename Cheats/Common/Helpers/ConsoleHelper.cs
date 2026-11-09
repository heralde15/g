using System.Text;
using Microsoft.Xna.Framework;

namespace Terraria.UtilClient.Common.Helpers;

public static class ConsoleHelper
{
	extension(Color clr)
	{
		public string ToAscii(bool isForeground)
		{
			int present = (isForeground ? 38 : 48);
			return $"\u001b[{present};2;{((Color)(ref clr)).R};{((Color)(ref clr)).G};{((Color)(ref clr)).B}m";
		}

		public string ToConsoleText(string text, bool isForeground, Color? origColor = null)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			StringBuilder sb = new StringBuilder();
			sb.Append("\u001b[0m");
			sb.Append(clr.ToAscii(isForeground));
			sb.Append(text);
			sb.Append("\u001b[0m");
			if (origColor.HasValue)
			{
				sb.Append(origColor.Value.ToAscii(isForeground));
			}
			return sb.ToString();
		}
	}

	public const string Reset = "\u001b[0m";
}
