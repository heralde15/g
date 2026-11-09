using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Terraria.UtilClient.Config;
using Terraria.UtilClient.Config.Types;

namespace Terraria.UtilClient.Features;

public static class ProfanityFilter
{
	internal static bool TempFilterTextBypass;

	private static readonly Dictionary<char, char> CharMap = new Dictionary<char, char>
	{
		['а'] = 'a',
		['А'] = 'a',
		['е'] = 'e',
		['Е'] = 'e',
		['о'] = 'o',
		['О'] = 'o',
		['р'] = 'p',
		['Р'] = 'p',
		['с'] = 's',
		['С'] = 's',
		['х'] = 'x',
		['Х'] = 'x',
		['у'] = 'y',
		['У'] = 'y',
		['к'] = 'k',
		['К'] = 'k',
		['м'] = 'm',
		['М'] = 'm',
		['т'] = 't',
		['Т'] = 't',
		['і'] = 'i',
		['І'] = 'i',
		['н'] = 'n',
		['Н'] = 'n',
		['в'] = 'v',
		['В'] = 'v',
		['б'] = 'b',
		['Б'] = 'b',
		['ɑ'] = 'a',
		['ο'] = 'o',
		['ѕ'] = 's'
	};

	public static string FilterMessage(string message)
	{
		if (string.IsNullOrEmpty(message) || TempFilterTextBypass)
		{
			return message;
		}
		message = Normalize(message);
		return Filter(message);
	}

	public static string FilterName(string name, byte messageAuthor)
	{
		if (string.IsNullOrEmpty(name))
		{
			return name;
		}
		if (messageAuthor == byte.MaxValue)
		{
			return name;
		}
		string normalized = Normalize(name);
		foreach (Regex swearPattern in UConfigExtensions.get_Current<ProfanityFilterConfig>().SwearPatterns)
		{
			if (swearPattern.IsMatch(normalized))
			{
				return $"Player {messageAuthor}";
			}
		}
		return name;
	}

	private static string Filter(string input)
	{
		string[] words = input.Split(' ');
		for (int i = 0; i < words.Length; i++)
		{
			foreach (Regex swearPattern in UConfigExtensions.get_Current<ProfanityFilterConfig>().SwearPatterns)
			{
				Match match = swearPattern.Match(words[i]);
				if (match.Success)
				{
					words[i] = words[i].Replace(match.Value, new string('*', match.Value.Length));
					break;
				}
			}
		}
		return string.Join(" ", words);
	}

	private static string Normalize(string input)
	{
		input = input.Normalize(NormalizationForm.FormKC);
		StringBuilder sb = new StringBuilder(input.Length);
		string text = input;
		foreach (char ch in text)
		{
			UnicodeCategory category = char.GetUnicodeCategory(ch);
			if (category != UnicodeCategory.Format && category != UnicodeCategory.NonSpacingMark && category != UnicodeCategory.EnclosingMark)
			{
				if (CharMap.TryGetValue(ch, out var mapped))
				{
					sb.Append(mapped);
				}
				else
				{
					sb.Append(ch);
				}
			}
		}
		return sb.ToString();
	}
}
