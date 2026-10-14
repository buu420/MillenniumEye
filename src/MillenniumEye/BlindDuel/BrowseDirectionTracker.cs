// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using Il2CppYgomSystem;
using UnityEngine;

namespace MillenniumEye;

internal static class BrowseDirectionTracker
{
	public static void TrackKeyDown(int type, bool result)
	{
		if (DuelState.LastBrowsePosition >= 0 && Application.isFocused)
		{
			if (type == GamePad.BUTTON_DOWN && (result || Input.GetKeyDown(KeyCode.DownArrow)))
			{
				DuelState.BrowseDirection = 1;
			}
			else if (type == GamePad.BUTTON_UP && (result || Input.GetKeyDown(KeyCode.UpArrow)))
			{
				DuelState.BrowseDirection = -1;
			}
		}
	}

	public static void TrackKey(int type, bool result)
	{
		if (DuelState.LastBrowsePosition >= 0 && Application.isFocused)
		{
			if (type == GamePad.BUTTON_DOWN && (result || Input.GetKey(KeyCode.DownArrow)))
			{
				DuelState.BrowseDirection = 1;
			}
			else if (type == GamePad.BUTTON_UP && (result || Input.GetKey(KeyCode.UpArrow)))
			{
				DuelState.BrowseDirection = -1;
			}
		}
	}
}
