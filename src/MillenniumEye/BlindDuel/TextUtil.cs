// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Il2CppYgomGame.Card;
using UnityEngine;

namespace MillenniumEye;

public static class TextUtil
{
	private static readonly HashSet<string> BannedTexts = new HashSet<string> { "00:00", "You can add new Cards to your Deck.", "×n" };

	private static readonly Regex CardTagPattern = new Regex("<card\\s+mrk\\s*=\\s*'(\\d+)'\\s*/>", RegexOptions.Compiled);

	private static readonly Regex TagPattern = new Regex("<[^>]+>", RegexOptions.Compiled);

	private static readonly Regex WhitespaceOnly = new Regex("^\\s*$", RegexOptions.Compiled);

	private static readonly Regex WhitespaceOrPunctuation = new Regex("^\\s*$|[.!]+$", RegexOptions.Compiled);

	private static readonly Regex CJKPattern = new Regex("[\\u3000-\\u303F\\u3040-\\u309F\\u30A0-\\u30FF\\u4E00-\\u9FFF\\uFF00-\\uFFEF]+", RegexOptions.Compiled);

	private static readonly Regex PluralPattern = new Regex("(\\d+)\\s+(\\w+?)\\(s\\)", RegexOptions.Compiled);

	public static string StripTags(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		text = ResolveCardTags(text);
		return TagPattern.Replace(text, "");
	}

	private static string ResolveCardTags(string text)
	{
		return CardTagPattern.Replace(text, ResolveCardTagMatch);
	}

	private static string ResolveCardTagMatch(Match match)
	{
		try
		{
			string value = match.Groups[1].Value;
			if (int.TryParse(value, out var result))
			{
				Content s_instance = Content.s_instance;
				if (s_instance != null)
				{
					string name = s_instance.GetName(result);
					if (!string.IsNullOrEmpty(name))
					{
						return name;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Write("[TextUtil] ResolveCardTag error: " + ex.Message);
		}
		return "";
	}

	public static string StripCJK(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return CJKPattern.Replace(text, "").Trim();
	}

	public static string FixPlurals(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return PluralPattern.Replace(text, FixPluralMatch);
	}

	private static string FixPluralMatch(Match m)
	{
		string value = m.Groups[1].Value;
		string value2 = m.Groups[2].Value;
		return (value == "1") ? (value + " " + value2) : (value + " " + value2 + "s");
	}

	public static bool IsBannedText(string text, bool hasMenuContext)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}
		if (BannedTexts.Contains(text))
		{
			return true;
		}
		Regex regex = (hasMenuContext ? WhitespaceOnly : WhitespaceOrPunctuation);
		return regex.IsMatch(text);
	}

	public static bool IsBannedText(GameObject textElement, string text, bool hasMenuContext)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}
		if (BannedTexts.Contains(text))
		{
			return true;
		}
		Regex regex = ((hasMenuContext || textElement.name.Equals("Button")) ? WhitespaceOnly : WhitespaceOrPunctuation);
		return regex.IsMatch(text);
	}
}
