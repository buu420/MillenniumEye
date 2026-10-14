// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Linq;

namespace MillenniumEye;

public static class EnumUtil
{
	public static string ParseAttribute(string spriteName)
	{
		return ParseFromLastChar<CardAttribute>(spriteName);
	}

	public static string ParseRarity(string spriteName)
	{
		return ParseFromLastChar<CardRarity>(spriteName);
	}

	private static string ParseFromLastChar<T>(string text) where T : struct, Enum
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		if (int.TryParse(text.Last().ToString(), out var result) && Enum.IsDefined(typeof(T), result))
		{
			return ((T)(object)result/*cast due to constrained. prefix*/).ToString();
		}
		return "";
	}
}
