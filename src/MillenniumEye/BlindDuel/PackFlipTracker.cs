// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System.Collections.Generic;

namespace MillenniumEye;

internal static class PackFlipTracker
{
	private static readonly HashSet<int> _flippedMrks = new HashSet<int>();

	public static bool IsFlipped(int mrk)
	{
		return mrk > 0 && _flippedMrks.Contains(mrk);
	}

	public static void MarkFlipped(int mrk)
	{
		if (mrk > 0)
		{
			_flippedMrks.Add(mrk);
		}
	}

	public static void Reset()
	{
		_flippedMrks.Clear();
	}
}
