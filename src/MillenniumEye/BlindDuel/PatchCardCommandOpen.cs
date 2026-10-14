// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

[HarmonyPatch(typeof(CardCommand), "Open", new Type[] { })]
internal class PatchCardCommandOpen
{
	[HarmonyPostfix]
	private static void Postfix()
	{
		try
		{
			if (NavigationState.IsInDuel)
			{
				Log.Write("[CardCommand] Action menu opened — silencing card speech");
				Speech.Silence();
			}
		}
		catch (Exception ex)
		{
			Log.Write("[PatchCardCommandOpen] " + ex.Message);
		}
	}
}
