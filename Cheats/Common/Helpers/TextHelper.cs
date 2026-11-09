using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Terraria.ID;
using Terraria.UtilClient.Localization;
using Terraria.UtilClient.Tweaks;
using Terraria.UtilClient.Tweaks.Collections;

namespace Terraria.UtilClient.Common.Helpers;

public static class TextHelper
{
	public enum IdDisplayStyle
	{
		Name,
		Tooltip,
		NameAndTooltip,
		Legacy
	}

	public static readonly FrozenDictionary<int, string> KnownProtocolVersions = new Dictionary<int, string>
	{
		{ 279, "1.4.4.9" },
		{ 315, "1.4.5.0" },
		{ 316, "1.4.5.3" },
		{ 317, "1.4.5.4" },
		{ 318, "1.4.5.5" },
		{ 319, "1.4.5.6" }
	}.ToFrozenDictionary();

	public static double GetElapsedSeconds(long start, long? stop = null)
	{
		return (double)(stop ?? (Stopwatch.GetTimestamp() - start)) / (double)Stopwatch.Frequency;
	}

	public static string FormatLastPacketTime(long start, bool asMilliseconds = false)
	{
		if (start <= 0)
		{
			return ULang.UIVal("NoPacketsSent");
		}
		double elapsedSeconds = GetElapsedSeconds(start);
		if (asMilliseconds)
		{
			return $"{elapsedSeconds * 1000.0:0}ms";
		}
		return $"{elapsedSeconds:0.000}s";
	}

	public static string ItemIDName(int id)
	{
		if (Main.netMode == 2 || !TweaksCollectionExtensions.get_Current<UITweaks>().ShowItemIDs)
		{
			return string.Empty;
		}
		switch (TweaksCollectionExtensions.get_Current<VisualTweaks>().ItemIDDisplayStyle)
		{
		default:
			return string.Empty;
		case IdDisplayStyle.Name:
		case IdDisplayStyle.NameAndTooltip:
			return $" [{SafeItemIdAccess(id)}/{id}]";
		case IdDisplayStyle.Legacy:
			return $" (ID: {id})";
		}
	}

	public static string SafeItemIdAccess(int id)
	{
		string extra = "Unknown";
		if (id < 0)
		{
			id = ItemID.FromNetId((short)id);
		}
		if (id < ItemID.Count)
		{
			extra = ItemID.Search.GetName(id);
		}
		return extra;
	}

	public static IEnumerable<T> WhereStartsWith<T>(IEnumerable<T> source, Func<T, string> selector, string prefix, bool excludeSame = false, bool ignoreCase = false)
	{
		StringComparison comparison = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		int prefixLength = prefix.Length;
		foreach (T item in source)
		{
			string value = selector(item);
			if (value.StartsWith(prefix, comparison) && (!excludeSame || value.Length != prefixLength))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<string> GetMatches(string start, IEnumerable<string> values, bool excludeSame = false, bool ignoreCase = false)
	{
		return WhereStartsWith(values, (string s) => s, start, excludeSame, ignoreCase);
	}

	public static IEnumerable<Player> FindPlayers(string nameStart, bool excludeSame = false, bool ignoreCase = false)
	{
		return WhereStartsWith(Main.player.Where((Player p) => p.active), (Player p) => p.name, nameStart, excludeSame, ignoreCase);
	}

	public static string GetProtocolVersionName(int version)
	{
		if (KnownProtocolVersions.ContainsKey(version))
		{
			return $"{KnownProtocolVersions[version]} ({version})";
		}
		return $"{version}";
	}

	public static void CompressStackTrace(StringBuilder sb, StackTrace trace)
	{
		StackFrame[] frames = trace.GetFrames();
		if (frames == null || frames.Length == 0)
		{
			return;
		}
		string[] formatted = frames.Select(FormatFrame).ToArray();
		int i = 0;
		while (i < formatted.Length)
		{
			int patternLen = FindPattern(formatted, i);
			if (patternLen > 0)
			{
				int repeat = CountRepeats(formatted, i, patternLen);
				StringBuilder stringBuilder = sb;
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder);
				handler.AppendLiteral("... (repeated ");
				handler.AppendFormatted(repeat);
				handler.AppendLiteral(" times)");
				stringBuilder2.AppendLine(ref handler);
				for (int j = 0; j < patternLen; j++)
				{
					stringBuilder = sb;
					StringBuilder stringBuilder3 = stringBuilder;
					handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder);
					handler.AppendLiteral("   ↳ ");
					handler.AppendFormatted(formatted[i + j]);
					stringBuilder3.AppendLine(ref handler);
				}
				i += patternLen * repeat;
			}
			else
			{
				sb.AppendLine(formatted[i]);
				i++;
			}
			if (sb.Length >= 10000)
			{
				sb.AppendLine("... (truncated)");
				break;
			}
		}
		static int CountRepeats(string[] arr, int start, int len)
		{
			int count;
			for (count = 1; start + len * (count + 1) <= arr.Length; count++)
			{
				bool match = true;
				for (int k = 0; k < len; k++)
				{
					if (!(arr[start + k] == arr[start + k + len * count]))
					{
						match = false;
						break;
					}
				}
				if (!match)
				{
					break;
				}
			}
			return count;
		}
		static int FindPattern(string[] arr, int start)
		{
			for (int len = 1; len <= 4 && start + len * 2 <= arr.Length; len++)
			{
				bool match = true;
				for (int k = 0; k < len; k++)
				{
					if (!(arr[start + k] == arr[start + k + len]))
					{
						match = false;
						break;
					}
				}
				if (match)
				{
					return len;
				}
			}
			return 0;
		}
		static string FormatFrame(StackFrame frame)
		{
			MethodBase method = frame.GetMethod();
			if (method == null)
			{
				return "<unknown method>";
			}
			ParameterInfo[] parameters = method.GetParameters();
			string paramList = string.Join(", ", parameters.Select((ParameterInfo p) => p.ParameterType.Name + " " + p.Name));
			string frameString = $"at {method.DeclaringType?.FullName}.{method.Name}({paramList})";
			string file = frame.GetFileName();
			int line = frame.GetFileLineNumber();
			if (!string.IsNullOrEmpty(file) && line > 0)
			{
				frameString += $" in {file}:line {line}";
			}
			return frameString;
		}
	}
}
