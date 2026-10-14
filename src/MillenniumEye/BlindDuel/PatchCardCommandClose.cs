// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardCommand), "Close")]
internal class PatchCardCommandClose
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				DuelState.SuppressNextFieldFocus = true;
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchCardCommandClose] " + ex.Message);
		}
	}
}
