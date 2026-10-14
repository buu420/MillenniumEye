// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;

namespace MillenniumEye;

public class TextSearchOptions
{
	public static readonly TextSearchOptions Default = new TextSearchOptions();

	public static readonly TextSearchOptions ActiveFiltered = new TextSearchOptions
	{
		ActiveOnly = true,
		FilterBanned = true
	};

	public static readonly TextSearchOptions AllInclusive = new TextSearchOptions
	{
		ActiveOnly = false,
		FilterBanned = false
	};

	public bool ActiveOnly { get; init; } = true;

	public bool FilterBanned { get; init; } = true;

	public bool HasMenuContext { get; init; }

	public int MaxDepth { get; init; } = -1;

	public List<string> ExcludePathContaining { get; init; }

	public List<string> IncludePathContaining { get; init; }

	public string LogPrefix { get; init; }
}
